# Expose PlayFramework as MCP Server

This document describes how to expose a PlayFramework as an MCP (Model Context Protocol) server, allowing external MCP clients like Claude Desktop to consume your AI assistant.

## Overview

The ExposeAsMcpServer feature allows you to:
- Expose your PlayFramework as a single MCP tool
- Automatically generate resources (documentation) for each scene
- Configure authorization using .NET policies
- Use Minimal APIs for the endpoints

## Configuration

### 1. Register PlayFramework with ExposeAsMcpServer

```csharp
services.AddPlayFramework("MyAssistant", builder =>
{
    // Expose this PlayFramework as an MCP server
    builder.ExposeAsMcpServer(config =>
    {
        config.Description = "AI Assistant for customer support and order management";
        config.Prompt = "You are a helpful assistant...";  // Optional
        config.EnableResources = true;  // Default: true - generates scene docs
        config.AuthorizationPolicy = "ApiKeyPolicy";  // Optional - null = public
    });

    // Add your scenes
    builder.AddScene(scene =>
    {
        scene.WithName("CustomerSupport")
             .WithDescription("Handles customer inquiries");
    });

    builder.AddScene(scene =>
    {
        scene.WithName("OrderManagement")
             .WithDescription("Manages orders and returns");
    });
});
```

### 2. Map MCP Endpoints

In your `Program.cs`:

```csharp
var app = builder.Build();

// Map MCP endpoints for all exposed PlayFrameworks
app.MapPlayFrameworkMcpEndpoints("/mcp");

app.Run();
```

This creates endpoints like:
- `POST /mcp/MyAssistant` - JSON-RPC endpoint for MCP protocol

## MCP Protocol Methods

The exposed MCP server supports the following methods:

### tools/list
Returns the PlayFramework as a single tool with an input schema for the message parameter.

### tools/call
Executes the PlayFramework with the provided message. The SceneManager routes the request to the appropriate scene.

### resources/list
Returns documentation resources for:
- Overview of the entire PlayFramework
- Each configured scene

### resources/read
Returns the markdown documentation for a specific resource.

### prompts/list
Returns configured prompts (only if `Prompt` is set in config).

### prompts/get
Returns the prompt content with optional context parameter.

## Authorization

Authorization is configured at the PlayFramework level using standard .NET authorization policies:

```csharp
// Configure authorization policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiKeyPolicy", policy =>
        policy.RequireAssertion(ctx =>
        {
            var httpContext = ctx.Resource as HttpContext;
            return httpContext?.Request.Headers["X-Api-Key"] == "your-secret-key";
        }));
});

// Apply to PlayFramework
builder.ExposeAsMcpServer(config =>
{
    config.AuthorizationPolicy = "ApiKeyPolicy";
});
```

If `AuthorizationPolicy` is null, the endpoint allows anonymous access.

## Using with Claude Desktop

Add to your Claude Desktop configuration (`claude_desktop_config.json`):

```json
{
  "mcpServers": {
    "my-assistant": {
      "url": "http://localhost:5000/mcp/MyAssistant"
    }
  }
}
```

## Example Request/Response

### tools/call Request
```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "method": "tools/call",
  "params": {
    "name": "MyAssistant",
    "arguments": {
      "message": "What's the status of order #12345?"
    }
  }
}
```

### Response
```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "result": {
    "content": [
      {
        "type": "text",
        "text": "Order #12345 is currently being shipped and will arrive by Friday."
      }
    ],
    "isError": false
  }
}
```

## Architecture

```
MCP Client (Claude Desktop, etc.)
        ↓ POST /mcp/{PlayFrameworkName}
        ↓ JSON-RPC Request
        ↓
   Minimal API Endpoint
        ↓ Authorization Check (if policy configured)
        ↓
   McpMethodRouter
        ↓ Routes to appropriate handler
        ↓
   Handler (ToolsCallHandler, etc.)
        ↓ Uses IFactory<ISceneManager>
        ↓
   SceneManager.ExecuteAsync()
        ↓ Routes to appropriate scene
        ↓
   JSON-RPC Response
```

## File Structure

```
Mcp/Server/
├── Models/
│   ├── ExposeAsMcpServerConfig.cs     # Configuration options
│   ├── ExposedPlayFrameworkInfo.cs    # Registry entry info
│   ├── McpJsonRpcModels.cs            # JSON-RPC request/response
│   └── McpServerResponseModels.cs     # MCP-specific response types
├── Registry/
│   └── PlayFrameworkMcpServerRegistry.cs  # Tracks exposed frameworks
├── Documentation/
│   └── PlayFrameworkDocumentationBuilder.cs  # Generates markdown docs
├── Handlers/
│   ├── IMcpMethodHandler.cs           # Handler interface
│   ├── ToolsListHandler.cs            # tools/list
│   ├── ToolsCallHandler.cs            # tools/call
│   ├── ResourcesListHandler.cs        # resources/list
│   ├── ResourcesReadHandler.cs        # resources/read
│   ├── PromptsListHandler.cs          # prompts/list
│   └── PromptsGetHandler.cs           # prompts/get
├── Routing/
│   └── McpMethodRouter.cs             # Routes requests to handlers
└── Endpoints/
    └── PlayFrameworkMcpEndpointExtensions.cs  # MapPlayFrameworkMcpEndpoints
```
