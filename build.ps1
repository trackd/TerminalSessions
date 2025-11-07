#Requires -Version 5.1
<#
.SYNOPSIS
    Build script for TerminalSessions module

.DESCRIPTION
    This script builds the TerminalSessions module, copies files to the output directory,
    and generates help documentation using PlatyPS.

.PARAMETER Configuration
    Build configuration (Debug or Release). Default is Release.

.PARAMETER SkipHelp
    Skip generating help documentation.

.EXAMPLE
    .\build.ps1
    Builds the module in Release configuration with help generation.

.EXAMPLE
    .\build.ps1 -Configuration Debug -SkipHelp
    Builds the module in Debug configuration without generating help.
#>
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [switch]$SkipHelp
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Paths
$ProjectRoot = $PSScriptRoot
$SourcePath = Join-Path $ProjectRoot 'src'
$OutputPath = Join-Path $ProjectRoot 'output'
$ModuleSourcePath = Join-Path $ProjectRoot 'module'
$DocsPath = Join-Path (Join-Path $ProjectRoot 'docs') 'en-US'
$CsprojPath = Join-Path $SourcePath 'TerminalSessions.csproj'
$BinPath = Join-Path (Join-Path (Join-Path $SourcePath 'bin') $Configuration) 'netstandard2.0'

Write-Host "==> Building TerminalSessions Module" -ForegroundColor Cyan
Write-Host "    Configuration: $Configuration" -ForegroundColor Gray
Write-Host "    Output Path: $OutputPath" -ForegroundColor Gray

# Clean output directory
if (Test-Path $OutputPath) {
    Write-Host "==> Cleaning output directory..." -ForegroundColor Yellow
    Remove-Item -Path $OutputPath -Recurse -Force
}

# Create output directory
Write-Host "==> Creating output directory..." -ForegroundColor Yellow
New-Item -Path $OutputPath -ItemType Directory -Force | Out-Null

# Build the C# project
Write-Host "==> Building C# project..." -ForegroundColor Yellow
Push-Location $SourcePath
try {
    $buildOutput = dotnet build $CsprojPath --configuration $Configuration --nologo --verbosity minimal 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed:`n$buildOutput"
    }
    Write-Host "    Build succeeded" -ForegroundColor Green
}
finally {
    Pop-Location
}

# Copy DLL to output
Write-Host "==> Copying DLL to output..." -ForegroundColor Yellow
$dllPath = Join-Path $BinPath 'TerminalSessions.dll'
if (-not (Test-Path $dllPath)) {
    Write-Error "DLL not found at: $dllPath"
}
Copy-Item -Path $dllPath -Destination $OutputPath -Force
Write-Host "    Copied TerminalSessions.dll" -ForegroundColor Green

# Copy module files
Write-Host "==> Copying module files..." -ForegroundColor Yellow
if (Test-Path $ModuleSourcePath) {
    $moduleFiles = Get-ChildItem -Path $ModuleSourcePath -File
    foreach ($file in $moduleFiles) {
        Copy-Item -Path $file.FullName -Destination $OutputPath -Force
        Write-Host "    Copied $($file.Name)" -ForegroundColor Green
    }
}
else {
    Write-Warning "Module directory not found at: $ModuleSourcePath"
}

# Generate help documentation
if (-not $SkipHelp) {
    Write-Host "==> Generating help documentation..." -ForegroundColor Yellow

    # Check if Microsoft.PowerShell.PlatyPS is installed
    if (-not (Get-Module -ListAvailable -Name Microsoft.PowerShell.PlatyPS)) {
        Write-Host "    Microsoft.PowerShell.PlatyPS not found, installing..." -ForegroundColor Yellow
        Install-Module -Name Microsoft.PowerShell.PlatyPS -Scope CurrentUser -Force -AllowClobber
    }

    Import-Module Microsoft.PowerShell.PlatyPS -ErrorAction Stop

    # Import the module
    $modulePath = Join-Path $OutputPath 'TerminalSessions.psd1'
    if (Test-Path $modulePath) {
        Write-Host "    Importing module..." -ForegroundColor Gray
        Import-Module $modulePath -Force
    }
    else {
        Write-Warning "Module manifest not found at: $modulePath. Skipping help generation."
        $SkipHelp = $true
    }

    if (-not $SkipHelp) {
        # Create help output directory
        $helpOutputPath = Join-Path $OutputPath 'en-US'
        New-Item -Path $helpOutputPath -ItemType Directory -Force | Out-Null

        # Generate external help from markdown using new PlatyPS
        if (Test-Path $DocsPath) {
            Write-Host "    Generating MAML help from markdown..." -ForegroundColor Gray

            # Import all markdown command help files and export as MAML
            $allCommandHelp = Get-ChildItem -Path $DocsPath -Filter '*.md' -Recurse -File |
                Where-Object { $_.Name -ne 'TerminalSessions.md' } |
                Import-MarkdownCommandHelp

            # Export all commands to a single MAML file
            if ($allCommandHelp.Count -gt 0) {
                # Export-MamlCommandHelp creates a subdirectory with the module name
                # We need to move the file to the correct location
                $tempOutputPath = Join-Path $helpOutputPath 'temp'
                Export-MamlCommandHelp -CommandHelp $allCommandHelp -OutputFolder $tempOutputPath -Force | Out-Null

                # Move the generated file from subdirectory to the en-US folder
                $generatedFile = Get-ChildItem -Path $tempOutputPath -Filter '*.xml' -Recurse -File | Select-Object -First 1
                if ($generatedFile) {
                    Move-Item -Path $generatedFile.FullName -Destination $helpOutputPath -Force
                    Write-Host "    Generated external help XML" -ForegroundColor Green
                }

                # Clean up temp directory
                Remove-Item -Path $tempOutputPath -Recurse -Force -ErrorAction SilentlyContinue
            }
        }
        else {
            Write-Warning "Documentation path not found at: $DocsPath"
        }
    }
}
else {
    Write-Host "==> Skipping help generation" -ForegroundColor Yellow
}

# Display output contents
Write-Host "`n==> Build complete!" -ForegroundColor Green
Write-Host "`nOutput directory contents:" -ForegroundColor Cyan
Get-ChildItem -Path $OutputPath -Recurse | ForEach-Object {
    $relativePath = $_.FullName.Substring($OutputPath.Length + 1)
    if ($_.PSIsContainer) {
        Write-Host "    [DIR]  $relativePath" -ForegroundColor Blue
    }
    else {
        $size = if ($_.Length -lt 1KB) { "$($_.Length) B" }
        elseif ($_.Length -lt 1MB) { "{0:N2} KB" -f ($_.Length / 1KB) }
        else { "{0:N2} MB" -f ($_.Length / 1MB) }
        Write-Host "    [FILE] $relativePath ($size)" -ForegroundColor Gray
    }
}

Write-Host "\nTo test the module, run:" -ForegroundColor Cyan
Write-Host "    Import-Module $OutputPath\TerminalSessions.psd1" -ForegroundColor White
