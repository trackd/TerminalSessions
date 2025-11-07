using System.Runtime.InteropServices;

namespace TerminalSessions.Native;

/// <summary>
/// Native structures for Windows Terminal Services (WTS) API
/// </summary>
internal static class WtsStructures
{
  /// <summary>
  /// WTS_SESSION_INFO_1 structure containing extended session information
  /// https://learn.microsoft.com/en-us/windows/win32/api/wtsapi32/ns-wtsapi32-wts_session_info_1w
  /// </summary>
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  internal struct WTS_SESSION_INFO_1W
  {
    public uint ExecEnvId;
    public WtsConnectState State;
    public uint SessionId;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pSessionName;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pHostName;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pUserName;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pDomainName;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pFarmName;
  }

  /// <summary>
  /// WTSCLIENT structure containing client connection information
  /// https://learn.microsoft.com/en-us/windows/win32/api/wtsapi32/ns-wtsapi32-wtsclientw
  /// </summary>
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  internal struct WTSCLIENT
  {
    private const int CLIENTNAME_LENGTH = 20;
    private const int DOMAIN_LENGTH = 17;
    private const int USERNAME_LENGTH = 20;
    private const int MAX_PATH = 260;
    private const int CLIENTADDRESS_LENGTH = 30;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CLIENTNAME_LENGTH + 1)]
    public string ClientName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DOMAIN_LENGTH + 1)]
    public string Domain;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = USERNAME_LENGTH + 1)]
    public string UserName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH + 1)]
    public string WorkDirectory;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH + 1)]
    public string InitialProgram;
    public byte EncryptionLevel;
    public uint ClientAddressFamily;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = CLIENTADDRESS_LENGTH + 1)]
    public ushort[] ClientAddress;
    public ushort HRes;
    public ushort VRes;
    public ushort ColorDepth;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH + 1)]
    public string ClientDirectory;
    public uint ClientBuildNumber;
    public uint ClientHardwareId;
    public ushort ClientProductId;
    public ushort OutBufCountHost;
    public ushort OutBufCountClient;
    public ushort OutBufLength;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH + 1)]
    public string DeviceId;
  }

  /// <summary>
  /// WTSINFO structure containing detailed session information
  /// https://learn.microsoft.com/en-us/windows/win32/api/wtsapi32/ns-wtsapi32-wtsinfow
  /// </summary>
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  internal struct WTSINFOW
  {
    private const int WINSTATIONNAME_LENGTH = 32;
    private const int DOMAIN_LENGTH = 17;
    private const int USERNAME_LENGTH = 20;

    public WtsConnectState State;
    public uint SessionId;
    public uint IncomingBytes;
    public uint OutgoingBytes;
    public uint IncomingFrames;
    public uint OutgoingFrames;
    public uint IncomingCompressedBytes;
    public uint OutgoingCompressedBytes;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = WINSTATIONNAME_LENGTH)]
    public string WinStationName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DOMAIN_LENGTH)]
    public string Domain;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = USERNAME_LENGTH + 2)]
    public string UserName;
    public long ConnectTimeUTC;
    public long DisconnectTimeUTC;
    public long LastInputTimeUTC;
    public long LogonTimeUTC;
    public long CurrentTimeUTC;
  }
}
