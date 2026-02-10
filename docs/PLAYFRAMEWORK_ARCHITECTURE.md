# PlayFramework - Complete Architecture Guide

## Table of Contents
1. [Vision & Goals](#vision--goals)
2. [Core Architecture](#core-architecture)
3. [Execution Flow](#execution-flow)
4. [Core Components](#core-components)
5. [Scene System](#scene-system)
6. [Actor System](#actor-system)
7. [Planning System](#planning-system)
8. [Summarization System](#summarization-system)
9. [Caching System](#caching-system)
10. [MCP Integration](#mcp-integration)
11. [Cost & Token Tracking](#cost--token-tracking)
12. [Loop Prevention & Safety](#loop-prevention--safety)
13. [Architectural Decisions](#architectural-decisions)
14. [Extension Points](#extension-points)

---

## Vision & Goals

### What is PlayFramework?

PlayFramework is an **AI orchestration framework** that enables building complex, multi-step AI applications by:

- Breaking down complex tasks into **scenes** (specialized contexts)
- Using **actors** to provide dynamic context
- Creating **execution plans** with deterministic or AI-driven planning
- Managing **conversation history** with intelligent summarization
- **Caching** responses for performance
- Integrating with **Model Context Protocol (MCP)** servers
- Tracking **costs and tokens** across all operations
- Preventing **infinite loops** and redundant executions

### Design Philosophy

1. **Scene-Based Architecture**: Like a theatrical play, complex interactions are broken into scenes, each with specific actors and tools
2. **Context Management**: Intelligent handling of conversation history through summarization and caching
3. **Deterministic Planning**: AI-driven planning creates execution plans before running, reducing costs and improving reliability
4. **Extensibility**: Every component (planner, summarizer, director, cache) can be customized
5. **Safety First**: Multiple mechanisms prevent infinite loops, redundant executions, and runaway costs
6. **Observable**: Complete tracking of tokens, costs, and execution flow

---

## Core Architecture

### High-Level Components

```
┌─────────────────────────────────────────────────────────────┐
│                    PlayFramework                             │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │   Planner    │  │ Summarizer   │  │   Director   │     │
│  │ (Strategy)   │  │ (Context)    │  │ (Orchestr.)  │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │              Scene Manager (Core)                     │  │
│  │  - Execution flow control                            │  │
│  │  - Context management                                │  │
│  │  - Scene orchestration                               │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                              │
│  ┌───────────┐  ┌───────────┐  ┌───────────┐  ┌─────────┐ │
│  │  Scenes   │  │  Actors   │  │   Tools   │  │  Cache  │ │
│  │ (Context) │  │ (Dynamic) │  │ (Actions) │  │ (Speed) │ │
│  └───────────┘  └───────────┘  └───────────┘  └─────────┘ │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │           OpenAI Integration Layer                    │  │
│  │  - Chat API                                          │  │
│  │  - Function calling                                  │  │
│  │  - Cost calculation                                  │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │           MCP Integration Layer                       │  │
│  │  - Tools, Resources, Prompts from external servers  │  │
│  │  - Server as MCP (expose PlayFramework via MCP)     │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### Key Design Patterns

1. **Factory Pattern**: Scene creation, OpenAI client creation
2. **Strategy Pattern**: Planner, Summarizer, Director are pluggable
3. **Builder Pattern**: Fluent API for configuring scenes, actors, tools
4. **Chain of Responsibility**: Scene execution with fallbacks
5. **Command Pattern**: Tool execution encapsulation
6. **Observer Pattern**: Streaming responses with `IAsyncEnumerable`
7. **Repository Pattern**: Cache service abstraction

---

## Execution Flow

### Simple Flow (No Planning)

```
User Request
    ↓
Initialize Context (load cache, create chat client)
    ↓
Execute Main Actors (add system messages)
    ↓
Add User Message
    ↓
Call OpenAI with all available scenes as tools
    ↓
OpenAI selects scene(s)
    ↓
For each selected scene:
    - Clear tools
    - Add scene-specific actors
    - Add scene-specific tools
    - Execute tools if called
    ↓
Director decides: continue or stop?
    ↓
If continue: repeat scene selection
If stop: generate final response
    ↓
Save to cache
```

### Planning Flow (Deterministic)

```
User Request
    ↓
Initialize Context (load cache, create chat client)
    ↓
Execute Main Actors → Track Context for Planner
    ↓
Create Execution Plan
    ├─ Analyze: current_context (from actors)
    ├─ Analyze: available_scenes (with tools & actors info)
    └─ Decision:
        ├─ needs_execution=false → Direct Answer
        └─ needs_execution=true → Structured Plan
    ↓
If Direct Answer:
    └─ Return reasoning as final response
    
If Structured Plan:
    ↓
    Execute Plan Steps (in order)
        ├─ Step 1: Scene A with specific tools
        ├─ Step 2: Scene B (may depend on Step 1)
        └─ Step N: Scene N
    ↓
    Check Continuation
        ├─ All steps completed?
        │   └─ Generate final response
        │
        └─ Need more data?
            ├─ Filter already-executed scenes
            ├─ Create new plan
            └─ Execute recursively (max depth: 5)
    ↓
Save to cache
```

---

## Core Components

### SceneManager

**Purpose**: Central orchestrator that manages the entire execution flow.

**Key Responsibilities**:
- Initialize context (cache, chat client)
- Execute main actors to provide base context
- Coordinate with Planner for strategy
- Execute scenes in correct order
- Manage conversation history with Summarizer
- Coordinate with Director for multi-scene orchestration
- Track costs and tokens
- Prevent infinite loops
- Stream responses to caller

**Key Methods**:
- `ExecuteAsync()`: Main entry point
- `RequestAsync()`: Scene selection and execution (non-planning mode)
- `ExecutePlanAsync()`: Plan-based execution (planning mode)
- `GenerateFinalResponseAsync()`: Create final answer from gathered data
- `EnsureSummarizedForNextRequestAsync()`: Runtime summarization

**Architectural Decisions**:
1. **Centralized ChatClient**: `SceneContext.ChatClient` is shared across all operations to maintain conversation continuity
2. **Streaming Responses**: Returns `IAsyncEnumerable<AiSceneResponse>` for real-time feedback
3. **YieldAndTrack Pattern**: All responses are tracked in context AND yielded to caller
4. **Fuzzy Scene Matching**: Handles LLM name variations (e.g., "Navigation-Site-Helper" vs "Navigation Site Helper")

### SceneContext

**Purpose**: Holds all state for a single request execution.

**Key Properties**:
```csharp
public sealed class SceneContext
{
    // Core
    public required IServiceProvider ServiceProvider { get; init; }
    public required string InputMessage { get; set; }
    public IOpenAiChat ChatClient { get; set; }
    
    // Tracking
    public List<AiSceneResponse> Responses { get; init; }
    public Dictionary<string, HashSet<SceneRequestContext>> ExecutedScenes { get; }
    public HashSet<string> ExecutedTools { get; }
    
    // Context Management
    public string? ConversationSummary { get; set; }
    public List<string> MainActorContext { get; } // For planner
    
    // Planning
    public ExecutionPlan? ExecutionPlan { get; set; }
    
    // Cost Tracking
    public decimal TotalCost { get; set; }
    
    // Dynamic Properties
    public Dictionary<object, object> Properties { get; init; }
}
```

**Why This Design?**:
- **Mutable State**: Context evolves during execution, so immutable patterns would be inefficient
- **Centralized State**: All components access same context for consistency
- **Cost Aggregation**: Total cost accumulates across all operations
- **Loop Prevention**: `ExecutedScenes` and `ExecutedTools` prevent re-execution

---

## Scene System

### What is a Scene?

A **scene** is a specialized context with:
- **Name & Description**: Identifies purpose
- **Tools**: Functions the LLM can call in this context
- **Actors**: Provide dynamic system messages
- **MCP Server**: Optional integration with external MCP servers

### Scene Configuration

```csharp
builder.AddScene(scene => scene
    .WithName("User Profile Manager")
    .WithDescription("Manages user profile operations")
    
    // Service-based tools
    .WithService<IUserService>(svc => svc
        .WithMethod(x => x.GetUserAsync, "GetUser", "Retrieve user by ID")
        .WithMethod(x => x.UpdateUserAsync, "UpdateUser", "Update user info"))
    
    // Scene-specific actors
    .WithActors(actors => actors
        .AddActor("You are a friendly customer service agent")
        .AddActor<ValidationActor>())
    
    // Optional: MCP server integration
    .UseMcpServer("user-data-server", filter => filter
        .WithTools(tools => tools
            .Include("search_users", "delete_user"))));
```

### Tool Types

1. **Service Methods**: C# methods exposed as tools
2. **HTTP Requests**: API calls with parameter mapping
3. **MCP Tools**: Tools from external MCP servers

### Scene Selection

**Non-Planning Mode**:
- All scenes exposed as tools to OpenAI
- LLM selects which scene(s) to use
- Director can trigger re-selection

**Planning Mode**:
- Planner analyzes and creates execution order
- Scenes executed in planned sequence
- No re-selection (unless plan incomplete)

---

## Actor System

### What is an Actor?

An **actor** provides dynamic context (system messages) that can:
- Read from database
- Access user session
- Inject domain knowledge
- Provide validation rules

### Actor Types

```csharp
// 1. Simple Actor (static message)
.AddActor("You are an expert in...")

// 2. Dynamic Actor (function)
.AddActor(context => {
    var user = context.GetProperty<User>("CurrentUser");
    return $"User: {user.Name}, Role: {user.Role}";
})

// 3. Async Actor
.AddActor(async (context, ct) => {
    var settings = await db.GetSettingsAsync(ct);
    return $"Settings: {settings.ToJson()}";
})

// 4. Custom Actor (class)
.AddActor<DatabaseContextActor>()
```

### Main Actors vs Scene Actors

**Main Actors**:
- Execute ONCE at start
- Available in ALL scenes
- Their context is passed to Planner
- Example: User session, global settings

**Scene Actors**:
- Execute when scene is entered
- Only available in that scene
- Example: Scene-specific rules, validation

### Actor Context Tracking

**Key Innovation**: Main actors' output is tracked in `MainActorContext` and passed to the Planner.

```csharp
// Main actor produces:
"UserId: user@example.com
Books: [
  {Id: '0a8d3ff1...', Name: 'Il grande libro dei Galli'},
  {Id: 'f8bc4e6d...', Name: 'G'}
]"

// Planner receives this in current_context
// Can answer "navigate to book X" directly without API calls!
```

---

## Planning System

### Why Planning?

**Problems Solved**:
1. **Redundant API Calls**: Without planning, LLM may call same tools multiple times
2. **Wrong Order**: Tools may be called in illogical sequence
3. **Unnecessary Execution**: May execute when data is already in context
4. **Cost**: Every LLM call costs money

### Deterministic Planner

**Strategy**: Use LLM to create a **structured execution plan** before executing anything.

**Input to Planner**:
```json
{
  "user_request": "Navigate to 'Il grande libro dei Galli'",
  "current_context": [
    "Books: [{Id:'0a8d3ff1...',Name:'Il grande libro dei Galli'}]"
  ],
  "available_scenes": [
    {
      "name": "Navigation-Site-Helper",
      "description": "Use this scene to navigate",
      "available_tools": [
        {"name": "RetrieveBookInformation", "description": "Get book info"}
      ],
      "actors": [
        {"role": "Answer with SPECIFIC_COMMAND:Navigate(/path)"}
      ]
    }
  ],
  "main_actors": [
    {"role": "You're a skilled novelist", "type": null},
    {"role": null, "type": "UserSettingsActor"}
  ]
}
```

**Output from Planner**:

**Option A - Direct Answer** (data already in context):
```json
{
  "needs_execution": false,
  "reasoning": "SPECIFIC_COMMAND:Navigate(/book/0a8d3ff1-14ff-4ffa-bdb8-75bfef069713)",
  "steps": []
}
```

**Option B - Execution Plan** (need to gather data):
```json
{
  "needs_execution": true,
  "reasoning": "Need to retrieve chapter info before generating",
  "steps": [
    {
      "step_number": 1,
      "scene_name": "Text-writer-for-paragraph-and-chapter",
      "purpose": "Retrieve chapter information",
      "expected_tools": ["RetrieveChapterInformation"],
      "depends_on_step": null
    },
    {
      "step_number": 2,
      "scene_name": "Text-writer-for-paragraph-and-chapter",
      "purpose": "Generate new paragraph",
      "expected_tools": ["CreateParagraph"],
      "depends_on_step": 1
    }
  ]
}
```

### Planning Prompt Strategy

**Key Instructions**:
1. **Check current_context FIRST**: Data may already be available
2. **Use actor patterns**: If actor says "respond with SPECIFIC_COMMAND:", use that format
3. **Exact scene names**: Copy-paste from available_scenes
4. **Logical order**: Some steps may depend on others

### Continuation Check

After executing planned steps, the planner checks:
1. **All steps completed?** → Generate final response
2. **Can answer with gathered data?** → Generate final response
3. **Need more data?** → Create new plan (filtered by already-executed scenes)

**Safety**: Maximum recursion depth of 5 to prevent infinite planning loops.

---

## Summarization System

### Why Summarization?

**Problem**: As conversations grow, token costs explode and context window limits are hit.

**Solution**: Periodically summarize conversation history and replace messages with summary.

### Two Types of Summarization

#### 1. Initial Summarization (from cache)

When loading cached conversation:
- If message count exceeds threshold
- Summarize entire history
- Load summary instead of all messages

#### 2. Runtime Summarization

During execution:
- Monitor `context.Responses` count
- When threshold exceeded
- Summarize current context
- **Clear messages** but **keep tools**
- Add summary as system message

**Key Method**:
```csharp
private async ValueTask<AiSceneResponse?> EnsureSummarizedForNextRequestAsync(
    SceneContext context,
    SceneRequestSettings requestSettings,
    CancellationToken cancellationToken)
{
    if (_summarizer == null || !_summarizer.ShouldSummarize(context.Responses))
        return null;

    var summary = await _summarizer.SummarizeAsync(context.Responses, cancellationToken);
    
    context.ConversationSummary = summary;
    context.Responses.Clear();
    
    // CRITICAL: Clear messages but keep tools
    context.ChatClient.ClearMessages();
    context.ChatClient.AddSystemMessage(summary);
    
    return /* summarization response */;
}
```

### ClearMessages() Innovation

**Problem**: Summarizing required creating a new chat client, losing:
- Configured tools
- Temperature/model settings
- Any other configurations

**Solution**: Add `ClearMessages()` method that clears ONLY messages, preserving everything else.

```csharp
public interface IOpenAiChat
{
    // ...
    
    /// <summary>
    /// Clear all messages (system, user, assistant) while preserving tools and settings.
    /// Used for conversation summarization.
    /// </summary>
    IOpenAiChat ClearMessages();
}
```

---

## Caching System

### Purpose

**Cache** responses to:
- Avoid redundant OpenAI calls
- Improve response time
- Reduce costs
- Maintain conversation history

### Cache Behavior

**Default** (`CacheBehavior.Default`):
- Read from cache on request
- Write to cache after execution
- Cache key: user-provided or auto-generated GUID

**Avoidable** (`CacheBehavior.Avoidable`):
- Skip cache completely
- Useful for real-time data

**Forever** (`CacheBehavior.Forever`):
- Never expires
- Useful for static content

### Custom Cache Implementation

```csharp
public interface ICacheService
{
    Task<List<AiSceneResponse>?> GetAsync(string key, CancellationToken cancellationToken);
    Task SetAsync(string key, List<AiSceneResponse> value, CacheBehavior behavior, CancellationToken cancellationToken);
}
```

**Implementations**:
- `CacheService`: In-memory with Redis/distributed cache support
- Custom: Implement `ICustomCache` for database, file system, etc.

### Cache + Summarization

**Integration**:
1. Load from cache
2. Check if summarization needed
3. If yes: summarize and store summary
4. Execute new request
5. Append new responses to cache

---

## MCP Integration

### What is MCP?

**Model Context Protocol** is a standardized way for AI applications to access:
- **Tools**: Executable functions
- **Resources**: Documents, data files
- **Prompts**: Reusable prompt templates

### Client Mode (Consume MCP Servers)

**Use Case**: Your PlayFramework app consumes tools/resources from external MCP servers.

```csharp
services.AddPlayFramework(builder => 
{
    // Register MCP servers
    builder.ConfigureMcpServers(mcp => mcp
        .AddServer("filesystem", config => config
            .UseStdio("path/to/mcp-server-filesystem", "arg1"))
        .AddServer("database", config => config
            .UseSse("https://mcp-server.com/sse")));
    
    // Use MCP server in scene
    builder.AddScene(scene => scene
        .WithName("File Manager")
        .UseMcpServer("filesystem", filter => filter
            .WithTools(tools => tools
                .Include("read_file", "write_file")
                .Exclude("delete_file"))
            .WithResources(resources => resources
                .IncludeByUri("file:///*"))
            .OnlyTools())); // Disable resources & prompts
});
```

**Filtering System**:
- **Include/Exclude**: By name or regex
- **Predicates**: Custom logic
- **Element Types**: Tools, Resources, Prompts can be enabled/disabled independently

### Server Mode (Expose as MCP Server)

**Use Case**: Expose your PlayFramework scenes as MCP tools for other applications.

```csharp
services.AddPlayFramework(builder => 
{
    builder.AddScene(/* ... */);
    
    // Expose entire PlayFramework as MCP server
    builder.ExposeAsMcpServer(expose => expose
        .WithDescription("My AI Assistant")
        .WithPrompt("You are a helpful assistant...")
        .EnableResources() // Expose scene documentation as resources
        .WithAuthorization("MyPolicy")); // Optional auth
});

// In Program.cs
app.MapPlayFrameworkMcp(); // Endpoint: POST /mcp
```

**What Gets Exposed**:
- **Tools**: Each scene becomes an MCP tool
- **Resources**: Scene documentation (markdown)
- **Prompts**: Configured prompts for using the system

**MCP Endpoint**:
```
POST /mcp
Content-Type: application/json

{
  "jsonrpc": "2.0",
  "method": "tools/list",
  "id": 1
}
```

### MCP Tool Execution

When LLM calls MCP tool:
1. `McpToolExecutor` receives call
2. Look up `McpToolCall` (server name + tool name + client)
3. Call `client.CallToolAsync(toolName, arguments)`
4. Return result to LLM

---

## Cost & Token Tracking

### Why Track Costs?

- **Transparency**: Know exactly what each request costs
- **Budgeting**: Set limits and alerts
- **Optimization**: Identify expensive operations
- **Billing**: Charge customers accurately

### Token Tracking

**Where Tokens are Tracked**:
- Input tokens (prompt)
- Cached input tokens (10% cost)
- Output tokens (completion)
- Total tokens

**Aggregation**:
```csharp
public sealed class AiSceneResponse
{
    public int? InputTokens { get; set; }
    public int? CachedInputTokens { get; set; }
    public int? OutputTokens { get; set; }
    public int? TotalTokens { get; set; }
    
    public decimal? Cost { get; set; }  // Cost of this operation
    public decimal TotalCost { get; set; }  // Cumulative cost
}
```

### Cost Calculation

**Price Service**:
```csharp
services.AddOpenAi(settings => 
{
    settings.Price
        .Set(ChatModelName.Gpt4_o, input: 2.5m, output: 10m)
        .Set(ChatModelName.Gpt4_o_Mini, input: 0.15m, output: 0.6m);
});
```

**Automatic Calculation**:
```csharp
var cost = context.ChatClient.CalculateCost();
context.AddCost(cost); // Accumulate in context
```

**Price Per Million Tokens**:
- Input: $2.50 per 1M tokens
- Output: $10.00 per 1M tokens
- Cached: $0.25 per 1M tokens (10% of input)

### Cost in Responses

Every response includes:
- `Cost`: Cost of this specific operation (null if no OpenAI call)
- `TotalCost`: Cumulative cost so far

**Example**:
```json
{
  "status": "FunctionRequest",
  "function_name": "GetUser",
  "cost": 0.00015,
  "total_cost": 0.00042,
  "input_tokens": 1250,
  "cached_input_tokens": 500,
  "output_tokens": 200,
  "total_tokens": 1950
}
```

---

## Loop Prevention & Safety

### The Loop Problem

**Scenario**: LLM calls same tool repeatedly with same arguments.

**Causes**:
1. Tool returns ambiguous result
2. LLM doesn't understand response format
3. LLM "forgets" it already called the tool
4. Scene names mismatch (e.g., "Navigation-Site-Helper" vs "Navigation Site Helper")

### Safety Mechanisms

#### 1. Tool Execution Tracking

```csharp
public class SceneContext
{
    public Dictionary<string, HashSet<SceneRequestContext>> ExecutedScenes { get; }
    public HashSet<string> ExecutedTools { get; }
    
    public bool HasExecutedTool(string scene, string tool, string? args)
        => ExecutedTools.Contains($"{scene}.{tool}.{args}");
}
```

**Logic**:
- Before executing tool: check if already executed
- If yes: return `ToolSkipped` status
- Track: `anyToolExecuted` flag
- If ALL tools skipped: yield break (prevent infinite loop)

#### 2. Fuzzy Scene Name Matching

**Problem**: LLM generates "Navigation-Site-Helper", but scene is registered as "Navigation Site Helper".

**Solution**:
```csharp
private IScene? FindSceneByFuzzyMatch(string requestedName)
{
    var normalized = NormalizeSceneName(requestedName);
    // Remove hyphens, underscores, lowercase, trim
    
    foreach (var sceneName in _playHandler.GetScenes())
    {
        if (NormalizeSceneName(sceneName) == normalized)
            return _sceneFactory.Create(sceneName);
    }
    return null;
}
```

#### 3. Scene Execution Tracking

**For Planning Mode**:
```csharp
if (!context.ExecutedScenes.ContainsKey(scene.Name))
    context.ExecutedScenes[scene.Name] = [];
```

**Filter Re-execution**:
```csharp
var alreadyExecutedScenes = context.ExecutedScenes.Keys.ToList();
var filteredSteps = newPlan.Steps
    .Where(s => !alreadyExecutedScenes.Contains(s.SceneName, StringComparer.OrdinalIgnoreCase))
    .ToList();
```

#### 4. Maximum Recursion Depth

```csharp
private async IAsyncEnumerable<AiSceneResponse> ExecutePlanAsync(
    /* ... */,
    int recursionDepth = 0)
{
    const int MaxRecursionDepth = 5;
    
    if (recursionDepth >= MaxRecursionDepth)
    {
        yield return /* Max depth reached message */;
        await foreach (var response in GenerateFinalResponseAsync(/*...*/))
            yield return response;
        yield break;
    }
    // ...
}
```

#### 5. Completed Plan Steps

```csharp
public class PlanStep
{
    public bool IsCompleted { get; set; }
}

// In ExecutePlanAsync
step.IsCompleted = true;

// In continuation check
var allStepsCompleted = context.ExecutionPlan?.Steps.All(s => s.IsCompleted);
if (allStepsCompleted)
{
    // Skip LLM call, go directly to final response
}
```

#### 6. Preserve Direct Responses

**Problem**: Scene returns `SPECIFIC_COMMAND:Navigate(...)`, but final response generation loses it.

**Solution**:
```csharp
private async IAsyncEnumerable<AiSceneResponse> GenerateFinalResponseAsync(/*...*/)
{
    // Check if any scene already provided a specific command
    var directAnswer = context.Responses
        .Where(r => r.Status == AiResponseStatus.Running 
                 && r.Message?.Contains("SPECIFIC_COMMAND:") == true)
        .LastOrDefault();
    
    if (directAnswer != null)
    {
        // Use scene's direct answer, don't generate new one
        yield return /* directAnswer */;
        yield break;
    }
    
    // Otherwise, generate final response...
}
```

---

## Architectural Decisions

### 1. Scene-Based Architecture

**Decision**: Break complex interactions into specialized scenes.

**Rationale**:
- **Separation of Concerns**: Each scene has specific purpose
- **Context Switching**: LLM works better in specialized contexts
- **Tool Organization**: Tools grouped by functionality
- **Reusability**: Scenes can be reused across different workflows

**Alternative Considered**: Single chat with all tools available.
**Why Rejected**: Too many tools confuse LLM, increase costs, reduce accuracy.

### 2. Deterministic Planning

**Decision**: Use LLM to create execution plan BEFORE executing.

**Rationale**:
- **Cost Reduction**: Fewer redundant API calls
- **Predictability**: Know what will happen before it happens
- **Optimization**: Can analyze plan and optimize
- **Debugging**: Clear trace of what was planned vs executed

**Alternative Considered**: Director-based (react after each step).
**Why Both Exist**: Planning for complex workflows, Director for dynamic scenarios.

### 3. Centralized ChatClient

**Decision**: `SceneContext.ChatClient` shared across all operations.

**Rationale**:
- **Conversation Continuity**: All messages in one history
- **Tool Preservation**: Tools carry across scene switches
- **Simplification**: No need to pass chat client everywhere
- **Summarization**: Can clear messages and add summary to same client

**Alternative Considered**: New chat client per scene.
**Why Rejected**: Loses conversation history, more complex state management.

### 4. Streaming Responses

**Decision**: Return `IAsyncEnumerable<AiSceneResponse>`.

**Rationale**:
- **Real-time Feedback**: UI can show progress
- **Early Display**: Show partial results before completion
- **Cancellation**: Can cancel mid-execution
- **Large Responses**: Don't wait for entire completion

**Alternative Considered**: Return `List<AiSceneResponse>`.
**Why Rejected**: Blocks until fully complete, poor UX for long operations.

### 5. YieldAndTrack Pattern

**Decision**: Every response is both yielded AND added to context.

**Rationale**:
- **Observability**: Caller sees all responses
- **Context Building**: Responses tracked for final answer generation
- **Caching**: All responses saved to cache
- **Debugging**: Complete execution trace available

**Pattern**:
```csharp
private AiSceneResponse YieldAndTrack(SceneContext context, AiSceneResponse response)
{
    context.Responses.Add(response);
    return response;
}

// Usage
yield return YieldAndTrack(context, new AiSceneResponse { /* ... */ });
```

### 6. Mutable Context

**Decision**: `SceneContext` is mutable, not immutable.

**Rationale**:
- **Performance**: No copying large objects on each change
- **Natural Evolution**: Context naturally evolves during execution
- **Cost Tracking**: Accumulating costs is natural with mutation
- **Simplicity**: Less boilerplate than immutable patterns

**Alternative Considered**: Immutable context with copy-on-write.
**Why Rejected**: Performance overhead, more complex code, no real benefit for this use case.

### 7. Actor System for Dynamic Context

**Decision**: Actors provide dynamic system messages instead of static configuration.

**Rationale**:
- **Runtime Data**: Can read from database, APIs, session
- **User-Specific**: Different context for different users
- **Reusable Logic**: Actor classes can be reused across scenes
- **Testable**: Actors can be unit tested independently

**Alternative Considered**: Static system messages in configuration.
**Why Both Exist**: Static for simple cases, actors for dynamic scenarios.

### 8. Pluggable Components

**Decision**: Planner, Summarizer, Director, Cache are interfaces with default implementations.

**Rationale**:
- **Extensibility**: Users can provide custom implementations
- **Testing**: Can mock for unit tests
- **Evolution**: Can add new strategies without breaking changes
- **Configuration**: Different implementations for different scenarios

**Default Implementations**:
- `DeterministicPlanner`: AI-driven planning
- `DefaultSummarizer`: AI-powered summarization
- `MainDirector`: Multi-scene orchestration
- `CacheService`: Memory/distributed cache

### 9. MCP Integration

**Decision**: Full MCP support as both client and server.

**Rationale**:
- **Standardization**: MCP is emerging standard for AI tool integration
- **Interoperability**: Can work with any MCP-compatible tools
- **Ecosystem**: Access to growing MCP tool ecosystem
- **Exposure**: Other MCP-compatible apps can use your scenes

**Dual Role**:
- **Client**: Consume external MCP servers in scenes
- **Server**: Expose scenes as MCP tools

### 10. Fine-Grained Filtering for MCP

**Decision**: Rich filtering system for MCP elements (include/exclude, regex, predicates).

**Rationale**:
- **Security**: Don't expose dangerous tools (e.g., `delete_file`)
- **Cost Control**: Only enable expensive tools when needed
- **Context Clarity**: Fewer tools = better LLM performance
- **Flexibility**: Different scenes need different subsets

**Filtering Options**:
```csharp
.WithTools(tools => tools
    .Include("read_*")           // Regex
    .Exclude("delete_*")         // Regex
    .IncludeByName("search")     // Exact
    .ExcludeByName("admin")      // Exact
    .WithPredicate(tool =>       // Custom logic
        tool.Name.StartsWith("safe_")))
```

---

## Extension Points

### Custom Planner

```csharp
public class MyPlanner : IPlanner
{
    public Task<ExecutionPlan> CreatePlanAsync(
        SceneContext context, 
        SceneRequestSettings settings, 
        CancellationToken ct)
    {
        // Your planning logic
        // Could use rules engine, ML model, heuristics, etc.
    }
}

// Register
services.AddPlayFramework(builder => 
    builder.AddCustomPlanner<MyPlanner>());
```

### Custom Summarizer

```csharp
public class MySum summarizer : ISummarizer
{
    public bool ShouldSummarize(List<AiSceneResponse> responses)
        => responses.Count > 50; // Your threshold
    
    public async Task<string> SummarizeAsync(
        List<AiSceneResponse> responses, 
        CancellationToken ct)
    {
        // Your summarization logic
        // Could use different LLM, extractive summarization, etc.
    }
}
```

### Custom Director

```csharp
public class MyDirector : IDirector
{
    public Task<DirectorResponse> DirectAsync(
        SceneContext context, 
        SceneRequestSettings settings, 
        CancellationToken ct)
    {
        // Decide if execution should continue
        return new DirectorResponse
        {
            ExecuteAgain = /* decision */,
            CutScenes = /* scenes to avoid */
        };
    }
}
```

### Custom Cache

```csharp
public class DatabaseCache : ICustomCache
{
    public async Task<List<AiSceneResponse>?> GetAsync(string key, CancellationToken ct)
    {
        // Load from database
    }
    
    public async Task SetAsync(
        string key, 
        List<AiSceneResponse> value, 
        CacheBehavior behavior,
        CancellationToken ct)
    {
        // Save to database
    }
}

// Register
services.AddPlayFramework(builder => 
    builder.ConfigureCache(cache => 
        cache.UseCustomCache<DatabaseCache>()));
```

### Custom Response Parser

```csharp
public class MyParser : IResponseParser
{
    public object? ParseResponse(object response)
    {
        // Transform response before returning to LLM
        // Could format JSON, extract fields, sanitize, etc.
    }
}
```

### Custom Actor

```csharp
public class DatabaseContextActor : IActor
{
    private readonly IDbContext _db;
    
    public DatabaseContextActor(IDbContext db)
    {
        _db = db;
    }
    
    public async Task<ActorResponse> PlayAsync(
        SceneContext context, 
        CancellationToken ct)
    {
        var userId = context.GetProperty<string>("UserId");
        var user = await _db.Users.FindAsync(userId);
        
        return new ActorResponse
        {
            Message = $"User: {user.Name}\nRole: {user.Role}\nPermissions: {user.Permissions}"
        };
    }
}
```

---

## Summary

PlayFramework is a sophisticated AI orchestration system that:

✅ **Organizes complexity** through scene-based architecture  
✅ **Optimizes costs** through intelligent planning and caching  
✅ **Manages context** through summarization and centralized chat client  
✅ **Prevents issues** through multiple loop prevention mechanisms  
✅ **Tracks everything** through comprehensive cost and token tracking  
✅ **Integrates broadly** through MCP client and server support  
✅ **Extends easily** through pluggable components  
✅ **Provides observability** through streaming responses  

### Key Innovation: Planning with Actor Context

The deterministic planner receives **current_context** from main actors, enabling it to:
1. **Detect when data is already available** → skip scene execution
2. **Understand user state** → make better planning decisions  
3. **Generate direct responses** → reduce API calls dramatically
4. **Use actor patterns** → preserve scene-specific response formats (like `SPECIFIC_COMMAND:`)

This makes PlayFramework uniquely efficient for complex, stateful AI applications.

---

## Further Reading

- [Deterministic Planning](./DETERMINISTIC_PLANNING.md)
- [MCP Integration](./MCP_INTEGRATION.md)
- [Cost Tracking](./COST_TRACKING.md)
- [Loop Prevention](./TOOL_SKIPPED_STATUS.md)
- [Summarization](./SUMMARIZING_STATUS.md)
- [ChatClient Refactoring](./CHATCLIENT_REFACTORING.md)
