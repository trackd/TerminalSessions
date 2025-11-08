using System.Management.Automation;

namespace TerminalSessions.Cmdlets;

  /// <summary>
  /// <para type="description">Retrieves WTS information for a session on a Windows host through the WTSQueryWTSINFO API.</para>
  /// </summary>
  [Cmdlet(VerbsCommon.Get, "WTSInfo")]
  [OutputType(typeof(WTSInfo))]
  public class GetWTSInfoCommand : PSCmdlet
  {
    /// <summary>
    /// <para type="description">The session info object returned from Get-WTSSession.</para>
    /// </summary>
    [Parameter(ValueFromPipeline = true, Mandatory = true)]
    public SessionInfo SessionInfo { get; set; } = null!;

    protected override void ProcessRecord()
    {
      IntPtr serverInfo = IntPtr.Zero;

      try
      {
        serverInfo = WtsNative.WTSOpenServerEx(SessionInfo.ComputerName);
        var info = WtsNative.WTSQueryWTSINFO(serverInfo, SessionInfo.SessionId);
        WriteObject(info);
      }
      catch (Exception ex)
      {
        WriteError(new ErrorRecord(
          ex,
          "WTSInfoError",
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
