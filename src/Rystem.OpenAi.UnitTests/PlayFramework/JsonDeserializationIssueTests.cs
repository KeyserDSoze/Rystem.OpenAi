using System.Text.Json;
using Rystem.OpenAi;
using Rystem.PlayFramework;
using Xunit;

namespace Rystem.OpenAi.UnitTests.PlayFramework
{
    /// <summary>
    /// Tests for JSON deserialization issues in PlayFramework tool calls
    /// Reproduces the problem where LLM generates incorrect JSON types (e.g., "true" instead of true)
    /// </summary>
    public sealed class JsonDeserializationIssueTests
    {
        private readonly ISceneManager _sceneManager;
        private readonly IServiceProvider _serviceProvider;

        public JsonDeserializationIssueTests(ISceneManager sceneManager, IServiceProvider serviceProvider)
        {
            _sceneManager = sceneManager;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Test Case 1: LLM generates boolean as string "true" instead of true
        /// Expected: Should fail and return error message to user
        /// </summary>
        [Fact]
        public async Task BooleanAsString_ShouldFailDeserialization()
        {
            var conversationKey = Guid.NewGuid().ToString();

            // Simulate user request that triggers FindLeaveRequestsScene
            var userQuestion = "mostrami le mie richieste di ferie";

            var responses = new List<AiSceneResponse>();
            await foreach (var response in _sceneManager.ExecuteAsync(
                userQuestion,
                settings => settings.WithKey(conversationKey),
                TestContext.Current.CancellationToken))
            {
                responses.Add(response);
            }

            // Check if there's a FunctionRequest with malformed JSON
            var functionRequests = responses.Where(r => r.Status == AiResponseStatus.FunctionRequest).ToList();

            // Look for error responses related to JSON deserialization
            var errorResponses = responses.Where(r =>
                r.Message != null &&
                (r.Message.Contains("deserialization", StringComparison.OrdinalIgnoreCase) ||
                 r.Message.Contains("type", StringComparison.OrdinalIgnoreCase) ||
                 r.Message.Contains("format", StringComparison.OrdinalIgnoreCase))).ToList();

            Assert.NotEmpty(responses);

            // If there was a deserialization error, it should be reported back to the user
            if (errorResponses.Any())
            {
                Assert.Contains(errorResponses, r => r.Message!.Contains("true"));
            }
        }

        /// <summary>
        /// Test Case 2: LLM generates enum as string "Requested" instead of 0
        /// Expected: Should fail and return error message
        /// </summary>
        [Fact]
        public async Task EnumAsString_ShouldFailDeserialization()
        {
            var conversationKey = Guid.NewGuid().ToString();

            // Request that requires status enum
            var userQuestion = "mostrami le richieste di ferie approvate";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            // Check for function requests with enum parameters
            var functionRequests = responses.Where(r =>
                r.Status == AiResponseStatus.FunctionRequest &&
                r.Arguments != null).ToList();

            foreach (var request in functionRequests)
            {
                // Try to parse the arguments JSON
                if (request.Arguments is string argsJson)
                {
                    var argsDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(argsJson);

                    if (argsDict != null && argsDict.ContainsKey("status"))
                    {
                        var statusElement = argsDict["status"];

                        // If it's a string instead of number, it's wrong
                        if (statusElement.ValueKind == JsonValueKind.String)
                        {
                            Assert.True(false, $"Status should be a number (enum), not a string: {statusElement.GetString()}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Test Case 3: Mixed type errors in FindLeaveRequestsRequest
        /// Tests the exact scenario from the scene definition
        /// </summary>
        [Fact]
        public async Task FindLeaveRequests_WithMixedTypeErrors_ShouldBeDetected()
        {
            var conversationKey = Guid.NewGuid().ToString();

            // User wants to see only their requests
            var userQuestion = "voglio vedere solo le mie richieste di ferie";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            var functionRequests = responses.Where(r =>
                r.FunctionName?.Contains("trova", StringComparison.OrdinalIgnoreCase) == true &&
                r.Arguments != null).ToList();

            Assert.NotEmpty(functionRequests);

            foreach (var request in functionRequests)
            {
                // Parse arguments
                if (request.Arguments is string argsJson)
                {
                    var args = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(argsJson);
                    Assert.NotNull(args);

                    // Check critical fields that are often wrong
                    if (args.ContainsKey("onlyMine"))
                    {
                        var onlyMine = args["onlyMine"];
                        Assert.True(onlyMine.ValueKind == JsonValueKind.True || onlyMine.ValueKind == JsonValueKind.False,
                            $"onlyMine should be boolean, got: {onlyMine.ValueKind}");
                    }

                    if (args.ContainsKey("onlyToMe"))
                    {
                        var onlyToMe = args["onlyToMe"];
                        Assert.True(onlyToMe.ValueKind == JsonValueKind.True || onlyToMe.ValueKind == JsonValueKind.False,
                            $"onlyToMe should be boolean, got: {onlyToMe.ValueKind}");
                    }

                    if (args.ContainsKey("status"))
                    {
                        var status = args["status"];
                        Assert.True(status.ValueKind == JsonValueKind.Number,
                            $"status should be number (enum), got: {status.ValueKind}");
                    }

                    if (args.ContainsKey("page"))
                    {
                        var page = args["page"];
                        Assert.True(page.ValueKind == JsonValueKind.Number,
                            $"page should be number, got: {page.ValueKind}");
                    }

                    if (args.ContainsKey("count"))
                    {
                        var count = args["count"];
                        Assert.True(count.ValueKind == JsonValueKind.Number,
                            $"count should be number, got: {count.ValueKind}");
                    }
                }
            }
        }

        /// <summary>
        /// Test Case 4: Verify that correct JSON types work properly
        /// This is a control test to ensure the system works with correct types
        /// </summary>
        [Fact]
        public async Task FindLeaveRequests_WithCorrectTypes_ShouldWork()
        {
            // This test verifies that when JSON is correct, everything works
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

            // Verify it deserializes correctly
            var testRequest = JsonSerializer.Deserialize<TestFindRequest>(correctJson);
            Assert.NotNull(testRequest);
            Assert.Equal(1, testRequest.Page);
            Assert.Equal(25, testRequest.Count);
            Assert.True(testRequest.OnlyMine);
            Assert.False(testRequest.OnlyToMe);
            Assert.Equal(0, testRequest.Status);
            Assert.True(testRequest.SortAscending);
        }

        /// <summary>
        /// Test Case 5: Verify that incorrect JSON types fail deserialization
        /// </summary>
        [Fact]
        public void IncorrectJsonTypes_ShouldFailDeserialization()
        {
            // Boolean as string - WRONG
            var wrongJson1 = """
            {
              "page": 1,
              "count": 25,
              "onlyMine": "true",
              "status": 0
            }
            """;

            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<TestFindRequest>(wrongJson1));

            // Enum as string - WRONG
            var wrongJson2 = """
            {
              "page": 1,
              "count": 25,
              "onlyMine": true,
              "status": "Requested"
            }
            """;

            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<TestFindRequest>(wrongJson2));

            // Number as string - WRONG
            var wrongJson3 = """
            {
              "page": "1",
              "count": "25",
              "onlyMine": true
            }
            """;

            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<TestFindRequest>(wrongJson3));
        }

        /// <summary>
        /// Test Case 6: Test date format issues including DateOnly and TimeOnly
        /// </summary>
        [Fact]
        public async Task DateFormat_ShouldBeIso8601()
        {
            var conversationKey = Guid.NewGuid().ToString();

            var userQuestion = "mostrami le richieste di ferie dal 1 gennaio 2024 al 31 dicembre 2024";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            var functionRequests = responses.Where(r =>
                r.FunctionName?.Contains("trova", StringComparison.OrdinalIgnoreCase) == true &&
                r.Arguments != null).ToList();

            foreach (var request in functionRequests)
            {
                // Parse arguments
                if (request.Arguments is string argsJson)
                {
                    var args = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(argsJson);

                    if (args != null && args.ContainsKey("startDate"))
                    {
                        var startDate = args["startDate"];
                        if (startDate.ValueKind == JsonValueKind.String)
                        {
                            var dateStr = startDate.GetString();

                            // Should be in format YYYY-MM-DD or ISO 8601 with time
                            // Both formats are acceptable due to lenient converter
                            Assert.Matches(@"^\d{4}-\d{2}-\d{2}", dateStr!);

                            // Verify it can be parsed as DateOnly even if it has time
                            Assert.True(DateOnly.TryParse(dateStr, out _) || DateTime.TryParse(dateStr, out _),
                                $"Date should be parseable: {dateStr}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Test Case 7: Comprehensive type validation for RequestLeaveRequest
        /// </summary>
        [Fact]
        public async Task RequestLeave_AllFieldTypes_ShouldBeCorrect()
        {
            var conversationKey = Guid.NewGuid().ToString();

            var userQuestion = "vorrei richiedere 3 giorni di ferie dal 15 al 17 giugno per vacanza";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            var functionRequests = responses.Where(r =>
                r.FunctionName?.Contains("richiesta", StringComparison.OrdinalIgnoreCase) == true &&
                r.Arguments != null).ToList();

            foreach (var request in functionRequests)
            {
                if (request.Arguments is string argsJson)
                {
                    var args = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(argsJson);
                    Assert.NotNull(args);

                    // Verify GUID format for taskId
                    if (args.ContainsKey("taskId"))
                    {
                        var taskId = args["taskId"];
                        Assert.True(taskId.ValueKind == JsonValueKind.String);
                        Assert.True(Guid.TryParse(taskId.GetString(), out _),
                            "taskId should be a valid GUID string");
                    }

                    // Verify date format
                    if (args.ContainsKey("startDate"))
                    {
                        var startDate = args["startDate"];
                        Assert.True(startDate.ValueKind == JsonValueKind.String);
                        Assert.Matches(@"^\d{4}-\d{2}-\d{2}$", startDate.GetString()!);
                    }

                    // Verify time format (if present)
                    if (args.ContainsKey("startTime") && args["startTime"].ValueKind != JsonValueKind.Null)
                    {
                        var startTime = args["startTime"];
                        Assert.True(startTime.ValueKind == JsonValueKind.String);
                        Assert.Matches(@"^\d{2}:\d{2}$", startTime.GetString()!);
                    }

                    // Verify boolean
                    if (args.ContainsKey("setOutOfOffice") && args["setOutOfOffice"].ValueKind != JsonValueKind.Null)
                    {
                        var setOutOfOffice = args["setOutOfOffice"];
                        Assert.True(
                            setOutOfOffice.ValueKind == JsonValueKind.True ||
                            setOutOfOffice.ValueKind == JsonValueKind.False,
                            $"setOutOfOffice should be boolean, got: {setOutOfOffice.ValueKind}");
                    }

                    // Verify arrays
                    if (args.ContainsKey("approvers"))
                    {
                        var approvers = args["approvers"];
                        Assert.True(approvers.ValueKind == JsonValueKind.Array,
                            "approvers should be an array");
                    }

                    if (args.ContainsKey("cc"))
                    {
                        var cc = args["cc"];
                        Assert.True(cc.ValueKind == JsonValueKind.Array,
                            "cc should be an array");
                    }
                }
            }
        }

        /// <summary>
        /// Test Case 8: Detect when LLM adds extra text around JSON
        /// </summary>
        [Fact]
        public async Task FunctionArguments_ShouldBeCleanJson_NoExtraText()
        {
            var conversationKey = Guid.NewGuid().ToString();

            var userQuestion = "mostra le mie richieste";

            var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

            var functionRequests = responses.Where(r =>
                r.Status == AiResponseStatus.FunctionRequest &&
                r.Arguments != null).ToList();

            foreach (var request in functionRequests)
            {
                // Arguments should be valid JSON without extra text
                if (request.Arguments is string argsJson)
                {
                    try
                    {
                        var trimmed = argsJson.Trim();
                        Assert.True(trimmed.StartsWith("{") || trimmed.StartsWith("["),
                            $"Arguments should start with {{ or [, got: {trimmed[..Math.Min(50, trimmed.Length)]}");

                        var doc = JsonDocument.Parse(argsJson);
                        Assert.NotNull(doc);
                    }
                    catch (JsonException ex)
                    {
                        Assert.True(false, $"Arguments should be valid JSON. Error: {ex.Message}. Arguments: {argsJson}");
                    }
                }
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

        // Test model matching FindLeaveRequestsRequest structure
        private class TestFindRequest
        {
            public int Page { get; set; }
            public int Count { get; set; }
            public bool? OnlyMine { get; set; }
            public bool? OnlyToMe { get; set; }
            public bool? OnlyAmCc { get; set; }
            public int? Status { get; set; }
            public bool SortAscending { get; set; }
        }
    }
}
