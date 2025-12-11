#! /usr/bin/pwsh
#Requires -Version 5.1 -Module InvokeBuild
param(
    [string]$Configuration = 'Release',
    [switch]$SkipHelp,
    [switch]$SkipTests
)
Write-Host "$($PSBoundParameters.GetEnumerator())" -ForegroundColor Cyan

$script:folders = @{
        ProjectRoot      = $PSScriptRoot
        SourcePath       = Join-Path $PSScriptRoot 'src'
        OutputPath       = Join-Path $PSScriptRoot 'output'
        ModuleSourcePath = Join-Path $PSScriptRoot 'module'
        DocsPath         = Join-Path $PSScriptRoot 'docs' 'en-US'
        TestPath         = Join-Path $PSScriptRoot 'tests'
        CsprojPath       = Join-Path $PSScriptRoot 'src' 'TerminalSessions.csproj'
        BinPath          = Join-Path $PSScriptRoot 'src' 'bin' $Configuration 'netstandard2.0'
}

task Clean {
    if (Test-Path $folders.OutputPath) {
        Remove-Item -Path $folders.OutputPath -Recurse -Force
    }
    New-Item -Path $folders.OutputPath -ItemType Directory -Force | Out-Null
}

task Build {
    if (-not (Test-Path $folders.CsprojPath)) {
        Write-Warning 'C# project not found, skipping Build'
        return
    }
    Push-Location $folders.SourcePath
    try {
        $buildOutput = dotnet build $folders.CsprojPath --configuration $Configuration --nologo --verbosity minimal 2>&1
        if ($LASTEXITCODE -ne 0) {
            throw "Build failed:`n$buildOutput"
        }
    }
    finally {
        Pop-Location
    }
}

task CopyArtifacts {
    if (-not (Test-Path $folders.BinPath)) {
        Write-Warning 'Binary output path not found, skipping CopyArtifacts'
        return
    }

    $dllPath = Join-Path $folders.BinPath 'TerminalSessions.dll'
    if (-not (Test-Path $dllPath)) {
        throw "DLL not found at: $dllPath"
    }
    Copy-Item -Path $dllPath -Destination $folders.OutputPath -Force

    # Copy PDB in Debug configuration
    if ($Configuration -eq 'Debug') {
        $pdbPath = Join-Path $folders.BinPath 'TerminalSessions.pdb'
        if (Test-Path $pdbPath) {
            Copy-Item -Path $pdbPath -Destination $folders.OutputPath -Force
        }
    }
}

task ModuleFiles {
    if (Test-Path $folders.ModuleSourcePath) {
        Get-ChildItem -Path $folders.ModuleSourcePath -File | ForEach-Object {
            Copy-Item -Path $_.FullName -Destination $folders.OutputPath -Force
        }
    }
    else {
        Write-Warning "Module directory not found at: $($folders.ModuleSourcePath)"
    }
    $moduleName = 'TerminalSessions'
    $testModuleManifestSplat = @{
        # ErrorAction   = 'Ignore'
        # WarningAction = 'Ignore'
        Path = Join-Path $folders.OutputPath "$moduleName.psd1"
    }
    $manifest = Test-ModuleManifest @testModuleManifestSplat
}

task GenerateHelp -if (-not $SkipHelp) {
    if (-not (Test-Path $folders.DocsPath)) {
        Write-Warning "Documentation path not found at: $($folders.DocsPath)"
        return
    }
    if (-not (Get-Module -ListAvailable -Name Microsoft.PowerShell.PlatyPS)) {
        Write-Host '    Installing Microsoft.PowerShell.PlatyPS...' -ForegroundColor Yellow
        Install-Module -Name Microsoft.PowerShell.PlatyPS -Scope CurrentUser -Force -AllowClobber
    }

    Import-Module Microsoft.PowerShell.PlatyPS -ErrorAction Stop

    $modulePath = Join-Path $folders.OutputPath 'TerminalSessions.psd1'
    if (-not (Test-Path $modulePath)) {
        Write-Warning "Module manifest not found at: $modulePath. Skipping help generation."
        return
    }

    Import-Module $modulePath -Force

    $helpOutputPath = Join-Path $folders.OutputPath 'en-US'
    New-Item -Path $helpOutputPath -ItemType Directory -Force | Out-Null

    $allCommandHelp = Get-ChildItem -Path $folders.DocsPath -Filter '*.md' -Recurse -File |
        Where-Object { $_.Name -ne 'TerminalSessions.md' } |
        Import-MarkdownCommandHelp

    if ($allCommandHelp.Count -gt 0) {
        $tempOutputPath = Join-Path $helpOutputPath 'temp'
        Export-MamlCommandHelp -CommandHelp $allCommandHelp -OutputFolder $tempOutputPath -Force | Out-Null

        $generatedFile = Get-ChildItem -Path $tempOutputPath -Filter '*.xml' -Recurse -File | Select-Object -First 1
        if ($generatedFile) {
            Move-Item -Path $generatedFile.FullName -Destination $helpOutputPath -Force
        }
        Remove-Item -Path $tempOutputPath -Recurse -Force -ErrorAction SilentlyContinue
    }
}

task Test -if (-not $SkipTests) {
    if (-not (Test-Path $folders.TestPath)) {
        Write-Warning "Test directory not found at: $($folders.TestPath)"
        return
    }
    $pesterConfig = New-PesterConfiguration
    # $pesterConfig.Output.Verbosity = 'Detailed'
    $pesterConfig.Run.Path = $folders.TestPath
    $pesterConfig.Run.Throw = $true
    $pesterConfig.Debug.WriteDebugMessages = $false
    Invoke-Pester -Configuration $pesterConfig
}

task All -Jobs Clean, Build, CopyArtifacts, ModuleFiles, GenerateHelp, Test
task BuildAndTest -Jobs Clean, Build, CopyArtifacts, ModuleFiles, Test
