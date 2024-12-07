using Microsoft.Extensions.DependencyInjection;

namespace Rystem.PlayFramework
{
    internal sealed class MainDirector : IDirector
    {
        private readonly PlayHandler _playHandler;
        private readonly IFactory<IScene> _sceneFactory;

        public MainDirector(PlayHandler playHandler,
            IFactory<IScene> sceneFactory)
        {
            _playHandler = playHandler;
            _sceneFactory = sceneFactory;
        }

        public async Task<DirectorResponse> DirectAsync(SceneContext context, SceneRequestSettings requestSettings, CancellationToken cancellationToken)
        {
            if (context.CreateNewDefaultChatClient != null)
            {
                var lastMessage = context.Responses.Where(x => x.Message != null).LastOrDefault()?.Message;
                if (lastMessage != null)
                {
                    var chatClient = context.CreateNewDefaultChatClient();
                    var usedScenes = context.Responses.Where(x => x.Name != null).Select(x => x.Name!).Distinct().ToList();
                    var avoidableScenes = usedScenes.ToList();
                    if (requestSettings.ScenesToAvoid != null)
                        avoidableScenes.AddRange(requestSettings.ScenesToAvoid);
                    var scenes = _playHandler.GetScenes(avoidableScenes);
                    chatClient = chatClient
                        .AddSystemMessage($"I'm putting in assistant message the last message you provide for a user request. You need only understand if assistant has responded to the user or not. If it does respond with the word 'Yes' otherwise with the word 'No'. Do not provide further information, only these two words are allowed responses. You can say 'No' if you think that this functions can help you to get further information for the task, the functions with name and descriptions will be passed as next system messages.");
                    foreach (var scene in scenes)
                    {
                        var currentScene = _sceneFactory.Create(scene);
                        if (currentScene != null)
                            chatClient = chatClient
                                .AddSystemMessage($"Name: {currentScene.Name}, Description: {currentScene.Description}");
                    }
                    chatClient
                        .AddAssistantMessage(lastMessage)
                        .AddUserMessage(context.InputMessage);
                    var response = await chatClient.ExecuteAsync(cancellationToken);
                    return new DirectorResponse
                    {
                        CutScenes = usedScenes,
                        ExecuteAgain = response.Choices?[0]?.Message?.Content?.ToLower() == No
                    };
                }
            }
            return new DirectorResponse
            {
                ExecuteAgain = false
            };
        }
        private const string No = "no";
    }
}
