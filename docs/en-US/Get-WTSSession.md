---
external help file: TerminalSessions.dll-Help.xml
Module Name: TerminalSessions
online version:
schema: 2.0.0
---

# Get-WTSSession

## SYNOPSIS

Enumerates sessions on Windows Terminal Services hosts.  

## SYNTAX

```
Get-WTSSession [[-ComputerName] <String[]>] [-WithDetails] [<CommonParameters>]
```

## DESCRIPTION

The Get-WTSSession cmdlet retrieves all active and disconnected sessions on one or more Windows Terminal Services hosts. It uses the WTSEnumerateSessionsExW API to query session information including session ID, state, user name, domain, and client information.  

## EXAMPLES

### Example 1: Get all sessions on the local computer

```powershell
Get-WTSSession
```

This command retrieves all sessions on the local computer.  

### Example 2: Get sessions from multiple computers

```powershell
Get-WTSSession -ComputerName "Server01", "Server02"
```

This command retrieves sessions from two remote computers named Server01 and Server02.  

### Example 3: Get sessions with detailed information

```powershell
Get-WTSSession -Detailed
```

This command retrieves all sessions on the local computer with additional details including idle time and logon time.  

### Example 4: Get sessions from pipeline

```powershell
"Server01", "Server02", "Server03" | Get-WTSSession
```

This command retrieves sessions from multiple computers passed through the pipeline.  

### Example 5: Filter disconnected sessions

```powershell
Get-WTSSession | Where-Object State -eq Disconnected
```

This command retrieves all disconnected sessions on the local computer.  

## PARAMETERS

### -ComputerName

Specifies one or more computer names to query for sessions. If not specified or set to an empty array, the cmdlet queries the local computer. You can specify multiple computer names separated by commas or pass them through the pipeline.  

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: Local computer name
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -Detailed

When specified, retrieves additional session details including idle time and logon time. This provides more comprehensive information about each session but may take slightly longer to execute.  

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).  

## INPUTS

### System.String[]

You can pipe computer names to this cmdlet.  

## OUTPUTS

### PSWTSSession.SessionInfo

Returns SessionInfo objects by default, containing session ID, state, session name, user name, domain name, computer name, and client name.  

### PSWTSSession.SessionInfoExtra

Returns SessionInfoExtra objects when -Detailed is specified, containing all SessionInfo properties plus idle time and logon time.  

## NOTES

This cmdlet requires administrative privileges to query remote computers. Ensure you have the necessary permissions before running this cmdlet against remote systems.  

## RELATED LINKS

[Get-WTSInfo](Get-WTSInfo.md)  
[Get-WTSClientInfo](Get-WTSClientInfo.md)  
[Remove-WTSSession](Remove-WTSSession.md)  
