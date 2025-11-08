param()
Set-StrictMode -Version Latest
$moduleManifest = Join-Path $PSScriptRoot '..' 'output' 'TerminalSessions.psd1'
if (-not (Test-Path $moduleManifest)) {
    Write-Host 'Module manifest not found. Run build task first.' -ForegroundColor Yellow
    $GitRoot = Split-Path $PSScriptRoot  -Parent
    $buildScript = Join-Path $GitRoot 'build.ps1'
    Write-Host "Attempting to build module by running '$buildScript'..." -Foreground
    & $buildScript | Out-Null
}
else {
    Import-Module $moduleManifest -Force
    Write-Host 'Imported TerminalSessions module from output folder.' -ForegroundColor Green
}
