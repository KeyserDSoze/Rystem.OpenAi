using System.Text.Json;
using Rystem.OpenAi;
using Rystem.OpenAi.Chat;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.PlayFramework.Test
{
    /// <summary>
    /// Helper class to validate AI responses using LLM
    /// </summary>
    public static class ResponseValidator
    {
        /// <summary>
        /// Validates if the final response contains all required information to answer the user's question
        /// </summary>
        /// <param name="userQuestion">The original user question</param>
        /// <param name="responses">All responses from the scene manager</param>
        /// <param name="serviceProvider">Service provider to get OpenAI client</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Validation result</returns>
        public static async Task<ValidationResult> ValidateResponseAsync(
            string userQuestion,
            List<AiSceneResponse> responses,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default)
        {
            // Get the OpenAI client
            var openAiFactory = serviceProvider.GetRequiredService<IFactory<IOpenAi>>();
            var chatClient = openAiFactory.Create("Azure2")?.Chat;
            
            if (chatClient == null)
            {
                return new ValidationResult
                {
                    IsValid = false,
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
                    IsValid = false,
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
Your job is to verify if the AI's responses fully answer the user's original question.

VALIDATION CRITERIA:
1. Does the response directly address the user's question?
2. Is all requested information present?
3. Are there any missing pieces that the user asked for?
4. Is the response coherent and complete?

You MUST call the ValidateResponse function with your assessment.");

            chatClient.AddUserMessage($@"Validate if these AI responses fully answer the user's question:

User Question: {userQuestion}

AI Responses: {JsonSerializer.Serialize(finalMessages, new JsonSerializerOptions { WriteIndented = true })}

Context (tools called, scenes used): {JsonSerializer.Serialize(validationRequest.context, new JsonSerializerOptions { WriteIndented = true })}");

            var response = await chatClient.ExecuteAsync(cancellationToken);
            var toolCall = response?.Choices?[0]?.Message?.ToolCalls?.FirstOrDefault();

            if (toolCall?.Function?.Arguments == null)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Reasoning = "No validation response from LLM"
                };
            }

            try
            {
                return JsonSerializer.Deserialize<ValidationResult>(toolCall.Function.Arguments) ?? new ValidationResult
                {
                    IsValid = false,
                    Reasoning = "Failed to parse validation result"
                };
            }
            catch (JsonException ex)
            {
                return new ValidationResult
                {
                    IsValid = false,
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

            parameters.AddPrimitive("is_valid", new FunctionToolPrimitiveProperty
            {
                Type = "boolean",
                Description = "True if the AI responses fully answer the user's question"
            });

            parameters.AddPrimitive("reasoning", new FunctionToolPrimitiveProperty
            {
                Type = "string",
                Description = "Detailed explanation of why the response is valid or invalid"
            });

            parameters.AddPrimitive("missing_information", new FunctionToolPrimitiveProperty
            {
                Type = "string",
                Description = "What information is missing (if invalid), null if valid"
            });

            parameters.AddNumber("completeness_score", new FunctionToolNumberProperty
            {
                Type = "number",
                Description = "Score from 0 to 1 indicating how complete the answer is",
                Minimum = 0,
                Maximum = 1
            });

            parameters.AddNumber("relevance_score", new FunctionToolNumberProperty
            {
                Type = "number",
                Description = "Score from 0 to 1 indicating how relevant the answer is",
                Minimum = 0,
                Maximum = 1
            });

            parameters.AddRequired("is_valid", "reasoning");

            return new FunctionTool
            {
                Name = "ValidateResponse",
                Description = "Validates if AI responses fully answer the user's question",
                Parameters = parameters
            };
        }
    }

    /// <summary>
    /// Result of response validation
    /// </summary>
    public class ValidationResult
    {
        [System.Text.Json.Serialization.JsonPropertyName("is_valid")]
        public required bool IsValid { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("reasoning")]
        public required string Reasoning { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("missing_information")]
        public string? MissingInformation { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("completeness_score")]
        public double? CompletenessScore { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("relevance_score")]
        public double? RelevanceScore { get; set; }
    }
}
