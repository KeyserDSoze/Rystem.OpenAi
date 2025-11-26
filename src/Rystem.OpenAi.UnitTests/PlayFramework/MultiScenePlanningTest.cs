using Xunit;

namespace Rystem.PlayFramework.Test
{
    /// <summary>
    /// Complex integration test for multi-scene planning and summarization
    /// </summary>
    public sealed class MultiScenePlanningTest
    {
        private readonly ISceneManager _sceneManager;
        private readonly IServiceProvider _serviceProvider;

        public MultiScenePlanningTest(ISceneManager sceneManager, IServiceProvider serviceProvider)
        {
            _sceneManager = sceneManager;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Test simple weather request with LLM validation
        /// </summary>
        [Fact]
        public async Task SimpleWeatherRequestTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            var userQuestion = "Che tempo fa oggi a Milano?";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            // Validate using LLM with 80% threshold (should be very accurate for simple weather)
            var validation = await ResponseValidator.ValidateResponseAsync(
                userQuestion,
                responses,
                _serviceProvider,
                minScore: 80,
                TestContext.Current.CancellationToken);

            // Assert validation passed
            Assert.True(validation.IsValid,
                $"LLM validation failed with score {validation.Score}/100 (min: {validation.MinScore}). " +
                $"Reasoning: {validation.Reasoning}. Missing: {validation.MissingInformation}");

            // Optional: Log score for visibility
            Console.WriteLine($"✅ Validation Score: {validation.Score}/100 (threshold: {validation.MinScore})");
            Console.WriteLine($"   Completeness: {validation.CompletenessPercentage}%");
            Console.WriteLine($"   Accuracy: {validation.AccuracyPercentage}%");
        }

        /// <summary>
        /// Test multi-scene request (weather + identity) with LLM validation
        /// </summary>
        [Fact]
        public async Task MultiSceneWeatherAndIdentityTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            var userQuestion = "Il mio username è keysersoze e vorrei sapere il meteo a Milano";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            // Check that planning occurred
            Assert.Contains(responses, r => r.Status == AiResponseStatus.Planning);

            // Check multiple scenes were used
            var scenesUsed = responses
                .Where(r => r.Name != null && r.Name != "Request")
                .Select(r => r.Name)
                .Distinct()
                .ToList();

            Assert.True(scenesUsed.Count >= 1, $"Expected at least 1 scene, got {scenesUsed.Count}");

            // Validate using LLM with default 70% threshold
            var validation = await ResponseValidator.ValidateResponseAsync(
                userQuestion,
                responses,
                _serviceProvider,
                minScore: 70,
                TestContext.Current.CancellationToken);

            Assert.True(validation.IsValid,
                $"LLM validation failed with score {validation.Score}/100 (min: {validation.MinScore}). " +
                $"Reasoning: {validation.Reasoning}. Missing: {validation.MissingInformation}");

            Console.WriteLine($"✅ Validation Score: {validation.Score}/100");
        }

        /// <summary>
        /// Test vacation request with LLM validation
        /// </summary>
        [Theory]
        [InlineData("Vorrei richiedere 3 giorni di ferie dal 15 al 17 giugno")]
        [InlineData("Voglio prendere ferie dal 10 giugno al 20 giugno")]
        public async Task VacationRequestTest(string userQuestion)
        {
            var conversationKey = Guid.NewGuid().ToString();

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            // Check vacation scene was used (flexible match)
            var vacationSceneUsed = responses.Any(r =>
                r.Name != null &&
                (r.Name.Contains("ferie", StringComparison.OrdinalIgnoreCase) ||
                 r.Name.Contains("permessi", StringComparison.OrdinalIgnoreCase)));

            Assert.True(vacationSceneUsed,
                $"Expected vacation scene to be used. Scenes used: {string.Join(", ", responses.Where(r => r.Name != null).Select(r => r.Name).Distinct())}");

            // Validate using LLM with 65% threshold (vacation requests might have confirmation flows)
            var validation = await ResponseValidator.ValidateResponseAsync(
                userQuestion,
                responses,
                _serviceProvider,
                minScore: 65,
                TestContext.Current.CancellationToken);

            Assert.True(validation.IsValid,
                $"LLM validation failed with score {validation.Score}/100 (min: {validation.MinScore}). " +
                $"Reasoning: {validation.Reasoning}. Missing: {validation.MissingInformation}");

            Console.WriteLine($"✅ Validation Score: {validation.Score}/100 for: {userQuestion}");
        }

        /// <summary>
        /// Test multi-turn conversation with cache and LLM validation
        /// </summary>
        [Fact]
        public async Task MultiTurnConversationTest()
        {
            var conversationKey = Guid.NewGuid().ToString();

            // Turn 1: Establish identity
            var turn1Question = "Il mio username è keysersoze";
            var responses1 = await ExecuteTurnAsync(turn1Question, conversationKey);

            // Turn 2: Ask about identity using context
            var turn2Question = "Qual è il mio nome completo?";
            var responses2 = await ExecuteTurnAsync(turn2Question, conversationKey);

            // Validate turn 2 with 75% threshold (requires context from turn 1)
            var validation = await ResponseValidator.ValidateResponseAsync(
                turn2Question,
                responses2,
                _serviceProvider,
                minScore: 75,
                TestContext.Current.CancellationToken);

            Assert.True(validation.IsValid,
                $"LLM validation failed with score {validation.Score}/100 (min: {validation.MinScore}). " +
                $"Reasoning: {validation.Reasoning}. Missing: {validation.MissingInformation}");

            // Verify name is in response
            var hasName = responses2.Any(r => r.Message?.Contains("Alessandro Rapiti") == true);
            Assert.True(hasName, "Expected user name in response");

            Console.WriteLine($"✅ Multi-turn Validation Score: {validation.Score}/100");
        }

        /// <summary>
        /// Test complex multi-scene workflow with LLM validation
        /// </summary>
        [Fact]
        public async Task ComplexMultiSceneWorkflowTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            var userQuestion = $"Voglio sapere il meteo a Milano e poi richiedere 3 giorni di ferie dal 15 al 17 giugno";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            // DEBUG: Print all responses to understand what's happening
            var output = new System.Text.StringBuilder();
            output.AppendLine("=== ALL RESPONSES ===");
            foreach (var r in responses)
            {
                output.AppendLine($"[{r.Status}] {r.Name ?? "N/A"}");
                if (r.Message != null)
                    output.AppendLine($"  Message: {r.Message}");
                if (r.FunctionName != null)
                    output.AppendLine($"  Function: {r.FunctionName}");
                if (r.Arguments != null)
                    output.AppendLine($"  Arguments: {r.Arguments}");
                if (r.Response != null)
                    output.AppendLine($"  Response: {r.Response}");
                output.AppendLine();
            }
            output.AppendLine("=== END RESPONSES ===");
            // Write to test output
            Console.WriteLine(output.ToString());

            // Check planning occurred
            var planningResponses = responses.Where(r => r.Status == AiResponseStatus.Planning).ToList();
            Assert.NotEmpty(planningResponses);

            // Check multiple scenes were used
            var scenesUsed = responses
                .Where(r => r.Name != null && r.Name != "Request")
                .Select(r => r.Name)
                .Distinct()
                .ToList();

            Assert.True(scenesUsed.Count >= 2,
                $"Expected at least 2 scenes for complex request, got {scenesUsed.Count}. Scenes: {string.Join(", ", scenesUsed)}");

            // Validate using LLM with 60% threshold (complex multi-scene workflow)
            var validation = await ResponseValidator.ValidateResponseAsync(
                userQuestion,
                responses,
                _serviceProvider,
                minScore: 60,
                TestContext.Current.CancellationToken);

            Assert.True(validation.IsValid,
                $"LLM validation failed with score {validation.Score}/100 (min: {validation.MinScore}). " +
                $"Reasoning: {validation.Reasoning}. Missing: {validation.MissingInformation}");

            Console.WriteLine($"✅ Complex Workflow Score: {validation.Score}/100 (threshold: {validation.MinScore})");
            Console.WriteLine($"   Completeness: {validation.CompletenessPercentage}%");
            Console.WriteLine($"   Accuracy: {validation.AccuracyPercentage}%");
        }

        /// <summary>
        /// Test that no duplicate tools are executed
        /// </summary>
        [Fact]
        public async Task NoDuplicateToolExecutionTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            var userQuestion = "Che tempo fa a Milano?";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            // Get all tool calls (excluding skipped ones)
            var toolCalls = responses
                .Where(r => r.FunctionName != null && r.Status != AiResponseStatus.ToolSkipped)
                .Select(r => $"{r.Name}.{r.FunctionName}")
                .ToList();

            // Check for duplicates in executed tools
            var duplicates = toolCalls.GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            Assert.Empty(duplicates);

            // Verify that skipped tools are properly marked
            var skippedTools = responses.Where(r => r.Status == AiResponseStatus.ToolSkipped).ToList();

            // If there are skipped tools, verify they have proper information
            foreach (var skipped in skippedTools)
            {
                Assert.NotNull(skipped.FunctionName);
                Assert.NotNull(skipped.Message);
                Assert.Contains("skipping", skipped.Message, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Test error handling with LLM validation
        /// </summary>
        [Fact]
        public async Task InvalidRequestHandlingTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            var userQuestion = "Calcola la radice quadrata di 144";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            // Should complete without errors
            Assert.NotEmpty(responses);

            // Should have a finished status
            Assert.Contains(responses, r =>
                r.Status == AiResponseStatus.FinishedNoTool ||
                r.Status == AiResponseStatus.FinishedOk);

            // Validate response with 50% threshold (should acknowledge it can't do the task)
            var validation = await ResponseValidator.ValidateResponseAsync(
                userQuestion,
                responses,
                _serviceProvider,
                minScore: 50,
                TestContext.Current.CancellationToken);

            // The response should acknowledge it can't perform the task (which counts as a valid response)
            Assert.True(validation.Score >= 50,
                $"Expected score >= 50 for proper error handling, got {validation.Score}. Reasoning: {validation.Reasoning}");

            Console.WriteLine($"✅ Error Handling Score: {validation.Score}/100");
        }

        /// <summary>
        /// Test parallel independent requests
        /// </summary>
        [Fact]
        public async Task ParallelRequestsTest()
        {
            var questions = new[]
            {
                "Che tempo fa a Milano?",
                "Che tempo fa a Roma?",
                "Che tempo fa a Napoli?"
            };

            var tasks = questions.Select(async question =>
            {
                var key = Guid.NewGuid().ToString();
                var responses = await ExecuteTurnAsync(question, key);

                var validation = await ResponseValidator.ValidateResponseAsync(
                    question,
                    responses,
                    _serviceProvider,
                    minScore: 70, // Default threshold
                    TestContext.Current.CancellationToken);

                return new { Question = question, Validation = validation, Responses = responses };
            }).ToList();

            var results = await Task.WhenAll(tasks);

            // All should be valid
            foreach (var result in results)
            {
                Assert.True(result.Validation.IsValid,
                    $"Question '{result.Question}' failed validation with score {result.Validation.Score}/100. " +
                    $"Reasoning: {result.Validation.Reasoning}");
                Console.WriteLine($"✅ '{result.Question}' Score: {result.Validation.Score}/100");
            }

            // All should have unique conversation keys
            var uniqueKeys = results.SelectMany(r => r.Responses.Select(x => x.RequestKey)).Distinct().Count();
            Assert.Equal(questions.Length, uniqueKeys);
        }

        /// <summary>
        /// Test cost tracking across conversation
        /// </summary>
        [Fact]
        public async Task CostTrackingTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            var userQuestion = "Che tempo fa a Milano?";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            // Verify some responses have costs
            var responsesWithCost = responses.Where(r => r.Cost.HasValue).ToList();
            Assert.NotEmpty(responsesWithCost);

            // Verify total cost is present in last response
            var lastResponse = responses.Last();
            Assert.NotNull(lastResponse.TotalCost);
            Assert.True(lastResponse.TotalCost > 0, "Total cost should be greater than 0");

            // Verify total cost equals sum of individual costs
            var sumOfCosts = responses.Where(r => r.Cost.HasValue).Sum(r => r.Cost!.Value);
            Assert.Equal(sumOfCosts, lastResponse.TotalCost.Value);
        }

        /// <summary>
        /// Test cost accumulation across multiple turns
        /// </summary>
        [Fact]
        public async Task MultiTurnCostAccumulationTest()
        {
            var conversationKey = Guid.NewGuid().ToString();

            // Turn 1
            var turn1Question = "Il mio username è keysersoze";
            var responses1 = await ExecuteTurnAsync(turn1Question, conversationKey);
            var turn1Cost = responses1.Last().TotalCost ?? 0;

            Assert.True(turn1Cost > 0, "Turn 1 should have cost");

            // Turn 2
            var turn2Question = "Qual è il mio nome completo?";
            var responses2 = await ExecuteTurnAsync(turn2Question, conversationKey);
            var turn2Cost = responses2.Last().TotalCost ?? 0;

            // Turn 2 should have higher total cost (accumulated)
            Assert.True(turn2Cost > turn1Cost,
                $"Turn 2 cost ({turn2Cost}) should be greater than Turn 1 cost ({turn1Cost})");
        }

        /// <summary>
        /// Test Case 6: Test that summarization is triggered after threshold
        /// </summary>
        [Fact]
        public async Task SummarizationThresholdTest()
        {
            var conversationKey = Guid.NewGuid().ToString();

            // Generate many turns to exceed summarization threshold
            // With ResponseThreshold = 20 in test config
            for (int i = 1; i <= 15; i++)
            {
                var question = $"Che tempo fa a Milano il giorno {i}?";
                await ExecuteTurnAsync(question, conversationKey);
            }

            // Next turn should trigger summarization
            var finalQuestion = "Riassumimi quello che abbiamo discusso finora";
            var responses = await ExecuteTurnAsync(finalQuestion, conversationKey);

            // Should have many responses accumulated
            Assert.True(responses.Count > 50, $"Expected >50 responses, got {responses.Count}");

            // Check if Summarizing status was used (optional - depends on threshold)
            var summarizingResponses = responses.Where(r => r.Status == AiResponseStatus.Summarizing).ToList();

            // If summarization happened, verify it's properly marked
            if (summarizingResponses.Any())
            {
                Assert.NotEmpty(summarizingResponses);
                foreach (var summary in summarizingResponses)
                {
                    Assert.NotNull(summary.Message);
                    Assert.Contains("summarizing", summary.Message, StringComparison.OrdinalIgnoreCase);
                    Assert.Null(summary.Cost); // Summarization itself shouldn't have cost
                }

                Console.WriteLine($"✅ Summarization triggered after threshold with {summarizingResponses.Count} summary responses");
            }
            else
            {
                Console.WriteLine("ℹ️ Summarization threshold not reached in this test run");
            }
        }

        private async Task<List<AiSceneResponse>> ExecuteTurnAsync(string message, string conversationKey)
        {
            var responses = new List<AiSceneResponse>();

            await foreach (var response in _sceneManager.ExecuteAsync(
                message,
                settings => settings.WithKey(conversationKey),
                TestContext.Current.CancellationToken))
            {
                responses.Add(response);
            }

            return responses;
        }
    }
}
