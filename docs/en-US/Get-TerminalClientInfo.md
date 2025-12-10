---
external help file: TerminalSessions.dll-Help.xml
Module Name: TerminalSessions
online version: https://github.com/trackd/TerminalSessions/blob/main/docs/en-US/Get-TerminalClientInfo.md
schema: 2.0.0
---

# Get-TerminalClientInfo

## SYNOPSIS

Retrieves client information for a session on a Windows host.

## SYNTAX

### BySessionInfo (Default)

```
Get-TerminalClientInfo -SessionInfo <SessionInfo> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### ByManual

```
Get-TerminalClientInfo -ComputerName <String> -SessionId <UInt32> [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION

The Get-TerminalClientInfo cmdlet retrieves detailed client connection information for a specific Terminal Services session using the WTSQueryClientInfo API.
This includes client computer details, display settings, encryption level, network information, and device identifiers.

## EXAMPLES

### Example 1: Get client info for all remote sessions

```powershell
PS C:\> Get-TerminalSession | Where-Object ClientName -ne $null | Get-TerminalClientInfo
```

This command retrieves client information for all remote desktop sessions (excluding console sessions).

### Example 2: Get client info for a specific user

```powershell
PS C:\> $session = Get-TerminalSession | Where-Object UserName -eq "admin"
PS C:\> Get-TerminalClientInfo -SessionInfo $session
```

This command retrieves client information for all remote desktop sessions (excluding console sessions).

### Example 3: View client display resolution

```powershell
PS C:\> Get-TerminalSession | Get-TerminalClientInfo | Select-Object ClientName, HRes, VRes, ColorDepth
```

### Example 3: Check client IP addresses

```powershell
PS C:\> Get-TerminalSession | Get-TerminalClientInfo | Select-Object UserName, ClientName, ClientAddress
```

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

### TerminalSessions.ClientInfo

## NOTES

## RELATED LINKS
