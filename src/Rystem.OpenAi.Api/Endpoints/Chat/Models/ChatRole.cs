namespace Rystem.OpenAi.Chat
{
    public enum ChatRole
    {
        User,
        System,
        Assistant
    }
    public static class ChatRoleExtensions
    {
        private const string UserLabel = "user";
        private const string SystemLabel = "system";
        private const string AssistantLabel = "assistant";
        public static string AsString(this ChatRole chatRole)
        {
            switch (chatRole)
            {
                case ChatRole.User:
                    return UserLabel;
                case ChatRole.Assistant:
                    return AssistantLabel;
                default:
                case ChatRole.System:
                    return SystemLabel;
            }
        }
    }
}
