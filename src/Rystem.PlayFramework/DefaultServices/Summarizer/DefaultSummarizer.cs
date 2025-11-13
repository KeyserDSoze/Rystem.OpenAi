using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi;
using Rystem.OpenAi.Chat;

namespace Rystem.PlayFramework
{
    internal sealed class DefaultSummarizer : ISummarizer
    {
        private readonly IFactory<IOpenAi>? _openAiFactory;
        private readonly SceneManagerSettings? _settings;

        public DefaultSummarizer(
            IFactory<IOpenAi>? openAiFactory = null,
            SceneManagerSettings? settings = null)
        {
            _openAiFactory = openAiFactory;
            _settings = settings;
        }

        public bool ShouldSummarize(List<AiSceneResponse> responses)
        {
            if (_settings?.Summarization.Enabled == false)
            {
                return false;
            }

            var responseThreshold = _settings?.Summarization.ResponseThreshold ?? 50;
            var characterThreshold = _settings?.Summarization.CharacterThreshold ?? 10000;

            // Check response count threshold
            if (responses.Count >= responseThreshold)
            {
                return true;
            }

            // Check character count threshold
            var totalCharacters = responses.Sum(r =>
                (r.Message?.Length ?? 0) +
                (r.Response?.Length ?? 0) +
                (r.FunctionName?.Length ?? 0));

            return totalCharacters >= characterThreshold;
        }

        public async Task<string> SummarizeAsync(List<AiSceneResponse> responses, CancellationToken cancellationToken)
        {
            // Try to use OpenAI for intelligent summarization
            if (_openAiFactory != null && _settings?.OpenAi.Name != null)
            {
                try
                {
                    return await SummarizeWithOpenAiAsync(responses, cancellationToken);
                }
                catch
                {
                    // Fallback to simple summarization if OpenAI fails
                    return BuildSimpleSummary(responses);
                }
            }

            // Fallback to simple summarization
            return BuildSimpleSummary(responses);
        }

        private async Task<string> SummarizeWithOpenAiAsync(List<AiSceneResponse> responses, CancellationToken cancellationToken)
        {
            var chatClient = _openAiFactory!.Create(_settings?.OpenAi.Name)?.Chat;
            if (chatClient == null)
            {
                return BuildSimpleSummary(responses);
            }

            // Build the conversation history to summarize
            var conversationHistory = BuildConversationHistory(responses);

            // Create summarization prompt
            chatClient
                .AddSystemMessage(@"You are an expert at summarizing conversation histories. 
Create a concise but comprehensive summary of the following conversation between a user and an AI assistant.
Focus on:
1. What the user requested
2. Which scenes/tools were used
3. Key results and outcomes
4. Important data or information exchanged

Keep the summary structured and factual. Use bullet points for clarity.")
                .AddUserMessage($@"Please summarize this conversation history:

{conversationHistory}

Provide a clear, concise summary that captures all essential information.");

            var response = await chatClient.ExecuteAsync(cancellationToken);
            var summary = response?.Choices?[0]?.Message?.Content;

            if (!string.IsNullOrWhiteSpace(summary))
            {
                return $"Conversation Summary:\n\n{summary}";
            }

            // Fallback if no content returned
            return BuildSimpleSummary(responses);
        }

        private static string BuildConversationHistory(List<AiSceneResponse> responses)
        {
            var history = new StringBuilder();
            var counter = 1;

            foreach (var response in responses)
            {
                history.AppendLine($"{counter}. [{response.Status}] {response.ResponseTime:yyyy-MM-dd HH:mm:ss}");

                if (!string.IsNullOrWhiteSpace(response.Name) && response.Name != ScenesBuilder.Request)
                {
                    history.AppendLine($"   Scene: {response.Name}");
                }

                if (!string.IsNullOrWhiteSpace(response.FunctionName))
                {
                    history.AppendLine($"   Tool: {response.FunctionName}");
                }

                if (response.Arguments != null)
                {
                    var argsPreview = response.Arguments.ToString();
                    if (argsPreview?.Length > 200)
                        argsPreview = argsPreview.Substring(0, 200) + "...";
                    history.AppendLine($"   Arguments: {argsPreview}");
                }

                if (!string.IsNullOrWhiteSpace(response.Response))
                {
                    var responsePreview = response.Response;
                    if (responsePreview.Length > 300)
                        responsePreview = responsePreview.Substring(0, 300) + "...";
                    history.AppendLine($"   Response: {responsePreview}");
                }

                if (!string.IsNullOrWhiteSpace(response.Message))
                {
                    history.AppendLine($"   Message: {response.Message}");
                }

                history.AppendLine();
                counter++;
            }

            return history.ToString();
        }

        private static string BuildSimpleSummary(List<AiSceneResponse> responses)
        {
            var summary = new StringBuilder();
            summary.AppendLine("Summary of previous actions:");
            summary.AppendLine();

            var groupedByScene = responses
                .Where(r => r.Name != null && r.Name != ScenesBuilder.Request)
                .GroupBy(r => r.Name);

            foreach (var sceneGroup in groupedByScene)
            {
                summary.AppendLine($"Scene: {sceneGroup.Key}");

                var functions = sceneGroup
                    .Where(r => r.FunctionName != null)
                    .Select(r => r.FunctionName)
                    .Distinct();

                if (functions.Any())
                {
                    summary.AppendLine($"  Tools used: {string.Join(", ", functions)}");
                }

                var lastMessage = sceneGroup
                    .Where(r => r.Message != null && r.Status == AiResponseStatus.Running)
                    .LastOrDefault();

                if (lastMessage != null)
                {
                    summary.AppendLine($"  Result: {lastMessage.Message}");
                }
                summary.AppendLine();
            }

            var finalResponses = responses
                .Where(r => (r.Status & (AiResponseStatus.FinishedOk | AiResponseStatus.FinishedNoTool)) != 0)
                .ToList();

            if (finalResponses.Any())
            {
                summary.AppendLine("Final outcomes:");
                foreach (var finalResponse in finalResponses)
                {
                    if (!string.IsNullOrWhiteSpace(finalResponse.Message))
                    {
                        summary.AppendLine($"  - {finalResponse.Message}");
                    }
                }
            }

            return summary.ToString();
        }
    }
}
