---
external help file: TerminalSessions.dll-Help.xml
Module Name: TerminalSessions
online version: https://github.com/trackd/TerminalSessions/blob/main/docs/en-US/Remove-TerminalSession.md
schema: 2.0.0
---

# Remove-TerminalSession

## SYNOPSIS

Removes (logs off) a session from a Windows host.

## SYNTAX

### BySessionInfo (Default)

```
Remove-TerminalSession -SessionInfo <SessionInfo> [-WaitForLogoff] [-ProgressAction <ActionPreference>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### ByManual

```
Remove-TerminalSession -ComputerName <String> -SessionId <UInt32> [-WaitForLogoff]
 [-ProgressAction <ActionPreference>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION

The Remove-TerminalSession cmdlet logs off a user session on a Windows Terminal Services host using the WTSLogoffSession API.
This cmdlet supports ShouldProcess, so it will prompt for confirmation before logging off sessions.
The confirmation impact is set to High due to the disruptive nature of logging off users.

## EXAMPLES

### Example 1: Log off a specific session

```powershell
PS C:\> Get-TerminalSession | Where-Object UserName -eq "john.doe" | Remove-TerminalSession
```

This command logs off all sessions for user "john.doe" after prompting for confirmation.  

### Example 2: Log off all disconnected sessions

```powershell
PS C:\> Get-TerminalSession | Where-Object State -eq Disconnected | Remove-TerminalSession -Confirm:$false
```

This command logs off all disconnected sessions without prompting for confirmation.  

### Example 3: Log off and wait for completion

```powershell
PS C:\> $session = Get-TerminalSession | Where-Object SessionId -eq 5
PS C:\> Remove-TerminalSession -SessionInfo $session -WaitForLogoff
```

This command logs off session ID 5 and waits for the logoff operation to complete before continuing.  

### Example 4: Preview what would be logged off

```powershell
PS C:\> Get-TerminalSession | Where-Object State -ne Active | Remove-TerminalSession -WhatIf
```

This command shows what sessions would be logged off without actually performing the logoff operation.  

### Example 5: Force log off multiple sessions

```powershell
PS C:\> Get-TerminalSession -ComputerName "Server01" | Where-Object {$_.IdleTime -gt (New-TimeSpan -Hours 8)} | Remove-TerminalSession -Confirm:$false
```

This command forcefully logs off all sessions on Server01 that have been idle for more than 8 hours.  

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

### -Confirm

Prompts you for confirmation before running the cmdlet.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: cf

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SessionId

Specifies the session information object to log off.
This parameter accepts SessionInfo objects from Get-TerminalSession and is mandatory.
The object must contain the ComputerName, SessionId, UserName, and State properties.

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

### -WaitForLogoff

When specified, the cmdlet waits for the logoff operation to complete before returning.
Without this switch, the logoff is initiated asynchronously and the cmdlet returns immediately.

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

### -WhatIf

Shows what would happen if the cmdlet runs.
The cmdlet is not run.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: wi

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
