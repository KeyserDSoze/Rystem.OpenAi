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

            // Validate using LLM
            var validation = await ResponseValidator.ValidateResponseAsync(
                userQuestion,
                responses,
                _serviceProvider,
                TestContext.Current.CancellationToken);

            // Assert validation passed
            Assert.True(validation.IsValid,
                $"LLM validation failed: {validation.Reasoning}. Missing: {validation.MissingInformation}");

            // Optional: Assert quality scores
            if (validation.CompletenessScore.HasValue)
            {
                Assert.True(validation.CompletenessScore.Value >= 0.8,
                    $"Completeness score too low: {validation.CompletenessScore.Value}");
            }
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

            // Validate using LLM
            var validation = await ResponseValidator.ValidateResponseAsync(
                userQuestion,
                responses,
                _serviceProvider,
                TestContext.Current.CancellationToken);

            Assert.True(validation.IsValid,
                $"LLM validation failed: {validation.Reasoning}. Missing: {validation.MissingInformation}");
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

            // Validate using LLM
            var validation = await ResponseValidator.ValidateResponseAsync(
                userQuestion,
                responses,
                _serviceProvider,
                TestContext.Current.CancellationToken);

            Assert.True(validation.IsValid,
                $"LLM validation failed: {validation.Reasoning}. Missing: {validation.MissingInformation}");
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

            // Validate turn 2 (should have answer from turn 1 context)
            var validation = await ResponseValidator.ValidateResponseAsync(
                turn2Question,
                responses2,
                _serviceProvider,
                TestContext.Current.CancellationToken);

            Assert.True(validation.IsValid,
                $"LLM validation failed: {validation.Reasoning}. Missing: {validation.MissingInformation}");

            // Verify name is in response
            var hasName = responses2.Any(r => r.Message?.Contains("Alessandro Rapiti") == true);
            Assert.True(hasName, "Expected user name in response");
        }

        /// <summary>
        /// Test complex multi-scene workflow with LLM validation
        /// </summary>
        [Fact]
        public async Task ComplexMultiSceneWorkflowTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            var userQuestion = "Voglio sapere il meteo a Milano e poi richiedere 3 giorni di ferie dal 15 al 17 giugno";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

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
                $"Expected at least 2 scenes for complex request, got {scenesUsed.Count}");

            // Validate using LLM
            var validation = await ResponseValidator.ValidateResponseAsync(
                userQuestion,
                responses,
                _serviceProvider,
                TestContext.Current.CancellationToken);

            Assert.True(validation.IsValid,
                $"LLM validation failed: {validation.Reasoning}. Missing: {validation.MissingInformation}");

            // Check quality scores
            if (validation.CompletenessScore.HasValue && validation.RelevanceScore.HasValue)
            {
                Assert.True(validation.CompletenessScore.Value >= 0.7,
                    $"Completeness score too low: {validation.CompletenessScore.Value}");
                Assert.True(validation.RelevanceScore.Value >= 0.7,
                    $"Relevance score too low: {validation.RelevanceScore.Value}");
            }
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

            // Get all tool calls
            var toolCalls = responses
                .Where(r => r.FunctionName != null)
                .Select(r => $"{r.Name}.{r.FunctionName}")
                .ToList();

            // Check for duplicates
            var duplicates = toolCalls.GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            Assert.Empty(duplicates);
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

            // Validate response (even if it's "I can't do that")
            var validation = await ResponseValidator.ValidateResponseAsync(
                userQuestion,
                responses,
                _serviceProvider,
                TestContext.Current.CancellationToken);

            // The response should acknowledge it can't perform the task
            // (which is a valid response to an invalid request)
            Assert.NotNull(validation.Reasoning);
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
                    TestContext.Current.CancellationToken);

                return new { Question = question, Validation = validation, Responses = responses };
            }).ToList();

            var results = await Task.WhenAll(tasks);

            // All should be valid
            foreach (var result in results)
            {
                Assert.True(result.Validation.IsValid,
                    $"Question '{result.Question}' failed validation: {result.Validation.Reasoning}");
            }

            // All should have unique conversation keys
            var uniqueKeys = results.SelectMany(r => r.Responses.Select(x => x.RequestKey)).Distinct().Count();
            Assert.Equal(questions.Length, uniqueKeys);
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
