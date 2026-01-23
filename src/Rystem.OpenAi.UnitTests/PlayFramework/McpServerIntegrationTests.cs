using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Rystem.PlayFramework;
using Rystem.PlayFramework.Mcp.Server;
using Xunit;

namespace Rystem.OpenAi.Test.PlayFramework;

/// <summary>
/// Unit tests for MCP server handlers.
/// </summary>
public sealed class McpServerHandlerTests
{
    #region ToolsListHandler Tests

    [Fact]
    public async Task ToolsListHandler_ReturnsPlayFrameworkAsTool()
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
        var result = await handler.HandleAsync("TestAssistant", null);

        // Assert
        Assert.NotNull(result);
        var response = result as McpListResponse<McpServerToolInfo>;
        Assert.NotNull(response?.Tools);
        Assert.Single(response.Tools);
        Assert.Equal("TestAssistant", response.Tools[0].Name);
        Assert.Equal("A test assistant", response.Tools[0].Description);
    }

    [Fact]
    public async Task ToolsListHandler_InputSchema_HasMessageProperty()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            Description = "Test"
        });

        var handler = new ToolsListHandler(registry);

        // Act
        var result = await handler.HandleAsync("TestAssistant", null);

        // Assert
        var response = result as McpListResponse<McpServerToolInfo>;
        var tool = response!.Tools![0];

        Assert.Equal("object", tool.InputSchema.Type);
        Assert.True(tool.InputSchema.Properties.ContainsKey("message"));
        Assert.Equal("string", tool.InputSchema.Properties["message"].Type);
        Assert.Contains("message", tool.InputSchema.Required);
    }

    [Fact]
    public async Task ToolsListHandler_UnknownPlayFramework_ReturnsEmptyList()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        var handler = new ToolsListHandler(registry);

        // Act
        var result = await handler.HandleAsync("NonExistent", null);

        // Assert
        var response = result as McpListResponse<McpServerToolInfo>;
        Assert.NotNull(response?.Tools);
        Assert.Empty(response.Tools);
    }

    #endregion

    #region ResourcesListHandler Tests

    [Fact]
    public async Task ResourcesListHandler_ReturnsOverviewAndScenes()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            EnableResources = true,
            SceneDocumentations =
            [
                new SceneDocumentation { SceneName = "Scene1", Description = "First" },
                new SceneDocumentation { SceneName = "Scene2", Description = "Second" }
            ]
        });

        var handler = new ResourcesListHandler(registry);

        // Act
        var result = await handler.HandleAsync("TestAssistant", null);

        // Assert
        var response = result as McpListResponse<McpServerResourceInfo>;
        Assert.NotNull(response?.Resources);
        Assert.Equal(3, response.Resources.Count); // Overview + 2 scenes

        Assert.Contains(response.Resources, r => r.Uri.Contains("overview"));
        Assert.Contains(response.Resources, r => r.Uri.Contains("Scene1"));
        Assert.Contains(response.Resources, r => r.Uri.Contains("Scene2"));
    }

    [Fact]
    public async Task ResourcesListHandler_ResourcesDisabled_ReturnsEmpty()
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
        var result = await handler.HandleAsync("TestAssistant", null);

        // Assert
        var response = result as McpListResponse<McpServerResourceInfo>;
        Assert.Empty(response!.Resources!);
    }

    [Fact]
    public async Task ResourcesListHandler_AllHaveMarkdownMimeType()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            EnableResources = true,
            SceneDocumentations = [new SceneDocumentation { SceneName = "Scene1" }]
        });

        var handler = new ResourcesListHandler(registry);

        // Act
        var result = await handler.HandleAsync("TestAssistant", null);

        // Assert
        var response = result as McpListResponse<McpServerResourceInfo>;
        Assert.All(response!.Resources!, r => Assert.Equal("text/markdown", r.MimeType));
    }

    #endregion

    #region ResourcesReadHandler Tests

    [Fact]
    public async Task ResourcesReadHandler_ReturnsOverviewDocumentation()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            Description = "Test description",
            EnableResources = true,
            SceneDocumentations = []
        });

        var docBuilder = new PlayFrameworkDocumentationBuilder();
        var handler = new ResourcesReadHandler(registry, docBuilder);

        var parameters = JsonDocument.Parse("""{"uri": "playframework://TestAssistant/overview"}""").RootElement;

        // Act
        var result = await handler.HandleAsync("TestAssistant", parameters);

        // Assert
        var response = result as McpResourceReadResponse;
        Assert.NotNull(response);
        Assert.Single(response.Contents);
        Assert.Contains("TestAssistant", response.Contents[0].Text);
        Assert.Contains("Overview", response.Contents[0].Text);
    }

    [Fact]
    public async Task ResourcesReadHandler_ReturnsSceneDocumentation()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            EnableResources = true,
            SceneDocumentations =
            [
                new SceneDocumentation
                {
                    SceneName = "CustomerSupport",
                    Description = "Handles customer inquiries",
                    AvailableTools = ["GetTicket"]
                }
            ]
        });

        var docBuilder = new PlayFrameworkDocumentationBuilder();
        var handler = new ResourcesReadHandler(registry, docBuilder);

        var parameters = JsonDocument.Parse("""{"uri": "playframework://TestAssistant/scenes/CustomerSupport"}""").RootElement;

        // Act
        var result = await handler.HandleAsync("TestAssistant", parameters);

        // Assert
        var response = result as McpResourceReadResponse;
        Assert.Single(response!.Contents);
        Assert.Contains("CustomerSupport", response.Contents[0].Text);
        Assert.Contains("customer inquiries", response.Contents[0].Text);
        Assert.Contains("GetTicket", response.Contents[0].Text);
    }

    [Fact]
    public async Task ResourcesReadHandler_InvalidUri_ReturnsEmpty()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            EnableResources = true
        });

        var handler = new ResourcesReadHandler(registry, new PlayFrameworkDocumentationBuilder());
        var parameters = JsonDocument.Parse("""{"uri": "invalid://uri"}""").RootElement;

        // Act
        var result = await handler.HandleAsync("TestAssistant", parameters);

        // Assert
        var response = result as McpResourceReadResponse;
        Assert.Empty(response!.Contents);
    }

    #endregion

    #region PromptsListHandler Tests

    [Fact]
    public async Task PromptsListHandler_ReturnsPromptWhenConfigured()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            Description = "Test assistant",
            Prompt = "You are a helpful assistant."
        });

        var handler = new PromptsListHandler(registry);

        // Act
        var result = await handler.HandleAsync("TestAssistant", null);

        // Assert
        var response = result as McpListResponse<McpServerPromptInfo>;
        Assert.Single(response!.Prompts!);
        Assert.Equal("TestAssistant-system-prompt", response.Prompts[0].Name);
        Assert.NotNull(response.Prompts[0].Arguments);
        Assert.Contains(response.Prompts[0].Arguments, a => a.Name == "context");
    }

    [Fact]
    public async Task PromptsListHandler_NoPromptConfigured_ReturnsEmpty()
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
        var result = await handler.HandleAsync("TestAssistant", null);

        // Assert
        var response = result as McpListResponse<McpServerPromptInfo>;
        Assert.Empty(response!.Prompts!);
    }

    #endregion

    #region PromptsGetHandler Tests

    [Fact]
    public async Task PromptsGetHandler_ReturnsPromptContent()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            Prompt = "You are a helpful assistant."
        });

        var handler = new PromptsGetHandler(registry);

        // Act
        var result = await handler.HandleAsync("TestAssistant", null);

        // Assert
        var response = result as McpPromptGetResponse;
        Assert.Single(response!.Messages);
        Assert.Equal("user", response.Messages[0].Role);
        Assert.Equal("text", response.Messages[0].Content.Type);
        Assert.Equal("You are a helpful assistant.", response.Messages[0].Content.Text);
    }

    [Fact]
    public async Task PromptsGetHandler_WithContext_AppendsToPrompt()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            Prompt = "Base prompt."
        });

        var handler = new PromptsGetHandler(registry);
        var parameters = JsonDocument.Parse("""
            {"arguments": {"context": "Additional context"}}
            """).RootElement;

        // Act
        var result = await handler.HandleAsync("TestAssistant", parameters);

        // Assert
        var response = result as McpPromptGetResponse;
        Assert.Contains("Base prompt", response!.Messages[0].Content.Text);
        Assert.Contains("Additional context", response.Messages[0].Content.Text);
    }

    [Fact]
    public async Task PromptsGetHandler_NoPromptConfigured_ReturnsEmptyMessages()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            Prompt = null
        });

        var handler = new PromptsGetHandler(registry);

        // Act
        var result = await handler.HandleAsync("TestAssistant", null);

        // Assert
        var response = result as McpPromptGetResponse;
        Assert.Empty(response!.Messages);
    }

    #endregion
}

/// <summary>
/// Tests for the McpMethodRouter.
/// </summary>
public sealed class McpMethodRouterTests
{
    [Fact]
    public void SupportedMethods_ReturnsAllRegisteredMethods()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        var handlers = new List<IMcpMethodHandler>
        {
            new ToolsListHandler(registry),
            new ResourcesListHandler(registry),
            new PromptsListHandler(registry)
        };

        var router = new McpMethodRouter(handlers);

        // Act
        var methods = router.SupportedMethods.ToList();

        // Assert
        Assert.Contains("tools/list", methods);
        Assert.Contains("resources/list", methods);
        Assert.Contains("prompts/list", methods);
    }

    [Fact]
    public async Task RouteAsync_CallsCorrectHandler()
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
            new ToolsListHandler(registry)
        };

        var router = new McpMethodRouter(handlers);
        var request = new McpJsonRpcRequest { Id = 1, Method = "tools/list" };

        // Act
        var response = await router.RouteAsync("TestAssistant", request);

        // Assert
        Assert.NotNull(response.Result);
        Assert.Null(response.Error);
    }

    [Fact]
    public async Task RouteAsync_UnknownMethod_ReturnsMethodNotFound()
    {
        // Arrange
        var router = new McpMethodRouter([]);
        var request = new McpJsonRpcRequest { Id = 1, Method = "unknown/method" };

        // Act
        var response = await router.RouteAsync("Test", request);

        // Assert
        Assert.NotNull(response.Error);
        Assert.Equal(McpJsonRpcErrorCodes.MethodNotFound, response.Error.Code);
        Assert.Contains("not found", response.Error.Message);
    }

    [Fact]
    public async Task RouteAsync_EmptyMethod_ReturnsInvalidRequest()
    {
        // Arrange
        var router = new McpMethodRouter([]);
        var request = new McpJsonRpcRequest { Id = 1, Method = "" };

        // Act
        var response = await router.RouteAsync("Test", request);

        // Assert
        Assert.NotNull(response.Error);
        Assert.Equal(McpJsonRpcErrorCodes.InvalidRequest, response.Error.Code);
    }

    [Fact]
    public async Task RouteAsync_PreservesRequestId()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo { Name = "Test" });

        var router = new McpMethodRouter([new ToolsListHandler(registry)]);

        // Act
        var response1 = await router.RouteAsync("Test", new McpJsonRpcRequest { Id = 42, Method = "tools/list" });
        var response2 = await router.RouteAsync("Test", new McpJsonRpcRequest { Id = "string-id", Method = "tools/list" });

        // Assert
        Assert.Equal(42, response1.Id);
        Assert.Equal("string-id", response2.Id);
    }

    [Fact]
    public async Task RouteAsync_WithException_ReturnsInternalError()
    {
        // Arrange
        var faultyHandler = new FaultyHandler();
        var router = new McpMethodRouter([faultyHandler]);
        var request = new McpJsonRpcRequest { Id = 1, Method = "faulty/method" };

        // Act
        var response = await router.RouteAsync("Test", request);

        // Assert
        Assert.NotNull(response.Error);
        Assert.Equal(McpJsonRpcErrorCodes.InternalError, response.Error.Code);
        Assert.Contains("test exception", response.Error.Message);
    }

    private sealed class FaultyHandler : IMcpMethodHandler
    {
        public string Method => "faulty/method";

        public Task<object?> HandleAsync(string playFrameworkName, JsonElement? parameters, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Intentional test exception");
        }
    }
}

/// <summary>
/// Tests for documentation generation.
/// </summary>
public sealed class DocumentationBuilderTests
{
    private readonly PlayFrameworkDocumentationBuilder _builder = new();

    [Fact]
    public void BuildSceneDocumentation_IncludesAllSections()
    {
        // Arrange
        var scene = new SceneDocumentation
        {
            SceneName = "CustomerSupport",
            Description = "Handles customer inquiries",
            SystemMessage = "You are a support agent",
            AvailableTools = ["GetTicket", "CreateTicket"],
            AvailableActors = ["SupportAgent"]
        };

        // Act
        var markdown = _builder.BuildSceneDocumentation(scene);

        // Assert
        Assert.Contains("# CustomerSupport", markdown);
        Assert.Contains("## Description", markdown);
        Assert.Contains("Handles customer inquiries", markdown);
        Assert.Contains("## System Behavior", markdown);
        Assert.Contains("support agent", markdown);
        Assert.Contains("## Available Tools", markdown);
        Assert.Contains("GetTicket", markdown);
        Assert.Contains("CreateTicket", markdown);
        Assert.Contains("## Available Actors", markdown);
        Assert.Contains("SupportAgent", markdown);
    }

    [Fact]
    public void BuildSceneDocumentation_OmitsEmptySections()
    {
        // Arrange
        var scene = new SceneDocumentation
        {
            SceneName = "SimpleScene"
        };

        // Act
        var markdown = _builder.BuildSceneDocumentation(scene);

        // Assert
        Assert.Contains("# SimpleScene", markdown);
        Assert.DoesNotContain("## Description", markdown);
        Assert.DoesNotContain("## System Behavior", markdown);
        Assert.DoesNotContain("## Available Tools", markdown);
        Assert.DoesNotContain("## Available Actors", markdown);
    }

    [Fact]
    public void BuildOverviewDocumentation_IncludesAllScenes()
    {
        // Arrange
        var info = new ExposedPlayFrameworkInfo
        {
            Name = "MyAssistant",
            Description = "Multi-scene assistant",
            SceneDocumentations =
            [
                new SceneDocumentation { SceneName = "Scene1", Description = "First" },
                new SceneDocumentation { SceneName = "Scene2", Description = "Second" }
            ]
        };

        // Act
        var markdown = _builder.BuildOverviewDocumentation(info);

        // Assert
        Assert.Contains("# MyAssistant", markdown);
        Assert.Contains("Multi-scene assistant", markdown);
        Assert.Contains("## Available Scenes", markdown);
        Assert.Contains("### Scene1", markdown);
        Assert.Contains("### Scene2", markdown);
        Assert.Contains("## Usage", markdown);
    }

    [Fact]
    public void BuildOverviewDocumentation_IncludesUsageExample()
    {
        // Arrange
        var info = new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            SceneDocumentations = []
        };

        // Act
        var markdown = _builder.BuildOverviewDocumentation(info);

        // Assert
        Assert.Contains("tools/call", markdown);
        Assert.Contains("TestAssistant", markdown);
        Assert.Contains("message", markdown);
    }

    [Fact]
    public void BuildSceneDocumentation_FormatsToolsAsList()
    {
        // Arrange
        var scene = new SceneDocumentation
        {
            SceneName = "Test",
            AvailableTools = ["Tool1", "Tool2", "Tool3"]
        };

        // Act
        var markdown = _builder.BuildSceneDocumentation(scene);

        // Assert
        Assert.Contains("- `Tool1`", markdown);
        Assert.Contains("- `Tool2`", markdown);
        Assert.Contains("- `Tool3`", markdown);
    }
}

/// <summary>
/// Tests for PlayFrameworkMcpServerRegistry.
/// </summary>
public sealed class McpServerRegistryTests
{
    [Fact]
    public void Register_AddsToRegistry()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        var info = new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            Description = "Test"
        };

        // Act
        registry.Register(info);

        // Assert
        Assert.True(registry.IsRegistered("TestAssistant"));
    }

    [Fact]
    public void Get_ReturnsRegisteredInfo()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        var info = new ExposedPlayFrameworkInfo
        {
            Name = "TestAssistant",
            Description = "Test Description"
        };
        registry.Register(info);

        // Act
        var result = registry.Get("TestAssistant");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Description", result.Description);
    }

    [Fact]
    public void Get_UnknownName_ReturnsNull()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();

        // Act
        var result = registry.Get("NonExistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAll_ReturnsAllRegistered()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo { Name = "Assistant1" });
        registry.Register(new ExposedPlayFrameworkInfo { Name = "Assistant2" });
        registry.Register(new ExposedPlayFrameworkInfo { Name = "Assistant3" });

        // Act
        var all = registry.GetAll().ToList();

        // Assert
        Assert.Equal(3, all.Count);
        Assert.Contains(all, i => i.Name == "Assistant1");
        Assert.Contains(all, i => i.Name == "Assistant2");
        Assert.Contains(all, i => i.Name == "Assistant3");
    }

    [Fact]
    public void Register_OverwritesExisting()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo { Name = "Test", Description = "First" });

        // Act
        registry.Register(new ExposedPlayFrameworkInfo { Name = "Test", Description = "Second" });

        // Assert
        var result = registry.Get("Test");
        Assert.Equal("Second", result!.Description);
    }

    [Fact]
    public void GetAuthorizationPolicy_ReturnsPolicy()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo
        {
            Name = "Test",
            AuthorizationPolicy = "AdminPolicy"
        });

        // Act
        var policy = registry.GetAuthorizationPolicy("Test");

        // Assert
        Assert.Equal("AdminPolicy", policy);
    }

    [Fact]
    public void GetAuthorizationPolicy_NoPolicy_ReturnsNull()
    {
        // Arrange
        var registry = new PlayFrameworkMcpServerRegistry();
        registry.Register(new ExposedPlayFrameworkInfo
        {
            Name = "Test",
            AuthorizationPolicy = null
        });

        // Act
        var policy = registry.GetAuthorizationPolicy("Test");

        // Assert
        Assert.Null(policy);
    }
}

/// <summary>
/// Tests for JSON-RPC models.
/// </summary>
public sealed class McpJsonRpcModelsTests
{
    [Fact]
    public void McpJsonRpcResponse_Success_HasCorrectFormat()
    {
        // Act
        var response = McpJsonRpcResponse.Success(42, new { data = "test" });

        // Assert
        Assert.Equal("2.0", response.JsonRpc);
        Assert.Equal(42, response.Id);
        Assert.NotNull(response.Result);
        Assert.Null(response.Error);
    }

    [Fact]
    public void McpJsonRpcResponse_Failure_HasCorrectFormat()
    {
        // Act
        var response = McpJsonRpcResponse.Failure(42, -32600, "Invalid request");

        // Assert
        Assert.Equal("2.0", response.JsonRpc);
        Assert.Equal(42, response.Id);
        Assert.Null(response.Result);
        Assert.NotNull(response.Error);
        Assert.Equal(-32600, response.Error.Code);
        Assert.Equal("Invalid request", response.Error.Message);
    }

    [Fact]
    public void McpJsonRpcErrorCodes_HasCorrectValues()
    {
        Assert.Equal(-32700, McpJsonRpcErrorCodes.ParseError);
        Assert.Equal(-32600, McpJsonRpcErrorCodes.InvalidRequest);
        Assert.Equal(-32601, McpJsonRpcErrorCodes.MethodNotFound);
        Assert.Equal(-32602, McpJsonRpcErrorCodes.InvalidParams);
        Assert.Equal(-32603, McpJsonRpcErrorCodes.InternalError);
    }
}
