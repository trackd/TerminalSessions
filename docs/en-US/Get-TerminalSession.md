---
external help file: TerminalSessions.dll-Help.xml
Module Name: TerminalSessions
online version: https://github.com/trackd/TerminalSessions/blob/main/docs/en-US/Get-TerminalSession.md
schema: 2.0.0
---

# Get-TerminalSession

## SYNOPSIS

Enumerates sessions on Windows Terminal Services hosts.

## SYNTAX

### SessionInfo (Default)

```
Get-TerminalSession [[-ComputerName] <String[]>] [-OnlineOnly] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

### SessionInfoExtra

```
Get-TerminalSession [[-ComputerName] <String[]>] [-Detailed] [-OnlineOnly] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION

The Get-TerminalSession cmdlet retrieves all active and disconnected sessions on one or more Windows Terminal Services hosts.
It uses the WTSEnumerateSessionsExW API to query session information including session ID, state, user name, domain, and client information.

## EXAMPLES

### Example 1: Get all sessions on the local computer

```powershell
PS C:\> Get-TerminalSession
```

This command retrieves all sessions on the local computer.  

### Example 2: Get sessions from multiple computers

```powershell
PS C:\> Get-TerminalSession -ComputerName "Server01", "Server02"
```

This command retrieves sessions from two remote computers named Server01 and Server02.  

### Example 3: Get sessions with detailed information

```powershell
PS C:\> Get-TerminalSession -Detailed
```

This command retrieves all sessions on the local computer with additional details including idle time and logon time.  

### Example 4: Get sessions from pipeline

```powershell
PS C:\> "Server01", "Server02", "Server03" | Get-TerminalSession
```

This command retrieves sessions from multiple computers passed through the pipeline.  

### Example 5: Filter disconnected sessions

```powershell
PS C:\> Get-TerminalSession | Where-Object State -eq Disconnected
```

This command retrieves all disconnected sessions on the local computer.

## PARAMETERS

### -ComputerName

Specifies one or more computer names to query for sessions.
If not specified or set to an empty array, the cmdlet queries the local computer.
You can specify multiple computer names separated by commas or pass them through the pipeline.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -Detailed

When specified, retrieves additional session details including idle time and logon time.
This provides more comprehensive information about each session but may take slightly longer to execute.

```yaml
Type: SwitchParameter
Parameter Sets: SessionInfoExtra
Aliases: Details

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OnlineOnly

filters out disconnected sessions

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

## OUTPUTS

### TerminalSessions.SessionInfoExtra

### TerminalSessions.SessionInfo

## NOTES

## RELATED LINKS
