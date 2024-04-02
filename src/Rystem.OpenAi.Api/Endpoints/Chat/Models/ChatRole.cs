namespace Rystem.OpenAi.Chat
{
    public enum ChatRole
    {
        User,
        System,
        Assistant,
        Tool,
    }
    public static class ChatRoleExtensions
    {
        private const string UserLabel = "user";
        private const string SystemLabel = "system";
        private const string AssistantLabel = "assistant";
        private const string ToolLabel = "tool";
        public static string AsString(this ChatRole chatRole)
        {
            return chatRole switch
            {
                ChatRole.User => UserLabel,
                ChatRole.Assistant => AssistantLabel,
                ChatRole.Tool => ToolLabel,
                _ => SystemLabel,
            };
        }
    }
}
