using System;
using System.Runtime.InteropServices;

namespace TerminalSessions.Native;

/// <summary>
/// P/Invoke declarations for Windows Terminal Services (WTS) API
/// </summary>
internal static class WtsInterop
{
    private const string Wtsapi32 = "Wtsapi32.dll";

    /// <summary>
    /// Closes an open handle to a terminal server
    /// https://learn.microsoft.com/en-us/windows/win32/api/wtsapi32/nf-wtsapi32-wtscloseserver
    /// </summary>
    [DllImport(Wtsapi32)]
    internal static extern void WTSCloseServer(IntPtr hServer);

    /// <summary>
    /// Retrieves extended session information for the specified terminal server
    /// https://learn.microsoft.com/en-us/windows/win32/api/wtsapi32/nf-wtsapi32-wtsenumeratesessionsexw
    /// </summary>
    [DllImport(Wtsapi32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool WTSEnumerateSessionsExW(
        IntPtr hServer,
        ref int pLevel,
        int Filter,
        out IntPtr ppSessionInfo,
        out int pCount);

    /// <summary>
    /// Frees memory allocated by the WTS API functions
    /// https://learn.microsoft.com/en-us/windows/win32/api/wtsapi32/nf-wtsapi32-wtsfreememoryexw
    /// </summary>
    [DllImport(Wtsapi32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool WTSFreeMemoryExW(
        int WTSTypeClass,
        IntPtr pMemory,
        int NumberOfEntries);

    /// <summary>
    /// Frees memory allocated by the WTS API functions
    /// https://learn.microsoft.com/en-us/windows/win32/api/wtsapi32/nf-wtsapi32-wtsfreememory
    /// </summary>
    [DllImport(Wtsapi32)]
    internal static extern void WTSFreeMemory(IntPtr memory);

    /// <summary>
    /// Opens a handle to the specified terminal server
    /// https://learn.microsoft.com/en-us/windows/win32/api/wtsapi32/nf-wtsapi32-wtsopenserverexw
    /// </summary>
    [DllImport(Wtsapi32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern IntPtr WTSOpenServerExW(string pServerName);

    /// <summary>
    /// Logs off a specified terminal services session
    /// https://learn.microsoft.com/en-us/windows/win32/api/wtsapi32/nf-wtsapi32-wtslogoffsession
    /// </summary>
    [DllImport(Wtsapi32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool WTSLogoffSession(
        IntPtr hServer,
        uint SessionId,
        [MarshalAs(UnmanagedType.Bool)] bool bWait);

    /// <summary>
    /// Retrieves session information for the specified session on the specified terminal server
    /// https://learn.microsoft.com/en-us/windows/win32/api/wtsapi32/nf-wtsapi32-wtsquerysessioninformationw
    /// </summary>
    [DllImport(Wtsapi32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool WTSQuerySessionInformation(
        IntPtr hServer,
        uint sessionId,
        WTS_INFO_CLASS wtsInfoClass,
        out IntPtr ppBuffer,
        out uint pBytesReturned);
}
