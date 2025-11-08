using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using TerminalSessions.Native;
using Windows.Win32.Foundation;
using Windows.Win32.System.RemoteDesktop;

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
    unsafe
    {
      var h = WtsInterop.OpenServer(serverName);
      if (h.IsNull)
      {
        throw new Win32Exception();
      }
      return (IntPtr)h;
    }
  }

  /// <summary>
  /// Closes an open handle to a terminal server
  /// </summary>
  /// <param name="hServer">The server handle to close</param>
  public static void WTSCloseServer(IntPtr hServer)
  {
    WtsInterop.CloseServer((HANDLE)hServer);
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
    unsafe
    {
      var result = WtsInterop.Logoff((HANDLE)hServer, SessionId, bWait ? new BOOL(1) : new BOOL(0));
      return result.Value != 0;
    }
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
    unsafe
    {
      uint level = 1;
      if (!WtsInterop.EnumerateSessions((HANDLE)serverHandle, ref level, out WTS_SESSION_INFO_1W* pSessions, out uint uCount))
      {
        throw new Win32Exception();
      }

      if (level != 1)
      {
        // Free any buffer before throwing
        if (pSessions != null) {
          WtsInterop.FreeSessions(pSessions, uCount);
        }
        throw new InvalidOperationException($"Expected level 1 but got level {level}");
      }

      if (pSessions == null)
      {
        return [];
      }

      try
      {
        int count = (int)uCount;
        List<SessionInfo> sessions = new(count);
        for (int i = 0; i < count; i++)
        {
          var info = pSessions[i];
          string user = info.pUserName.ToString();
          if (string.IsNullOrEmpty(user))
          {
            continue;
          }

          string sessionName = info.pSessionName.ToString();
          string domain = info.pDomainName.ToString();

          string? clientName = null;
          if (WtsHelpers.ShouldQueryClientName(info.State.ToPublic(), sessionName))
          {
            clientName = QuerySessionInfoString(serverHandle, info.SessionId, (int)WTS_INFO_CLASS.WTSClientName);
          }

          sessions.Add(new SessionInfo {
            SessionId = info.SessionId,
            State = info.State.ToPublic(),
            SessionName = sessionName ?? string.Empty,
            UserName = user,
            DomainName = domain ?? string.Empty,
            ComputerName = serverName,
            ClientName = clientName,
          });
        }
        return [.. sessions];
      }
      finally {
        WtsInterop.FreeSessions(pSessions, uCount);
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
    unsafe
    {
      uint level = 1;

      if (!WtsInterop.EnumerateSessions((HANDLE)serverHandle, ref level, out WTS_SESSION_INFO_1W* pSessions, out uint uCount))
      {
        throw new Win32Exception();
      }

      if (level != 1)
      {
        if (pSessions is not null) {
          WtsInterop.FreeSessions(pSessions, uCount);
        }
        throw new InvalidOperationException($"Expected level 1 but got level {level}");
      }

      if (pSessions is null)
      {
        return [];
      }

      try
      {
        int count = (int)uCount;
        List<SessionInfoExtra> sessions = new List<SessionInfoExtra>(count);
        for (int i = 0; i < count; i++)
        {
          var info = pSessions[i];

          string user = info.pUserName.ToString();
          if (string.IsNullOrEmpty(user))
          {
            continue;
          }

          string sessionName = info.pSessionName.ToString();
          string domain = info.pDomainName.ToString();

          string? clientName = null;
          TimeSpan? idleTime = null;
          DateTime? logonTime = null;

          try
          {
            WTSInfo? wtsInfo = WTSQueryWTSINFO(serverHandle, info.SessionId);
            if (wtsInfo is not null)
            {
              logonTime = wtsInfo.LogonTime;
              if (wtsInfo.LastInputTime.HasValue && wtsInfo.CurrentTime.HasValue)
              {
                idleTime = wtsInfo.CurrentTime.Value - wtsInfo.LastInputTime.Value;
              }
            }
          }
          catch { }

          if (WtsHelpers.ShouldQueryClientName(info.State.ToPublic(), sessionName))
          {
            clientName = QuerySessionInfoString(serverHandle, info.SessionId, (int)WTS_INFO_CLASS.WTSClientName);
          }

          sessions.Add(new SessionInfoExtra {
            SessionId = info.SessionId,
            State = info.State.ToPublic(),
            SessionName = sessionName ?? string.Empty,
            UserName = user,
            DomainName = domain ?? string.Empty,
            ComputerName = serverName,
            ClientName = clientName,
            IdleTime = idleTime,
            LogonTime = logonTime,
          });
        }
        return [.. sessions];
      }
      finally { WtsInterop.FreeSessions(pSessions, uCount); }
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
      unsafe
      {
        if (!WtsInterop.QuerySessionInfo((HANDLE)hServer, sessionId, WTS_INFO_CLASS.WTSClientInfo, out PWSTR p, out uint bytesReturned))
        {
          throw new Win32Exception();
        }
        buffer = (IntPtr)p.Value;
      }

      WTSCLIENTW rawClient = Marshal.PtrToStructure<WTSCLIENTW>(buffer);

      return new ClientInfo {
        ClientName = rawClient.ClientName.ToString() ?? string.Empty,
        Domain = rawClient.Domain.ToString() ?? string.Empty,
        UserName = rawClient.UserName.ToString() ?? string.Empty,
        WorkDirectory = rawClient.WorkDirectory.ToString() ?? string.Empty,
        InitialProgram = rawClient.InitialProgram.ToString() ?? string.Empty,
        EncryptionLevel = rawClient.EncryptionLevel,
        ClientAddressFamily = (AddressFamily)rawClient.ClientAddressFamily,
        ClientAddress = WtsHelpers.ConvertToIPAddress(rawClient.ClientAddress.ToArray(rawClient.ClientAddress.Length), (AddressFamily)rawClient.ClientAddressFamily),
        HRes = rawClient.HRes,
        VRes = rawClient.VRes,
        ColorDepth = rawClient.ColorDepth,
        ClientDirectory = rawClient.ClientDirectory.ToString() ?? string.Empty,
        ClientBuildNumber = rawClient.ClientBuildNumber,
        ClientHardwareId = rawClient.ClientHardwareId,
        ClientProductId = rawClient.ClientProductId,
        OutBufCountHost = rawClient.OutBufCountHost,
        OutBufCountClient = rawClient.OutBufCountClient,
        OutBufLength = rawClient.OutBufLength,
        DeviceId = rawClient.DeviceId.ToString() ?? string.Empty,
      };
    }
    finally
    {
      if (buffer != IntPtr.Zero)
      {
        unsafe {
          WtsInterop.Free((void*)buffer);
        }
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
      unsafe
      {
        if (!WtsInterop.QuerySessionInfo((HANDLE)hServer, sessionId, WTS_INFO_CLASS.WTSSessionInfo, out PWSTR p, out uint bytesReturned))
        {
          throw new Win32Exception();
        }
        buffer = (IntPtr)p.Value;
      }
      WTSINFOW rawInfo = Marshal.PtrToStructure<WTSINFOW>(buffer);

      return new WTSInfo {
        State = rawInfo.State.ToPublic(),
        SessionId = rawInfo.SessionId,
        IncomingBytes = rawInfo.IncomingBytes,
        OutgoingBytes = rawInfo.OutgoingBytes,
        IncomingFrames = rawInfo.IncomingFrames,
        OutgoingFrames = rawInfo.OutgoingFrames,
        IncomingCompressedBytes = rawInfo.IncomingCompressedBytes,
        OutgoingCompressedBy = rawInfo.OutgoingCompressedBytes,
        WinStationName = rawInfo.WinStationName.ToString(),
        Domain = rawInfo.Domain.ToString(),
        UserName = rawInfo.UserName.ToString(),
        ConnectTime = WtsHelpers.ConvertFileTimeToLocal(rawInfo.ConnectTime),
        DisconnectTime = rawInfo.State == WTS_CONNECTSTATE_CLASS.WTSDisconnected ? WtsHelpers.ConvertFileTimeToLocal(rawInfo.DisconnectTime) : null,
        LastInputTime = WtsHelpers.ConvertFileTimeToLocal(rawInfo.LastInputTime),
        LogonTime = WtsHelpers.ConvertFileTimeToLocal(rawInfo.LogonTime),
        CurrentTime = WtsHelpers.ConvertFileTimeToLocal(rawInfo.CurrentTime),
      };
    }
    finally
    {
      if (buffer != IntPtr.Zero)
      {
        unsafe
        {
          WtsInterop.Free((void*)buffer);
        }
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
  public static string? QuerySessionInfoString(IntPtr hServer, uint sessionId, int wtsInfoClass)
  {
    IntPtr buffer = IntPtr.Zero;
    try
    {
      unsafe
      {
        if (!WtsInterop.QuerySessionInfo((HANDLE)hServer, sessionId, (WTS_INFO_CLASS)wtsInfoClass, out PWSTR p, out uint bytesReturned))
        {
          return null;
        }
        buffer = (IntPtr)p.Value;
      }
      return Marshal.PtrToStringUni(buffer);
    }
    finally
    {
      if (buffer != IntPtr.Zero)
      {
        unsafe
        {
          WtsInterop.Free((void*)buffer);
        }
      }
    }
  }
  #endregion
}
