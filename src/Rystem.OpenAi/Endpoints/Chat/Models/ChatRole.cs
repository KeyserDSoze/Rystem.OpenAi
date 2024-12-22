namespace Rystem.OpenAi.Chat
{
    public enum ChatRole
    {
        User,
        System,
        Assistant,
        Developer,
        Tool,
        ToolCall
    }
    public static class ChatRoleExtensions
    {
        private const string UserLabel = "user";
        private const string SystemLabel = "system";
        private const string AssistantLabel = "assistant";
        private const string DeveloperLabel = "developer";
        private const string ToolLabel = "tool";
        private const string ToolCallsLabel = "tool_calls";
        public static string AsString(this ChatRole chatRole)
        {
            return chatRole switch
            {
                ChatRole.User => UserLabel,
                ChatRole.Assistant => AssistantLabel,
                ChatRole.Developer => DeveloperLabel,
                ChatRole.Tool => ToolLabel,
                ChatRole.ToolCall => ToolCallsLabel,
                _ => SystemLabel,
            };
        }
    }
}
