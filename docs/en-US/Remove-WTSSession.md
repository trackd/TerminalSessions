---
external help file: TerminalSessions.dll-Help.xml
Module Name: TerminalSessions
online version: https://github.com/trackd/TerminalSessions/blob/main/docs/en-US/Remove-WTSSession.md
schema: 2.0.0
---

# Remove-WTSSession

## SYNOPSIS

Removes (logs off) a session from a Windows host.  

## SYNTAX

```powershell
Remove-WTSSession [-SessionInfo] <SessionInfo> [-WaitForLogoff] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION

The Remove-WTSSession cmdlet logs off a user session on a Windows Terminal Services host using the WTSLogoffSession API. This cmdlet supports ShouldProcess, so it will prompt for confirmation before logging off sessions. The confirmation impact is set to High due to the disruptive nature of logging off users.  

## EXAMPLES

### Example 1: Log off a specific session

```powershell
Get-WTSSession | Where-Object UserName -eq "john.doe" | Remove-WTSSession
```

This command logs off all sessions for user "john.doe" after prompting for confirmation.  

### Example 2: Log off all disconnected sessions

```powershell
Get-WTSSession | Where-Object State -eq Disconnected | Remove-WTSSession -Confirm:$false
```

This command logs off all disconnected sessions without prompting for confirmation.  

### Example 3: Log off and wait for completion

```powershell
$session = Get-WTSSession | Where-Object SessionId -eq 5
Remove-WTSSession -SessionInfo $session -WaitForLogoff
```

This command logs off session ID 5 and waits for the logoff operation to complete before continuing.  

### Example 4: Preview what would be logged off

```powershell
Get-WTSSession | Where-Object State -ne Active | Remove-WTSSession -WhatIf
```

This command shows what sessions would be logged off without actually performing the logoff operation.  

### Example 5: Force log off multiple sessions

```powershell
Get-WTSSession -ComputerName "Server01" | Where-Object {$_.IdleTime -gt (New-TimeSpan -Hours 8)} | Remove-WTSSession -Confirm:$false
```

This command forcefully logs off all sessions on Server01 that have been idle for more than 8 hours.  

## PARAMETERS

### -SessionInfo

Specifies the session information object to log off. This parameter accepts SessionInfo objects from Get-WTSSession and is mandatory. The object must contain the ComputerName, SessionId, UserName, and State properties.  

```yaml
Type: SessionInfo
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -ComputerName

ComputerName to connect to (requires SessionId)  

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

Session to logoff.  

```yaml
Type: uint
Parameter Sets: ByManual
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -WaitForLogoff

When specified, the cmdlet waits for the logoff operation to complete before returning. Without this switch, the logoff is initiated asynchronously and the cmdlet returns immediately.  

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

## INPUTS

### TerminalSessions.SessionInfo

You can pipe SessionInfo objects from Get-WTSSession to this cmdlet.  

## OUTPUTS

### System.Management.Automation.PSObject

Returns a custom object containing:

- SessionId: The session ID that was logged off
- UserName: The user name of the logged off session
- ComputerName: The computer where the session was logged off
- State: The state of the session before logoff
- Result: Boolean indicating success (True) or failure (False)

## NOTES

This cmdlet requires administrative privileges to log off sessions. Use with caution as it will forcefully disconnect users and may result in data loss if users have unsaved work. The cmdlet prompts for confirmation by default to prevent accidental logoffs.  

## RELATED LINKS

[Get-WTSSession](Get-WTSSession.md)  
[Get-WTSInfo](Get-WTSInfo.md)  
[Get-WTSClientInfo](Get-WTSClientInfo.md)  
