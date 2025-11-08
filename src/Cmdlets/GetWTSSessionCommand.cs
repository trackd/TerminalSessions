using System;
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
  public SwitchParameter Detailed { get; set; }

  protected override void ProcessRecord()
  {
    foreach (var name in ComputerName)
    {
      IntPtr serverInfo = IntPtr.Zero;
      try
      {
        serverInfo = WtsNative.WTSOpenServerEx(name);
        if (Detailed.IsPresent)
        {
          var Sessions = WtsNative.WTSEnumerateSessionsWithDetails(serverInfo, name);
          WriteObject(Sessions, true);
        }
        else
        {
          var Sessions = WtsNative.WTSEnumerateSessionsExtra(serverInfo, name);
          WriteObject(Sessions, true);
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
