using System;
using System.Management.Automation;

namespace TerminalSessions.Cmdlets;

/// <summary>
/// <para type="synopsis">Removes a session from a Windows host.</para>
  /// <para type="description">Removes a session from a Windows host through the WTSLogoffSession API.</para>
  /// </summary>
  [Cmdlet(VerbsCommon.Remove, "WTSSession", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
  public class RemoveWTSSessionCommand : PSCmdlet
  {
    /// <summary>
    /// <para type="description">The session info object returned from Get-WTSSession.</para>
    /// </summary>
    [Parameter(ValueFromPipeline = true, Mandatory = true)]
    public SessionInfo SessionInfo { get; set; } = null!;

    /// <summary>
    /// <para type="description">Wait for the logoff operation to complete before returning.</para>
    /// </summary>
    [Parameter]
    public SwitchParameter WaitForLogoff { get; set; }

    protected override void ProcessRecord()
    {
      var target = $"Logoff Session {SessionInfo.SessionId} for {SessionInfo.UserName}@{SessionInfo.ComputerName} State: {SessionInfo.State}";

      if (ShouldProcess(target))
      {
        IntPtr serverInfo = IntPtr.Zero;

        try
        {
          serverInfo = WtsNative.WTSOpenServerEx(SessionInfo.ComputerName);
          bool result = WtsNative.WTSLogoffSession(serverInfo, SessionInfo.SessionId, WaitForLogoff.IsPresent);

          var output = new PSObject();
          output.Properties.Add(new PSNoteProperty("SessionId", SessionInfo.SessionId));
          output.Properties.Add(new PSNoteProperty("UserName", SessionInfo.UserName));
          output.Properties.Add(new PSNoteProperty("ComputerName", SessionInfo.ComputerName));
          output.Properties.Add(new PSNoteProperty("Result", result));

          WriteObject(output);
        }
        catch (Exception ex)
        {
          WriteError(new ErrorRecord(
            ex,
            "WTSLogoffError",
            ErrorCategory.InvalidOperation,
            SessionInfo));
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
