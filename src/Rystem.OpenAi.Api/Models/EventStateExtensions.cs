namespace Rystem.OpenAi.Extensions
{
    public static class EventStateExtensions
    {
        public static EventState FromString(this string value)
        {
            switch (value)
            {
                case "notrunning":
                    return EventState.NotRunning;
                case "pending":
                    return EventState.Pending;
                case "running":
                    return EventState.Running;
                case "succeeded":
                    return EventState.Succeeded;
                case "failed":
                    return EventState.Failed;
                case "canceled":
                    return EventState.Canceled;
                case "deleted":
                    return EventState.Deleted;
                default:
                    return EventState.None;
            }
        }
    }
}
