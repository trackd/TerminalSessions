using System;
using System.Management.Automation;

namespace TerminalSessions.Cmdlets;

/// <summary>
/// Disconnects a WTS session on a Windows host.
/// </summary>
[Cmdlet(VerbsCommunications.Disconnect, "TerminalSession", DefaultParameterSetName = "BySessionInfo")]
[Alias("Disconnect-WTSSession", "dcts")]
[OutputType(typeof(bool))]
public class DisconnectTerminalSession : PSCmdlet
{
  /// <summary>
  /// The session info object returned from Get-WTSSession.
  /// </summary>
  [Parameter(
    ValueFromPipeline = true,
    Mandatory = true,
    ParameterSetName = "BySessionInfo"
  )]
  public SessionInfo SessionInfo { get; set; } = null!;

  [Parameter(
    ValueFromPipelineByPropertyName = true,
    Mandatory = true,
    ParameterSetName = "ByManual"
  )]
  [ValidateNotNullOrEmpty]
  public string ComputerName { get; set; } = null!;
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
      foreach (var session in group)
      {
        try
        {
          bool result = WtsNative.DisconnectSession(session.ComputerName, session.SessionId);
          WriteVerbose($"Disconnected session {session.SessionId} on {session.ComputerName}: {result}");
          WriteObject(result);
        }
        catch (Exception ex)
        {
          WriteError(new ErrorRecord(
            ex,
            "DisconnectTerminalSession",
            ErrorCategory.OperationStopped,
            session));
        }
      }
    }
  }
}
