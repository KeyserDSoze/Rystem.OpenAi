using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi;
using Rystem.PlayFramework;
using Xunit;

namespace Rystem.OpenAi.UnitTests.PlayFramework
{
    /// <summary>
    /// Isolated tests for JSON type validation in tool calls
    /// Uses mock services to precisely control the JSON generation scenario
    /// </summary>
    public sealed class JsonTypeValidationTests
    {
        /// <summary>
        /// Test that simulates LLM generating wrong JSON types
        /// This reproduces the exact issue described in the scene
        /// </summary>
        [Fact]
        public async Task SimulateLlmGeneratingWrongTypes_ShouldBeDetected()
        {
            // Arrange: Create a service collection with mock services
            var services = new ServiceCollection();
            
            // Add a mock service that will receive the JSON
            services.AddScoped<IMockLeaveService, MockLeaveService>();
            
            // Configure OpenAI and PlayFramework
            services.AddOpenAi(settings =>
            {
                settings.ApiKey = "test-key";
                settings.Azure.ResourceName = "test-resource";
            });

            services.AddPlayFramework(builder =>
            {
                builder.AddMainActor("Test actor", true);
                
                builder.AddScene(sceneBuilder =>
                {
                    sceneBuilder
                        .WithName("TestFindRequests")
                        .WithDescription("Test scene for finding requests")
                        .WithService<IMockLeaveService>(serviceBuilder =>
                        {
                            serviceBuilder.WithMethod(
                                service => service.FindRequestsAsync,
                                "find_requests",
                                "Finds leave requests with filters");
                        });
                });
            });

            var provider = services.BuildServiceProvider();
            var sceneManager = provider.GetRequiredService<ISceneManager>();
            var mockService = provider.GetRequiredService<IMockLeaveService>() as MockLeaveService;

            // Act: Simulate different JSON scenarios
            var scenarios = new[]
            {
                new
                {
                    Name = "Correct Types",
                    Json = """{"page": 1, "count": 25, "onlyMine": true, "status": 0}""",
                    ShouldFail = false
                },
                new
                {
                    Name = "Boolean as String",
                    Json = """{"page": 1, "count": 25, "onlyMine": "true", "status": 0}""",
                    ShouldFail = true
                },
                new
                {
                    Name = "Enum as String",
                    Json = """{"page": 1, "count": 25, "onlyMine": true, "status": "Requested"}""",
                    ShouldFail = true
                },
                new
                {
                    Name = "Number as String",
                    Json = """{"page": "1", "count": "25", "onlyMine": true}""",
                    ShouldFail = true
                }
            };

            foreach (var scenario in scenarios)
            {
                try
                {
                    var request = JsonSerializer.Deserialize<MockFindRequest>(scenario.Json);
                    
                    if (scenario.ShouldFail)
                    {
                        Assert.True(false, $"Scenario '{scenario.Name}' should have failed deserialization but succeeded");
                    }
                    else
                    {
                        Assert.NotNull(request);
                    }
                }
                catch (JsonException)
                {
                    if (!scenario.ShouldFail)
                    {
                        Assert.True(false, $"Scenario '{scenario.Name}' should have succeeded but failed");
                    }
                }
            }
        }

        /// <summary>
        /// Test JSON schema validation helper
        /// </summary>
        [Fact]
        public void ValidateJsonSchema_DetectsTypeErrors()
        {
            var correctJson = """
                {
                  "page": 1,
                  "count": 25,
                  "onlyMine": true,
                  "onlyToMe": false,
                  "status": 0,
                  "sortAscending": true
                }
                """;

            var wrongJson1 = """
                {
                  "page": 1,
                  "count": 25,
                  "onlyMine": "true",
                  "status": 0
                }
                """;

            var wrongJson2 = """
                {
                  "page": 1,
                  "count": 25,
                  "onlyMine": true,
                  "status": "Requested"
                }
                """;

            // Test correct JSON
            var (isValid1, errors1) = ValidateJsonTypes(correctJson, new Dictionary<string, JsonValueKind>
            {
                ["page"] = JsonValueKind.Number,
                ["count"] = JsonValueKind.Number,
                ["onlyMine"] = JsonValueKind.True, // or False
                ["status"] = JsonValueKind.Number,
                ["sortAscending"] = JsonValueKind.True
            });
            Assert.True(isValid1, $"Correct JSON should be valid. Errors: {string.Join(", ", errors1)}");

            // Test wrong JSON (boolean as string)
            var (isValid2, errors2) = ValidateJsonTypes(wrongJson1, new Dictionary<string, JsonValueKind>
            {
                ["page"] = JsonValueKind.Number,
                ["count"] = JsonValueKind.Number,
                ["onlyMine"] = JsonValueKind.True,
                ["status"] = JsonValueKind.Number
            });
            Assert.False(isValid2, "JSON with boolean as string should be invalid");
            Assert.Contains(errors2, e => e.Contains("onlyMine"));

            // Test wrong JSON (enum as string)
            var (isValid3, errors3) = ValidateJsonTypes(wrongJson2, new Dictionary<string, JsonValueKind>
            {
                ["page"] = JsonValueKind.Number,
                ["count"] = JsonValueKind.Number,
                ["onlyMine"] = JsonValueKind.True,
                ["status"] = JsonValueKind.Number
            });
            Assert.False(isValid3, "JSON with enum as string should be invalid");
            Assert.Contains(errors3, e => e.Contains("status"));
        }

        /// <summary>
        /// Test array type validation
        /// </summary>
        [Fact]
        public void ValidateArrayTypes_DetectsErrors()
        {
            var correctJson = """
                {
                  "approvers": ["user1@test.com", "user2@test.com"],
                  "cc": ["user3@test.com"]
                }
                """;

            var wrongJson = """
                {
                  "approvers": "user1@test.com",
                  "cc": ["user3@test.com"]
                }
                """;

            var doc1 = JsonDocument.Parse(correctJson);
            Assert.True(doc1.RootElement.GetProperty("approvers").ValueKind == JsonValueKind.Array);
            Assert.True(doc1.RootElement.GetProperty("cc").ValueKind == JsonValueKind.Array);

            var doc2 = JsonDocument.Parse(wrongJson);
            Assert.False(doc2.RootElement.GetProperty("approvers").ValueKind == JsonValueKind.Array);
        }

        /// <summary>
        /// Helper to validate JSON types
        /// </summary>
        private (bool IsValid, List<string> Errors) ValidateJsonTypes(
            string json, 
            Dictionary<string, JsonValueKind> expectedTypes)
        {
            var errors = new List<string>();
            
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                foreach (var (key, expectedKind) in expectedTypes)
                {
                    if (!root.TryGetProperty(key, out var element))
                        continue;

                    var actualKind = element.ValueKind;
                    
                    // Special handling for booleans (True or False)
                    if (expectedKind is JsonValueKind.True or JsonValueKind.False)
                    {
                        if (actualKind is not JsonValueKind.True and not JsonValueKind.False)
                        {
                            errors.Add($"Property '{key}' should be boolean but is {actualKind}");
                        }
                    }
                    else if (actualKind != expectedKind)
                    {
                        errors.Add($"Property '{key}' should be {expectedKind} but is {actualKind}");
                    }
                }
            }
            catch (JsonException ex)
            {
                errors.Add($"JSON parsing error: {ex.Message}");
            }

            return (errors.Count == 0, errors);
        }

        // Mock interfaces and classes
        public interface IMockLeaveService
        {
            Task<List<MockLeaveRequest>> FindRequestsAsync(MockFindRequest request, CancellationToken cts = default);
        }

        public class MockLeaveService : IMockLeaveService
        {
            public List<(string Json, DateTime Timestamp)> ReceivedRequests { get; } = new();

            public Task<List<MockLeaveRequest>> FindRequestsAsync(MockFindRequest request, CancellationToken cts = default)
            {
                var json = JsonSerializer.Serialize(request);
                ReceivedRequests.Add((json, DateTime.UtcNow));

                return Task.FromResult(new List<MockLeaveRequest>());
            }
        }

        public class MockFindRequest
        {
            public int Page { get; set; }
            public int Count { get; set; }
            public bool? OnlyMine { get; set; }
            public bool? OnlyToMe { get; set; }
            public int? Status { get; set; }
            public bool SortAscending { get; set; }
        }

        public class MockLeaveRequest
        {
            public int Id { get; set; }
            public string UserName { get; set; } = string.Empty;
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
    }
}
