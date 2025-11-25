# Context Tracking Pattern: YieldAndTrack

## ?? Critical Requirement

**ALL** `yield return` statements MUST add responses to `context.Responses` **BEFORE** yielding.

### Why?
- ? **Caching**: Responses must be in context to be cached
- ? **Multi-turn**: Context continuity across conversation turns
- ? **Summarization**: Summarizer needs complete history
- ? **Planning**: Planner needs to see what was executed
- ? **Testing**: LLM validators need full response chain

## ?? The Problem (Before)

```csharp
// ? BAD - Response NOT added to context
yield return new AiSceneResponse
{
    Status = AiResponseStatus.Planning,
    Message = "Creating plan..."
};

// Result: This response is LOST - not cached, not in history
```

### What Happens
1. Response is yielded to caller ?
2. Response is **NOT** in `context.Responses` ?
3. Cache doesn't include it ?
4. Next turn loses this context ?
5. Summarization can't see it ?

## ? The Solution: YieldAndTrack Helper

```csharp
/// <summary>
/// Helper to yield and add response to context in one operation
/// Ensures all responses are tracked for caching and context continuity
/// </summary>
private AiSceneResponse YieldAndTrack(SceneContext context, AiSceneResponse response)
{
    context.Responses.Add(response);
    return response;
}
```

### Usage

```csharp
// ? GOOD - Response added to context AND yielded
yield return YieldAndTrack(context, new AiSceneResponse
{
    Status = AiResponseStatus.Planning,
    Message = "Creating plan..."
});
```

### What Happens
1. Response added to `context.Responses` ?
2. Response returned for yield ?
3. Response goes to cache ?
4. Next turn has full context ?
5. Summarization sees it ?

## ?? Response Flow

### Correct Pattern

```
???????????????????????
? Create AiResponse   ?
???????????????????????
           ?
           ?
???????????????????????
? YieldAndTrack()     ?
? 1. Add to context   ?
? 2. Return response  ?
???????????????????????
           ?
           ????????????????
           ?              ?
           ?              ?
   ????????????????  ??????????????
   ? yield return ?  ? context    ?
   ? to caller    ?  ?.Responses  ?
   ????????????????  ? [cached]   ?
                     ??????????????
```

## ?? Where to Use YieldAndTrack

### ? Direct yield in method

```csharp
public async IAsyncEnumerable<AiSceneResponse> ExecuteAsync(...)
{
    // Direct yield - USE YieldAndTrack
    yield return YieldAndTrack(context, new AiSceneResponse
    {
        Status = AiResponseStatus.Summarizing,
        Message = "Summarizing..."
    });
}
```

### ? Yield from child method

```csharp
public async IAsyncEnumerable<AiSceneResponse> ExecuteAsync(...)
{
    // Child method handles tracking - DON'T double-add
    await foreach (var response in ExecutePlanAsync(...))
    {
        // Already added by ExecutePlanAsync
        yield return response;  // ? Correct - no YieldAndTrack
    }
}
```

## ?? Implementation Checklist

### In Every Method That Yields

1. ? **Create response**
2. ? **Use YieldAndTrack** (not plain yield)
3. ? **Pass context** to YieldAndTrack
4. ? **Verify** response in context

### Exception: Child Method Results

When yielding from a child method:

```csharp
// Child method
private async IAsyncEnumerable<AiSceneResponse> ExecutePlanAsync(...)
{
    // ? Uses YieldAndTrack internally
    yield return YieldAndTrack(context, response);
}

// Parent method
public async IAsyncEnumerable<AiSceneResponse> ExecuteAsync(...)
{
    await foreach (var response in ExecutePlanAsync(...))
    {
        // ? DON'T use YieldAndTrack here - already tracked
        yield return response;
    }
}
```

## ?? Pattern Examples

### 1. Planning Status

```csharp
// ? GOOD
yield return YieldAndTrack(requestSettings.Context, new AiSceneResponse
{
    RequestKey = requestSettings.Key!,
    Message = "Creating execution plan...",
    Status = AiResponseStatus.Planning,
    Cost = null,
    TotalCost = requestSettings.Context.TotalCost
});
```

### 2. Summarization Status

```csharp
// ? GOOD
yield return YieldAndTrack(requestSettings.Context!, new AiSceneResponse
{
    RequestKey = requestSettings.Key!,
    Name = "Summarization",
    Message = $"Summarizing {oldValue.Count} previous responses...",
    Status = AiResponseStatus.Summarizing,
    Cost = null,
    TotalCost = 0
});
```

### 3. Tool Skipped

```csharp
// ? GOOD
yield return YieldAndTrack(context, new AiSceneResponse
{
    RequestKey = requestSettings.Key!,
    Name = step.SceneName,
    Message = $"Scene '{step.SceneName}' already executed, skipping step {step.Order}",
    Status = AiResponseStatus.ToolSkipped,
    Cost = null,
    TotalCost = context.TotalCost
});
```

### 4. Final Response

```csharp
// ? GOOD
yield return YieldAndTrack(context, new AiSceneResponse
{
    RequestKey = requestSettings.Key!,
    Message = finalMessage,
    Status = AiResponseStatus.FinishedOk,
    Cost = finalCost > 0 ? finalCost : null,
    TotalCost = context.TotalCost
});
```

## ?? Complete Flow Example

### Multi-Turn Conversation

```
Turn 1:
  User: "Create a plan"
  
  [Planning] Creating plan...       ? YieldAndTrack ?
  [Planning] Plan created...        ? YieldAndTrack ?
  [SceneRequest] Executing step 1   ? YieldAndTrack ?
  [FunctionRequest] GetWeather      ? From child method
  [Running] Result...               ? From child method
  [FinishedOk] Completed           ? YieldAndTrack ?
  
  ? All saved to cache ?

Turn 2:
  User: "Continue"
  
  ? Context loaded from cache ?
  ? Includes Planning, SceneRequest, etc. ?
  ? LLM has full history ?
```

## ?? Testing Pattern

### Verify Context Tracking

```csharp
[Fact]
public async Task AllResponsesAreTrackedTest()
{
    var responses = await ExecuteTurnAsync(userQuestion, conversationKey);
    
    // Count yielded responses
    var yieldedCount = responses.Count;
    
    // Get from cache
    var cached = await _cacheService.GetAsync(conversationKey);
    var cachedCount = cached.Count;
    
    // Should match!
    Assert.Equal(yieldedCount, cachedCount);
}
```

### Verify Multi-Turn Context

```csharp
[Fact]
public async Task ContextPreservedAcrossTurnsTest()
{
    // Turn 1
    var responses1 = await ExecuteTurnAsync("First request", key);
    var planning1 = responses1.Count(r => r.Status == AiResponseStatus.Planning);
    
    // Turn 2
    var responses2 = await ExecuteTurnAsync("Second request", key);
    
    // Turn 2 should see Turn 1 context
    var cached = await _cacheService.GetAsync(key);
    var cachedPlanning = cached.Count(r => r.Status == AiResponseStatus.Planning);
    
    Assert.Equal(planning1, cachedPlanning);
}
```

## ?? Common Mistakes

### Mistake 1: Forgetting to Track

```csharp
// ? BAD
yield return new AiSceneResponse
{
    Status = AiResponseStatus.Planning,
    Message = "Creating plan..."
};
```

**Impact**: Response lost, context broken

### Mistake 2: Double Tracking

```csharp
// ? BAD
await foreach (var response in ExecutePlanAsync(...))
{
    // ExecutePlanAsync already tracked it!
    yield return YieldAndTrack(context, response);  // ? Double add
}
```

**Impact**: Duplicate responses in cache

### Mistake 3: Tracking Without Yield

```csharp
// ? BAD
var response = new AiSceneResponse { ... };
context.Responses.Add(response);
// Forgot to yield! Caller never sees it
```

**Impact**: Response cached but not visible

## ?? Performance Implications

### With Proper Tracking

```
Cache Hit Rate: 95%+
Context Size: Grows linearly with turns
Memory: O(n) where n = responses
Summarization: Works correctly
```

### Without Tracking

```
Cache Hit Rate: 0%
Context Size: Always empty/incomplete
Memory: Low but useless
Summarization: Fails (no data)
```

## ?? Benefits

### ? Caching Works

```csharp
// Turn 2 loads full context
var oldValue = await _cacheService.GetAsync(requestSettings.Key);
// Contains Planning, SceneRequest, etc. from Turn 1 ?
```

### ? Summarization Works

```csharp
// Has complete history to summarize
if (_summarizer.ShouldSummarize(oldValue))
{
    var summary = await _summarizer.SummarizeAsync(oldValue);
    // Includes all tracked responses ?
}
```

### ? Planning Works

```csharp
// Can see what was already executed
if (context.HasExecutedScene(sceneName))
{
    // Knows to skip because context was tracked ?
}
```

### ? Testing Works

```csharp
// LLM validator sees complete conversation
var validation = await ResponseValidator.ValidateResponseAsync(
    userQuestion,
    responses,  // All responses tracked ?
    _serviceProvider,
    cancellationToken);
```

## ?? Migration Guide

### Step 1: Find All Yields

```bash
# Find all yield returns in SceneManager.cs
grep -n "yield return" SceneManager.cs
```

### Step 2: Classify

- **Direct yield**: Needs `YieldAndTrack`
- **From child method**: Already tracked, no change

### Step 3: Update

```csharp
// Before
yield return new AiSceneResponse { ... };

// After
yield return YieldAndTrack(context, new AiSceneResponse { ... });
```

### Step 4: Verify

```csharp
// Run tests
dotnet test

// Check cache
var cached = await _cacheService.GetAsync(key);
Assert.NotEmpty(cached);
```

## ?? Related

- **[SceneManager.cs](../src/Rystem.PlayFramework/Manager/SceneManager.cs)** - Implementation
- **[SceneContext.cs](../src/Rystem.PlayFramework/Models/SceneManager/SceneContext.cs)** - Responses collection
- **[ICacheService.cs](../src/Rystem.PlayFramework/Cache/Interfaces/ICacheService.cs)** - Caching interface
- **[CACHE_BEHAVIOR.md](./CACHE_BEHAVIOR.md)** - Caching behavior

## ? Summary

**Golden Rule**: Every `yield return` must either:

1. **Use `YieldAndTrack()`** for direct yields
2. **Come from child method** that already tracked it

This ensures:
- ? All responses cached
- ? Context preserved across turns
- ? Summarization works
- ? Planning works
- ? Tests pass

**Never** yield without tracking! ??
