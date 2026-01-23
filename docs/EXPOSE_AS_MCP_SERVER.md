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

## All MCP Methods - Request/Response Examples

### tools/list

**Request:**
```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "method": "tools/list"
}
```

**Response:**
```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "result": {
    "tools": [
      {
        "name": "MyAssistant",
        "description": "AI Assistant for customer support and order management",
        "inputSchema": {
          "type": "object",
          "properties": {
            "message": {
              "type": "string",
              "description": "The message or request to send to the AI assistant"
            }
          },
          "required": ["message"]
        }
      }
    ]
  }
}
```

### resources/list

**Request:**
```json
{
  "jsonrpc": "2.0",
  "id": 2,
  "method": "resources/list"
}
```

**Response:**
```json
{
  "jsonrpc": "2.0",
  "id": 2,
  "result": {
    "resources": [
      {
        "uri": "playframework://MyAssistant/overview",
        "name": "MyAssistant Overview",
        "description": "Complete documentation for MyAssistant PlayFramework",
        "mimeType": "text/markdown"
      },
      {
        "uri": "playframework://MyAssistant/scenes/CustomerSupport",
        "name": "CustomerSupport",
        "description": "Handles customer inquiries",
        "mimeType": "text/markdown"
      }
    ]
  }
}
```

### resources/read

**Request:**
```json
{
  "jsonrpc": "2.0",
  "id": 3,
  "method": "resources/read",
  "params": {
    "uri": "playframework://MyAssistant/overview"
  }
}
```

**Response:**
```json
{
  "jsonrpc": "2.0",
  "id": 3,
  "result": {
    "contents": [
      {
        "uri": "playframework://MyAssistant/overview",
        "mimeType": "text/markdown",
        "text": "# MyAssistant - PlayFramework Overview\n\n## Description\nAI Assistant for customer support...\n"
      }
    ]
  }
}
```

### prompts/list

**Request:**
```json
{
  "jsonrpc": "2.0",
  "id": 4,
  "method": "prompts/list"
}
```

**Response:**
```json
{
  "jsonrpc": "2.0",
  "id": 4,
  "result": {
    "prompts": [
      {
        "name": "MyAssistant-system-prompt",
        "description": "AI Assistant for customer support and order management",
        "arguments": [
          {
            "name": "context",
            "description": "Optional additional context to include",
            "required": false
          }
        ]
      }
    ]
  }
}
```

### prompts/get

**Request:**
```json
{
  "jsonrpc": "2.0",
  "id": 5,
  "method": "prompts/get",
  "params": {
    "name": "MyAssistant-system-prompt",
    "arguments": {
      "context": "The user is asking about returns"
    }
  }
}
```

**Response:**
```json
{
  "jsonrpc": "2.0",
  "id": 5,
  "result": {
    "description": "AI Assistant for customer support and order management",
    "messages": [
      {
        "role": "user",
        "content": {
          "type": "text",
          "text": "You are a helpful assistant...\n\nAdditional Context:\nThe user is asking about returns"
        }
      }
    ]
  }
}
```

## Error Handling

The MCP server returns standard JSON-RPC 2.0 error responses:

| Error Code | Name | Description |
|------------|------|-------------|
| -32700 | Parse Error | Invalid JSON |
| -32600 | Invalid Request | Missing or empty method |
| -32601 | Method Not Found | Unknown MCP method |
| -32602 | Invalid Params | Invalid parameters |
| -32603 | Internal Error | Server error during execution |

**Error Response Example:**
```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "error": {
    "code": -32601,
    "message": "Method 'unknown/method' not found"
  }
}
```

## Configuration Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `Description` | `string?` | `null` | Description shown in tools/list |
| `Prompt` | `string?` | `null` | System prompt exposed via prompts/list |
| `EnableResources` | `bool` | `true` | Generate scene documentation as resources |
| `AuthorizationPolicy` | `string?` | `null` | .NET authorization policy name (null = public) |

## Multiple PlayFrameworks

You can expose multiple PlayFrameworks, each with its own endpoint:

```csharp
services.AddPlayFramework(builder =>
{
    builder.ExposeAsMcpServer(c => c.Description = "Customer Assistant");
    builder.AddScene(scene => scene.WithName("Support"));
}, name: "CustomerAssistant");

services.AddPlayFramework(builder =>
{
    builder.ExposeAsMcpServer(c => c.Description = "Sales Assistant");
    builder.AddScene(scene => scene.WithName("Sales"));
}, name: "SalesAssistant");

// In Program.cs
app.MapPlayFrameworkMcpEndpoints("/mcp");
// Creates:
// - POST /mcp/CustomerAssistant
// - POST /mcp/SalesAssistant
```

## Testing the MCP Server

You can test your MCP server using curl:

```bash
# Test tools/list
curl -X POST http://localhost:5000/mcp/MyAssistant \
  -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","id":1,"method":"tools/list"}'

# Test tools/call
curl -X POST http://localhost:5000/mcp/MyAssistant \
  -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","id":2,"method":"tools/call","params":{"name":"MyAssistant","arguments":{"message":"Hello!"}}}'
```

## Troubleshooting

### Endpoint returns 404
- Ensure `MapPlayFrameworkMcpEndpoints()` is called after `UseRouting()`
- Verify the PlayFramework name matches the URL path

### Authorization fails
- Check the authorization policy is registered in DI
- Verify the policy name matches exactly

### Empty tools/list response
- Ensure `ExposeAsMcpServer()` was called in the builder
- Check the PlayFramework name is registered correctly

### Resources not appearing
- Verify `EnableResources = true` (default)
- Check that scenes have descriptions set
