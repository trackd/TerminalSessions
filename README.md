# TerminalSessions

PowerShell module for managing Windows Terminal Services (Remote Desktop Services) sessions.  

## Description

TerminalSessions provides cmdlets to query and manage user sessions on local and remote Windows computers. It uses native Windows APIs to retrieve detailed session information and perform administrative actions.  

## Installation

```powershell
Install-Module TerminalSessions
```

## Cmdlets

- **Get-WTSSession** - List all sessions on Windows hosts
- **Get-WTSClientInfo** - Get detailed client connection information
- **Get-WTSInfo** - Get extended session statistics and timing
- **Remove-WTSSession** - Log off user sessions

## Examples

```powershell
# Get all sessions on the local computer
Get-WTSSession

# Get sessions from remote computers
Get-WTSSession -ComputerName "Server01", "Server02"

# Get detailed session information
Get-WTSSession -Detailed

# Get client connection details
Get-WTSSession | Get-WTSClientInfo

# Log off a disconnected session
Get-WTSSession | Where-Object State -eq Disconnected | Remove-WTSSession
```

## Building

```powershell
.\build.ps1
```
