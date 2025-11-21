using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi;
using Xunit;

namespace Rystem.PlayFramework.Test
{
    /// <summary>
    /// Tests for OpenAI pricing configuration and calculation
    /// </summary>
    public sealed class PricingConfigurationTest
    {
        private readonly ISceneManager _sceneManager;
        private readonly IServiceProvider _serviceProvider;

        public PricingConfigurationTest(ISceneManager sceneManager, IServiceProvider serviceProvider)
        {
            _sceneManager = sceneManager;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Test cost calculation accuracy - sum equals total
        /// </summary>
        [Fact]
        public async Task CostCalculationAccuracyTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            
            var responses = await ExecuteTurnAsync("What is 2+2?", conversationKey);

            var responsesWithCost = responses.Where(r => r.Cost.HasValue).ToList();
            if (responsesWithCost.Any())
            {
                // Verify sum equals total
                var sumOfCosts = responsesWithCost.Sum(r => r.Cost!.Value);
                var totalCost = responses.Last().TotalCost!.Value;
                
                // Allow for floating point precision issues
                var difference = Math.Abs(sumOfCosts - totalCost);
                Assert.True(difference < 0.000001m, 
                    $"Cost sum mismatch: Sum={sumOfCosts:F8}, Total={totalCost:F8}, Diff={difference:F8}");
            }
        }

        /// <summary>
        /// Test that costs are consistent for identical requests
        /// </summary>
        [Fact]
        public async Task ConsistentCostForIdenticalRequestsTest()
        {
            var question = "What is the capital of Italy?";
            
            // Execute same question twice in different conversations
            var costs = new List<decimal>();
            
            for (int i = 0; i < 2; i++)
            {
                var conversationKey = Guid.NewGuid().ToString();
                var responses = await ExecuteTurnAsync(question, conversationKey);
                
                var totalCost = responses.Last().TotalCost ?? 0;
                costs.Add(totalCost);
            }

            if (costs.All(c => c > 0))
            {
                // Costs should be similar (allowing for small variations)
                var averageCost = costs.Average();
                var maxDeviation = costs.Max(c => Math.Abs(c - averageCost));
                var maxDeviationPercentage = (maxDeviation / averageCost) * 100;

                Assert.True(maxDeviationPercentage < 20,
                    $"Cost inconsistency: Costs={string.Join(", ", costs.Select(c => c.ToString("F6")))}, Deviation={maxDeviationPercentage:F2}%");
            }
        }

        /// <summary>
        /// Test cost increases with longer conversations
        /// </summary>
        [Fact]
        public async Task CostIncreasesWithConversationLengthTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            
            var turnCosts = new List<decimal>();
            var questions = new[]
            {
                "What is 2+2?",
                "What is 3+3?",
                "What is 4+4?"
            };

            foreach (var question in questions)
            {
                var responses = await ExecuteTurnAsync(question, conversationKey);
                var totalCost = responses.Last().TotalCost ?? 0;
                turnCosts.Add(totalCost);
            }

            // Verify costs are monotonically increasing
            for (int i = 1; i < turnCosts.Count; i++)
            {
                Assert.True(turnCosts[i] >= turnCosts[i - 1],
                    $"Cost should increase or stay same: Turn {i}={turnCosts[i]:F6} should be >= Turn {i-1}={turnCosts[i-1]:F6}");
            }

            var finalCost = turnCosts.Last();
            Assert.True(finalCost > 0, "Final accumulated cost should be greater than 0");
        }

        /// <summary>
        /// Test that cost values are within expected ranges
        /// </summary>
        [Fact]
        public async Task CostValuesWithinExpectedRangesTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            var responses = await ExecuteTurnAsync("Simple question", conversationKey);

            var responsesWithCost = responses.Where(r => r.Cost.HasValue).ToList();
            
            foreach (var response in responsesWithCost)
            {
                Assert.True(response.Cost >= 0, "Cost should not be negative");
                Assert.True(response.Cost < 0.1m, 
                    $"Individual request cost seems too high: {response.Cost:F6}");
            }

            var totalCost = responses.Last().TotalCost ?? 0;
            if (totalCost > 0)
            {
                Assert.True(totalCost < 1.0m,
                    $"Total cost seems too high: {totalCost:F6}");
            }
        }

        /// <summary>
        /// Test cost precision to 6 decimal places
        /// </summary>
        [Fact]
        public async Task CostPrecisionSixDecimalPlacesTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            var responses = await ExecuteTurnAsync("Test precision", conversationKey);

            var responsesWithCost = responses.Where(r => r.Cost.HasValue).ToList();
            
            foreach (var response in responsesWithCost)
            {
                var formattedCost = response.Cost!.Value.ToString("F6");
                var parsedCost = decimal.Parse(formattedCost);
                
                var difference = Math.Abs(response.Cost.Value - parsedCost);
                Assert.True(difference < 0.0000001m,
                    $"Cost precision issue: Original={response.Cost.Value}, Formatted={parsedCost}");
            }
        }

        /// <summary>
        /// Test that null costs are handled correctly
        /// </summary>
        [Fact]
        public async Task NullCostHandlingTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            var responses = await ExecuteTurnAsync("Test null handling", conversationKey);

            // TotalCost should always be present
            foreach (var response in responses.Where(r => r.TotalCost.HasValue))
            {
                Assert.True(response.TotalCost >= 0, "TotalCost should never be negative");
            }

            // Final response should always have TotalCost
            var lastResponse = responses.Last();
            Assert.NotNull(lastResponse.TotalCost);
        }

        /// <summary>
        /// Test cost breakdown message format (no currency symbols)
        /// </summary>
        [Fact]
        public async Task CostBreakdownMessageFormatTest()
        {
            var conversationKey = Guid.NewGuid().ToString();
            var responses = await ExecuteTurnAsync("Test cost message", conversationKey);

            var finalResponses = responses.Where(r => 
                r.Status == AiResponseStatus.FinishedOk && 
                r.Message != null &&
                r.Message.Contains("cost", StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var response in finalResponses)
            {
                // Verify no currency symbols
                Assert.DoesNotContain("$", response.Message);
                Assert.DoesNotContain("€", response.Message);
                Assert.DoesNotContain("£", response.Message);
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
