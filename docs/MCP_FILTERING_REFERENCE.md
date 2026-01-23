# MCP Filtering Reference

Quick reference guide for filtering MCP elements in PlayFramework scenes.

## Filter Configuration Objects

### IMcpToolFilterConfig

Filters which tools are available to the scene:

```csharp
filterBuilder.WithTools(toolConfig =>
{
    // ... filtering methods
});
```

### IMcpResourceFilterConfig

Filters which resources are available to the scene:

```csharp
filterBuilder.WithResources(resourceConfig =>
{
    // ... filtering methods
});
```

### IMcpPromptFilterConfig

Filters which prompts are available to the scene:

```csharp
filterBuilder.WithPrompts(promptConfig =>
{
    // ... filtering methods
});
```

## Available Filter Methods

### Whitelist

Include only specific elements by name:

```csharp
toolConfig.Whitelist("get_user", "create_task", "update_item");
toolConfig.Whitelist("get_*", "list_*"); // Wildcard support
```

### Regex

Match elements using regular expressions:

```csharp
toolConfig.Regex("^get_");                    // Starts with "get_"
toolConfig.Regex("^(get|list)_");             // Starts with "get_" or "list_"
toolConfig.Regex(".*_admin$");                // Ends with "_admin"
toolConfig.Regex("^[a-z]+_[a-z]+_[a-z]+$");  // Exactly 3 parts separated by "_"
```

### StartsWith

Match elements that start with a specific prefix:

```csharp
toolConfig.StartsWith("admin_");
toolConfig.StartsWith("get_");
toolConfig.StartsWith("system_");
```

### Predicate

Use custom C# logic for filtering:

```csharp
// Include only short names
toolConfig.Predicate(name => name.Length < 20);

// Include only names without underscores
toolConfig.Predicate(name => !name.Contains("_"));

// Include only alphabetic names
toolConfig.Predicate(name => name.All(char.IsLetter));

// Complex logic
toolConfig.Predicate(name => 
{
    var parts = name.Split('_');
    return parts.Length >= 2 && parts[0] == "api";
});
```

### Exclude

Explicitly exclude specific elements:

```csharp
toolConfig.Exclude("delete_user", "destroy_database");
toolConfig.Exclude("admin_*", "internal_*", "debug_*");
```

## Common Patterns

### Public API Scene

Only expose safe, read-only operations:

```csharp
.UseMcpServer("dataMcp", filterBuilder =>
{
    filterBuilder.WithTools(tc =>
    {
        tc.Whitelist("get_*", "list_*", "search_*");
        tc.Exclude("get_admin_*");
    });
})
```

### Admin Scene

Allow everything except dangerous operations:

```csharp
.UseMcpServer("dataMcp", filterBuilder =>
{
    filterBuilder.WithTools(tc =>
    {
        tc.Exclude("delete_database", "reset_*", "*_destructive");
    });
})
```

### Resource-Only Scene

Provide data without allowing actions:

```csharp
.UseMcpServer("refMcp", filterBuilder =>
{
    filterBuilder.OnlyResources();
})
```

### Documentation Scene

Provide guidance and instructions:

```csharp
.UseMcpServer("docMcp", filterBuilder =>
{
    filterBuilder.OnlyPrompts();
})
```

### Multi-Scenario Scene

Different access for different operations:

```csharp
.UseMcpServer("mainMcp", filterBuilder =>
{
    filterBuilder
        .WithTools(tc => tc.Whitelist("read_*", "process_*"))
        .WithResources(rc => rc.StartsWith("public_"))
        .WithPrompts(pc => tc.Exclude("deprecated_*"));
})
```

## Element Type Controls

### Only Tools (Disable Resources and Prompts)

```csharp
.UseMcpServer("mcp", filterBuilder =>
{
    filterBuilder.OnlyTools();
})
```

### Only Resources (Disable Tools and Prompts)

```csharp
.UseMcpServer("mcp", filterBuilder =>
{
    filterBuilder.OnlyResources();
})
```

### Only Prompts (Disable Tools and Resources)

```csharp
.UseMcpServer("mcp", filterBuilder =>
{
    filterBuilder.OnlyPrompts();
})
```

### Tools + Resources (Disable Prompts)

```csharp
.UseMcpServer("mcp", filterBuilder =>
{
    filterBuilder
        .WithTools()
        .WithResources();
})
```

## Combining Filters

You can apply multiple filter types to the same element:

```csharp
toolConfig
    .Whitelist("get_*", "list_*")     // Start with whitelist
    .Exclude("*_internal");            // Then exclude specific items
```

**Note**: When combining, the first filter (Whitelist, Regex, etc.) defines the base set, and subsequent filters narrow it down.

## Filter Matching Rules

### Whitelist with Wildcards

```
Pattern         Matches
"get_*"         get_user, get_profile, get_data, get_admin
"*_admin"       sys_admin, user_admin, data_admin
"get_???"       get_abc, get_def (exactly 3 after get_)
```

### Predicate Matching

The predicate function receives the element name and should return:
- `true` to include the element
- `false` to exclude the element

```csharp
// Always include
toolConfig.Predicate(_ => true);

// Never include (disable element type)
toolConfig.Predicate(_ => false);

// Include if name contains specific word
toolConfig.Predicate(name => name.Contains("user"));

// Include based on length
toolConfig.Predicate(name => name.Length > 5);
```

## Best Practices

1. **Be Explicit**: Use whitelist for public scenes, exclude for private scenes
2. **Use Patterns**: Leverage wildcards and regex for maintainability
3. **Document Filtering**: Add comments explaining why filters are needed
4. **Test Filtering**: Verify available elements match expectations
5. **Version Controls**: When MCP servers update, review filter effectiveness

## Example: Tiered Access

```csharp
// Public scene - minimal access
.AddScene(publicScene =>
{
    publicScene.UseMcpServer("mcp", f =>
    {
        f.OnlyTools(tc => tc.Whitelist("get_*"));
    });
})

// User scene - more access
.AddScene(userScene =>
{
    userScene.UseMcpServer("mcp", f =>
    {
        f.WithTools(tc => tc.Whitelist("get_*", "create_*"))
         .WithResources(rc => rc.StartsWith("user_"));
    });
})

// Admin scene - full access except dangerous operations
.AddScene(adminScene =>
{
    adminScene.UseMcpServer("mcp", f =>
    {
        f.WithTools(tc => tc.Exclude("*_destructive", "delete_all"))
         .WithResources()
         .WithPrompts();
    });
})
```

## Troubleshooting

### Pattern Not Matching

Check that:
1. Element names match the pattern exactly (case-sensitive)
2. Wildcards (`*`) are in the correct positions
3. Regex syntax is valid

### All Elements Excluded

Verify:
1. Filters aren't too restrictive
2. Element names in MCP server match filter patterns
3. Exclude patterns aren't inadvertently blocking everything

### Inconsistent Results

Common causes:
1. Multiple conflicting filters applied
2. MCP server names inconsistently case-sensitive
3. Element names changed in MCP server update
