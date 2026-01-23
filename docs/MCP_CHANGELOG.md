# MCP Integration Changelog

## Overview

This document summarizes the Model Context Protocol (MCP) integration added to Rystem.PlayFramework, enabling seamless access to external tools, resources, and prompts.

## What's New

### 1. Global MCP Server Registration

Register MCP servers globally during application startup:

```csharp
.AddMcpServer("myServer", mcp =>
{
    mcp.WithHttpServer("http://localhost:3000");
})
```

**Location**: `ScenesBuilder.AddMcpServer()`

### 2. Scene-Based MCP Usage

Each scene independently decides which MCP elements to use:

```csharp
.AddScene(scene =>
{
    scene.UseMcpServer("myServer");
})
```

**Location**: `SceneBuilder.UseMcpServer()`

### 3. Flexible Element Filtering

Fine-grained control over Tools, Resources, and Prompts:

- **OnlyTools**: Use only tools
- **OnlyResources**: Use only resources
- **OnlyPrompts**: Use only prompts
- **WithTools/WithResources/WithPrompts**: Use with optional filtering

### 4. Multiple Filtering Strategies

Filter MCP elements by:
- Whitelist (exact names or patterns)
- Regex (regular expressions)
- Prefix (StartsWith)
- Custom predicates
- Exclusion patterns

### 5. Support for Multiple Communication Methods

- **HTTP Servers**: Remote MCP servers via HTTP
- **Stdio Commands**: Local executables via stdio

## New Components

### Models (`src/Rystem.PlayFramework/Models/Mcp/`)

- **McpToolCall.cs**: Information about executed MCP tool calls
- **McpServerConfiguration.cs**: MCP server connection configuration
- **McpSceneFilter.cs**: Filtering rules for Tools, Resources, Prompts per scene
- **McpResource.cs**: MCP resource representation
- **McpPrompt.cs**: MCP prompt with content

### Services (`src/Rystem.PlayFramework/Services/Mcp/`)

- **IMcpClient.cs**: Interface for MCP protocol operations
  - ListToolsAsync()
  - CallToolAsync()
  - ListResourcesAsync()
  - ReadResourceAsync()
  - ListPromptsAsync()
  - GetPromptAsync()

- **HttpMcpClient.cs**: HTTP-based MCP client implementation

- **McpRegistry.cs**: Singleton service managing registered MCP servers

- **McpClientFactory.cs**: Factory for creating MCP clients

- **McpToolExecutor.cs**: Executes MCP tool calls within scene context

- **McpInitializationHostedService.cs**: Initializes MCP servers on startup

### Builders (`src/Rystem.PlayFramework/Builder/Mcp/`)

- **McpServerBuilder.cs**: Fluent builder for server configuration
- **McpServerToolFilterBuilder.cs**: Fluent builder for element filtering

### Filter Configurations (`src/Rystem.PlayFramework/Builder/Mcp/Config/`)

- **McpToolFilterConfig.cs**: Tool filtering configuration
- **McpResourceFilterConfig.cs**: Resource filtering configuration
- **McpPromptFilterConfig.cs**: Prompt filtering configuration

## Modified Files

### Scene Model

- **Scene.cs**: Added `McpServerName` and `McpSceneFilter` properties
- **IScene.cs**: Added interface members for MCP configuration

### Scene Builder

- **SceneBuilder.cs**: Added `UseMcpServer()` method with fluent API

### Scenes Builder

- **ScenesBuilder.cs**: Added `AddMcpServer()` method and MCP initialization logic
- **IScenesBuilder.cs**: Added interface for `AddMcpServer()`

### Scene Manager

- **SceneManager.cs**: 
  - Added `ExecuteMcpToolAsync()` method
  - Modified `GetResponseFromSceneAsync()` to inject MCP elements (tools, resources, prompts)
  - Added MCP tool invocation handling

### Function Handler

- **FunctionHandler.cs**: Added `McpToolCall` support for tracking MCP tool executions

### Dependency Injection

- **ServiceCollectionExtensions.cs**: 
  - Registered `McpRegistry` as singleton
  - Registered `McpClientFactory`
  - Registered `McpToolExecutor` as scoped
  - Registered `McpInitializationHostedService`

## Key Features

### 1. Global Server Registration Once

```csharp
// Registered once globally
.AddMcpServer("myServer", config => { })
```

### 2. Per-Scene Configuration

```csharp
// Each scene decides what to use
.UseMcpServer("myServer", filter => { })
```

### 3. Independent Element Control

```csharp
filterBuilder
    .WithTools(tc => tc.Whitelist("get_*"))
    .WithResources(rc => rc.Whitelist("public_*"))
    .OnlyPrompts();  // Only prompts, no tools/resources
```

### 4. Smart Filtering

```csharp
// Multiple ways to filter
toolConfig.Whitelist("a", "b*", "c");    // Exact + pattern
toolConfig.Regex("^get_");                // Regex
toolConfig.StartsWith("admin_");          // Prefix
toolConfig.Predicate(name => name.Length < 20);  // Custom logic
toolConfig.Exclude("dangerous_*");        // Exclusion
```

### 5. Element Injection in Scenes

- **Tools**: Available as callable functions the AI can invoke
- **Resources**: Injected as system messages for context
- **Prompts**: Injected as system messages for guidance

## Integration Points

### With PlayFramework

- Seamlessly works with existing Scene, Actor, and Service integrations
- Tools appear alongside scene services and actors
- Resources and prompts provide additional context
- Works with HTTP clients and OpenAI integration

### Architecture Compatibility

```
Scene + MCP Server
├─ Tools (callable by AI)
├─ Resources (context)
├─ Prompts (guidance)
├─ Services (methods)
├─ Actors (instructions)
└─ HTTP APIs (external calls)
```

## Performance Considerations

### MCP Server Connection

- **Initialization**: MCP servers initialize once on startup via hosted service
- **Caching**: Tool/resource/prompt lists cached per server
- **Timeouts**: Configurable per server (default 30 seconds)

### Per-Scene Filtering

- **Filter Compilation**: Filters compiled once during scene configuration
- **Matching**: O(1) filter matching during scene execution
- **Overhead**: Minimal - filtering happens before AI execution

## Migration Guide

### From Previous Versions

If you were using PlayFramework without MCP:

1. No changes required - MCP is opt-in
2. Call `AddMcpServer()` only if you want to use MCP
3. Call `UseMcpServer()` in scenes that need MCP elements

### Upgrading Existing Scenes

To add MCP to an existing scene:

```csharp
// Before
.AddScene(scene =>
{
    scene.WithName("MyScene").WithOpenAi("openai");
})

// After
.AddScene(scene =>
{
    scene
        .WithName("MyScene")
        .WithOpenAi("openai")
        .UseMcpServer("myServer");  // Add this line
})
```

## Documentation Files

### New Documentation

1. **MCP_INTEGRATION.md**: Comprehensive integration guide
2. **MCP_FILTERING_REFERENCE.md**: Quick reference for filtering syntax
3. **MCP_CHANGELOG.md**: This file

### Updated Documentation

1. **README.md**: Added MCP section and quick start
2. **src/Rystem.PlayFramework/README.md**: Added extensive MCP documentation

## Example: Complete Setup

```csharp
services.AddPlayFramework(scenes =>
{
    scenes.Configure(settings => settings.OpenAi.Name = "playframework")
        
        // Register MCP servers globally
        .AddMcpServer("dataMcp", mcp =>
        {
            mcp.WithHttpServer("http://localhost:3000");
            mcp.WithTimeout(TimeSpan.FromSeconds(60));
        })
        
        // Scene 1: Read-only data access
        .AddScene(scene =>
        {
            scene
                .WithName("DataRead")
                .WithOpenAi("playframework")
                .UseMcpServer("dataMcp", fb =>
                {
                    fb.WithTools(tc => tc.Whitelist("get_*", "list_*"));
                });
        })
        
        // Scene 2: Full access
        .AddScene(scene =>
        {
            scene
                .WithName("DataProcess")
                .WithOpenAi("playframework")
                .UseMcpServer("dataMcp");  // All elements
        });
});
```

## Future Enhancements

Potential future additions:

- [ ] Direct WebSocket support for real-time updates
- [ ] Built-in caching for tool/resource/prompt lists
- [ ] Tool result streaming
- [ ] MCP server health monitoring
- [ ] Metrics and tracing integration

## Support and Resources

- **Full Documentation**: [MCP_INTEGRATION.md](MCP_INTEGRATION.md)
- **Filter Reference**: [MCP_FILTERING_REFERENCE.md](MCP_FILTERING_REFERENCE.md)
- **PlayFramework Guide**: [PlayFramework README](src/Rystem.PlayFramework/README.md)
- **MCP Specification**: https://modelcontextprotocol.io/
