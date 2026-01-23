# MCP Documentation Index

Complete guide to Model Context Protocol (MCP) integration in Rystem.PlayFramework.

## Quick Links

- **[MCP Integration Guide](MCP_INTEGRATION.md)** - Comprehensive guide for implementing MCP
- **[MCP Filtering Reference](MCP_FILTERING_REFERENCE.md)** - Quick reference for filter syntax
- **[MCP Changelog](MCP_CHANGELOG.md)** - What's new and changes
- **[PlayFramework README](../src/Rystem.PlayFramework/README.md)** - Full PlayFramework documentation

## Getting Started

### 1. Register an MCP Server

```csharp
.AddMcpServer("myServer", mcp =>
{
    mcp.WithHttpServer("http://localhost:3000");
})
```

### 2. Use in a Scene

```csharp
.AddScene(scene =>
{
    scene.UseMcpServer("myServer");
})
```

### 3. Filter Elements (Optional)

```csharp
.UseMcpServer("myServer", filter =>
{
    filter.WithTools(tc => tc.Whitelist("get_*"));
})
```

## Documentation Hierarchy

```
MCP Documentation
├── MCP_QUICK_START.md (this file)
├── MCP_INTEGRATION.md (full guide)
├── MCP_FILTERING_REFERENCE.md (filter syntax)
├── MCP_CHANGELOG.md (what's new)
└── PlayFramework README (full framework docs)
    └── MCP Section (comprehensive examples)
```

## Topics by Experience Level

### Beginner

Start here if you're new to MCP or PlayFramework:

1. Read the "Getting Started" section above
2. Check [MCP_INTEGRATION.md - Quick Start](MCP_INTEGRATION.md#quick-start)
3. Review [MCP_CHANGELOG.md - Example: Complete Setup](MCP_CHANGELOG.md#example-complete-setup)
4. Try the complete example in [PlayFramework README](../src/Rystem.PlayFramework/README.md#complete-example-multi-service-scene-with-mcp)

### Intermediate

Deepen your understanding of MCP features:

1. Read [MCP_INTEGRATION.md - Advanced Configuration](MCP_INTEGRATION.md#advanced-configuration)
2. Study [MCP_FILTERING_REFERENCE.md - Common Patterns](MCP_FILTERING_REFERENCE.md#common-patterns)
3. Explore [MCP_INTEGRATION.md - Complete Example](MCP_INTEGRATION.md#complete-example)
4. Review [MCP_FILTERING_REFERENCE.md - Example: Tiered Access](MCP_FILTERING_REFERENCE.md#example-tiered-access)

### Advanced

Master MCP for production use:

1. Study [MCP_INTEGRATION.md - Architecture](MCP_INTEGRATION.md#architecture)
2. Review [MCP_INTEGRATION.md - Best Practices](MCP_INTEGRATION.md#best-practices)
3. Understand [MCP_FILTERING_REFERENCE.md - Filter Matching Rules](MCP_FILTERING_REFERENCE.md#filter-matching-rules)
4. Explore error handling in [MCP_INTEGRATION.md - Error Handling](MCP_INTEGRATION.md#error-handling)
5. Learn about performance in [MCP_CHANGELOG.md - Performance Considerations](MCP_CHANGELOG.md#performance-considerations)

## Common Tasks

### Register HTTP MCP Server

```csharp
.AddMcpServer("httpServer", mcp =>
{
    mcp.WithHttpServer("http://localhost:3000");
    mcp.WithTimeout(TimeSpan.FromSeconds(30));
})
```

**See**: [MCP_INTEGRATION.md - HTTP Server](MCP_INTEGRATION.md#http-server)

### Register Local Command MCP Server

```csharp
.AddMcpServer("localServer", mcp =>
{
    mcp.WithCommand("node", "path/to/server.js");
})
```

**See**: [MCP_INTEGRATION.md - Stdio Command](MCP_INTEGRATION.md#stdio-command)

### Use All MCP Elements

```csharp
.UseMcpServer("myServer")
```

**See**: [MCP_FILTERING_REFERENCE.md - Filter Configuration Objects](MCP_FILTERING_REFERENCE.md#filter-configuration-objects)

### Use Only Tools

```csharp
.UseMcpServer("myServer", fb =>
{
    fb.OnlyTools();
})
```

**See**: [MCP_FILTERING_REFERENCE.md - Only Tools](MCP_FILTERING_REFERENCE.md#only-tools-disable-resources-and-prompts)

### Use Only Resources

```csharp
.UseMcpServer("myServer", fb =>
{
    fb.OnlyResources();
})
```

**See**: [MCP_FILTERING_REFERENCE.md - Only Resources](MCP_FILTERING_REFERENCE.md#only-resources-disable-tools-and-prompts)

### Filter by Pattern

```csharp
.UseMcpServer("myServer", fb =>
{
    fb.WithTools(tc => tc.Whitelist("get_*", "list_*"));
})
```

**See**: [MCP_FILTERING_REFERENCE.md - Whitelist](MCP_FILTERING_REFERENCE.md#whitelist)

### Filter by Regex

```csharp
.UseMcpServer("myServer", fb =>
{
    fb.WithTools(tc => tc.Regex("^(get|list)_"));
})
```

**See**: [MCP_FILTERING_REFERENCE.md - Regex](MCP_FILTERING_REFERENCE.md#regex)

### Exclude Specific Elements

```csharp
.UseMcpServer("myServer", fb =>
{
    fb.WithTools(tc => tc.Exclude("delete_*", "admin_*"));
})
```

**See**: [MCP_FILTERING_REFERENCE.md - Exclude](MCP_FILTERING_REFERENCE.md#exclude)

### Create Multi-Server Scene

```csharp
.AddScene(scene =>
{
    scene
        .WithName("MultiMcp")
        .WithOpenAi("playframework")
        .UseMcpServer("dataMcp")
        .UseMcpServer("agentMcp", fb =>
        {
            fb.OnlyTools();
        });
})
```

**See**: [MCP_INTEGRATION.md - Complete Example](MCP_INTEGRATION.md#complete-example)

### Create Tiered Access

```csharp
// Public scene - minimal access
.AddScene(publicScene =>
{
    publicScene.UseMcpServer("mcp", f =>
    {
        f.OnlyTools(tc => tc.Whitelist("get_*"));
    });
})

// Admin scene - full access
.AddScene(adminScene =>
{
    adminScene.UseMcpServer("mcp", f =>
    {
        f.WithTools(tc => tc.Exclude("*_destructive"))
         .WithResources()
         .WithPrompts();
    });
})
```

**See**: [MCP_FILTERING_REFERENCE.md - Example: Tiered Access](MCP_FILTERING_REFERENCE.md#example-tiered-access)

## Troubleshooting

### "MCP server 'X' not registered"

- Ensure `AddMcpServer()` is called before scene configuration
- Check server name spelling (case-sensitive)

**See**: [MCP_INTEGRATION.md - Troubleshooting](MCP_INTEGRATION.md#troubleshooting)

### "Tool 'X' is not available in this scene"

- Verify tool name matches filter pattern
- Check MCP server is running and returns the tool
- Review filter configuration

**See**: [MCP_FILTERING_REFERENCE.md - Troubleshooting](MCP_FILTERING_REFERENCE.md#troubleshooting)

### Connection Timeout

- Increase timeout: `WithTimeout(TimeSpan.FromSeconds(60))`
- Verify MCP server is reachable
- Check network connectivity

**See**: [MCP_INTEGRATION.md - Server Health](MCP_INTEGRATION.md#best-practices)

### Empty Results

- Verify MCP server returns elements
- Check filter patterns match element names
- Review element names in server

**See**: [MCP_FILTERING_REFERENCE.md - All Elements Excluded](MCP_FILTERING_REFERENCE.md#all-elements-excluded)

## Key Concepts

### Server Registration vs. Scene Configuration

- **AddMcpServer()**: Registers server globally (once per application)
- **UseMcpServer()**: Configures scene to use registered server (per-scene)

### Filter Types

- **OnlyX**: Disable other element types
- **WithX**: Enable with optional filtering
- **No configuration**: All elements, no filtering

### Filter Methods

- **Whitelist**: Include by name
- **Regex**: Match by pattern
- **StartsWith**: Match by prefix
- **Predicate**: Custom logic
- **Exclude**: Explicitly exclude

### Element Types

- **Tools**: Callable functions
- **Resources**: Data context
- **Prompts**: Instruction context

## API Reference

### Server Configuration

```csharp
IMcpServerBuilder WithHttpServer(string serverUrl);
IMcpServerBuilder WithCommand(string executable, params string[] args);
IMcpServerBuilder WithTimeout(TimeSpan timeout);
```

### Scene Configuration

```csharp
ISceneBuilder UseMcpServer(string name);
ISceneBuilder UseMcpServer(string name, 
    Action<IMcpServerToolFilterBuilder> filterBuilder);
```

### Filter Configuration

```csharp
IMcpServerToolFilterBuilder OnlyTools(Action<IMcpToolFilterConfig>? builder = null);
IMcpServerToolFilterBuilder OnlyResources(Action<IMcpResourceFilterConfig>? builder = null);
IMcpServerToolFilterBuilder OnlyPrompts(Action<IMcpPromptFilterConfig>? builder = null);
IMcpServerToolFilterBuilder WithTools(Action<IMcpToolFilterConfig>? builder = null);
IMcpServerToolFilterBuilder WithResources(Action<IMcpResourceFilterConfig>? builder = null);
IMcpServerToolFilterBuilder WithPrompts(Action<IMcpPromptFilterConfig>? builder = null);
```

### Tool Filter Methods

```csharp
IMcpToolFilterConfig Whitelist(params string[] names);
IMcpToolFilterConfig Regex(string pattern);
IMcpToolFilterConfig StartsWith(string prefix);
IMcpToolFilterConfig Predicate(Func<string, bool> filter);
IMcpToolFilterConfig Exclude(params string[] names);
```

Same methods available for `IMcpResourceFilterConfig` and `IMcpPromptFilterConfig`.

## Related Resources

- **[Rystem Framework](https://github.com/KeyserDSoze/Rystem)** - Main framework
- **[MCP Specification](https://modelcontextprotocol.io/)** - Official MCP docs
- **[OpenAI Integration](../src/Rystem.OpenAi/README.md)** - OpenAI documentation
- **[PlayFramework Overview](../src/Rystem.PlayFramework/README.md)** - Full PlayFramework guide

## Contributing

Found issues or have suggestions for improvement?

- Create an issue: [GitHub Issues](https://github.com/KeyserDSoze/Rystem.OpenAi/issues)
- Submit a PR: [GitHub PRs](https://github.com/KeyserDSoze/Rystem.OpenAi/pulls)
- Join community: [Discord](https://discord.gg/wUh2fppr)

## Version Information

- **PlayFramework**: .NET 10
- **MCP Support**: Added in latest version
- **Last Updated**: 2025
