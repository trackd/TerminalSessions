---
external help file: TerminalSessions.dll-Help.xml
Module Name: TerminalSessions
online version: https://github.com/trackd/TerminalSessions/blob/main/docs/en-US/Send-WTSMessage.md
schema: 2.0.0
---

# Send-WTSMessage

## SYNOPSIS

Sends a popup message to a Windows Terminal Services session on a specified computer.  

## SYNTAX

```powershell
# Parameter Set: ByManual
Send-WTSMessage -ComputerName <string> -SessionId <uint> -Title <string> -Body <string> [-TimeoutSeconds <int>] [-Type <MessageBoxType>] [<CommonParameters>]

# Parameter Set: BySessionInfo (pipeline)
Send-WTSMessage -SessionInfo <SessionInfo> -Title <string> -Body <string> [-TimeoutSeconds <int>] [-Type <MessageBoxType>] [<CommonParameters>]
```

## DESCRIPTION

The Send-WTSMessage cmdlet sends a popup message dialog to a user session on a local or remote Windows Terminal Services host. You can specify the target session either by computer name and session ID, or by passing a SessionInfo object from the pipeline. The dialog can be customized with a title, body, timeout, and button style. If a MessageBoxType is specified, the user's response is returned.  

## EXAMPLES

### Example 1: Send a simple message to a session

```powershell
Send-WTSMessage -ComputerName "SERVER01" -SessionId 5 -Title "Maintenance" -Body "Your session will be logged off in 10 minutes."
```

### Example 2: Send a message with a response dialog

```powershell
Send-WTSMessage -ComputerName "SERVER01" -SessionId 5 -Title "Confirm" -Body "Do you wish to continue?" -Type BUTTON_YESNO
```

### Example 3: Send a message using a SessionInfo object from the pipeline

```powershell
Get-WTSSession -ComputerName "SERVER01" | Where-Object UserName -eq "jdoe" | Send-WTSMessage -Title "Alert" -Body "Please save your work."
```

## PARAMETERS

### -SessionInfo

Specifies the session info object to receive the message. Used in BySessionInfo parameter set, accepts pipeline input.  

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

Specifies the session ID to receive the message. Required for ByManual parameter set.  

```yaml
Type: uint
Parameter Set: ByManual
Required: True
Position: 1
Accept pipeline input: False
Accept wildcard characters: False
```

### -Title

The title of the message dialog.  

```yaml
Type: String
Parameter Set: All
Required: True
Position: Named
Accept pipeline input: False
Accept wildcard characters: False
```

### -Body

The body text of the message dialog.  

```yaml
Type: String
Parameter Set: All
Required: True
Position: Named
Accept pipeline input: False
Accept wildcard characters: False
```

### -Type

MessageBoxType for getting a response from the message. Optional; if not specified, a simple OK dialog is shown.  

```yaml
Type: MessageBoxType
Parameter Set: All
Required: False
Position: Named
Default Value: BUTTON_OK
Accept pipeline input: False
Accept wildcard characters: False
```

### -TimeoutSeconds

Timeout for the message (in seconds). Optional; defaults to 60 seconds.  

```yaml
Type: int
Parameter Set: All
Required: False
Position: Named
Default Value: 60
Accept pipeline input: False
Accept wildcard characters: False
```

## INPUTS

### TerminalSessions.SessionInfo

You can pipe SessionInfo objects to this cmdlet.  

## OUTPUTS

If neither -Type nor -TimeoutSeconds is specified, the cmdlet returns nothing (void) and writes a verbose message for troubleshooting.  

If -Type or -TimeoutSeconds is specified, the cmdlet returns a MessageBoxResult indicating the user's response.  

## NOTES

## RELATED LINKS

[Get-WTSSession](Get-WTSSession.md)  
[Remove-WTSSession](Remove-WTSSession.md)  
