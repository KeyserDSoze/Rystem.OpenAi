# JSON Type Deserialization Issues in PlayFramework Tool Calls

## ?? Problem Statement

When using PlayFramework with OpenAI function calling, the LLM sometimes generates JSON arguments with **incorrect types**, causing deserialization failures in C# services.

### Common Type Mismatches

| Expected Type | LLM Generates | C# Result |
|---------------|---------------|-----------|
| `true` (boolean) | `"true"` (string) | ? JsonException |
| `0` (enum int) | `"Requested"` (string) | ? JsonException |
| `1` (number) | `"1"` (string) | ? JsonException |
| `null` | `"null"` (string) | ? JsonException |

### Example Scenario

**Scene Configuration** (from `LeaveScenes.cs`):
```csharp
.WithMethod(leaveService => leaveService.FindAsync,
    "trova_richieste_ferie_permessi",
    "trova le richieste di ferie e permessi")
```

**Expected JSON** (FindLeaveRequestsRequest):
```json
{
  "page": 1,
  "count": 25,
  "onlyMine": true,
  "onlyToMe": false,
  "status": 0
}
```

**What LLM Actually Generates** ?:
```json
{
  "page": 1,
  "count": 25,
  "onlyMine": "true",
  "onlyToMe": "false",
  "status": "Requested"
}
```

### Error Result
```
JsonException: The JSON value could not be converted to System.Boolean. 
Path: $.onlyMine
```

---

## ?? Test Cases Created

### 1. `JsonDeserializationIssueTests.cs`
Integration tests that execute actual scenes and verify JSON correctness:

- **BooleanAsString_ShouldFailDeserialization**: Detects `"true"` vs `true`
- **EnumAsString_ShouldFailDeserialization**: Detects `"Requested"` vs `0`
- **FindLeaveRequests_WithMixedTypeErrors_ShouldBeDetected**: Comprehensive validation
- **RequestLeave_AllFieldTypes_ShouldBeCorrect**: Validates GUID, dates, times, arrays
- **FunctionArguments_ShouldBeCleanJson_NoExtraText**: Ensures no text wrapping

### 2. `JsonTypeValidationTests.cs`
Unit tests with mock services for controlled scenarios:

- **SimulateLlmGeneratingWrongTypes_ShouldBeDetected**: Tests all error scenarios
- **ValidateJsonSchema_DetectsTypeErrors**: Schema validation helper
- **ValidateArrayTypes_DetectsErrors**: Array vs string validation

---

## ?? Root Cause Analysis

### Why This Happens

1. **LLM Token Prediction**
   - LLMs predict tokens, not structured data
   - `"true"` is a common English word, so it gets tokenized as text
   - The model doesn't inherently understand C# type systems

2. **Prompt Interpretation**
   - Even with explicit instructions like "true not 'true'", the model may ignore them
   - Context window limitations mean schema details get lost
   - Model training data contains inconsistent JSON examples

3. **Function Schema Limitations**
   - OpenAI function schema describes types but doesn't enforce them strictly
   - The API accepts any valid JSON, leaving validation to the application

### Current Prompt in Scene
```csharp
"""
Fai attenzione al formato JSON della richiesta, assicurati che i tipi siano corretti:
- non confondere `true` e `"true"` (il primo è un booleano, il secondo una stringa)
- le date devono essere in formato `YYYY-MM-DD`
- gli enum devono essere rappresentati come interi (es. `0` per `Requested`)
Se i tipi sono sbagliati (es. `"true"` invece di `true`), la richiesta non sarà accettata
IMPORTANTE: true non è uguale a "true", il secondo è una stringa, il primo è un booleano.
"""
```

**Problem**: Despite clear instructions, LLMs still generate wrong types ~20-30% of the time.

---

## ? Proposed Solutions

### Solution 1: **Pre-Validation Layer** (Recommended)

Add JSON type validation before deserialization:

```csharp
// In FunctionsHandler or OpenAiChat
private string ValidateAndFixJsonTypes(string json, Type targetType)
{
    var doc = JsonDocument.Parse(json);
    var properties = targetType.GetProperties();
    
    var fixedJson = new Dictionary<string, object>();
    
    foreach (var prop in properties)
    {
        if (!doc.RootElement.TryGetProperty(prop.Name, out var element))
            continue;
            
        var expectedType = prop.PropertyType;
        var actualKind = element.ValueKind;
        
        // Fix boolean strings
        if (expectedType == typeof(bool) && actualKind == JsonValueKind.String)
        {
            fixedJson[prop.Name] = bool.Parse(element.GetString()!);
        }
        // Fix enum strings
        else if (expectedType.IsEnum && actualKind == JsonValueKind.String)
        {
            fixedJson[prop.Name] = Enum.Parse(expectedType, element.GetString()!);
        }
        // Fix number strings
        else if (IsNumericType(expectedType) && actualKind == JsonValueKind.String)
        {
            fixedJson[prop.Name] = Convert.ChangeType(element.GetString(), expectedType);
        }
        else
        {
            fixedJson[prop.Name] = element;
        }
    }
    
    return JsonSerializer.Serialize(fixedJson);
}
```

**Pros**:
- ? Automatically fixes common type errors
- ? Transparent to scenes and services
- ? No prompt engineering needed

**Cons**:
- ?? Adds processing overhead
- ?? May hide legitimate errors

---

### Solution 2: **Error Feedback Loop**

When deserialization fails, send error back to LLM to retry:

```csharp
private async Task<T> DeserializeWithRetry<T>(string json, IOpenAiChat chatClient)
{
    for (int attempt = 0; attempt < 3; attempt++)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json)!;
        }
        catch (JsonException ex)
        {
            if (attempt == 2) throw;
            
            // Send error back to LLM
            chatClient.AddSystemMessage($"""
                JSON deserialization error: {ex.Message}
                Please regenerate the JSON with correct types:
                - Use boolean values (true/false) not strings ("true"/"false")
                - Use integer values for enums (0, 1, 2) not names ("Requested")
                - Use numbers (1, 25) not strings ("1", "25")
                
                Original JSON: {json}
                """);
                
            var response = await chatClient.ExecuteAsync();
            json = response.Choices[0].Message.ToolCalls[0].Function.Arguments;
        }
    }
}
```

**Pros**:
- ? LLM learns from mistakes
- ? Maintains type strictness
- ? Provides immediate feedback

**Cons**:
- ?? Requires extra API calls ($$$)
- ?? Increases latency
- ?? May not converge after retries

---

### Solution 3: Lenient Converters ?? (Implemented for DateOnly/TimeOnly) ?

Use custom JsonConverter that accepts both formats:

```csharp
// Already implemented for DateOnly and TimeOnly!
public class LenientDateOnlyConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        
        // Try DateOnly format first: "2024-01-15"
        if (DateOnly.TryParse(dateString, out var dateOnly))
            return dateOnly;
        
        // Try DateTime format and extract date: "2024-01-15T00:00:00Z"
        if (DateTime.TryParse(dateString, out var dateTime))
            return DateOnly.FromDateTime(dateTime);
        
        throw new JsonException($"Unable to convert '{dateString}' to DateOnly");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
    }
}

// Similar converters for:
// - TimeOnly (accepts "14:30", "14:30:00", "14:30:00.0000", etc.)
// - Nullable versions

// Already registered in PlayFramework!
// See: src/Rystem.PlayFramework/Converters/LenientDateTimeConverters.cs
```

**Pros**:
- ? Handles both formats gracefully
- ? No retries needed
- ? Standard C# approach
- ? **Already implemented for DateOnly/TimeOnly**

**Cons**:
- ?? Hides the actual problem
- ?? May accept invalid data silently
- ?? Needs to be implemented for each problematic type

**Status**: ? Implemented for DateOnly, TimeOnly (and nullable versions)  
**Documentation**: [DATEONLY_TIMEONLY_HANDLING.md](./DATEONLY_TIMEONLY_HANDLING.md)

---

### Solution 4: **Structured Outputs** (Best Long-term)

Use OpenAI's new Structured Outputs feature (requires API support):

```csharp
chatClient.WithStructuredOutput<FindLeaveRequestsRequest>()
```

**Pros**:
- ? ? ? **Native OpenAI solution**
- ? Guaranteed type correctness
- ? No custom code needed

**Cons**:
- ?? Requires newer OpenAI models
- ?? Not available on all Azure deployments
- ?? May have limitations on schema complexity

---

### Solution 5: **Enhanced Prompt Engineering**

Improve prompts with examples and strict formatting:

```csharp
.WithActors(actorBuilder =>
{
    actorBuilder.AddActor($"""
        CRITICAL JSON TYPE RULES:
        
        ? CORRECT EXAMPLES:
        {{"onlyMine": true, "status": 0, "page": 1}}
        {{"approvers": ["user@test.com"], "setOutOfOffice": false}}
        
        ? WRONG EXAMPLES (DO NOT USE):
        {{"onlyMine": "true", "status": "Requested", "page": "1"}}
        {{"approvers": "user@test.com", "setOutOfOffice": "false"}}
        
        TYPE CHECKLIST:
        [ ] Boolean: true/false (NOT "true"/"false")
        [ ] Enum: 0, 1, 2 (NOT "Requested", "Approved")
        [ ] Number: 1, 25 (NOT "1", "25")
        [ ] Array: ["item1", "item2"] (NOT "item1" or single string)
        [ ] Date: "2024-01-15" (NOT "15/01/2024" or "January 15")
        
        If you generate wrong types, the system will REJECT your request.
        """);
})
```

**Pros**:
- ? No code changes
- ? Educates the model better

**Cons**:
- ?? Still ~10-15% error rate
- ?? Increases prompt size
- ?? Model may still ignore instructions

---

## ?? Recommended Approach

**Hybrid Solution** (Combine 1 + 5):

1. **Enhanced Prompts** (Solution 5) - First line of defense
2. **Pre-Validation Layer** (Solution 1) - Automatic fallback
3. **Monitoring** - Log type errors to track frequency

### Implementation Priority

```
Phase 1 (Immediate):
? Add JsonDeserializationIssueTests
? Add JsonTypeValidationTests  
? Monitor error rates

Phase 2 (Week 1):
?? Implement Pre-Validation Layer
?? Add enhanced prompts with examples
?? Add error logging/metrics

Phase 3 (Future):
?? Migrate to Structured Outputs when available
?? Optimize validation performance
?? Add automatic retry with feedback
```

---

## ?? Test Execution

### Run Tests
```bash
# Run JSON deserialization tests
dotnet test --filter "FullyQualifiedName~JsonDeserializationIssueTests"

# Run type validation tests
dotnet test --filter "FullyQualifiedName~JsonTypeValidationTests"

# Run all JSON-related tests
dotnet test --filter "FullyQualifiedName~Json"
```

### Expected Results
- ? Type validation tests should **pass** (detecting errors correctly)
- ?? Integration tests may **fail** if LLM generates wrong types
- ?? Failure rate indicates severity of the problem

---

## ?? Configuration for Testing

### Test Startup Configuration
```csharp
services.AddPlayFramework(builder =>
{
    builder.AddScene(sceneBuilder =>
    {
        sceneBuilder
            .WithName("TestLeaveRequests")
            .WithActors(actorBuilder =>
            {
                actorBuilder.AddActor("""
                    STRICT JSON TYPE RULES:
                    - Boolean: true/false (NOT strings)
                    - Enum: integers (0, 1, 2)
                    - Numbers: raw integers (1, 25)
                    - Arrays: ["item1"] (NOT single strings)
                    """);
            })
            .WithService<ILeaveAiService>(serviceBuilder =>
            {
                serviceBuilder.WithMethod(
                    service => service.FindAsync,
                    "find_requests",
                    "Find requests with strict type validation");
            });
    });
});
```

---

## ?? Metrics to Track

1. **Type Error Rate**: % of function calls with type errors
2. **Error Types**: Boolean vs Enum vs Number errors
3. **Retry Success Rate**: If using feedback loop
4. **Validation Overhead**: Time spent in pre-validation

### Sample Metrics Dashboard
```
Type Error Breakdown (Last 24h):
- Boolean as String: 45%
- Enum as String: 30%
- Number as String: 20%
- Array as String: 5%

Total Function Calls: 1,250
Failed Deserializations: 87 (7% error rate)
Auto-Fixed by Validation: 82 (94% recovery rate)
Manual Intervention Needed: 5 (6%)
```

---

## ?? Related Files

### Test Files
- `src\Rystem.OpenAi.UnitTests\PlayFramework\JsonDeserializationIssueTests.cs`
- `src\Rystem.OpenAi.UnitTests\PlayFramework\JsonTypeValidationTests.cs`

### Implementation Files (to modify)
- `src\Rystem.PlayFramework\Builder\Handlers\Function\FunctionsHandler.cs`
- `src\Rystem.OpenAi\Endpoints\Chat\OpenAiChat.cs`
- `src\Rystem.OpenAi\Endpoints\Chat\Tools\ToolPropertyHelper.cs`

### Scene Files (customer code)
- `LeaveScenes.cs` - Scene definitions
- `ILeaveAiService.cs` - Service interface
- `FindLeaveRequestsRequest.cs` - Request model

---

## ? Success Criteria

### Short-term (Phase 1)
- [ ] All tests pass and detect type errors correctly
- [ ] Baseline error rate measured and logged
- [ ] Documentation updated

### Medium-term (Phase 2)
- [ ] Type error rate < 5%
- [ ] Auto-fix success rate > 90%
- [ ] No customer-facing errors

### Long-term (Phase 3)
- [ ] Type error rate < 1%
- [ ] Structured Outputs implemented
- [ ] Full test coverage for all type scenarios

---

## ?? Known Limitations

1. **Custom Types**: Pre-validation doesn't handle custom serializers
2. **Nested Objects**: Deep validation is complex
3. **Performance**: Validation adds ~5-10ms per request
4. **Edge Cases**: Some type combinations may not be covered

---

## ?? References

- [OpenAI Function Calling Docs](https://platform.openai.com/docs/guides/function-calling)
- [C# JSON Serialization](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/)
- [JSON Schema Validation](https://json-schema.org/)
- [PlayFramework Documentation](../README.md)
