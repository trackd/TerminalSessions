using System;
using System.Management.Automation;

namespace TerminalSessions.Cmdlets;

/// <summary>
/// <para type="synopsis">Retrieves WTS information for a session on a Windows host.</para>
/// <para type="description">Retrieves WTS information for a session on a Windows host through the WTSQueryWTSINFO API.</para>
/// </summary>
[Cmdlet(VerbsCommon.Get, "TerminalInfo", DefaultParameterSetName = "BySessionInfo")]
[Alias("Get-WTSInfo", "gti")]
[OutputType(typeof(WTSInfo))]
public class GetTerminalInfo : PSCmdlet
{
  /// <summary>
  /// <para type="description">The session info object returned from Get-WTSSession.</para>
  /// </summary>
  [Parameter(ValueFromPipeline = true, ParameterSetName = "BySessionInfo", Mandatory = true)]
  public SessionInfo? SessionInfo { get; set; }

  [Parameter(
    ValueFromPipelineByPropertyName = true,
    Mandatory = true,
    ParameterSetName = "ByManual"
  )]
  [ValidateNotNullOrEmpty]
  public string? ComputerName { get; set; }

  [Parameter(
    ValueFromPipelineByPropertyName = true,
    Mandatory = true,
    ParameterSetName = "ByManual"
  )]
  [ValidateRange(1, uint.MaxValue)]
  public uint SessionId { get; set; }

  private readonly List<SessionInfo> _sessions = [];

  protected override void ProcessRecord()
  {
    if (ParameterSetName == "BySessionInfo" && SessionInfo is not null)
    {
      _sessions.Add(SessionInfo);
    }
    else if (ParameterSetName == "ByManual" && ComputerName is not null)
    {
      _sessions.Add(new SessionInfo {
        ComputerName = ComputerName,
        SessionId = SessionId
      });
    }
  }
  protected override void EndProcessing()
  {
    foreach (var group in _sessions.GroupBy(s => s.ComputerName))
    {
      IntPtr serverInfo = IntPtr.Zero;
      try
      {
        serverInfo = WtsNative.WTSOpenServerEx(group.Key);
        foreach (var session in group)
        {
          try
          {
            var info = WtsNative.WTSQueryWTSINFO(serverInfo, session.SessionId);
            WriteObject(info);
          }
          catch (Exception ex)
          {
            WriteError(new ErrorRecord(
              ex,
              "TerminalInfo",
              ErrorCategory.InvalidOperation,
              session));
          }
        }
      }
      catch (Exception ex)
      {
        WriteError(new ErrorRecord(
          ex,
          "TerminalSessionsServer",
          ErrorCategory.OpenError,
          group.Key));
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
