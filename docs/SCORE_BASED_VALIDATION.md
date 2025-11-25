# Score-Based LLM Validation System

## ?? Overview

The validation system has been upgraded from **boolean (pass/fail)** to **score-based (0-100)** with **configurable thresholds** per test.

## ?? Scoring System

### Score Range: 0-100

| Score | Category | Description |
|-------|----------|-------------|
| **100** | Perfect | All information present, accurate, and complete |
| **90-99** | Excellent | Minor details missing but essentially complete |
| **80-89** | Very Good | Most information present, minor gaps |
| **70-79** | Good | Core question answered, some details missing |
| **60-69** | Adequate | Basic answer present, significant gaps |
| **50-59** | Partial | Some relevant information, major gaps |
| **30-49** | Poor | Very incomplete or partially incorrect |
| **10-29** | Very Poor | Mostly irrelevant or incorrect |
| **0-9** | Failed | Completely wrong or no answer |

### Default Threshold: 70

By default, tests pass with a score of **70 or higher**.

## ?? Usage

### Basic Usage (Default 70% threshold)

```csharp
[Fact]
public async Task MyTest()
{
    var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

    var validation = await ResponseValidator.ValidateResponseAsync(
        userQuestion,
        responses,
        _serviceProvider,
        TestContext.Current.CancellationToken);

    Assert.True(validation.IsValid,
        $"Score: {validation.Score}/100 (min: {validation.MinScore}). " +
        $"Reasoning: {validation.Reasoning}");
}
```

### Custom Threshold

```csharp
[Fact]
public async Task StrictTest()
{
    var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

    // Require 90% for this test
    var validation = await ResponseValidator.ValidateResponseAsync(
        userQuestion,
        responses,
        _serviceProvider,
        minScore: 90,  // ? Custom threshold
        TestContext.Current.CancellationToken);

    Assert.True(validation.IsValid,
        $"Score: {validation.Score}/100 (min: {validation.MinScore})");
}
```

## ?? Test Thresholds by Complexity

### Simple Tests (80-90%)

For straightforward single-scene operations:

```csharp
[Fact]
public async Task SimpleWeatherRequestTest()
{
    var validation = await ResponseValidator.ValidateResponseAsync(
        "Che tempo fa oggi a Milano?",
        responses,
        _serviceProvider,
        minScore: 80,  // High threshold for simple queries
        cancellationToken);
}
```

**Why high threshold?**
- Single scene
- Direct question
- Clear expected answer
- Should be nearly perfect

### Multi-Scene Tests (70-75%)

For tests involving multiple scenes:

```csharp
[Fact]
public async Task MultiSceneWeatherAndIdentityTest()
{
    var validation = await ResponseValidator.ValidateResponseAsync(
        "Il mio username è keysersoze e vorrei sapere il meteo a Milano",
        responses,
        _serviceProvider,
        minScore: 70,  // Standard threshold
        cancellationToken);
}
```

**Why standard threshold?**
- Multiple scenes coordinated
- More room for minor issues
- Still expect good results

### Complex Workflow Tests (60-65%)

For complex multi-step workflows:

```csharp
[Fact]
public async Task ComplexMultiSceneWorkflowTest()
{
    var validation = await ResponseValidator.ValidateResponseAsync(
        "Voglio sapere il meteo a Milano e poi richiedere 3 giorni di ferie dal 15 al 17 giugno",
        responses,
        _serviceProvider,
        minScore: 60,  // Lower threshold for complexity
        cancellationToken);
}
```

**Why lower threshold?**
- Multiple scenes in sequence
- Inter-scene dependencies
- Complex data passing
- More potential for gaps

### Error Handling Tests (50%)

For tests validating error handling:

```csharp
[Fact]
public async Task InvalidRequestHandlingTest()
{
    var validation = await ResponseValidator.ValidateResponseAsync(
        "Calcola la radice quadrata di 144",
        responses,
        _serviceProvider,
        minScore: 50,  // Low threshold - just needs proper error response
        cancellationToken);
}
```

**Why low threshold?**
- Not about correct answer
- Just proper error acknowledgment
- "I can't do that" is valid

## ?? ValidationResult Properties

```csharp
public class ValidationResult
{
    /// Main score (0-100)
    public int Score { get; set; }
    
    /// Threshold for this test
    public int MinScore { get; set; }
    
    /// True if Score >= MinScore
    public bool IsValid { get; }
    
    /// Detailed explanation
    public string Reasoning { get; set; }
    
    /// What's missing (if score < 100)
    public string? MissingInformation { get; set; }
    
    /// Completeness percentage (0-100)
    public int? CompletenessPercentage { get; set; }
    
    /// Accuracy percentage (0-100)
    public int? AccuracyPercentage { get; set; }
}
```

## ?? Best Practices

### 1. Match Threshold to Complexity

```csharp
// Simple ?
minScore: 80  // Single scene, straightforward

// Medium ?
minScore: 70  // Multi-scene, standard complexity

// Complex ?
minScore: 60  // Many scenes, complex workflow

// Error Handling ?
minScore: 50  // Just need proper error response
```

### 2. Log Scores for Visibility

```csharp
Console.WriteLine($"? Score: {validation.Score}/100 (threshold: {validation.MinScore})");
Console.WriteLine($"   Completeness: {validation.CompletenessPercentage}%");
Console.WriteLine($"   Accuracy: {validation.AccuracyPercentage}%");
```

### 3. Use Detailed Reasoning in Assertions

```csharp
Assert.True(validation.IsValid,
    $"LLM validation failed with score {validation.Score}/100 (min: {validation.MinScore}). " +
    $"Reasoning: {validation.Reasoning}. " +
    $"Missing: {validation.MissingInformation}");
```

## ?? Evaluation Criteria

The LLM evaluates responses based on:

### 1. Completeness (0-100%)
- Does the response address **ALL** parts of the question?
- Are there any missing pieces?

### 2. Accuracy (0-100%)
- Is the information **correct**?
- Are the details accurate?

### 3. Clarity
- Is the answer clear and coherent?
- Is it easy to understand?

### 4. Context Usage
- Does it use available tools appropriately?
- Does it leverage context correctly?

## ?? Example Scoring

### Example 1: Perfect Answer (100)

**Question**: "Che tempo fa oggi a Milano?"  
**Response**: "A Milano oggi c'è sole con una temperatura di 22°C"  
**Score**: 100/100  
**Reasoning**: "Complete answer with all requested information (location, date, conditions, temperature)"

### Example 2: Good Answer (75)

**Question**: "Voglio sapere il meteo a Milano"  
**Response**: "Il meteo a Milano è soleggiato"  
**Score**: 75/100  
**Reasoning**: "Answers the core question but missing details like temperature, date, forecast"  
**Missing**: "Temperature, specific date/time, detailed forecast"

### Example 3: Partial Answer (55)

**Question**: "Richiedere ferie dal 15 al 17 giugno"  
**Response**: "Ho inviato la richiesta"  
**Score**: 55/100  
**Reasoning**: "Acknowledges the action but doesn't confirm dates, provide confirmation number, or next steps"  
**Missing**: "Confirmation of specific dates, request ID, approval workflow info"

### Example 4: Poor Answer (25)

**Question**: "Che tempo fa a Milano?"  
**Response**: "Roma è una bella città"  
**Score**: 25/100  
**Reasoning**: "Completely irrelevant answer - mentions wrong city and doesn't address weather"

## ?? Migration from Boolean System

### Before (Boolean)

```csharp
var validation = await ResponseValidator.ValidateResponseAsync(...);

Assert.True(validation.IsValid,
    $"Validation failed: {validation.Reasoning}");
```

### After (Score-Based)

```csharp
var validation = await ResponseValidator.ValidateResponseAsync(
    userQuestion,
    responses,
    _serviceProvider,
    minScore: 70,  // ? New parameter
    cancellationToken);

Assert.True(validation.IsValid,  // Still works!
    $"Score: {validation.Score}/100 (min: {validation.MinScore}). " +
    $"Reasoning: {validation.Reasoning}");
```

### Backward Compatibility

The `IsValid` property is maintained for backward compatibility:
- `IsValid = true` when `Score >= MinScore`
- `IsValid = false` when `Score < MinScore`

Legacy properties still available:
- `CompletenessScore` (double 0-1)
- `RelevanceScore` (double 0-1)

## ?? Test Summary Table

| Test | Question Type | Threshold | Rationale |
|------|--------------|-----------|-----------|
| `SimpleWeatherRequestTest` | Single scene | **80** | Should be very accurate |
| `MultiSceneWeatherAndIdentityTest` | Multi-scene | **70** | Standard complexity |
| `VacationRequestTest` | Workflow | **65** | Confirmation flows |
| `MultiTurnConversationTest` | Context | **75** | Requires cache |
| `ComplexMultiSceneWorkflowTest` | Complex | **60** | Very complex |
| `InvalidRequestHandlingTest` | Error | **50** | Just error handling |
| `ParallelRequestsTest` | Parallel | **70** | Standard per request |

## ?? Tuning Guidelines

### When to Increase Threshold

- Test is consistently passing with 95+
- Question is very simple
- Expected answer is clear and unambiguous

### When to Decrease Threshold

- Test is flaky (sometimes 65, sometimes 75)
- Question involves multiple complex steps
- Expected answer has multiple valid interpretations
- Workflow includes confirmation/feedback loops

### When to Keep at 70

- Standard multi-scene operations
- Typical business workflows
- Good balance of strictness and flexibility

## ?? Benefits

### 1. More Nuanced Feedback

Instead of:
```
? Test failed (boolean)
```

You get:
```
Score: 65/100 (min: 70)
Completeness: 70%
Accuracy: 85%
Missing: Confirmation number and approval workflow
```

### 2. Better Test Tuning

You can adjust thresholds based on:
- Test complexity
- Question ambiguity
- Workflow steps
- Historical scores

### 3. Trend Analysis

Track scores over time:
```
Commit A: 75/100
Commit B: 82/100 ? Improvement!
Commit C: 68/100 ? Regression!
```

### 4. Fair Evaluation

Different tests can have different expectations:
- Simple queries: 80-90% (strict)
- Complex workflows: 60-70% (lenient)
- Error handling: 50% (very lenient)

## ?? Summary

? **Score-based**: 0-100 instead of boolean  
? **Configurable**: Each test sets its own threshold  
? **Default**: 70 for good balance  
? **Detailed**: Completeness, Accuracy, Missing info  
? **Flexible**: Adjust per test complexity  
? **Backward Compatible**: `IsValid` still works  

**Default Rule**: Test passes if `Score >= MinScore` (default 70)

---

**Related Documentation**:
- [ResponseValidator.cs](../src/Rystem.OpenAi.UnitTests/Helpers/ResponseValidator.cs)
- [MultiScenePlanningTest.cs](../src/Rystem.OpenAi.UnitTests/PlayFramework/MultiScenePlanningTest.cs)
- [LLM_VALIDATED_TESTS.md](./LLM_VALIDATED_TESTS.md)
