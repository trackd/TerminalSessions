#! /usr/bin/pwsh
#Requires -Version 5.1 -Module InvokeBuild
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [switch]$SkipHelp,
    [switch]$SkipTests,
    [Switch]$BuildAndTestOnly,
    [string] $Task
)

$ErrorActionPreference = 'Stop'
# Helper function to get paths
$buildparams = @{
    Configuration = $Configuration
    SkipHelp      = $SkipHelp.IsPresent
    SkipTests     = $SkipTests.IsPresent
    File          = Join-Path $PSScriptRoot 'TerminalSessions.build.ps1'
    Task          = 'All'
    Result        = 'Result'
    Safe          = $true
}
if ($task) {
    $buildparams.Task = $task
}
if ($BuildAndTestOnly) {
    $buildparams.Task = 'BuildAndTest'
}
if (-Not $env:CI) {
    # this is just so the dll doesn't get locked on and i can rebuild without restarting terminal
    $sb = {
        param($bp)
        Invoke-Build @bp
    }
    pwsh -NoProfile -Command $sb -args $buildparams
}
else {
    Invoke-Build @buildparams
}
