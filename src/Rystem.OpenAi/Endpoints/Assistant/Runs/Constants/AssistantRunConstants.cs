namespace Rystem.OpenAi.Assistant
{
    internal static class AssistantRunConstants
    {
        public static class Streaming
        {
            public const string StartingWith = "data: ";
            public const string Done = "[DONE]";
            public const string Event = "event: ";
        }
    }
}
