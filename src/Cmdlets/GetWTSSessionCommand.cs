using System;
using System.Linq;
using System.Management.Automation;

namespace TerminalSessions.Cmdlets;

/// <summary>
/// <para type="description">Enumerates all the sessions available on Windows hosts through the WTSEnumerateSessionsExW API.</para>
/// </summary>
[Cmdlet(VerbsCommon.Get, "WTSSession", DefaultParameterSetName = "SessionInfo")]
[Alias("Get-TerminalSession")]
[OutputType(typeof(SessionInfoExtra), ParameterSetName = new[] { "SessionInfoExtra" })]
[OutputType(typeof(SessionInfo))]
public class GetWTSSessionCommand : PSCmdlet
{
  /// <summary>
  /// <para type="description">A list of hosts to query the sessions for. Omit or set to an empty array to check the local host.</para>
  /// </summary>
  [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
  public string[] ComputerName { get; set; } = [Environment.MachineName];

  /// <summary>
  /// <para type="description">If set, retrieves additional details about each session such as idle time and logon time.</para>
  /// </summary>
  [Parameter(ParameterSetName = "SessionInfoExtra")]
  [Alias("Details")]
  public SwitchParameter Detailed { get; set; }

  /// <summary>
  /// <para type="description">If set, filters out disconnected sessions.</para>
  /// </summary>
  [Parameter()]
  public SwitchParameter OnlineOnly { get; set; }

  protected override void ProcessRecord()
  {
    foreach (var name in ComputerName)
    {
      IntPtr serverInfo = IntPtr.Zero;
      try
      {
        serverInfo = WtsNative.WTSOpenServerEx(name);

        // SessionInfoExtra inherits from SessionInfo, so this works for both
        SessionInfo[] sessions = null!;
        if (Detailed.IsPresent)
        {
          sessions = WtsNative.WTSEnumerateSessionsWithDetails(serverInfo, name);
        }
        else
        {
          sessions = WtsNative.WTSEnumerateSessionsExtra(serverInfo, name);
        }

        if (sessions is not null)
        {
          if (OnlineOnly.IsPresent)
          {
            sessions = [.. sessions.Where(s => s.State != WtsConnectState.Disconnected)];
          }
          WriteObject(sessions, true);
        }
      }
      catch (PipelineStoppedException)
      {
        // Pipeline was stopped by downstream cmdlet (e.g., Select-Object -First)
        // This is normal behavior, just rethrow to let PowerShell handle it
        throw;
      }
      catch (Exception ex)
      {
        WriteError(new ErrorRecord(
          ex,
          "WTSSessionError",
          ErrorCategory.InvalidOperation,
          name));
      }
      finally
      {
        if (serverInfo != IntPtr.Zero)
        {
          WtsNative.WTSCloseServer(serverInfo);
        }
      }
    }
  }
}
