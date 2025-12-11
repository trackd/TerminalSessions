# TerminalSessions

PowerShell module for managing Windows Terminal Services (Remote Desktop Services) sessions.  

## Description

TerminalSessions provides cmdlets to query and manage user sessions on local and remote Windows computers. It uses native Windows APIs to retrieve detailed session information and perform administrative actions.  

## Installation

```powershell
Install-Module TerminalSessions
```

## Cmdlets

- **Get-TerminalSession** - List all sessions on Windows hosts
- **Get-TerminalClientInfo** - Get detailed client connection information
- **Get-TerminalInfo** - Get extended session statistics and timing
- **Remove-TerminalSession** - Log off user sessions
- **Disconnect-TerminalSession** - Disconnect user sessions
- **Send-TerminalMessage** - Send Terminal Messages and optionally receive a response

## Examples

```powershell
# Get all sessions on the local computer
Get-TerminalSession

# Get sessions from remote computers
Get-TerminalSession -ComputerName "Server01", "Server02"

# Get detailed session information
Get-TerminalSession -Detailed

# Get client connection details
Get-TerminalSession | Get-TerminalClientInfo

# Log off a disconnected session
Get-TerminalSession | Where-Object State -eq Disconnected | Remove-TerminalSession
```

see docs folder for more examples

## Building

```powershell
.\build.ps1
```
