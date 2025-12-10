---
external help file: TerminalSessions.dll-Help.xml
Module Name: TerminalSessions
online version: https://github.com/trackd/TerminalSessions/blob/main/docs/en-US/Get-TerminalInfo.md
schema: 2.0.0
---

# Get-TerminalInfo

## SYNOPSIS

Retrieves detailed WTS information for a session on a Windows host.

## SYNTAX

### BySessionInfo (Default)

```
Get-TerminalInfo -SessionInfo <SessionInfo> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### ByManual

```
Get-TerminalInfo -ComputerName <String> -SessionId <UInt32> [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION

The Get-TerminalInfo cmdlet retrieves detailed Windows Terminal Services information for a specific session using the WTSQueryTerminalInfo API.
This includes connection state, byte and frame statistics, timing information, and session metadata.

## EXAMPLES

### Example 1: Get detailed info for all sessions

```powershell
PS C:\> Get-TerminalSession | Get-TerminalInfo
```

This command retrieves detailed information for all sessions on the local computer by piping session objects to Get-TerminalInfo.

### Example 2: Get info for a specific session

```powershell
PS C:\> $session = Get-TerminalSession | Where-Object UserName -eq "john.doe"
PS C:\> Get-TerminalInfo -SessionInfo $session
```

### Example 3: View session timing information

```powershell
PS C:\> Get-TerminalSession | Get-TerminalInfo | Select-Object UserName, LogonTime, ConnectTime, LastInputTime
```

This command retrieves timing information for all sessions, showing when users logged on, connected, and last provided input.  

### Example 4: Check session bandwidth usage

```powershell
PS C:\> Get-TerminalSession | Get-TerminalInfo | Select-Object UserName, IncomingBytes, OutgoingBytes, @{N="TotalMB";E={($_.IncomingBytes + $_.OutgoingBytes)/1MB}}
```

This command displays bandwidth usage statistics for all sessions, calculating the total data transferred in megabytes.  

## PARAMETERS

### -ComputerName

ComputerName to connect to (requires SessionId)

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

SessionId to get information about.

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

### TerminalSessions.WTSInfo

## NOTES

## RELATED LINKS
