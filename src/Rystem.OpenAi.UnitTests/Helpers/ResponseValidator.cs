using System.Text.Json;
using Rystem.OpenAi;
using Rystem.OpenAi.Chat;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.PlayFramework.Test
{
    /// <summary>
    /// Helper class to validate AI responses using LLM with score-based validation
    /// </summary>
    public static class ResponseValidator
    {
        /// <summary>
        /// Default threshold for test passing (70%)
        /// </summary>
        public const int DefaultThreshold = 70;

        /// <summary>
        /// Validates if the final response contains all required information to answer the user's question
        /// Uses a score-based system (0-100) instead of boolean
        /// </summary>
        /// <param name="userQuestion">The original user question</param>
        /// <param name="responses">All responses from the scene manager</param>
        /// <param name="serviceProvider">Service provider to get OpenAI client</param>
        /// <param name="minScore">Minimum score required for test to pass (0-100). Default is 70.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Validation result with score</returns>
        public static async Task<ValidationResult> ValidateResponseAsync(
            string userQuestion,
            List<AiSceneResponse> responses,
            IServiceProvider serviceProvider,
            int minScore = DefaultThreshold,
            CancellationToken cancellationToken = default)
        {
            // Validate minScore parameter
            if (minScore < 0 || minScore > 100)
            {
                throw new ArgumentException("minScore must be between 0 and 100", nameof(minScore));
            }

            // Get the OpenAI client
            var openAiFactory = serviceProvider.GetRequiredService<IFactory<IOpenAi>>();
            var chatClient = openAiFactory.Create("Azure2")?.Chat;
            
            if (chatClient == null)
            {
                return new ValidationResult
                {
                    Score = 0,
                    MinScore = minScore,
                    Reasoning = "OpenAI client not available for validation"
                };
            }

            // Extract final messages from responses
            var finalMessages = responses
                .Where(r => r.Message != null && 
                           (r.Status == AiResponseStatus.Running || 
                            r.Status == AiResponseStatus.FinishedOk ||
                            r.Status == AiResponseStatus.FinishedNoTool))
                .Select(r => r.Message)
                .Where(m => !string.IsNullOrEmpty(m))
                .ToList();

            if (finalMessages.Count == 0)
            {
                return new ValidationResult
                {
                    Score = 0,
                    MinScore = minScore,
                    Reasoning = "No final messages found in responses"
                };
            }

            // Build validation request
            var validationRequest = new
            {
                user_question = userQuestion,
                ai_responses = finalMessages,
                context = BuildContext(responses)
            };

            // Create validation tool
            var validationTool = CreateValidationTool();
            chatClient.AddFunctionTool(validationTool);

            // Add system message for validation
            chatClient.AddSystemMessage(@"You are a quality assurance validator for AI responses.
Your job is to score how well the AI's responses answer the user's original question.

SCORING SYSTEM (0-100):
- 100: Perfect answer - All information present, accurate, and complete
- 90-99: Excellent - Minor details missing but essentially complete
- 80-89: Very Good - Most information present, minor gaps
- 70-79: Good - Core question answered, some details missing
- 60-69: Adequate - Basic answer present, significant gaps
- 50-59: Partial - Some relevant information, major gaps
- 30-49: Poor - Very incomplete or partially incorrect
- 10-29: Very Poor - Mostly irrelevant or incorrect
- 0-9: Failed - Completely wrong or no answer

EVALUATION CRITERIA:
1. Completeness: Does the response address ALL parts of the question?
2. Accuracy: Is the information correct and relevant?
3. Clarity: Is the answer clear and coherent?
4. Context: Does it use available tools/data appropriately?

Be strict but fair. Only give high scores (90+) for truly complete answers.

You MUST call the ValidateResponse function with your score and detailed assessment.");

            chatClient.AddUserMessage($@"Score these AI responses for answering the user's question (0-100):

User Question: {userQuestion}

AI Responses: {JsonSerializer.Serialize(finalMessages, new JsonSerializerOptions { WriteIndented = true })}

Context (tools called, scenes used): {JsonSerializer.Serialize(validationRequest.context, new JsonSerializerOptions { WriteIndented = true })}

Provide a score from 0 to 100 and explain your reasoning.");

            var response = await chatClient.ExecuteAsync(cancellationToken);
            var toolCall = response?.Choices?[0]?.Message?.ToolCalls?.FirstOrDefault();

            if (toolCall?.Function?.Arguments == null)
            {
                return new ValidationResult
                {
                    Score = 0,
                    MinScore = minScore,
                    Reasoning = "No validation response from LLM"
                };
            }

            try
            {
                var result = JsonSerializer.Deserialize<ValidationResult>(toolCall.Function.Arguments) ?? new ValidationResult
                {
                    Score = 0,
                    MinScore = minScore,
                    Reasoning = "Failed to parse validation result"
                };

                // Set the minimum score threshold
                result.MinScore = minScore;

                return result;
            }
            catch (JsonException ex)
            {
                return new ValidationResult
                {
                    Score = 0,
                    MinScore = minScore,
                    Reasoning = $"Error parsing validation: {ex.Message}"
                };
            }
        }

        private static object BuildContext(List<AiSceneResponse> responses)
        {
            return new
            {
                scenes_used = responses
                    .Where(r => r.Name != null && r.Name != "Request")
                    .Select(r => r.Name)
                    .Distinct()
                    .ToList(),
                tools_called = responses
                    .Where(r => r.FunctionName != null)
                    .Select(r => new { scene = r.Name, tool = r.FunctionName })
                    .ToList(),
                total_responses = responses.Count
            };
        }

        private static FunctionTool CreateValidationTool()
        {
            var parameters = new FunctionToolMainProperty();

            parameters.AddNumber("score", new FunctionToolNumberProperty
            {
                Type = "integer",
                Description = "Score from 0 to 100 indicating how well the response answers the question. 100 = perfect, 0 = completely wrong/no answer",
                Minimum = 0,
                Maximum = 100
            });

            parameters.AddPrimitive("reasoning", new FunctionToolPrimitiveProperty
            {
                Type = "string",
                Description = "Detailed explanation of the score: what was answered well, what was missing, and why this score"
            });

            parameters.AddPrimitive("missing_information", new FunctionToolPrimitiveProperty
            {
                Type = "string",
                Description = "Specific information that is missing or incorrect (empty if score is 100)"
            });

            parameters.AddNumber("completeness_percentage", new FunctionToolNumberProperty
            {
                Type = "integer",
                Description = "Percentage of the question that was answered (0-100)",
                Minimum = 0,
                Maximum = 100
            });

            parameters.AddNumber("accuracy_percentage", new FunctionToolNumberProperty
            {
                Type = "integer",
                Description = "Percentage of information that is accurate (0-100)",
                Minimum = 0,
                Maximum = 100
            });

            parameters.AddRequired("score", "reasoning");

            return new FunctionTool
            {
                Name = "ValidateResponse",
                Description = "Scores AI responses from 0 to 100 based on how well they answer the user's question",
                Parameters = parameters
            };
        }
    }

    /// <summary>
    /// Result of response validation with score-based system
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Score from 0 to 100 indicating how well the response answers the question
        /// 100 = perfect answer, 0 = completely wrong/no answer
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("score")]
        public required int Score { get; set; }

        /// <summary>
        /// Minimum score required for test to pass (set by caller, default 70)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("min_score")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        public int MinScore { get; set; } = ResponseValidator.DefaultThreshold;

        /// <summary>
        /// True if Score >= MinScore
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsValid => Score >= MinScore;

        /// <summary>
        /// Detailed explanation of the score
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("reasoning")]
        public required string Reasoning { get; set; }

        /// <summary>
        /// Specific information that is missing or incorrect
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("missing_information")]
        public string? MissingInformation { get; set; }

        /// <summary>
        /// Percentage of the question that was answered (0-100)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("completeness_percentage")]
        public int? CompletenessPercentage { get; set; }

        /// <summary>
        /// Percentage of information that is accurate (0-100)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("accuracy_percentage")]
        public int? AccuracyPercentage { get; set; }

        /// <summary>
        /// Legacy compatibility - maps to Score/100
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public double? CompletenessScore => CompletenessPercentage / 100.0;

        /// <summary>
        /// Legacy compatibility - maps to Score/100
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public double? RelevanceScore => AccuracyPercentage / 100.0;
    }
}
