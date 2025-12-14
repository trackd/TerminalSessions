using System;

namespace TerminalSessions;

public enum WtsConnectState : int
{
  // https://learn.microsoft.com/en-us/windows/win32/api/wtsapi32/ne-wtsapi32-wts_connectstate_class
  Active,
  Connected,
  ConnectQuery,
  Shadow,
  Disconnected,
  Idle,
  Listen,
  Reset,
  Down,
  Init
}
public static class WtsConnectStateExtensions
{
  public static bool IsInactive(this WtsConnectState state)
      => state is WtsConnectState.Listen
                or WtsConnectState.Down
                or WtsConnectState.Init
                or WtsConnectState.Disconnected;

  public static bool IsOnline(this WtsConnectState state)
      => state is WtsConnectState.Active
                or WtsConnectState.Connected
                or WtsConnectState.ConnectQuery
                or WtsConnectState.Shadow
                or WtsConnectState.Idle
                or WtsConnectState.Reset;
}
public enum WTS_TYPE_CLASS : int
{
  WTSTypeProcessInfoLevel0 = 0,
  WTSTypeProcessInfoLevel1 = 1,
  WTSTypeSessionInfoLevel1 = 2
}

public enum WTS_INFO_CLASS : int
{
  WTSInitialProgram = 0,
  WTSApplicationName = 1,
  WTSWorkingDirectory = 2,
  WTSOEMId = 3,
  WTSSessionId = 4,
  WTSUserName = 5,
  WTSWinStationName = 6,
  WTSDomainName = 7,
  WTSConnectState = 8,
  WTSClientBuildNumber = 9,
  WTSClientName = 10,
  WTSClientDirectory = 11,
  WTSClientProductId = 12,
  WTSClientHardwareId = 13,
  WTSClientAddress = 14,
  WTSClientDisplay = 15,
  WTSClientProtocolType = 16,
  WTSIdleTime = 17,
  WTSLogonTime = 18,
  WTSIncomingBytes = 19,
  WTSOutgoingBytes = 20,
  WTSIncomingFrames = 21,
  WTSOutgoingFrames = 22,
  WTSClientInfo = 23,
  WTSSessionInfo = 24,
  WTSSessionInfoEx = 25,
  WTSConfigInfo = 26,
  WTSValidationInfo = 27,
  WTSSessionAddressV4 = 28,
  WTSIsRemoteSession = 29
}

[Flags]
public enum MessageBoxType : uint
{
  BUTTON_ABORTRETRYIGNORE = (uint)0x00000002L,
  BUTTON_CANCELTRYCONTINUE = (uint)0x00000006L,
  BUTTON_HELP = (uint)0x00004000L,
  BUTTON_OK = (uint)0x00000000L,
  BUTTON_OKCANCEL = (uint)0x00000001L,
  BUTTON_RETRYCANCEL = (uint)0x00000005L,
  BUTTON_YESNO = (uint)0x00000004L,
  BUTTON_YESNOCANCEL = (uint)0x00000003L,
  ICON_EXCLAMATION = (uint)0x00000030L,
  ICON_INFORMATION = (uint)0x00000040L,
  ICON_QUESTION = (uint)0x00000020L,
  ICON_STOP = (uint)0x00000010L,
  DEFAULT_BUTTON1 = (uint)0x00000000L,
  DEFAULT_BUTTON2 = (uint)0x00000100L,
  DEFAULT_BUTTON3 = (uint)0x00000200L,
  DEFAULT_BUTTON4 = (uint)0x00000300L,
  MB_DEFAULT_DESKTOP_ONLY = (uint)0x00020000L,
  MB_RIGHT = (uint)0x00080000L,
  MB_RTLREADING = (uint)0x00100000L,
  MB_SETFOREGROUND = (uint)0x00010000L,
  MB_TOPMOST = (uint)0x00040000L,
  MB_SERVICE_NOTIFICATION = (uint)0x00200000L
}
public enum MessageBoxResult : int
{
  Ok = 1,
  Cancel = 2,
  Abort = 3,
  Retry = 4,
  Ignore = 5,
  Yes = 6,
  No = 7,
  TryAgain = 10,
  Continue = 11,
  NoWait = 32001,
  Timeout = 32000,
  Failed = 64000
}
