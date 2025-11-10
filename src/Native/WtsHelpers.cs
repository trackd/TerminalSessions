using System;
using System.Net;
using System.Net.Sockets;

namespace TerminalSessions.Native;

/// <summary>
/// Helper methods for WTS API operations
/// </summary>
internal static class WtsHelpers
{
  /// <summary>
  /// Converts a WTS client address array to an IPAddress object
  /// </summary>
  /// <param name="clientAddress">The raw client address data from WTS API</param>
  /// <param name="addressFamily">The address family (IPv4 or IPv6)</param>
  /// <returns>An IPAddress object, or null if conversion fails</returns>
  internal static IPAddress? ConvertToIPAddress(ushort[]? clientAddress, AddressFamily addressFamily)
  {
    if (clientAddress is null || clientAddress.Length == 0)
    {
      return null;
    }

    try
    {
      if (addressFamily == AddressFamily.InterNetwork)
      {
        if (clientAddress.Length < 4)
        {
          return null;
        }

        byte[] ipBytes = new byte[4];
        for (int i = 0; i < 4; i++)
        {
          ipBytes[i] = (byte)(clientAddress[i] & 0xFF);
        }

        return new IPAddress(ipBytes);
      }
      else if (addressFamily == AddressFamily.InterNetworkV6)
      {
        if (clientAddress.Length < 16)
        {
          return null;
        }

        byte[] ipBytes = new byte[16];
        for (int i = 0; i < 16; i++)
        {
          ipBytes[i] = (byte)(clientAddress[i] & 0xFF);
        }

        return new IPAddress(ipBytes);
      }

      return null;
    }
    catch
    {
      // Return null on any conversion failure
      return null;
    }
  }

  /// <summary>
  /// Converts a Windows FILETIME (UTC) to a local DateTime
  /// </summary>
  /// <param name="fileTime">The FILETIME value in UTC</param>
  /// <returns>A nullable DateTime in local time, or null if conversion fails</returns>
  internal static DateTime? ConvertFileTimeToLocal(long fileTime)
  {
    // Zero indicates no valid time
    if (fileTime == 0)
    {
      return null;
    }

    try
    {

      // The WTSINFOW struct times are in UTC
      DateTime utcTime = DateTime.FromFileTimeUtc(fileTime);

      TimeSpan currentOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
      DateTime localTime = utcTime.Add(currentOffset);

      return localTime;
    }
    catch
    {
      // Return null on any conversion failure (e.g., invalid FILETIME)
      return null;
    }
  }

  /// <summary>
  /// Determines if a session should have client name queried
  /// Console sessions and disconnected sessions typically don't have remote clients
  /// </summary>
  /// <param name="state">The connection state of the session</param>
  /// <param name="sessionName">The name of the session</param>
  /// <returns>True if client name should be queried</returns>
  internal static bool ShouldQueryClientName(WtsConnectState state, string sessionName)
  {
    return state != WtsConnectState.Disconnected && sessionName != "console";
  }
}
