---
external help file: TerminalSessions.dll-Help.xml
Module Name: TerminalSessions
online version:
schema: 2.0.0
---

# Get-WTSClientInfo

## SYNOPSIS

Retrieves client information for a session on a Windows host.  

## DESCRIPTION

The Get-WTSClientInfo cmdlet retrieves detailed client connection information for a specific Terminal Services session using the WTSQueryClientInfo API. This includes client computer details, display settings, encryption level, network information, and device identifiers.  

## SYNTAX

```
Get-WTSClientInfo [-SessionInfo] <SessionInfo> [<CommonParameters>]
```

## EXAMPLES

### Example 1: Get client info for all remote sessions

```powershell
Get-WTSSession | Where-Object ClientName -ne $null | Get-WTSClientInfo
```

This command retrieves client information for all remote desktop sessions (excluding console sessions).  

### Example 2: Get client info for a specific user

```powershell
$session = Get-WTSSession | Where-Object UserName -eq "admin"
Get-WTSClientInfo -SessionInfo $session
```

This command retrieves client connection details for a specific user's session.  

### Example 3: View client display resolution

```powershell
Get-WTSSession | Get-WTSClientInfo | Select-Object ClientName, HRes, VRes, ColorDepth
```

This command displays the screen resolution and color depth settings for all client connections.  

### Example 4: Check client IP addresses

```powershell
Get-WTSSession | Get-WTSClientInfo | Select-Object UserName, ClientName, ClientAddress
```

This command shows the IP address of each connected client.  

### Example 5: View encryption levels

```powershell
Get-WTSSession | Get-WTSClientInfo | Select-Object UserName, EncryptionLevel, ClientAddressFamily
```

This command displays the encryption level and network protocol for each connection.  

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

### PSWTSSession.ClientInfo

Returns ClientInfo objects containing detailed client information including:

- ClientName: Name of the client computer
- Domain: Client's domain
- UserName: User name on the client
- WorkDirectory: Working directory
- InitialProgram: Initial program to run
- EncryptionLevel: Connection encryption level
- ClientAddressFamily: Network protocol (IPv4/IPv6)
- ClientAddress: IP address of the client
- HRes/VRes: Horizontal and vertical display resolution
- ColorDepth: Color depth in bits per pixel
- ClientDirectory: Client directory path
- ClientBuildNumber: Client OS build number
- ClientHardwareId: Hardware identifier
- ClientProductId: Product identifier
- OutBufCountHost/OutBufCountClient: Output buffer counts
- OutBufLength: Output buffer length
- DeviceId: Device identifier

## NOTES

This cmdlet requires administrative privileges to query session information. The ClientAddress property may be null for console sessions or if the connection doesn't provide IP information.  

## RELATED LINKS

[Get-WTSSession](Get-WTSSession.md)  
[Get-WTSInfo](Get-WTSInfo.md)  
[Remove-WTSSession](Remove-WTSSession.md)  
