Set-StrictMode -Version Latest

BeforeAll {
    $moduleManifest = Join-Path (Split-Path $PSScriptRoot -Parent) 'output' 'TerminalSessions.psd1'
    if (-not (Test-Path $moduleManifest)) {
        # Attempt build if missing
        $buildScript = Join-Path (Split-Path $PSScriptRoot -Parent) 'build.ps1'
        & $buildScript -Configuration Debug -SkipHelp:$true | Out-Null
    }
    Import-Module $moduleManifest -Force
}

Describe 'Get-WTSSession basic enumeration' {
    It 'Returns at least one active or disconnected session on local host' {
        $sessions = Get-WTSSession
        $sessions | Should -Not -BeNullOrEmpty
        $sessions | Should -Contain ($sessions[0]) # sanity
        ($sessions | Where-Object { $_.State -in 'Active', 'Disconnected' }).Count | Should -BeGreaterThan 0
    }

    It 'Supports -Detailed returning IdleTime or LogonTime for some sessions' {
        $detailed = Get-WTSSession -Detailed
        $detailed | Should -Not -BeNullOrEmpty
        ($detailed | Where-Object { $_.PSObject.Properties.Name -contains 'IdleTime' }).Count | Should -BeGreaterThan 0
    }

    It 'Pipeline stop with Select-Object -First does not error' {
        $result = Get-WTSSession | Select-Object -First 1
        $result | Should -Not -BeNullOrEmpty
    }
}


Describe 'Get-WTSSession vs quser comparison (if quser available)' {
    BeforeAll {
        $quserRaw = quser.exe 2>$null
        $lines = $quserRaw | Where-Object { $_ -and ($_ -notmatch 'USERNAME') }
        $pattern = '^(?>\s*)(?<Leader>>)?(?<User>\S+)\s+(?:(?<Session>\S+)\s+)?(?<Id>\d+)\s+(?<State>\S+)\s+(?<Idle>\S+)\s+(?<Date>\d{4}-\d{2}-\d{2}|\d{1,2}/\d{1,2}/\d{4})\s+(?<Time>\d{1,2}:\d{2}(?:\s?[AP]M)?)'
        $QUserSessions = foreach ($line in $lines) {
            $m = [regex]::Match($line, $pattern)
            if (-not $m.Success) { continue }
            $idleRaw = $m.Groups['Idle'].Value
            $idleTs = [TimeSpan]::Zero
            if ($idleRaw -in @('none', '.', 'Disc')) { }
            elseif ($idleRaw -match '^(\d+)\+(\d+):(\d+)$') {
                $idleTs = ([TimeSpan]::FromDays([int]$matches[1]) + [TimeSpan]::FromHours([int]$matches[2]) + [TimeSpan]::FromMinutes([int]$matches[3]))
            }
            elseif ($idleRaw -match '^(\d+):(\d+)$') {
                $idleTs = New-TimeSpan -Hours $matches[1] -Minutes $matches[2]
            }
            elseif ($idleRaw -match '^(\d+)$') {
                $idleTs = New-TimeSpan -Minutes $matches[1]
            }
            $dateString = $m.Groups['Date'].Value + ' ' + $m.Groups['Time'].Value
            $logon = $null
            try { $logon = Get-Date -Date $dateString } catch { $logon = $null }
            [pscustomobject]@{
                UserName    = $m.Groups['User'].Value.TrimStart('>')
                SessionName = $m.Groups['Session'].Value
                SessionId   = [uint32]$m.Groups['Id'].Value
                State       = $m.Groups['State'].Value
                IdleTime    = $idleTs
                LogonTime   = $logon
            }
        }
    }

    It 'Parses quser output if available' {
        $QUserSessions | Should -Not -BeNullOrEmpty
        ($QUserSessions | Where-Object LogonTime).Count | Should -BeGreaterThan 0
    }

    It 'Matches at least one username between quser and Get-WTSSession' {
        $sessions = Get-WTSSession
        $quserUsers = $QUserSessions.UserName | Where-Object { $_ -and $_ -ne '>' }
        ($sessions.UserName | Where-Object { $_ -in $quserUsers }).Count | Should -BeGreaterThan 0
    }

    It 'LogonTime difference between quser and -Detailed output is within tolerance' {
        $detailed = Get-WTSSession -Detailed
        $byId = $detailed | Group-Object SessionId -AsHashTable
        $matchesval = 0
        foreach ($q in $QUserSessions) {
            if (-not $q.LogonTime) { continue }
            $s = $null
            if ($byId.ContainsKey($q.SessionId)) {
                $s = $byId[$q.SessionId]
            }
            if ($s -and $s.LogonTime) {
                $delta = [math]::Abs((($s.LogonTime) - ($q.LogonTime)).TotalMinutes)
                # Allow up to 5 minutes discrepancy (timezone / rounding / enumeration delays)
                $delta | Should -BeLessThan 5
                $matchesval++
            }
        }
        $matchesval | Should -BeGreaterThan 0
    }

    It 'IdleTime difference between quser and -Detailed output is within tolerance' {
        $detailed = Get-WTSSession -Detailed
        $byId = $detailed | Group-Object SessionId -AsHashTable
        $idleComparisons = 0
        foreach ($q in $QUserSessions) {
            # Match by SessionId from hash table or fallback to UserName search
            if ($byId.ContainsKey($q.SessionId)) {
                $s = $byId[$q.SessionId]
                # Treat null IdleTime as zero to allow comparison for active sessions
                $moduleIdle = if ($s.IdleTime) { $s.IdleTime } else { [TimeSpan]::Zero }
                $qIdle = if ($q.IdleTime) { $q.IdleTime } else { [TimeSpan]::Zero }
                $deltaMinutes = [math]::Abs(($moduleIdle - $qIdle).TotalMinutes)
                # Allow 5 minute discrepancy for long disconnected sessions (WTS resolution + sampling drift)
                $deltaMinutes | Should -BeLessThan 5
                $idleComparisons++
            }
        }
        $idleComparisons | Should -BeGreaterThan 0
    }
}

Describe 'Get-WTSInfo and Get-WTSClientInfo integration' {
    It 'Get-WTSInfo returns objects with expected properties' {
        $session = Get-WTSSession | Select-Object -First 1
        $info = $session | Get-WTSInfo
        $info | Should -Not -BeNullOrEmpty
        $info[0].PSObject.Properties.Name | Should -Contain 'SessionId'
        $info[0].PSObject.Properties.Name | Should -Contain 'State'
    }

    It 'Get-WTSClientInfo returns client details when client name present' {
        $withClient = Get-WTSSession | Where-Object ClientName -NE $null | Select-Object -First 1
        if ($withClient) {
            $client = $withClient | Get-WTSClientInfo
            $client | Should -Not -BeNullOrEmpty
            $client[0].PSObject.Properties.Name | Should -Contain 'ClientName'
            $client[0].PSObject.Properties.Name | Should -Contain 'EncryptionLevel'
        }
        else {
            Set-ItResult -Skipped -Because 'No remote client sessions available to test client info'
        }
    }
}
