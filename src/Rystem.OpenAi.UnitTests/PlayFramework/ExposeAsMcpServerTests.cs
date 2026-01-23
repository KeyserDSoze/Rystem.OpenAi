using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Rystem.PlayFramework;
using Rystem.PlayFramework.Mcp.Server;
using Xunit;

namespace Rystem.OpenAi.Test.PlayFramework;

/// <summary>
/// Tests for exposing PlayFramework as an MCP server.
/// </summary>
public sealed class ExposeAsMcpServerTests
{
    [Fact]
    public void ExposeAsMcpServer_RegistersInRegistry()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddOpenAi(settings =>
        {
            settings.ApiKey = "test-key";
        });

        // Act
        services.AddPlayFramework(builder =>
        {
            builder.ExposeAsMcpServer(config =>
            {
                config.Description = "Test PlayFramework";
                config.EnableResources = true;
                config.AuthorizationPolicy = null;
            });

            builder.AddScene(scene =>
            {
                scene.WithName("TestScene")
                     .WithDescription("A test scene");
            });
        }, name: "TestFramework");

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var registry = serviceProvider.GetRequiredService<PlayFrameworkMcpServerRegistry>();
        Assert.NotNull(registry);

        // Force initialization by resolving ExposedPlayFrameworkInfo
        var info = serviceProvider.GetService<ExposedPlayFrameworkInfo>();
        Assert.NotNull(info);
        Assert.Equal("TestFramework", info.Name);
        Assert.Equal("Test PlayFramework", info.Description);
        Assert.True(info.EnableResources);
        Assert.Null(info.AuthorizationPolicy);
    }

    [Fact]
    public void ToolsListHandler_ReturnsPlayFrameworkAsTool()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            Description = "A test assistant",
            EnableResources = true
        });

        var handler = new ToolsListHandler(registry);

        // Act
        var result = handler.HandleAsync("TestAssistant", null).Result;

        // Assert
        Assert.NotNull(result);
        var response = result as McpListResponse<McpServerToolInfo>;
        Assert.NotNull(response?.Tools);
        Assert.Single(response.Tools);
        Assert.Equal("TestAssistant", response.Tools[0].Name);
        Assert.Equal("A test assistant", response.Tools[0].Description);
    }

    [Fact]
    public void ResourcesListHandler_ReturnsSceneResources()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            Description = "A test assistant",
            EnableResources = true,
            SceneDocumentations =
            [
                new SceneDocumentation
                {
                    SceneName = "CustomerSupport",
                    Description = "Handles customer inquiries"
                },
                new SceneDocumentation
                {
                    SceneName = "OrderManagement",
                    Description = "Manages orders"
                }
            ]
        });

        var handler = new ResourcesListHandler(registry);

        // Act
        var result = handler.HandleAsync("TestAssistant", null).Result;

        // Assert
        Assert.NotNull(result);
        var response = result as McpListResponse<McpServerResourceInfo>;
        Assert.NotNull(response?.Resources);
        // Overview + 2 scenes
        Assert.Equal(3, response.Resources.Count);
        Assert.Contains(response.Resources, r => r.Uri.Contains("overview"));
        Assert.Contains(response.Resources, r => r.Uri.Contains("CustomerSupport"));
        Assert.Contains(response.Resources, r => r.Uri.Contains("OrderManagement"));
    }

    [Fact]
    public void ResourcesListHandler_ReturnsEmptyWhenResourcesDisabled()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            EnableResources = false
        });

        var handler = new ResourcesListHandler(registry);

        // Act
        var result = handler.HandleAsync("TestAssistant", null).Result;

        // Assert
        Assert.NotNull(result);
        var response = result as McpListResponse<McpServerResourceInfo>;
        Assert.NotNull(response?.Resources);
        Assert.Empty(response.Resources);
    }

    [Fact]
    public void PromptsListHandler_ReturnsPromptWhenConfigured()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            Description = "A test assistant",
            Prompt = "You are a helpful assistant."
        });

        var handler = new PromptsListHandler(registry);

        // Act
        var result = handler.HandleAsync("TestAssistant", null).Result;

        // Assert
        Assert.NotNull(result);
        var response = result as McpListResponse<McpServerPromptInfo>;
        Assert.NotNull(response?.Prompts);
        Assert.Single(response.Prompts);
        Assert.Equal("TestAssistant-system-prompt", response.Prompts[0].Name);
    }

    [Fact]
    public void PromptsListHandler_ReturnsEmptyWhenNoPrompt()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            Prompt = null
        });

        var handler = new PromptsListHandler(registry);

        // Act
        var result = handler.HandleAsync("TestAssistant", null).Result;

        // Assert
        Assert.NotNull(result);
        var response = result as McpListResponse<McpServerPromptInfo>;
        Assert.NotNull(response?.Prompts);
        Assert.Empty(response.Prompts);
    }

    [Fact]
    public void McpMethodRouter_RoutesToCorrectHandler()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            Description = "Test"
        });

        var handlers = new List<IMcpMethodHandler>
        {
            new ToolsListHandler(registry),
            new ResourcesListHandler(registry),
            new PromptsListHandler(registry)
        };

        var router = new McpMethodRouter(handlers);

        // Act
        var request = new McpJsonRpcRequest
        {
            Id = 1,
            Method = "tools/list"
        };

        var response = router.RouteAsync("TestAssistant", request).Result;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(1, response.Id);
        Assert.Null(response.Error);
        Assert.NotNull(response.Result);
    }

    [Fact]
    public void McpMethodRouter_ReturnsErrorForUnknownMethod()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        var handlers = new List<IMcpMethodHandler>();
        var router = new McpMethodRouter(handlers);

        // Act
        var request = new McpJsonRpcRequest
        {
            Id = 1,
            Method = "unknown/method"
        };

        var response = router.RouteAsync("TestAssistant", request).Result;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(1, response.Id);
        Assert.NotNull(response.Error);
        Assert.Equal(McpJsonRpcErrorCodes.MethodNotFound, response.Error.Code);
    }

    [Fact]
    public void DocumentationBuilder_GeneratesSceneDocumentation()
    {
        // Arrange
        var builder = new PlayFrameworkDocumentationBuilder();
        var scene = new SceneDocumentation
        {
            SceneName = "CustomerSupport",
            Description = "Handles customer inquiries and complaints",
            SystemMessage = "You are a customer support agent.",
            AvailableTools = ["GetOrderStatus", "CreateTicket"],
            AvailableActors = ["SupportAgent", "Escalation"]
        };

        // Act
        var markdown = builder.BuildSceneDocumentation(scene);

        // Assert
        Assert.Contains("# CustomerSupport", markdown);
        Assert.Contains("Handles customer inquiries", markdown);
        Assert.Contains("customer support agent", markdown);
        Assert.Contains("GetOrderStatus", markdown);
        Assert.Contains("SupportAgent", markdown);
    }

    [Fact]
    public void DocumentationBuilder_GeneratesOverviewDocumentation()
    {
        // Arrange
        var builder = new PlayFrameworkDocumentationBuilder();
        var info = new ExposedPlayFrameworkInfo
        {
            Name = "MyAssistant",
            Description = "A multi-scene AI assistant",
            SceneDocumentations =
            [
                new SceneDocumentation { SceneName = "Scene1", Description = "First scene" },
                new SceneDocumentation { SceneName = "Scene2", Description = "Second scene" }
            ]
        };

        // Act
        var markdown = builder.BuildOverviewDocumentation(info);

        // Assert
        Assert.Contains("# MyAssistant", markdown);
        Assert.Contains("multi-scene AI assistant", markdown);
        Assert.Contains("Scene1", markdown);
        Assert.Contains("Scene2", markdown);
        Assert.Contains("tools/call", markdown);
    }
}
