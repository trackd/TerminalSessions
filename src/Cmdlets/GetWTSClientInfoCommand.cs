using System.Management.Automation;

namespace TerminalSessions.Cmdlets;

/// <summary>
/// <para type="synopsis">Retrieves client information for a session on a Windows host.</para>
  /// <para type="description">Retrieves client information for a session on a Windows host through the WTSQueryClientInfo API.</para>
  /// </summary>
  [Cmdlet(VerbsCommon.Get, "WTSClientInfo")]
  [OutputType(typeof(ClientInfo))]
  public class GetWTSClientInfoCommand : PSCmdlet
  {
    /// <summary>
    /// <para type="description">The session info object returned from Get-WTSSession.</para>
    /// </summary>
    [Parameter(ValueFromPipeline = true, Mandatory = true)]
    public SessionInfo SessionInfo { get; set; } = null!;

    protected override void ProcessRecord()
    {
      // Skip sessions that don't have client information (Listen, Down, Init, Disconnected)
          if (SessionInfo.State == WtsConnectState.Listen ||
            SessionInfo.State == WtsConnectState.Down ||
            SessionInfo.State == WtsConnectState.Init ||
            SessionInfo.State == WtsConnectState.Disconnected)
      {
        WriteVerbose($"Session {SessionInfo.SessionId} on {SessionInfo.ComputerName} has no client attached (State: {SessionInfo.State}). Skipping.");
        return;
      }

      IntPtr serverInfo = IntPtr.Zero;

      try
      {

        serverInfo = WtsNative.WTSOpenServerEx(SessionInfo.ComputerName);
        var info = WtsNative.WTSQueryClientInfo(serverInfo, SessionInfo.SessionId);
        WriteObject(info);
      }
      catch (Exception ex)
      {
        WriteError(new ErrorRecord(
          ex,
          "WTSClientInfoError",
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
