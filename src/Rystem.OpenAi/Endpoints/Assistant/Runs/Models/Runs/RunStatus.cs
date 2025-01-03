namespace Rystem.OpenAi.Assistant
{
    public enum RunStatus
    {
        Queued,
        InProgress,
        RequiresAction,
        Cancelling,
        Cancelled,
        Failed,
        Completed,
        Incomplete,
        Expired
    }
}
