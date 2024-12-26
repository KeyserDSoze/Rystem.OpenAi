namespace Rystem.OpenAi.Assistant
{
    internal static class RunStatusExtensions
    {
        public static RunStatus? ToRunStatus(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            return input switch
            {
                "queued" => RunStatus.Queued,
                "in_progress" => RunStatus.InProgress,
                "requires_action" => RunStatus.RequiresAction,
                "cancelling" => RunStatus.Cancelling,
                "cancelled" => RunStatus.Cancelled,
                "failed" => RunStatus.Failed,
                "completed" => RunStatus.Completed,
                "incomplete" => RunStatus.Incomplete,
                "expired" => RunStatus.Expired,
                _ => null
            };
        }
    }
}
