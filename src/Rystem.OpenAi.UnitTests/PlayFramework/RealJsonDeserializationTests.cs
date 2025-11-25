using System.Text.Json;
using Rystem.OpenAi;
using Rystem.PlayFramework;
using Xunit;

namespace Rystem.OpenAi.UnitTests.PlayFramework
{
    /// <summary>
    /// Tests for JSON deserialization through the ACTUAL PlayFramework pipeline
    /// Tests the real flow: LLM → FunctionsHandler → SceneBuilder → Service
    /// </summary>
    public sealed class RealJsonDeserializationTests
    {
        private readonly ISceneManager _sceneManager;
        private readonly IServiceProvider _serviceProvider;

        public RealJsonDeserializationTests(ISceneManager sceneManager, IServiceProvider serviceProvider)
        {
            _sceneManager = sceneManager;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Test that DateOnly with time component works through the real pipeline
        /// </summary>
        [Fact]
        public async Task DateOnly_WithTimeComponent_ShouldWorkThroughPipeline()
        {
            var conversationKey = Guid.NewGuid().ToString();
            
            // This will trigger a scene that uses DateOnly parameters
            var userQuestion = "mostrami le richieste di ferie dal 15 gennaio 2024 al 20 gennaio 2024";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            // Should complete without deserialization errors
            Assert.NotEmpty(responses);
            
            // Should have function requests (meaning deserialization worked)
            var functionRequests = responses.Where(r => r.Status == AiResponseStatus.FunctionRequest).ToList();
            
            // If there were function requests, verify they completed without errors
            if (functionRequests.Any())
            {
                // Check that responses include the function results, not errors
                var hasResults = responses.Any(r => 
                    r.Status == AiResponseStatus.Running && 
                    r.Response != null);
                
                Assert.True(hasResults || functionRequests.Count == 0, 
                    "Function requests should complete successfully or not be executed");
            }
        }

        /// <summary>
        /// Test that enum values work correctly through the pipeline
        /// The FlexibleEnumConverter should handle both string and number formats
        /// </summary>
        [Fact]
        public async Task Enum_FlexibleConversion_ShouldWorkThroughPipeline()
        {
            var conversationKey = Guid.NewGuid().ToString();
            
            // This will trigger a scene with enum parameters
            var userQuestion = "mostrami le richieste di ferie approvate";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            // Should complete without deserialization errors
            Assert.NotEmpty(responses);
            
            // Verify no JsonException in responses
            var hasJsonError = responses.Any(r => 
                r.Message != null && 
                r.Message.Contains("JsonException", StringComparison.OrdinalIgnoreCase));
            
            Assert.False(hasJsonError, "Should not have JSON deserialization errors");
        }

        /// <summary>
        /// Test TimeOnly formats work through the pipeline
        /// </summary>
        [Fact]
        public async Task TimeOnly_VariousFormats_ShouldWorkThroughPipeline()
        {
            var conversationKey = Guid.NewGuid().ToString();
            
            // Request with time specifications
            var userQuestion = "vorrei richiedere ferie dal 15 giugno dalle 9:00 al 17 giugno alle 18:00";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            // Should complete without deserialization errors
            Assert.NotEmpty(responses);
            
            // Check for successful completion
            var hasFinished = responses.Any(r => 
                r.Status == AiResponseStatus.FinishedOk || 
                r.Status == AiResponseStatus.FinishedNoTool);
            
            Assert.True(hasFinished, "Request should complete successfully");
        }

        /// <summary>
        /// Test that the pipeline handles missing optional parameters correctly
        /// </summary>
        [Fact]
        public async Task OptionalParameters_ShouldWorkThroughPipeline()
        {
            var conversationKey = Guid.NewGuid().ToString();
            
            // Simple request without all optional parameters
            var userQuestion = "mostrami le mie richieste di ferie";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            Assert.NotEmpty(responses);
            
            // Verify successful execution
            var hasError = responses.Any(r => 
                r.Message != null && 
                (r.Message.Contains("error", StringComparison.OrdinalIgnoreCase) ||
                 r.Message.Contains("exception", StringComparison.OrdinalIgnoreCase)));
            
            Assert.False(hasError, "Should handle optional parameters without errors");
        }

        /// <summary>
        /// Test real-world complex scenario with multiple types
        /// </summary>
        [Fact]
        public async Task ComplexRequest_WithMultipleTypes_ShouldWorkThroughPipeline()
        {
            var conversationKey = Guid.NewGuid().ToString();
            
            // Complex request with dates, times, arrays
            var userQuestion = "vorrei richiedere 3 giorni di ferie dal 15 al 17 giugno per vacanza, con approvatore manager@test.com";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            Assert.NotEmpty(responses);
            
            // Count function calls
            var functionCalls = responses.Where(r => 
                r.Status == AiResponseStatus.FunctionRequest).ToList();
            
            // Should have attempted to execute functions
            // (May not complete if services are mocked, but should not have deserialization errors)
            var hasDeserializationError = responses.Any(r => 
                r.Message != null && 
                (r.Message.Contains("convert", StringComparison.OrdinalIgnoreCase) ||
                 r.Message.Contains("deserialize", StringComparison.OrdinalIgnoreCase) ||
                 r.Message.Contains("type", StringComparison.OrdinalIgnoreCase)));
            
            Assert.False(hasDeserializationError, 
                "Complex requests should not have type conversion errors");
        }

        /// <summary>
        /// Test that nullable fields are handled correctly
        /// </summary>
        [Fact]
        public async Task NullableFields_ShouldWorkThroughPipeline()
        {
            var conversationKey = Guid.NewGuid().ToString();
            
            // Request without optional time fields
            var userQuestion = "vorrei richiedere ferie dal 15 al 17 giugno tutto il giorno";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            Assert.NotEmpty(responses);
            
            // Verify no null handling errors
            var hasNullError = responses.Any(r => 
                r.Message != null && 
                r.Message.Contains("null", StringComparison.OrdinalIgnoreCase) &&
                r.Message.Contains("error", StringComparison.OrdinalIgnoreCase));
            
            Assert.False(hasNullError, "Should handle null values correctly");
        }

        /// <summary>
        /// Verify that cost tracking works through deserialization
        /// </summary>
        [Fact]
        public async Task CostTracking_ShouldWorkWithDeserialization()
        {
            var conversationKey = Guid.NewGuid().ToString();
            
            var userQuestion = "che tempo fa a Milano?";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            Assert.NotEmpty(responses);
            
            // Verify cost tracking is present
            var lastResponse = responses.Last();
            Assert.NotNull(lastResponse.TotalCost);
            
            // Cost should be accumulated correctly even with type conversions
            Assert.True(lastResponse.TotalCost >= 0, 
                "Total cost should be non-negative even after deserialization");
        }

        /// <summary>
        /// Test that array parameters work correctly
        /// </summary>
        [Fact]
        public async Task ArrayParameters_ShouldWorkThroughPipeline()
        {
            var conversationKey = Guid.NewGuid().ToString();
            
            // Request with multiple approvers (array)
            var userQuestion = "vorrei richiedere ferie con approvatori manager@test.com e director@test.com";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            Assert.NotEmpty(responses);
            
            // Verify no array deserialization errors
            var hasArrayError = responses.Any(r => 
                r.Message != null && 
                r.Message.Contains("array", StringComparison.OrdinalIgnoreCase) &&
                r.Message.Contains("error", StringComparison.OrdinalIgnoreCase));
            
            Assert.False(hasArrayError, "Should handle array parameters correctly");
        }

        /// <summary>
        /// Test GUID parameters work correctly
        /// </summary>
        [Fact]
        public async Task GuidParameters_ShouldWorkThroughPipeline()
        {
            var conversationKey = Guid.NewGuid().ToString();
            
            // Request that requires GUID (taskId)
            var userQuestion = "vorrei richiedere ferie di tipo Vacation";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            Assert.NotEmpty(responses);
            
            // Verify no GUID conversion errors
            var hasGuidError = responses.Any(r => 
                r.Message != null && 
                (r.Message.Contains("guid", StringComparison.OrdinalIgnoreCase) ||
                 r.Message.Contains("format", StringComparison.OrdinalIgnoreCase)) &&
                r.Message.Contains("error", StringComparison.OrdinalIgnoreCase));
            
            Assert.False(hasGuidError, "Should handle GUID parameters correctly");
        }

        /// <summary>
        /// Integration test: Full conversation with multiple deserializations
        /// </summary>
        [Fact]
        public async Task FullConversation_WithMultipleDeserializations_ShouldWork()
        {
            var conversationKey = Guid.NewGuid().ToString();

            // Turn 1: Request with dates
            var turn1 = "mostrami le richieste di ferie dal 1 gennaio al 31 dicembre 2024";
            var responses1 = await ExecuteTurnAsync(turn1, conversationKey);
            Assert.NotEmpty(responses1);

            // Turn 2: Request with time
            var turn2 = "vorrei richiedere ferie dal 15 giugno alle 9:00";
            var responses2 = await ExecuteTurnAsync(turn2, conversationKey);
            Assert.NotEmpty(responses2);

            // Turn 3: Request with enum filter
            var turn3 = "mostrami solo quelle approvate";
            var responses3 = await ExecuteTurnAsync(turn3, conversationKey);
            Assert.NotEmpty(responses3);

            // Verify no deserialization errors across all turns
            var allResponses = responses1.Concat(responses2).Concat(responses3);
            var hasDeserializationError = allResponses.Any(r => 
                r.Message != null && 
                (r.Message.Contains("JsonException", StringComparison.OrdinalIgnoreCase) ||
                 r.Message.Contains("deserialize", StringComparison.OrdinalIgnoreCase) ||
                 r.Message.Contains("convert", StringComparison.OrdinalIgnoreCase)));
            
            Assert.False(hasDeserializationError, 
                "Multi-turn conversation should not have deserialization errors");
        }

        /// <summary>
        /// Test that cache doesn't break deserialization
        /// </summary>
        [Fact]
        public async Task CachedConversation_ShouldMaintainDeserialization()
        {
            var conversationKey = Guid.NewGuid().ToString();

            // Turn 1: Establish context with complex types
            var turn1 = "vorrei richiedere ferie dal 15 al 17 giugno";
            var responses1 = await ExecuteTurnAsync(turn1, conversationKey);
            Assert.NotEmpty(responses1);

            // Turn 2: Use cached context (should still deserialize correctly)
            var turn2 = "aggiungi approvatore manager@test.com";
            var responses2 = await ExecuteTurnAsync(turn2, conversationKey);
            Assert.NotEmpty(responses2);

            // Verify deserialization works with cache
            var hasError = responses2.Any(r => 
                r.Message != null && 
                r.Message.Contains("error", StringComparison.OrdinalIgnoreCase));
            
            Assert.False(hasError, "Cached conversation should maintain deserialization");
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
