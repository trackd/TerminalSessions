using System;
using System.Net;
using System.Net.Sockets;
using Windows.Win32.System.RemoteDesktop; // internal CsWin32 enum

namespace TerminalSessions;

public enum WtsConnectState
{
  Active = 0,
  Connected = 1,
  ConnectQuery = 2,
  Shadow = 3,
  Disconnected = 4,
  Idle = 5,
  Listen = 6,
  Reset = 7,
  Down = 8,
  Init = 9
}

internal static class WtsStateExtensions
{
  internal static WtsConnectState ToPublic(this WTS_CONNECTSTATE_CLASS value) => (WtsConnectState)(int)value;
  internal static WTS_CONNECTSTATE_CLASS ToInternal(this WtsConnectState value) => (WTS_CONNECTSTATE_CLASS)(int)value;
}

public class SessionInfo
{
  public uint SessionId { get; set; }
  public WtsConnectState State { get; set; }
  public string SessionName { get; set; } = string.Empty;
  public string UserName { get; set; } = string.Empty;
  public string DomainName { get; set; } = string.Empty;
  public string ComputerName { get; set; } = string.Empty;
  public string? ClientName { get; set; }

  public override string ToString()
  {
    return $"{ComputerName}: {UserName} (SessionId: {SessionId}, SessionName: {SessionName}, State: {State})";
  }
}

public class SessionInfoExtra : SessionInfo
{
  public TimeSpan? IdleTime { get; set; }
  public DateTime? LogonTime { get; set; }

  public override string ToString()
  {
    string idleStr = IdleTime.HasValue ? $", IdleTime: {IdleTime.Value:hh\\:mm\\:ss}" : "";
    string logonStr = LogonTime.HasValue ? $", LogonTime: {LogonTime.Value:yyyy-MM-dd HH:mm:ss}" : "";
    return $"{ComputerName}: {UserName} (SessionId: {SessionId}, SessionName: {SessionName}, State: {State}{logonStr}{idleStr})";
  }
}

public class ClientInfo
{
  public string ClientName { get; set; } = string.Empty;
  public string Domain { get; set; } = string.Empty;
  public string UserName { get; set; } = string.Empty;
  public string WorkDirectory { get; set; } = string.Empty;
  public string InitialProgram { get; set; } = string.Empty;
  public byte EncryptionLevel { get; set; }
  public AddressFamily ClientAddressFamily { get; set; }
  public IPAddress? ClientAddress { get; set; }
  public ushort HRes { get; set; }
  public ushort VRes { get; set; }
  public ushort ColorDepth { get; set; }
  public string ClientDirectory { get; set; } = string.Empty;
  public uint ClientBuildNumber { get; set; }
  public uint ClientHardwareId { get; set; }
  public ushort ClientProductId { get; set; }
  public ushort OutBufCountHost { get; set; }
  public ushort OutBufCountClient { get; set; }
  public ushort OutBufLength { get; set; }
  public string DeviceId { get; set; } = string.Empty;
}

public class WTSInfo
{
  public WtsConnectState State { get; set; }
  public uint SessionId { get; set; }
  public uint IncomingBytes { get; set; }
  public uint OutgoingBytes { get; set; }
  public uint IncomingFrames { get; set; }
  public uint OutgoingFrames { get; set; }
  public uint IncomingCompressedBytes { get; set; }
  public uint OutgoingCompressedBy { get; set; }
  public string WinStationName { get; set; } = string.Empty;
  public string Domain { get; set; } = string.Empty;
  public string UserName { get; set; } = string.Empty;
  public DateTime? ConnectTime { get; set; }
  public DateTime? DisconnectTime { get; set; }
  public DateTime? LastInputTime { get; set; }
  public DateTime? LogonTime { get; set; }
  public DateTime? CurrentTime { get; set; }
}
