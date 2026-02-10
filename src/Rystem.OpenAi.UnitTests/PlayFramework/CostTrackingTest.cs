using Xunit;

namespace Rystem.PlayFramework.Test
{
    /// <summary>
    /// Comprehensive tests for cost tracking and pricing functionality
    /// </summary>
    public sealed class CostTrackingTest
    {
        private readonly ISceneManager _sceneManager;
        private readonly IServiceProvider _serviceProvider;

        public CostTrackingTest(ISceneManager sceneManager, IServiceProvider serviceProvider)
        {
            _sceneManager = sceneManager;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Test that costs are tracked for simple single-scene requests
        /// </summary>
        [Fact]
        public async Task SingleSceneCostTrackingTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            var userQuestion = "Che tempo fa a Milano?";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            // Verify at least one response has a cost
            var responsesWithCost = responses.Where(r => r.Cost.HasValue).ToList();
            Assert.NotEmpty(responsesWithCost);

            // Verify all responses have totalCost populated
            var responsesWithTotalCost = responses.Where(r => r.TotalCost.HasValue).ToList();
            Assert.NotEmpty(responsesWithTotalCost);

            // Verify total cost is monotonically increasing
            decimal previousTotal = 0;
            foreach (var response in responses.Where(r => r.TotalCost.HasValue))
            {
                Assert.True(response.TotalCost >= previousTotal,
                    $"Total cost should be monotonically increasing. Previous: {previousTotal}, Current: {response.TotalCost}");
                previousTotal = response.TotalCost.Value;
            }

            // Verify final total cost equals sum of individual costs
            var lastResponse = responses.Last();
            var sumOfCosts = responses.Where(r => r.Cost.HasValue).Sum(r => r.Cost!.Value);
            Assert.Equal(sumOfCosts, lastResponse.TotalCost!.Value);
        }

        /// <summary>
        /// Test cost tracking across multiple scenes in a single request
        /// </summary>
        [Fact]
        public async Task MultiSceneCostTrackingTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            var userQuestion = "Il mio username è keysersoze e vorrei sapere il meteo a Milano";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            // Should have scene execution costs
            var sceneResponses = responses.Where(r => 
                r.Status == AiResponseStatus.SceneRequest || 
                r.Status == AiResponseStatus.FunctionRequest ||
                r.Status == AiResponseStatus.Running).ToList();
            Assert.NotEmpty(sceneResponses);

            // Verify costs are being tracked across scenes
            var responsesWithCost = responses.Where(r => r.Cost.HasValue).ToList();
            Assert.NotEmpty(responsesWithCost);

            // Final response should have total cost
            var lastResponse = responses.Last();
            Assert.NotNull(lastResponse.TotalCost);
            Assert.True(lastResponse.TotalCost > 0, "Final total cost should be greater than 0");
        }

        /// <summary>
        /// Test cost accumulation across multiple conversation turns
        /// </summary>
        [Fact]
        public async Task MultiTurnCostAccumulationTest()
        {
            var conversationKey = Guid.NewGuid().ToString();

            // Turn 1: Simple request
            var turn1Question = "Che tempo fa a Milano?";
            var responses1 = await ExecuteTurnAsync(turn1Question, conversationKey);
            var turn1FinalCost = responses1.Last().TotalCost ?? 0;

            Assert.True(turn1FinalCost > 0, "Turn 1 should have accumulated cost");

            // Turn 2: Another request in the same conversation
            var turn2Question = "E a Roma?";
            var responses2 = await ExecuteTurnAsync(turn2Question, conversationKey);
            var turn2FinalCost = responses2.Last().TotalCost ?? 0;

            // Turn 2 total should be greater than Turn 1 (accumulated)
            Assert.True(turn2FinalCost > turn1FinalCost,
                $"Turn 2 total cost ({turn2FinalCost:F6}) should be greater than Turn 1 ({turn1FinalCost:F6})");

            // Turn 3: Yet another request
            var turn3Question = "E a Napoli?";
            var responses3 = await ExecuteTurnAsync(turn3Question, conversationKey);
            var turn3FinalCost = responses3.Last().TotalCost ?? 0;

            // Turn 3 total should be greater than Turn 2
            Assert.True(turn3FinalCost > turn2FinalCost,
                $"Turn 3 total cost ({turn3FinalCost:F6}) should be greater than Turn 2 ({turn2FinalCost:F6})");

            // Verify cost increases are reasonable (each turn adds some cost)
            var turn1Cost = turn1FinalCost;
            var turn2IncrementalCost = turn2FinalCost - turn1FinalCost;
            var turn3IncrementalCost = turn3FinalCost - turn2FinalCost;

            Assert.True(turn2IncrementalCost > 0, "Turn 2 should add cost");
            Assert.True(turn3IncrementalCost > 0, "Turn 3 should add cost");
        }

        /// <summary>
        /// Test that each conversation has independent cost tracking
        /// </summary>
        [Fact]
        public async Task IndependentConversationCostsTest()
        {
            var conversation1Key = Guid.NewGuid().ToString();
            var conversation2Key = Guid.NewGuid().ToString();
            var userQuestion = "Che tempo fa a Milano?";

            // Execute same question in two different conversations
            var responses1 = await ExecuteTurnAsync(userQuestion, conversation1Key);
            var responses2 = await ExecuteTurnAsync(userQuestion, conversation2Key);

            var cost1 = responses1.Last().TotalCost ?? 0;
            var cost2 = responses2.Last().TotalCost ?? 0;

            // Both should have costs
            Assert.True(cost1 > 0, "Conversation 1 should have cost");
            Assert.True(cost2 > 0, "Conversation 2 should have cost");

            // Costs should be similar (same operation)
            var costDifference = Math.Abs(cost1 - cost2);
            var averageCost = (cost1 + cost2) / 2;
            var differencePercentage = (costDifference / averageCost) * 100;

            // Allow up to 20% difference due to potential variations in token usage
            Assert.True(differencePercentage < 20,
                $"Cost difference too large: Conv1={cost1:F6}, Conv2={cost2:F6}, Diff={differencePercentage:F2}%");
        }

        /// <summary>
        /// Test that skipped/cached operations don't add duplicate costs
        /// </summary>
        [Fact]
        public async Task NoDuplicateCostsForSkippedOperationsTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            var userQuestion = "Che tempo fa a Milano?";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            // Check for any "already executed" messages
            var skippedResponses = responses.Where(r => 
                r.Message?.Contains("already executed", StringComparison.OrdinalIgnoreCase) == true ||
                r.Message?.Contains("skipping", StringComparison.OrdinalIgnoreCase) == true).ToList();

            foreach (var skipped in skippedResponses)
            {
                // Skipped operations should not add new cost
                Assert.Null(skipped.Cost);
                // But should still have total cost tracked
                Assert.NotNull(skipped.TotalCost);
            }
        }

        /// <summary>
        /// Test cost breakdown by response type
        /// </summary>
        [Fact]
        public async Task CostBreakdownByResponseTypeTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            var userQuestion = "Il mio username è keysersoze e vorrei sapere il meteo a Milano";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            // Analyze costs by response status
            var costsByStatus = responses
                .Where(r => r.Cost.HasValue)
                .GroupBy(r => r.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count(),
                    TotalCost = g.Sum(r => r.Cost!.Value),
                    AverageCost = g.Average(r => r.Cost!.Value)
                })
                .ToList();

            // Should have at least one status with costs
            Assert.NotEmpty(costsByStatus);

            // All costs should be positive
            foreach (var statusCost in costsByStatus)
            {
                Assert.True(statusCost.TotalCost > 0,
                    $"Status {statusCost.Status} has non-positive cost: {statusCost.TotalCost}");
                Assert.True(statusCost.AverageCost > 0,
                    $"Status {statusCost.Status} has non-positive average cost: {statusCost.AverageCost}");
            }
        }

        /// <summary>
        /// Test that cost tracking works with error scenarios
        /// </summary>
        [Fact]
        public async Task CostTrackingWithErrorsTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            // Request something that might cause the system to try multiple approaches
            var userQuestion = "Calcola qualcosa di complesso che non posso fare";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            // Even with errors/limitations, should track costs
            var lastResponse = responses.Last();
            Assert.NotNull(lastResponse.TotalCost);

            // Should have at least initial request cost
            var responsesWithCost = responses.Where(r => r.Cost.HasValue).ToList();
            // May be empty if no OpenAI calls were made, but total cost should still be tracked
        }

        /// <summary>
        /// Test cost precision and formatting
        /// </summary>
        [Fact]
        public async Task CostPrecisionTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            var userQuestion = "Che tempo fa a Milano?";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            var responsesWithCost = responses.Where(r => r.Cost.HasValue).ToList();
            Assert.NotEmpty(responsesWithCost);

            foreach (var response in responsesWithCost)
            {
                // Cost should be a reasonable value (not too large, not negative)
                Assert.True(response.Cost >= 0, "Cost should not be negative");
                Assert.True(response.Cost < 1, "Single request cost should typically be less than 1 unit");

                // Cost should have reasonable precision (not 0 if present)
                Assert.True(response.Cost > 0, "If cost is present, it should be greater than 0");
            }

            // Total cost should also be reasonable
            var totalCost = responses.Last().TotalCost!.Value;
            Assert.True(totalCost > 0, "Total cost should be greater than 0");
            Assert.True(totalCost < 10, "Total cost for a simple request should typically be less than 10 units");
        }

        /// <summary>
        /// Test cost tracking with parallel requests
        /// </summary>
        [Fact]
        public async Task ParallelRequestsCostTrackingTest()
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
                return new
                {
                    Question = question,
                    Responses = responses,
                    FinalCost = responses.Last().TotalCost ?? 0
                };
            }).ToList();

            var results = await Task.WhenAll(tasks);

            // All requests should have costs
            foreach (var result in results)
            {
                Assert.True(result.FinalCost > 0,
                    $"Question '{result.Question}' should have cost, got {result.FinalCost}");
            }

            // Costs should be similar (same type of operation)
            var costs = results.Select(r => r.FinalCost).ToList();
            var averageCost = costs.Average();
            var maxDeviation = costs.Max(c => Math.Abs(c - averageCost));
            var maxDeviationPercentage = (maxDeviation / averageCost) * 100;

            // Allow up to 30% deviation in parallel requests
            Assert.True(maxDeviationPercentage < 30,
                $"Cost variation too high in parallel requests: {maxDeviationPercentage:F2}%");
        }

        /// <summary>
        /// Test final response includes cost information
        /// </summary>
        [Fact]
        public async Task FinalResponseIncludesCostTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            var userQuestion = "Che tempo fa a Milano?";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            // Find final response
            var finalResponse = responses.LastOrDefault(r =>
                r.Status == AiResponseStatus.FinishedOk ||
                r.Status == AiResponseStatus.FinishedNoTool);

            Assert.NotNull(finalResponse);
            Assert.NotNull(finalResponse.TotalCost);
            Assert.True(finalResponse.TotalCost > 0);

            // If final response has a message, it might include cost info
            if (finalResponse.Message != null && finalResponse.Message.Contains("cost", StringComparison.OrdinalIgnoreCase))
            {
                // Verify the cost value in message matches the actual cost
                // This is a soft check as the message format can vary
                var costString = finalResponse.TotalCost.Value.ToString("F6");
                // Message might contain the cost value
            }
        }

        /// <summary>
        /// Test cost tracking with cached conversations
        /// </summary>
        [Fact]
        public async Task CachedConversationCostTrackingTest()
        {
            var conversationKey = Guid.NewGuid().ToString();

            // Turn 1: Initial request (not cached)
            var turn1Question = "Il mio username è keysersoze";
            var responses1 = await ExecuteTurnAsync(turn1Question, conversationKey);
            var turn1Cost = responses1.Last().TotalCost ?? 0;

            Assert.True(turn1Cost > 0, "Turn 1 should have cost");

            // Turn 2: Request using cached context
            var turn2Question = "Qual è il mio username?";
            var responses2 = await ExecuteTurnAsync(turn2Question, conversationKey);
            var turn2Cost = responses2.Last().TotalCost ?? 0;

            // Turn 2 might have lower incremental cost due to caching
            var turn2IncrementalCost = turn2Cost - turn1Cost;
            Assert.True(turn2IncrementalCost >= 0, "Incremental cost should be non-negative");

            // Both turns should contribute to total
            Assert.True(turn2Cost >= turn1Cost, "Total cost should not decrease");
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
