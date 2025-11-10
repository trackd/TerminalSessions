---
external help file: TerminalSessions.dll-Help.xml
Module Name: TerminalSessions
online version: https://github.com/trackd/TerminalSessions/blob/main/docs/en-US/Disconnect-WTSSession.md
schema: 2.0.0
---

# Disconnect-WTSSession

## SYNOPSIS

Disconnects a user session from a Windows Terminal Services host without logging off the user.  

## SYNTAX

```powershell
# Parameter Set: BySessionInfo (pipeline)
Disconnect-WTSSession -SessionInfo <SessionInfo> [<CommonParameters>""]

# Parameter Set: ByManual
Disconnect-WTSSession -ComputerName <string> -SessionId <uint> [<CommonParameters>""]
```

## DESCRIPTION

The Disconnect-WTSSession cmdlet disconnects a user session on a local or remote Windows Terminal Services host. Disconnecting a session leaves the user logged in, but their session is no longer active on the client. You can specify the target session either by passing a SessionInfo object from the pipeline or by providing the computer name and session ID manually.  

## EXAMPLES

### Example 1: Disconnect a session by manual parameters

```powershell
Disconnect-WTSSession -ComputerName "SERVER01" -SessionId 5
```

Disconnects session ID 5 on SERVER01, leaving the user logged in but disconnected.  

### Example 2: Disconnect a session using a SessionInfo object from the pipeline

```powershell
Get-WTSSession -ComputerName "SERVER01" | Where-Object UserName -eq "jdoe" | Disconnect-WTSSession
```

Disconnects the session for user "jdoe" on SERVER01.  

## PARAMETERS

### -SessionInfo

Specifies the session info object to disconnect. Used in BySessionInfo parameter set, accepts pipeline input.  

```yaml
Type: SessionInfo
Parameter Set: BySessionInfo
Required: True
Position: 0
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -ComputerName

Specifies the target computer name. Required for ByManual parameter set.  

```yaml
Type: String
Parameter Set: ByManual
Required: True
Position: 0
Accept pipeline input: False
Accept wildcard characters: False
```

### -SessionId

Specifies the session ID to disconnect. Required for ByManual parameter set.  

```yaml
Type: UInt32
Parameter Set: ByManual
Required: True
Position: 1
Accept pipeline input: False
Accept wildcard characters: False
```

## INPUTS

### SessionInfo

You can pipe SessionInfo objects to this cmdlet.  

## OUTPUTS

### System.Boolean

Returns True if the session was successfully disconnected, otherwise False. Verbose output provides details for troubleshooting.  

## NOTES

- Requires administrative privileges on the target computer.
- Disconnecting a session does not log off the user; their programs continue running on the server.
- Use Remove-WTSSession to log off a session instead.

## RELATED LINKS

[Get-WTSSession](Get-WTSSession.md)  
[Remove-WTSSession](Remove-WTSSession.md)  
[Send-WTSMessage](Send-WTSMessage.md)  
