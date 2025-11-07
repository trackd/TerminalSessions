---
external help file: TerminalSessions.dll-Help.xml
Module Name: TerminalSessions
online version:
schema: 2.0.0
---

# Get-WTSInfo

## SYNOPSIS

Retrieves detailed WTS information for a session on a Windows host.  

## SYNTAX

```
Get-WTSInfo [-SessionInfo] <SessionInfo> [<CommonParameters>]
```

## DESCRIPTION

The Get-WTSInfo cmdlet retrieves detailed Windows Terminal Services information for a specific session using the WTSQueryWTSINFO API. This includes connection state, byte and frame statistics, timing information, and session metadata.  

## EXAMPLES

### Example 1: Get detailed info for all sessions

```powershell
Get-WTSSession | Get-WTSInfo
```

This command retrieves detailed information for all sessions on the local computer by piping session objects to Get-WTSInfo.  

### Example 2: Get info for a specific session

```powershell
$session = Get-WTSSession | Where-Object UserName -eq "john.doe"
Get-WTSInfo -SessionInfo $session
```

This command retrieves detailed information for a specific user's session.  

### Example 3: View session timing information

```powershell
Get-WTSSession | Get-WTSInfo | Select-Object UserName, LogonTime, ConnectTime, LastInputTime
```

This command retrieves timing information for all sessions, showing when users logged on, connected, and last provided input.  

### Example 4: Check session bandwidth usage

```powershell
Get-WTSSession | Get-WTSInfo | Select-Object UserName, IncomingBytes, OutgoingBytes, @{N="TotalMB";E={($_.IncomingBytes + $_.OutgoingBytes)/1MB}}
```

This command displays bandwidth usage statistics for all sessions, calculating the total data transferred in megabytes.  

## PARAMETERS

### -SessionInfo

Specifies the session information object to query. This parameter accepts SessionInfo objects from Get-WTSSession and is mandatory. The object must contain the ComputerName and SessionId properties.  

```yaml
Type: SessionInfo
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).  

## INPUTS

### PSWTSSession.SessionInfo

You can pipe SessionInfo objects from Get-WTSSession to this cmdlet.  

## OUTPUTS

### PSWTSSession.WTSInfo

Returns WTSInfo objects containing detailed session information including:

- State: Connection state
- SessionId: Unique session identifier
- IncomingBytes/OutgoingBytes: Data transfer statistics
- IncomingFrames/OutgoingFrames: Frame count statistics
- IncomingCompressedBytes/OutgoingCompressedBy: Compression statistics
- WinStationName: Name of the window station
- Domain: User's domain
- UserName: User's name
- ConnectTime: When the session connected
- DisconnectTime: When the session disconnected (if applicable)
- LastInputTime: When the user last provided input
- LogonTime: When the user logged on
- CurrentTime: Current system time

## NOTES

This cmdlet requires administrative privileges to query sessions. Some timing information may be null if not applicable to the session state.  

## RELATED LINKS

[Get-WTSSession](Get-WTSSession.md)  
[Get-WTSClientInfo](Get-WTSClientInfo.md)  
[Remove-WTSSession](Remove-WTSSession.md)  
