# MCP (Model Context Protocol) Integration Guide

## Overview

PlayFramework now includes comprehensive support for **Model Context Protocol (MCP)** servers, enabling your AI scenes to seamlessly access external tools, resources, and prompts. This guide covers installation, configuration, and best practices for MCP integration.

## What is MCP?

The Model Context Protocol is a standardized protocol for exposing capabilities to AI models. MCP servers expose three types of elements:

- **Tools**: Functions the AI can invoke to perform actions
- **Resources**: Data sources the AI can read and process
- **Prompts**: Pre-configured prompts providing context and guidance to the AI

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                     PlayFramework Application                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  Global MCP Server Registry (McpRegistry)                │  │
│  │  - Singleton service                                      │  │
│  │  - Registers servers globally (AddMcpServer)             │  │
│  │  - Each server has name, config, and initialized client  │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                 │
│  ┌────────────┐  ┌────────────┐  ┌────────────┐              │
│  │   Scene 1  │  │   Scene 2  │  │   Scene 3  │              │
│  ├────────────┤  ├────────────┤  ├────────────┤              │
│  │ Tools      │  │ Tools      │  │ Resources  │              │
│  │ Resources  │  │ Only       │  │ Prompts    │              │
│  │ Prompts    │  │            │  │            │              │
│  └────────────┘  └────────────┘  └────────────┘              │
│       ↓                ↓                ↓                      │
│  UseMcpServer()   UseMcpServer()   UseMcpServer()             │
│  (All elements)   (OnlyTools)      (WithResources/Prompts)   │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│           External MCP Servers (HTTP or Stdio)                 │
│                                                                 │
│  ┌──────────────────┐  ┌──────────────────┐                 │
│  │  HTTP MCP Server │  │ Stdio MCP Server │                 │
│  │  (e.g., local)   │  │ (e.g., Node.js)  │                 │
│  └──────────────────┘  └──────────────────┘                 │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Installation

MCP support is included in `Rystem.PlayFramework`. Ensure you have the latest version:

```bash
dotnet add package Rystem.PlayFramework
```

## Quick Start

### 1. Register an MCP Server

In your service configuration:

```csharp
services.AddPlayFramework(scenes =>
{
    scenes.Configure(settings =>
    {
        settings.OpenAi.Name = "playframework";
    })
    .AddMcpServer("myMcpServer", mcp =>
    {
        mcp.WithHttpServer("http://localhost:3000");
        mcp.WithTimeout(TimeSpan.FromSeconds(60));
    })
    .AddScene(scene =>
    {
        scene
            .WithName("DataScene")
            .WithOpenAi("playframework")
            .UseMcpServer("myMcpServer");
    });
});
```

### 2. Use in Scenes

Scenes automatically gain access to all registered MCP elements when `UseMcpServer()` is called.

## Advanced Configuration

### Server Communication Methods

#### HTTP Server

```csharp
.AddMcpServer("httpServer", mcp =>
{
    mcp.WithHttpServer("http://localhost:3000");
    mcp.WithTimeout(TimeSpan.FromSeconds(30));
})
```

#### Stdio Command

```csharp
.AddMcpServer("localServer", mcp =>
{
    mcp.WithCommand("node", "path/to/mcp-server.js");
    mcp.WithTimeout(TimeSpan.FromSeconds(60));
})
```

### Element Filtering

#### Allow All Elements (Default)

```csharp
.UseMcpServer("myMcpServer")
```

#### Use Only Tools

```csharp
.UseMcpServer("myMcpServer", filterBuilder =>
{
    filterBuilder.OnlyTools();
})
```

#### Use Only Resources

```csharp
.UseMcpServer("myMcpServer", filterBuilder =>
{
    filterBuilder.OnlyResources();
})
```

#### Use Only Prompts

```csharp
.UseMcpServer("myMcpServer", filterBuilder =>
{
    filterBuilder.OnlyPrompts();
})
```

### Name Filtering

Filter elements by name using various patterns:

```csharp
.UseMcpServer("myMcpServer", filterBuilder =>
{
    filterBuilder.WithTools(toolConfig =>
    {
        // Whitelist by exact name
        toolConfig.Whitelist("get_user", "create_task");
        
        // Pattern matching
        toolConfig.Whitelist("get_*", "list_*", "process_*");
        
        // Regex matching
        toolConfig.Regex("^(get|list)_");
        
        // Prefix matching
        toolConfig.StartsWith("admin_");
        
        // Custom predicate
        toolConfig.Predicate(name => name.Length < 20);
        
        // Exclude specific items
        toolConfig.Exclude("delete_*", "dangerous_op");
    });
})
```

The same filtering options work for resources and prompts:

```csharp
.UseMcpServer("myMcpServer", filterBuilder =>
{
    filterBuilder
        .WithTools(tc => tc.Whitelist("data_*"))
        .WithResources(rc => rc.Whitelist("system_*"))
        .WithPrompts(pc => pc.Exclude("internal_*"));
})
```

### Combining Filters

Enable specific types with independent filters:

```csharp
.UseMcpServer("myMcpServer", filterBuilder =>
{
    filterBuilder
        .WithTools(tc => tc.Whitelist("get_*", "process_*"))
        .WithResources(rc => rc.StartsWith("data_"))
        .OnlyPrompts(); // This disables tools and resources
})
```

**Note**: `OnlyTools()`, `OnlyResources()`, and `OnlyPrompts()` disable the other element types.

## Element Injection

### Tools

Tools are registered as callable functions that the AI can invoke:

- Listed as available functions to the AI
- Can be called by the AI to perform actions
- Return results that feed back into the AI's reasoning

### Resources

Resources are injected as system messages:

- Provided as context to guide the AI's responses
- Available for the AI to reference
- Useful for configuration, documentation, or data context

### Prompts

Prompts are injected as system messages:

- Provide specific instructions or context
- Guide the AI's behavior within the scene
- Can complement the scene's own actors/instructions

## Complete Example

```csharp
services.AddPlayFramework(scenes =>
{
    scenes.Configure(settings =>
    {
        settings.OpenAi.Name = "playframework";
    })
    // Register multiple MCP servers
    .AddMcpServer("dataMcp", mcp =>
    {
        mcp.WithHttpServer("http://localhost:3000");
    })
    .AddMcpServer("agentMcp", mcp =>
    {
        mcp.WithCommand("node", "agent-server.js");
    })
    
    // Add HTTP client for API integration
    .AddHttpClient("apiDomain", x =>
    {
        x.BaseAddress = new Uri("https://api.example.com/");
    })
    
    // Scene 1: Data processing with tools and resources
    .AddScene(scene =>
    {
        scene
            .WithName("DataProcessing")
            .WithDescription("Process data using MCP tools")
            .WithOpenAi("playframework")
            .WithHttpClient("apiDomain")
            .UseMcpServer("dataMcp", filterBuilder =>
            {
                filterBuilder.WithTools(tc => tc.Whitelist("get_*", "process_*"));
            });
    })
    
    // Scene 2: Read-only resource access
    .AddScene(scene =>
    {
        scene
            .WithName("Documentation")
            .WithDescription("Access documentation and guidance")
            .WithOpenAi("playframework")
            .UseMcpServer("agentMcp", filterBuilder =>
            {
                filterBuilder.OnlyResources();
            });
    })
    
    // Scene 3: Agent actions with all MCP elements
    .AddScene(scene =>
    {
        scene
            .WithName("AgentActions")
            .WithDescription("Execute agent actions with full MCP access")
            .WithOpenAi("playframework")
            .UseMcpServer("dataMcp")
            .UseMcpServer("agentMcp", filterBuilder =>
            {
                filterBuilder.OnlyTools();
            });
    });
});
```

## Error Handling

MCP integration includes built-in error handling:

```csharp
try
{
    var response = await sceneManager.GetResponseAsync(
        "Please get the user data",
        sceneName: "DataProcessing");
}
catch (HttpRequestException ex)
{
    // MCP server unreachable
}
catch (TimeoutException ex)
{
    // MCP operation timed out
}
catch (InvalidOperationException ex)
{
    // Tool/resource/prompt not available or filtering issue
}
```

## Best Practices

### 1. Naming Convention

Use consistent naming patterns for MCP elements:

```
Tools:     action_verb_noun (e.g., get_user, create_task, delete_item)
Resources: resource_type_name (e.g., system_config, api_docs)
Prompts:   context_instructions (e.g., safety_guidelines, domain_rules)
```

### 2. Filtering Strategy

Define clear filtering rules based on scene purpose:

- **Public-facing scenes**: Restrict to safe, read-only operations
- **Internal scenes**: Allow full access to tools and resources
- **Admin scenes**: Carefully curate available operations

### 3. Timeout Configuration

Set appropriate timeouts for different scenarios:

```csharp
// Fast, simple operations
.WithTimeout(TimeSpan.FromSeconds(10))

// Complex operations
.WithTimeout(TimeSpan.FromSeconds(30))

// Long-running operations
.WithTimeout(TimeSpan.FromSeconds(120))
```

### 4. Server Health

Implement health checks for critical MCP servers:

```csharp
var registry = serviceProvider.GetRequiredService<McpRegistry>();
var client = registry.GetClient("myMcpServer");
// Use client and handle connectivity issues
```

### 5. Logging

Monitor MCP operations through logging:

```
[Information] MCP tool 'get_user' executed successfully
[Warning] MCP resource 'system_config' not found
[Error] Failed to connect to MCP server 'myMcpServer': Connection timeout
```

## Troubleshooting

### Server Not Found

```
InvalidOperationException: MCP server 'myMcpServer' not registered
```

**Solution**: Ensure `AddMcpServer()` is called before `UsePlayFramework()`.

### Connection Timeout

```
TimeoutException: MCP operation exceeded timeout
```

**Solution**: Increase timeout with `WithTimeout()` or check server availability.

### Tool Not Available

```
InvalidOperationException: Tool 'get_user' is not available in this scene
```

**Solution**: Check filtering configuration and ensure tool matches filter pattern.

### Empty Results

If tools, resources, or prompts aren't appearing:

1. Verify MCP server is running and accessible
2. Check filtering configuration
3. Review server's exposed elements with MCP client
4. Check element names match filter patterns

## Integration with External Services

MCP servers can be combined with other PlayFramework integrations:

```csharp
.AddScene(scene =>
{
    scene
        .WithName("ComprehensiveScene")
        .WithOpenAi("playframework")
        .WithHttpClient("apiDomain")
        .WithService<CustomService>(builder =>
        {
            builder.WithMethod(x => x.GetDataAsync);
        })
        .UseMcpServer("dataMcp", filterBuilder =>
        {
            filterBuilder.WithTools();
        })
        .WithActors(actors =>
        {
            actors.AddActor("Instruction text");
        });
})
```

All elements work together:
- MCP tools appear as callable functions
- HTTP client can make API calls
- Custom services are available
- Actors provide additional context

## References

- [PlayFramework Documentation](../src/Rystem.PlayFramework/README.md)
- [MCP Specification](https://modelcontextprotocol.io/)
- [Example Integrations](#complete-example)
