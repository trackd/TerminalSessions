using System;
using System.Management.Automation;

namespace TerminalSessions.Cmdlets;

/// <summary>
/// <para type="synopsis">Removes a session from a Windows host.</para>
/// <para type="description">Removes a session from a Windows host through the WTSLogoffSession API.</para>
/// </summary>
[Cmdlet(
  VerbsCommon.Remove, "WTSSession",
  SupportsShouldProcess = true,
  ConfirmImpact = ConfirmImpact.High,
  DefaultParameterSetName = "BySessionInfo"
)]
public class RemoveWTSSessionCommand : PSCmdlet
{
  /// <summary>
  /// <para type="description">The session info object returned from Get-WTSSession.</para>
  /// </summary>
  [Parameter(
      ValueFromPipeline = true,
      ParameterSetName = "BySessionInfo",
      Mandatory = true
  )]
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
  public uint SessionId { get; set; }

  [Parameter]
  public SwitchParameter WaitForLogoff { get; set; }
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
          var target = $"Logoff Session {session.SessionId} for {session.UserName}@{session.ComputerName} State: {session.State}";
          if (ShouldProcess(target))
          {
            try
            {
              bool result = WtsNative.WTSLogoffSession(serverInfo, session.SessionId, WaitForLogoff.IsPresent);
              var output = new PSObject();
              output.Properties.Add(new PSNoteProperty("SessionId", session.SessionId));
              output.Properties.Add(new PSNoteProperty("UserName", session.UserName));
              output.Properties.Add(new PSNoteProperty("ComputerName", session.ComputerName));
              output.Properties.Add(new PSNoteProperty("Result", result));
              WriteObject(output);
            }
            catch (Exception ex)
            {
              WriteError(new ErrorRecord(
                ex,
                "WTSLogoffError",
                ErrorCategory.InvalidOperation,
                session));
            }
          }
        }
      }
      catch (Exception ex)
      {
        WriteError(new ErrorRecord(
          ex,
          "WTSLogoffServerError",
          ErrorCategory.InvalidOperation,
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
