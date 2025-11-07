---
Module Name: TerminalSessions
Module Guid: 6d76a8aa-b182-43e0-9663-1e8f3e514614
Download Help Link:
Help Version: 0.1.0
Locale: en-US
---

# TerminalSessions Module

## Description

The TerminalSessions module provides cmdlets for managing Windows Terminal Services (WTS) sessions. It enables administrators to query session information, retrieve detailed session and client data, and manage user sessions on local and remote computers.  

This module uses native Windows API calls (WTSEnumerateSessionsExW, WTSQuerySessionInformation, WTSQueryWTSINFO, WTSLogoffSession) to provide comprehensive session management capabilities.  

## TerminalSessions Cmdlets

### [Get-WTSClientInfo](Get-WTSClientInfo.md)

Retrieves detailed client connection information for a Terminal Services session, including display settings, IP address, encryption level, and device details.  

### [Get-WTSInfo](Get-WTSInfo.md)

Retrieves detailed Windows Terminal Services information for a session, including connection state, bandwidth statistics, and timing information.  

### [Get-WTSSession](Get-WTSSession.md)

Enumerates all sessions on one or more Windows Terminal Services hosts, returning session ID, state, user information, and optionally extended details like idle time.  

### [Remove-WTSSession](Remove-WTSSession.md)

Logs off a user session from a Windows Terminal Services host with optional confirmation prompts and wait behavior.  
