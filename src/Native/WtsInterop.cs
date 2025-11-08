using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.RemoteDesktop;

namespace TerminalSessions;

/// <summary>
/// Facade around CsWin32 for WTS APIs to avoid leaking Windows.Win32 into consumers.
/// </summary>
internal static class WtsInterop
{
  internal static unsafe HANDLE OpenServer(string name)
  {
    fixed (char* c = name)
    {
      return PInvoke.WTSOpenServerEx(new PWSTR(c));
    }
  }

  // Disambiguate by calling the HANDLE extern overload explicitly via cast
  internal static void CloseServer(HANDLE h)
  {
    PInvoke.WTSCloseServer(h);
  }

  internal static unsafe bool EnumerateSessions(HANDLE server, ref uint level, out WTS_SESSION_INFO_1W* sessions, out uint count)
  {
    fixed (uint* pLevel = &level)
    fixed (WTS_SESSION_INFO_1W** pSessions = &sessions)
    fixed (uint* pCount = &count)
    {
      return PInvoke.WTSEnumerateSessionsEx(server, pLevel, 0, pSessions, pCount);
    }
  }
  internal static unsafe void FreeSessions(WTS_SESSION_INFO_1W* sessions, uint count)
      => PInvoke.WTSFreeMemoryEx(WTS_TYPE_CLASS.WTSTypeSessionInfoLevel1, (void*)sessions, count);
  internal static unsafe bool QuerySessionInfo(HANDLE server, uint sessionId, WTS_INFO_CLASS klass, out PWSTR buffer, out uint bytes)
      => PInvoke.WTSQuerySessionInformation(server, sessionId, klass, out buffer, out bytes);

  internal static unsafe void Free(void* p) => PInvoke.WTSFreeMemory(p);

  internal static BOOL Logoff(HANDLE server, uint sessionId, BOOL wait)
      => PInvoke.WTSLogoffSession(server, sessionId, wait);
}
