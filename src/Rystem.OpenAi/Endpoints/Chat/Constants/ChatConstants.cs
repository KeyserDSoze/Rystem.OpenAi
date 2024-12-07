namespace Rystem.OpenAi.Chat
{
    internal static class ChatConstants
    {
        public static class ToolType
        {
            public const string Function = "function";
        }
        public static class ToolChoice
        {
            public const string Auto = "auto";
            public const string None = "none";
            public const string Required = "required";
        }
        public static class ResolutionVision
        {
            public const string High = "high";
            public const string Low = "low";
            public const string Auto = "auto";
        }
        public static class ContentType
        {
            public const string Text = "text";
            public const string Image = "image_url";
        }
        public static class FinishReason
        {
            public const string Null = "null";
            public const string FunctionAutoExecuted = "functionAutoExecuted";
            public const string FunctionExecuted = "functionExecuted";
            public const string Stop = "stop";
        }
        public static class Streaming
        {
            public const string StartingWith = "data: ";
            public const string Done = "[DONE]";
        }
        public static class ServiceTier
        {
            public const string Auto = "auto";
            public const string Default = "default";
        }
    }
}
