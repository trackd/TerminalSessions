using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using TerminalSessions.Native;

namespace TerminalSessions;

/// <summary>
/// High-level wrapper for Windows Terminal Services (WTS) API
/// Provides managed access to WTS session management and querying
/// </summary>
public static class WtsNative
{
    /// <summary>
    /// Represents the local terminal server handle
    /// </summary>
    public static readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

    /// <summary>
    /// Constant representing any session ID
    /// </summary>
    public const uint WTS_ANY_SESSION = 0xFFFFFFFF;

    #region Server Connection Management

    /// <summary>
    /// Opens a handle to the specified terminal server
    /// </summary>
    /// <param name="serverName">The name of the terminal server</param>
    /// <returns>A handle to the terminal server</returns>
    /// <exception cref="Win32Exception">Thrown when unable to open server connection</exception>
    public static IntPtr WTSOpenServerEx(string serverName)
    {
      IntPtr handle = WtsInterop.WTSOpenServerExW(serverName);
      if (handle == IntPtr.Zero)
      {
        throw new Win32Exception();
      }
      return handle;
    }

    /// <summary>
    /// Closes an open handle to a terminal server
    /// </summary>
    /// <param name="hServer">The server handle to close</param>
    public static void WTSCloseServer(IntPtr hServer)
    {
      WtsInterop.WTSCloseServer(hServer);
    }

    /// <summary>
    /// Logs off a specified terminal services session
    /// </summary>
    /// <param name="hServer">Handle to the terminal server</param>
    /// <param name="SessionId">The session ID to log off</param>
    /// <param name="bWait">Whether to wait for the logoff to complete</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool WTSLogoffSession(IntPtr hServer, uint SessionId, bool bWait)
    {
      return WtsInterop.WTSLogoffSession(hServer, SessionId, bWait);
    }

    #endregion

    #region Session Enumeration

    /// <summary>
    /// Enumerates terminal services sessions on the specified server
    /// </summary>
    /// <param name="serverHandle">Handle to the terminal server</param>
    /// <param name="serverName">Name of the server for result population</param>
    /// <returns>Array of SessionInfo objects</returns>
    /// <exception cref="Win32Exception">Thrown when enumeration fails</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected API level is returned</exception>
    public static SessionInfo[] WTSEnumerateSessionsExtra(IntPtr serverHandle, string serverName)
    {
      int level = 1;

      if (!WtsInterop.WTSEnumerateSessionsExW(serverHandle, ref level, 0, out IntPtr rawInfo, out int count))
      {
        throw new Win32Exception();
      }

      if (level != 1)
      {
        throw new InvalidOperationException($"Expected level 1 but got level {level}");
      }

      if (rawInfo == IntPtr.Zero)
      {
        return [];
      }

      try
      {
        List<SessionInfo> sessions = new List<SessionInfo>(count);
        int structSize = Marshal.SizeOf<WtsStructures.WTS_SESSION_INFO_1W>();
        IntPtr currentOffset = rawInfo;

        for (int i = 0; i < count; i++)
        {
          WtsStructures.WTS_SESSION_INFO_1W info = Marshal.PtrToStructure<WtsStructures.WTS_SESSION_INFO_1W>(currentOffset);

          // Skip sessions without a user
          if (info.pUserName is null)
          {
            currentOffset = IntPtr.Add(currentOffset, structSize);
            continue;
          }

          // Only query client name for remote, connected sessions
          string? clientName = null;
          if (WtsHelpers.ShouldQueryClientName(info.State, info.pSessionName))
          {
            clientName = QuerySessionInfoString(serverHandle, info.SessionId, WTS_INFO_CLASS.WTSClientName);
          }

          sessions.Add(new SessionInfo
          {
            SessionId = info.SessionId,
            State = info.State,
            SessionName = info.pSessionName ?? string.Empty,
            UserName = info.pUserName,
            DomainName = info.pDomainName ?? string.Empty,
            ComputerName = serverName,
            ClientName = clientName,
          });

          currentOffset = IntPtr.Add(currentOffset, structSize);
        }

        return [.. sessions];
      }
      finally
      {
        if (rawInfo != IntPtr.Zero)
        {
          WtsInterop.WTSFreeMemoryExW((int)WTS_TYPE_CLASS.WTSTypeSessionInfoLevel1, rawInfo, count);
        }
      }
    }

    /// <summary>
    /// Enumerates terminal services sessions with extended details including idle time and logon time
    /// </summary>
    /// <param name="serverHandle">Handle to the terminal server</param>
    /// <param name="serverName">Name of the server for result population</param>
    /// <returns>Array of SessionInfoExtra objects with additional timing information</returns>
    /// <exception cref="Win32Exception">Thrown when enumeration fails</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected API level is returned</exception>
    public static SessionInfoExtra[] WTSEnumerateSessionsWithDetails(IntPtr serverHandle, string serverName)
    {
      int level = 1;

      if (!WtsInterop.WTSEnumerateSessionsExW(serverHandle, ref level, 0, out IntPtr rawInfo, out int count))
      {
        throw new Win32Exception();
      }

      if (level != 1)
      {
        throw new InvalidOperationException($"Expected level 1 but got level {level}");
      }

      if (rawInfo == IntPtr.Zero)
      {
        return [];
      }

      try
      {
        List<SessionInfoExtra> sessions = new List<SessionInfoExtra>(count);
        int structSize = Marshal.SizeOf<WtsStructures.WTS_SESSION_INFO_1W>();
        IntPtr currentOffset = rawInfo;

        for (int i = 0; i < count; i++)
        {
          WtsStructures.WTS_SESSION_INFO_1W info = Marshal.PtrToStructure<WtsStructures.WTS_SESSION_INFO_1W>(currentOffset);

          // Skip sessions without a user
          if (info.pUserName is null)
          {
            currentOffset = IntPtr.Add(currentOffset, structSize);
            continue;
          }

          string? clientName = null;
          TimeSpan? idleTime = null;
          DateTime? logonTime = null;

          // Attempt to get extended session information
          try
          {
            WTSInfo? wtsInfo = WTSQueryWTSINFO(serverHandle, info.SessionId);
            if (wtsInfo is not null)
            {
              logonTime = wtsInfo.LogonTime;

              // Calculate idle time from last input
              if (wtsInfo.LastInputTime.HasValue && wtsInfo.CurrentTime.HasValue)
              {
                idleTime = wtsInfo.CurrentTime.Value - wtsInfo.LastInputTime.Value;
              }
            }
          }
          catch
          {
            // Silently ignore errors querying extended info (session may have ended)
          }

          // Only query client name for remote, connected sessions
          if (WtsHelpers.ShouldQueryClientName(info.State, info.pSessionName))
          {
            clientName = QuerySessionInfoString(serverHandle, info.SessionId, WTS_INFO_CLASS.WTSClientName);
          }

          sessions.Add(new SessionInfoExtra
          {
            SessionId = info.SessionId,
            State = info.State,
            SessionName = info.pSessionName ?? string.Empty,
            UserName = info.pUserName,
            DomainName = info.pDomainName ?? string.Empty,
            ComputerName = serverName,
            ClientName = clientName,
            IdleTime = idleTime,
            LogonTime = logonTime,
          });

          currentOffset = IntPtr.Add(currentOffset, structSize);
        }

        return [.. sessions];
      }
      finally
      {
        if (rawInfo != IntPtr.Zero)
        {
          WtsInterop.WTSFreeMemoryExW((int)WTS_TYPE_CLASS.WTSTypeSessionInfoLevel1, rawInfo, count);
        }
      }
    }

    #endregion

    #region Session Information Queries

    /// <summary>
    /// Queries client information for a specific session
    /// </summary>
    /// <param name="hServer">Handle to the terminal server</param>
    /// <param name="sessionId">The session ID to query</param>
    /// <returns>ClientInfo object with client details</returns>
    /// <exception cref="Win32Exception">Thrown when query fails</exception>
    public static ClientInfo WTSQueryClientInfo(IntPtr hServer, uint sessionId)
    {
      IntPtr buffer = IntPtr.Zero;

      try
      {
        if (!WtsInterop.WTSQuerySessionInformation(hServer, sessionId, WTS_INFO_CLASS.WTSClientInfo, out buffer, out uint bytesReturned))
        {
          throw new Win32Exception();
        }

        WtsStructures.WTSCLIENT rawClient = Marshal.PtrToStructure<WtsStructures.WTSCLIENT>(buffer);

        return new ClientInfo
        {
          ClientName = rawClient.ClientName ?? string.Empty,
          Domain = rawClient.Domain ?? string.Empty,
          UserName = rawClient.UserName ?? string.Empty,
          WorkDirectory = rawClient.WorkDirectory ?? string.Empty,
          InitialProgram = rawClient.InitialProgram ?? string.Empty,
          EncryptionLevel = rawClient.EncryptionLevel,
          ClientAddressFamily = (AddressFamily)rawClient.ClientAddressFamily,
          ClientAddress = WtsHelpers.ConvertToIPAddress(rawClient.ClientAddress, (AddressFamily)rawClient.ClientAddressFamily),
          HRes = rawClient.HRes,
          VRes = rawClient.VRes,
          ColorDepth = rawClient.ColorDepth,
          ClientDirectory = rawClient.ClientDirectory ?? string.Empty,
          ClientBuildNumber = rawClient.ClientBuildNumber,
          ClientHardwareId = rawClient.ClientHardwareId,
          ClientProductId = rawClient.ClientProductId,
          OutBufCountHost = rawClient.OutBufCountHost,
          OutBufCountClient = rawClient.OutBufCountClient,
          OutBufLength = rawClient.OutBufLength,
          DeviceId = rawClient.DeviceId ?? string.Empty,
        };
      }
      finally
      {
        if (buffer != IntPtr.Zero)
        {
          WtsInterop.WTSFreeMemory(buffer);
        }
      }
    }

    /// <summary>
    /// Queries detailed session information including timing and statistics
    /// </summary>
    /// <param name="hServer">Handle to the terminal server</param>
    /// <param name="sessionId">The session ID to query</param>
    /// <returns>WTSInfo object with detailed session information, or null if query fails</returns>
    public static WTSInfo? WTSQueryWTSINFO(IntPtr hServer, uint sessionId)
    {
      IntPtr buffer = IntPtr.Zero;

      try
      {
        if (!WtsInterop.WTSQuerySessionInformation(hServer, sessionId, WTS_INFO_CLASS.WTSSessionInfo, out buffer, out uint bytesReturned))
        {
          throw new Win32Exception();
        }

        WtsStructures.WTSINFOW rawInfo = Marshal.PtrToStructure<WtsStructures.WTSINFOW>(buffer);

        return new WTSInfo
        {
          State = rawInfo.State,
          SessionId = rawInfo.SessionId,
          IncomingBytes = rawInfo.IncomingBytes,
          OutgoingBytes = rawInfo.OutgoingBytes,
          IncomingFrames = rawInfo.IncomingFrames,
          OutgoingFrames = rawInfo.OutgoingFrames,
          IncomingCompressedBytes = rawInfo.IncomingCompressedBytes,
          OutgoingCompressedBy = rawInfo.OutgoingCompressedBytes,
          WinStationName = rawInfo.WinStationName,
          Domain = rawInfo.Domain,
          UserName = rawInfo.UserName,
          ConnectTime = WtsHelpers.ConvertFileTimeToLocal(rawInfo.ConnectTimeUTC),
          DisconnectTime = rawInfo.State == WtsConnectState.Disconnected ? WtsHelpers.ConvertFileTimeToLocal(rawInfo.DisconnectTimeUTC) : null,
          LastInputTime = WtsHelpers.ConvertFileTimeToLocal(rawInfo.LastInputTimeUTC),
          LogonTime = WtsHelpers.ConvertFileTimeToLocal(rawInfo.LogonTimeUTC),
          CurrentTime = WtsHelpers.ConvertFileTimeToLocal(rawInfo.CurrentTimeUTC),
        };
      }
      finally
      {
        if (buffer != IntPtr.Zero)
        {
          WtsInterop.WTSFreeMemory(buffer);
        }
      }
    }

    /// <summary>
    /// Queries a string value for a specific session information class
    /// </summary>
    /// <param name="hServer">Handle to the terminal server</param>
    /// <param name="sessionId">The session ID to query</param>
    /// <param name="wtsInfoClass">The information class to query</param>
    /// <returns>The string value, or null if query fails</returns>
    public static string? QuerySessionInfoString(IntPtr hServer, uint sessionId, WTS_INFO_CLASS wtsInfoClass)
    {
      IntPtr buffer = IntPtr.Zero;
      try
      {
        if (!WtsInterop.WTSQuerySessionInformation(hServer, sessionId, wtsInfoClass, out buffer, out uint bytesReturned))
        {
          return null;
        }
        return Marshal.PtrToStringUni(buffer);
      }
      finally
      {
        if (buffer != IntPtr.Zero)
        {
          WtsInterop.WTSFreeMemory(buffer);
        }
      }
    }
    #endregion
}
