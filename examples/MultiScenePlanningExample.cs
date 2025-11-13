using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi;
using Rystem.PlayFramework;

namespace Examples
{
    /// <summary>
    /// Example demonstrating multi-scene planning and summarization features
    /// </summary>
    public class MultiScenePlanningExample
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Setup OpenAI client
            services.AddOpenAi(x =>
            {
                x.ApiKey = configuration["OpenAi:ApiKey"]!;
                x.Azure.ResourceName = configuration["OpenAi:ResourceName"];
                x.Version = "2024-08-01-preview";
                x.DefaultRequestConfiguration.Chat = chatClient =>
                {
                    chatClient.WithModel("gpt-4o");
                };
            }, "playframework");

            // Setup HTTP clients for external APIs
            services.AddHttpClient("hrApi", x =>
            {
                x.BaseAddress = new Uri(configuration["Api:HrUri"]!);
            });

            services.AddHttpClient("weatherApi", x =>
            {
                x.BaseAddress = new Uri(configuration["Api:WeatherUri"]!);
            });

            // Configure PlayFramework with multi-scene planning
            services.AddPlayFramework(scenes =>
            {
                scenes.Configure(settings =>
                {
                    settings.OpenAi.Name = "playframework";

                    // Enable planning with customized settings
                    settings.Planning.Enabled = true;
                    settings.Planning.MaxScenesInPlan = 5;

                    // Enable summarization for long conversations
                    settings.Summarization.Enabled = true;
                    settings.Summarization.ResponseThreshold = 30;  // Summarize after 30 responses
                    settings.Summarization.CharacterThreshold = 8000;  // Or 8000 characters
                })
                .AddMainActor(context => $"Today is {DateTime.UtcNow:yyyy-MM-dd}. You are a helpful assistant.", true)
                
                // Scene 1: Vacation Management
                .AddScene(scene =>
                {
                    scene
                        .WithName("VacationManagement")
                        .WithDescription("Manage vacation and leave requests. Can request time off, check approvers, and verify available dates.")
                        .WithHttpClient("hrApi")
                        .WithOpenAi("playframework")
                        .WithApi(pathBuilder =>
                        {
                            pathBuilder
                                .Map(new Regex("Vacation/*"))
                                .Map(new Regex("Leave/*"))
                                .Map(new Regex("Approvers/*"));
                        })
                        .WithActors(actors =>
                        {
                            actors
                                .AddActor("If dates don't have a year, use the current year.")
                                .AddActor("Always exclude holidays - users know these won't be counted.")
                                .AddActor("Before submitting, verify all required information is available.");
                        });
                })
                
                // Scene 2: Weather Information
                .AddScene(scene =>
                {
                    scene
                        .WithName("WeatherInfo")
                        .WithDescription("Get weather forecasts and conditions for any city worldwide.")
                        .WithHttpClient("weatherApi")
                        .WithOpenAi("playframework")
                        .WithApi(pathBuilder =>
                        {
                            pathBuilder
                                .Map(new Regex("Weather/*"))
                                .Map(new Regex("Forecast/*"))
                                .Map(new Regex("City/*"));
                        })
                        .WithActors(actors =>
                        {
                            actors.AddActor("If the city doesn't exist in the system, add it with population data.");
                        });
                })
                
                // Scene 3: User Identity
                .AddScene(scene =>
                {
                    scene
                        .WithName("UserIdentity")
                        .WithDescription("Retrieve user information such as name, email, and profile details.")
                        .WithOpenAi("playframework")
                        .WithService<IdentityService>(builder =>
                        {
                            builder
                                .WithMethod(x => x.GetUserNameAsync, "get_user_name", "Get the user's full name")
                                .WithMethod(x => x.GetUserEmailAsync, "get_user_email", "Get the user's email address")
                                .WithMethod(x => x.GetUserProfileAsync, "get_user_profile", "Get complete user profile");
                        });
                })
                
                // Optional: Add cache for persistent conversations
                .AddCache(cache =>
                {
                    cache.WithInMemory(TimeSpan.FromHours(2));
                });
            });
        }

        /// <summary>
        /// Example of using the scene manager with planning
        /// </summary>
        public static async Task ExecuteComplexRequestAsync(ISceneManager sceneManager)
        {
            var conversationKey = Guid.NewGuid().ToString();

            // Complex multi-step request
            var message = "I want to request 3 days of vacation from March 15-17, " +
                         "then check the weather in Milan for those dates, " +
                         "and finally send the details to my email.";

            Console.WriteLine($"User: {message}");
            Console.WriteLine();

            await foreach (var response in sceneManager.ExecuteAsync(message, settings =>
            {
                settings.Key = conversationKey;
                // Optional: provide custom properties
                settings.Properties = new Dictionary<object, object>
                {
                    ["UserId"] = "user123",
                    ["Timezone"] = "Europe/Rome"
                };
            }))
            {
                switch (response.Status)
                {
                    case AiResponseStatus.Planning:
                        Console.WriteLine($"[PLANNING] {response.Message}");
                        break;

                    case AiResponseStatus.SceneRequest:
                        Console.WriteLine($"[SCENE] Entering scene: {response.Name}");
                        if (!string.IsNullOrEmpty(response.Message))
                            Console.WriteLine($"  → {response.Message}");
                        break;

                    case AiResponseStatus.FunctionRequest:
                        Console.WriteLine($"[TOOL] {response.FunctionName} in {response.Name}");
                        break;

                    case AiResponseStatus.Running:
                        if (!string.IsNullOrEmpty(response.Message))
                            Console.WriteLine($"[RESULT] {response.Message}");
                        break;

                    case AiResponseStatus.FinishedOk:
                        Console.WriteLine($"[COMPLETED] {response.Message ?? "Request completed successfully"}");
                        break;

                    case AiResponseStatus.Summarizing:
                        Console.WriteLine($"[SUMMARY] Creating summary of previous conversation...");
                        break;
                }
            }
        }

        /// <summary>
        /// Example with custom planner
        /// </summary>
        public static void ConfigureWithCustomPlanner(IServiceCollection services)
        {
            services.AddPlayFramework(scenes =>
            {
                scenes.Configure(settings =>
                {
                    settings.OpenAi.Name = "playframework";
                    settings.Planning.Enabled = true;
                });

                // Register custom planner
                scenes.AddCustomPlanner<MyCustomPlanner>();

                // ... scene configuration ...
            });
        }
    }

    // Example custom planner that prioritizes certain scenes
    public class MyCustomPlanner : IPlanner
    {
        private readonly PlayHandler _playHandler;
        private readonly IFactory<IScene> _sceneFactory;

        public MyCustomPlanner(PlayHandler playHandler, IFactory<IScene> sceneFactory)
        {
            _playHandler = playHandler;
            _sceneFactory = sceneFactory;
        }

        public async Task<ExecutionPlan> CreatePlanAsync(
            SceneContext context,
            SceneRequestSettings requestSettings,
            CancellationToken cancellationToken)
        {
            var message = context.InputMessage.ToLowerInvariant();
            var steps = new List<PlanStep>();

            // Custom logic: if message contains "vacation", prioritize VacationManagement
            if (message.Contains("vacation") || message.Contains("leave"))
            {
                steps.Add(new PlanStep
                {
                    SceneName = "VacationManagement",
                    Purpose = "Handle vacation request",
                    Order = 1
                });
            }

            // If message contains weather-related terms
            if (message.Contains("weather") || message.Contains("forecast"))
            {
                steps.Add(new PlanStep
                {
                    SceneName = "WeatherInfo",
                    Purpose = "Get weather information",
                    Order = steps.Count + 1
                });
            }

            // If we need user identity
            if (message.Contains("email") || message.Contains("profile"))
            {
                steps.Add(new PlanStep
                {
                    SceneName = "UserIdentity",
                    Purpose = "Retrieve user information",
                    Order = steps.Count + 1
                });
            }

            return await Task.FromResult(new ExecutionPlan
            {
                Steps = steps,
                IsValid = steps.Count > 0,
                Reasoning = $"Created plan with {steps.Count} steps based on request analysis"
            });
        }
    }

    // Example custom summarizer
    public class MyCustomSummarizer : ISummarizer
    {
        public bool ShouldSummarize(List<AiSceneResponse> responses)
        {
            // Custom threshold: summarize after 100 responses
            return responses.Count > 100;
        }

        public async Task<string> SummarizeAsync(
            List<AiSceneResponse> responses,
            CancellationToken cancellationToken)
        {
            // Custom summarization logic
            // Note: DefaultSummarizer uses OpenAI for intelligent summaries
            var summary = new StringBuilder();
            summary.AppendLine("Custom Summary:");
            
            var scenes = responses.Select(r => r.Name).Distinct().Where(n => n != null);
            summary.AppendLine($"Scenes used: {string.Join(", ", scenes)}");
            
            var tools = responses.Where(r => r.FunctionName != null).Select(r => r.FunctionName).Distinct();
            summary.AppendLine($"Tools executed: {string.Join(", ", tools)}");
            
            return await Task.FromResult(summary.ToString());
        }
    }

    // Example identity service
    public class IdentityService
    {
        public async Task<string> GetUserNameAsync(string userId)
        {
            await Task.Delay(100); // Simulate async operation
            return "Mario Rossi";
        }

        public async Task<string> GetUserEmailAsync(string userId)
        {
            await Task.Delay(100);
            return "mario.rossi@example.com";
        }

        public async Task<UserProfile> GetUserProfileAsync(string userId)
        {
            await Task.Delay(100);
            return new UserProfile
            {
                Name = "Mario Rossi",
                Email = "mario.rossi@example.com",
                Department = "Engineering"
            };
        }
    }

    public class UserProfile
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }
}
