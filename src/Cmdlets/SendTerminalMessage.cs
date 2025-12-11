// Cmdlet for sending a message to a WTS session
using System;
using System.Management.Automation;
using TerminalSessions.Native;

namespace TerminalSessions.Cmdlets;

[Cmdlet(VerbsCommunications.Send, "TerminalMessage", DefaultParameterSetName = "BySessionInfo")]
[Alias("Send-WTSMessage", "sdtm")]
[OutputType(typeof(MessageBoxResult))]
[OutputType(typeof(bool))]
public sealed class SendTerminalMessage : PSCmdlet
{
    [Parameter(
        ValueFromPipeline = true,
        ParameterSetName = "BySessionInfo",
        Mandatory = true
    )]
    public SessionInfo? SessionInfo { get; set; }

    [Parameter(
        ValueFromPipelineByPropertyName = true,
        Mandatory = true,
        ParameterSetName = "ByManual",
        Position = 0
    )]
    [ValidateNotNullOrEmpty]
    public string? ComputerName { get; set; }

    [Parameter(
        ValueFromPipelineByPropertyName = true,
        Mandatory = true,
        ParameterSetName = "ByManual",
        Position = 1
    )]
    [ValidateRange(1, uint.MaxValue)]
    public uint SessionId { get; set; }

    [Parameter(Mandatory = true, Position = 2)]
    [ValidateNotNullOrEmpty]
    public string? Title { get; set; }

    [Parameter(Mandatory = true, Position = 3)]
    [ValidateNotNullOrEmpty]
    public string? Body { get; set; }
    [Parameter]
    public MessageBoxType? Type { get; set; }
    [Parameter]
    public int? TimeoutSeconds { get; set; }
    protected override void ProcessRecord()
    {
        try
        {
            bool useAdvanced = Type.HasValue || TimeoutSeconds.HasValue;
            string title = Title ?? string.Empty;
            string body = Body ?? string.Empty;
            if (ParameterSetName == "BySessionInfo" && SessionInfo is not null)
            {
                ComputerName = SessionInfo.ComputerName;
                SessionId = SessionInfo.SessionId;
            }
            if (useAdvanced)
            {
                // we are expecting a response
                var response = WtsNative.SendMessage(
                    ComputerName!,
                    SessionId,
                    title,
                    body,
                    Type ?? MessageBoxType.BUTTON_OK,
                    TimeoutSeconds ?? 60,
                    true);
                WriteObject(response);
            }
            else
            {
                // fire and forget
                var (result, num) = WtsNative.SendMessage(ComputerName!, SessionId, title, body);
                WriteObject(result);
                WriteVerbose($"Response: {result} Number: {num}");
            }
        }
        catch (Exception ex)
        {
            WriteError(new ErrorRecord(
                ex,
                "SendTerminalMessage",
                ErrorCategory.OperationStopped,
                this));
        }
    }
}
