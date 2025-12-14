// Cmdlet for sending a message to a WTS session
using System;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TerminalSessions.Native;

namespace TerminalSessions.Cmdlets;

[Cmdlet(VerbsCommunications.Send, "TerminalMessage", DefaultParameterSetName = "BySessionInfo")]
[Alias("Send-WTSMessage", "sdtm")]
[OutputType(typeof(SendMessageResult))]
public sealed class SendTerminalMessage : PSCmdlet
{


    [Parameter(Mandatory = true, Position = 0)]
    [ValidateNotNullOrEmpty]
    [Alias("t")]
    public string? Title { get; set; }

    [Parameter(Mandatory = true, Position = 1)]
    [ValidateNotNullOrEmpty]
    public string? Body { get; set; }

    [Parameter(Position = 2)]
    public MessageBoxType? Type { get; set; }

    [Parameter(Position = 3)]
    [Alias("to")]
    public int? TimeoutSeconds { get; set; }

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
    [ValidateRange(1, uint.MaxValue)]
    public uint SessionId { get; set; }
    private readonly List<SessionInfo> _sessions = [];
    protected override void ProcessRecord()
    {
        if (ParameterSetName == "BySessionInfo" && SessionInfo is not null)
        {
            if (SessionInfo.State.IsInactive())
            {
                WriteVerbose($"Session {SessionInfo.SessionId} on {SessionInfo.ComputerName} has no client attached (State: {SessionInfo.State}). Skipping.");
                return;
            }
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
        bool useAdvanced = Type.HasValue || TimeoutSeconds.HasValue;
        string title = Title ?? string.Empty;
        string body = Body ?? string.Empty;

        var serverTasks = new List<Task<List<SendMessageResult>>>();

        foreach (var grouping in _sessions.GroupBy(s => s.ComputerName))
        {
            string server = grouping.Key;
            var sessionsOnServer = grouping.ToList();

            serverTasks.Add(Task.Run(() =>
            {
                IntPtr serverInfo = IntPtr.Zero;
                var results = new List<SendMessageResult>();
                try
                {
                    serverInfo = WtsNative.WTSOpenServerEx(server);

                    var sessionTasks = new List<Task<SendMessageResult>>();
                    foreach (var session in sessionsOnServer)
                    {
                        var localSession = session;
                        sessionTasks.Add(Task.Run(() =>
                        {
                            try
                            {
                                if (useAdvanced)
                                {
                                    var response = WtsNative.SendMessage(
                                        serverInfo,
                                        localSession.SessionId,
                                        title,
                                        body,
                                        Type ?? MessageBoxType.BUTTON_OK,
                                        TimeoutSeconds ?? 60,
                                        true
                                    );
                                    return new SendMessageResult {
                                        ComputerName = localSession.ComputerName,
                                        SessionId = localSession.SessionId,
                                        UserName = localSession.UserName,
                                        DomainName = localSession.DomainName,
                                        Success = response != MessageBoxResult.Failed,
                                        Response = response,
                                        Error = null
                                    };
                                }
                                else
                                {
                                    var response = WtsNative.SendMessage(
                                        serverInfo,
                                        localSession.SessionId,
                                        title,
                                        body,
                                        0,
                                        0,
                                        false
                                    );
                                    return new SendMessageResult {
                                        ComputerName = localSession.ComputerName,
                                        SessionId = localSession.SessionId,
                                        UserName = localSession.UserName,
                                        DomainName = localSession.DomainName,
                                        Success = response != MessageBoxResult.Failed,
                                        Response = response,
                                        Error = response != MessageBoxResult.Failed ? null : "Send-TerminalMessage failed"
                                    };
                                }
                            }
                            catch (Exception ex)
                            {
                                return new SendMessageResult {
                                    ComputerName = localSession.ComputerName,
                                    SessionId = localSession.SessionId,
                                    UserName = localSession.UserName,
                                    DomainName = localSession.DomainName,
                                    Success = false,
                                    Response = null,
                                    Error = ex.Message
                                };
                            }
                        }));
                    }

                    Task.WaitAll([.. sessionTasks]);
                    foreach (var task in sessionTasks)
                    {
                        results.Add(task.Result);
                    }
                }
                catch (Exception ex)
                {
                    results.Add(new SendMessageResult {
                        ComputerName = server,
                        SessionId = 0,
                        UserName = string.Empty,
                        DomainName = string.Empty,
                        Success = false,
                        Response = null,
                        Error = ex.Message
                    });
                }
                finally
                {
                    if (serverInfo != IntPtr.Zero)
                    {
                        WtsNative.WTSCloseServer(serverInfo);
                    }
                }

                return results;
            }));
        }

        Task.WaitAll([.. serverTasks]);
        foreach (var serverTask in serverTasks)
        {
            foreach (var result in serverTask.Result)
            {
                WriteVerbose($"SessionId: {result.SessionId}, response: {result.Response}");
                WriteObject(result);
            }
        }
    }
}
