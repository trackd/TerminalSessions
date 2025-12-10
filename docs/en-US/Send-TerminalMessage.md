---
external help file: TerminalSessions.dll-Help.xml
Module Name: TerminalSessions
online version: https://github.com/trackd/TerminalSessions/blob/main/docs/en-US/Send-TerminalMessage.md
schema: 2.0.0
---

# Send-TerminalMessage

## SYNOPSIS

Sends a popup message to a Windows Terminal Services session on a specified computer.

## SYNTAX

### SessionInfo (Default)

```
Send-TerminalMessage [-Title] <String> [-Body] <String> [-Type <MessageBoxType>] [-TimeoutSeconds <Int32>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### BySessionInfo

```
Send-TerminalMessage -SessionInfo <SessionInfo> [-Title] <String> [-Body] <String> [-Type <MessageBoxType>]
 [-TimeoutSeconds <Int32>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### ByManual

```
Send-TerminalMessage [-ComputerName] <String> [-SessionId] <UInt32> [-Title] <String> [-Body] <String>
 [-Type <MessageBoxType>] [-TimeoutSeconds <Int32>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION

The Send-TerminalMessage cmdlet sends a popup message dialog to a user session on a local or remote Windows Terminal Services host.
You can specify the target session either by computer name and session ID, or by passing a SessionInfo object from the pipeline.
The dialog can be customized with a title, body, timeout, and button style.
If a MessageBoxType is specified, the user's response is returned.

## EXAMPLES

### Example 1: Send a simple message to a session

```powershell
PS C:\> Send-TerminalMessage -ComputerName "SERVER01" -SessionId 5 -Title "Maintenance" -Body "Your session will be logged off in 10 minutes."
```

### Example 2: Send a message with a response dialog

```powershell
PS C:\> Send-TerminalMessage -ComputerName "SERVER01" -SessionId 5 -Title "Confirm" -Body "Do you wish to continue?" -Type BUTTON_YESNO
```

### Example 3: Send a message using a SessionInfo object from the pipeline

```powershell
PS C:\> Get-TerminalSession -ComputerName "SERVER01" | Where-Object UserName -eq "jdoe" | Send-TerminalMessage -Title "Alert" -Body "Please save your work."
```

## PARAMETERS

### -Body

The body text of the message dialog.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 3
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ComputerName

Specifies the target computer name.
Required for ByManual parameter set.

```yaml
Type: String
Parameter Sets: ByManual
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -SessionId

Specifies the session ID to receive the message.
Required for ByManual parameter set.

```yaml
Type: UInt32
Parameter Sets: ByManual
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -SessionInfo

Specifies the session info object to receive the message.
Used in BySessionInfo parameter set, accepts pipeline input.

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

### -TimeoutSeconds

Timeout for the message (in seconds).
Optional; defaults to 60 seconds.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Title

The title of the message dialog.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 2
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Type

MessageBoxType for getting a response from the message.
Optional; if not specified, a simple OK dialog is shown.

```yaml
Type: MessageBoxType
Parameter Sets: (All)
Aliases:
Accepted values: DEFAULT_BUTTON1, BUTTON_OK, BUTTON_OKCANCEL, BUTTON_ABORTRETRYIGNORE, BUTTON_YESNOCANCEL, BUTTON_YESNO, BUTTON_RETRYCANCEL, BUTTON_CANCELTRYCONTINUE, ICON_STOP, ICON_QUESTION, ICON_EXCLAMATION, ICON_INFORMATION, DEFAULT_BUTTON2, DEFAULT_BUTTON3, DEFAULT_BUTTON4, BUTTON_HELP, MB_SETFOREGROUND, MB_DEFAULT_DESKTOP_ONLY, MB_TOPMOST, MB_RIGHT, MB_RTLREADING, MB_SERVICE_NOTIFICATION

Required: False
Position: Named
Default value: None
Accept pipeline input: False
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
