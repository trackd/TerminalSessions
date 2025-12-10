---
external help file: TerminalSessions.dll-Help.xml
Module Name: TerminalSessions
online version: https://github.com/trackd/TerminalSessions/blob/main/docs/en-US/Disconnect-TerminalSession.md
schema: 2.0.0
---

# Disconnect-TerminalSession

## SYNOPSIS

Disconnects a user session from a Windows Terminal Services host without logging off the user.

## SYNTAX

### BySessionInfo (Default)

```
Disconnect-TerminalSession -SessionInfo <SessionInfo> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### ByManual

```
Disconnect-TerminalSession -ComputerName <String> -SessionId <UInt32> [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION

The Disconnect-TerminalSession cmdlet disconnects a user session on a local or remote Windows Terminal Services host.
Disconnecting a session leaves the user logged in, but their session is no longer active on the client.
You can specify the target session either by passing a SessionInfo object from the pipeline or by providing the computer name and session ID manually.

## EXAMPLES

### Example 1: Disconnect a session by manual parameters

```powershell
PS C:\> Disconnect-TerminalSession -ComputerName "SERVER01" -SessionId 5
```

Disconnect SessionId 5 on SERVER01

### Example 2: Disconnect a session using a SessionInfo object from the pipeline

```powershell
PS C:\> Get-TerminalSession -ComputerName "SERVER01" | Where-Object UserName -eq "jdoe" | Disconnect-TerminalSession
```

Disconnects the session for user "jdoe" on SERVER01.

## PARAMETERS

### -ComputerName

Specifies the target computer name.
Required for ByManual parameter set.

```yaml
Type: String
Parameter Sets: ByManual
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -SessionId

SessionId to disconnect.

```yaml
Type: UInt32
Parameter Sets: ByManual
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -SessionInfo

Specifies the session information object to query.
This parameter accepts SessionInfo objects from Get-TerminalSession and is mandatory.
The object must contain the ComputerName and SessionId properties.

```yaml
Type: SessionInfo
Parameter Sets: BySessionInfo
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### TerminalSessions.SessionInfo

### System.String

### System.UInt32

## OUTPUTS

### System.Object

## NOTES

## RELATED LINKS
