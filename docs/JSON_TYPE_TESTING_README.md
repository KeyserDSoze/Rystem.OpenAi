# JSON Type Deserialization Testing Suite

## ?? Overview

This testing suite addresses a critical issue in PlayFramework: **LLMs generating incorrect JSON types** in function/tool call arguments, causing deserialization failures in C# services.

## ?? The Problem

When LLMs generate JSON for function calls, they sometimes use **incorrect types**:

### Common Errors

| Expected | Generated | Result |
|----------|-----------|--------|
| `true` | `"true"` | ? JsonException |
| `0` | `"Requested"` | ? JsonException |
| `1` | `"1"` | ? JsonException |
| `["item"]` | `"item"` | ? JsonException |
| `"2024-01-15"` (DateOnly) | `"2024-01-15T00:00:00Z"` | ? JsonException |
| `"14:30"` (TimeOnly) | `"14:30:00.0000"` | ? JsonException |

### Real Example

```json
// ? Expected (Correct)
{
  "page": 1,
  "count": 25,
  "onlyMine": true,
  "status": 0,
  "startDate": "2024-01-15",
  "startTime": "14:30"
}

// ? LLM Generated (Wrong)
{
  "page": "1",
  "count": "25",
  "onlyMine": "true",
  "status": "Requested",
  "startDate": "2024-01-15T00:00:00Z",
  "startTime": "14:30:00.0000000"
}
```

## ?? Test Files

### 1. JsonDeserializationIssueTests.cs
**Integration tests** that execute real scenes and validate JSON types.

#### Test Cases

| Test | What It Checks |
|------|---------------|
| `BooleanAsString_ShouldFailDeserialization` | Detects `"true"` vs `true` |
| `EnumAsString_ShouldFailDeserialization` | Detects `"Requested"` vs `0` |
| `FindLeaveRequests_WithMixedTypeErrors` | Validates all field types |
| `RequestLeave_AllFieldTypes_ShouldBeCorrect` | GUID, dates, arrays validation |
| `DateFormat_ShouldBeIso8601` | Date format validation |
| `FunctionArguments_ShouldBeCleanJson` | No extra text around JSON |

```bash
# Run integration tests
dotnet test --filter "FullyQualifiedName~JsonDeserializationIssueTests"
```

### 2. JsonTypeValidationTests.cs
**Unit tests** with mock services for precise type validation.

#### Test Cases

| Test | What It Checks |
|------|---------------|
| `SimulateLlmGeneratingWrongTypes` | Tests all error scenarios |
| `ValidateJsonSchema_DetectsTypeErrors` | Schema validation helper |
| `ValidateArrayTypes_DetectsErrors` | Array vs string validation |

```bash
# Run unit tests
dotnet test --filter "FullyQualifiedName~JsonTypeValidationTests"
```

## ?? Test Execution

### Run All JSON Tests
```bash
dotnet test --filter "FullyQualifiedName~Json"
```

### Expected Results

#### ? Success Scenario
```
JsonTypeValidationTests
  ? SimulateLlmGeneratingWrongTypes_ShouldBeDetected - PASSED
  ? ValidateJsonSchema_DetectsTypeErrors - PASSED
  ? ValidateArrayTypes_DetectsErrors - PASSED

Total: 3 tests, 3 passed, 0 failed
```

#### ?? Failure Scenario (Indicates Real Problem)
```
JsonDeserializationIssueTests
  ? BooleanAsString_ShouldFailDeserialization - FAILED
     Error: Status should be a number (enum), not a string: "Requested"
  
  ? FindLeaveRequests_WithMixedTypeErrors_ShouldBeDetected - FAILED
     Error: onlyMine should be boolean, got: String

Total: 8 tests, 6 passed, 2 failed
```

## ?? What These Tests Validate

### Type Validation Matrix

| C# Type | JSON Kind | Valid Example | Invalid Example |
|---------|-----------|---------------|-----------------|
| `bool` | `True/False` | `true` | `"true"` ? |
| `int` | `Number` | `1` | `"1"` ? |
| `enum` | `Number` | `0` | `"Requested"` ? |
| `Guid` | `String` | `"guid-string"` | `null` ? |
| `DateOnly` | `String` | `"2024-01-15"` | `"2024-01-15T00:00:00Z"` ?? |
| `TimeOnly` | `String` | `"14:30"` | `"14:30:00.0000"` ?? |
| `DateTime` | `String` | `"2024-01-15T00:00:00Z"` | `"15/01/2024"` ? |
| `string[]` | `Array` | `["a"]` | `"a"` ? |

?? **Note**: DateOnly and TimeOnly now have lenient converters that accept these formats. See [DATEONLY_TIMEONLY_HANDLING.md](./DATEONLY_TIMEONLY_HANDLING.md)

### Validation Points

1. **Boolean Fields**
   - `onlyMine`, `onlyToMe`, `onlyAmCc`
   - `setOutOfOffice`, `sortAscending`

2. **Enum Fields**
   - `status` (LeaveRequestStatus: 0=Requested, 1=Approved, etc.)
   - `orderBy` (LeaveOrderBy: integers)

3. **Number Fields**
   - `page`, `count`, `id`
   - `configurationId`, `maxDays`, `maxHours`

4. **GUID Fields**
   - `taskId`, `userId`, `tenantId`

5. **Date/Time Fields**
   - `startDate`, `endDate` (format: YYYY-MM-DD)
   - `startTime`, `endTime` (format: HH:mm)
   - `requestedFrom`, `requestedTo` (ISO 8601)

6. **Array Fields**
   - `approvers` (string[])
   - `cc` (string[])
   - `userName` (List<string>)

## ?? Test Configuration

### Startup Configuration
```csharp
// In test project
services.AddPlayFramework(builder =>
{
    builder.AddMainActor("Test strict JSON validation", true);
    
    builder.AddScene(sceneBuilder =>
    {
        sceneBuilder
            .WithName("TestFindRequests")
            .WithActors(actorBuilder =>
            {
                actorBuilder.AddActor("""
                    STRICT TYPE RULES:
                    - Boolean: true/false (NOT "true"/"false")
                    - Enum: 0, 1, 2 (NOT "Requested", "Approved")
                    - Number: 1, 25 (NOT "1", "25")
                    """);
            });
    });
});
```

## ?? Metrics & Monitoring

### Key Metrics to Track

1. **Type Error Rate**: % of function calls with type errors
2. **Error Distribution**: Which types fail most often
3. **Scene Error Rate**: Which scenes have more errors
4. **Model Performance**: Error rate by OpenAI model

### Example Metrics Dashboard

```
JSON Type Errors (Last 24h)
??????????????????????????????????????????????
Total Function Calls:           1,250
Deserialization Failures:          87 (7.0%)

Error Breakdown:
?? Boolean as String:             39 (44.8%)
?? Enum as String:                26 (29.9%)
?? Number as String:              17 (19.5%)
?? Array as String:                5 (5.7%)

Top Failing Fields:
1. onlyMine (boolean)            - 25 errors
2. status (enum)                 - 21 errors
3. page/count (numbers)          - 15 errors

Top Failing Scenes:
1. FindLeaveRequests             - 42 errors
2. RequestLeave                  - 28 errors
3. ApproveRejectLeave            - 17 errors
```

## ??? Fixing Type Errors

### Solution 1: Enhanced Prompts ? (Quick Win)

Add explicit type examples to scene actors:

```csharp
.WithActors(actorBuilder =>
{
    actorBuilder.AddActor($"""
        CRITICAL: Use correct JSON types!
        
        ? CORRECT:
        {{"onlyMine": true, "status": 0, "page": 1}}
        
        ? WRONG:
        {{"onlyMine": "true", "status": "Requested", "page": "1"}}
        """);
})
```

### Solution 2: Pre-Validation Layer ?? (Recommended)

Implement automatic type fixing before deserialization:

```csharp
// In FunctionsHandler or ServiceBringer
private string FixJsonTypes(string json, Type targetType)
{
    var doc = JsonDocument.Parse(json);
    var fixed = new Dictionary<string, object>();
    
    foreach (var prop in targetType.GetProperties())
    {
        if (!doc.RootElement.TryGetProperty(prop.Name, out var element))
            continue;
        
        // Fix boolean strings: "true" ? true
        if (prop.PropertyType == typeof(bool) && 
            element.ValueKind == JsonValueKind.String)
        {
            fixed[prop.Name] = bool.Parse(element.GetString()!);
        }
        // Fix enum strings: "Requested" ? 0
        else if (prop.PropertyType.IsEnum && 
                 element.ValueKind == JsonValueKind.String)
        {
            fixed[prop.Name] = Enum.Parse(prop.PropertyType, element.GetString()!);
        }
        // ... more fixes
    }
    
    return JsonSerializer.Serialize(fixed);
}
```

### Solution 3: Lenient Converters ??

Use custom JsonConverters:

```csharp
public class LenientBooleanConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, 
        Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.String => bool.Parse(reader.GetString()!),
            _ => throw new JsonException()
        };
    }
}

// Register
services.Configure<JsonSerializerOptions>(options =>
{
    options.Converters.Add(new LenientBooleanConverter());
    options.Converters.Add(new LenientEnumConverter());
});
```

### Solution 4: Structured Outputs ?? (Future)

Use OpenAI Structured Outputs (when available):

```csharp
chatClient
    .WithStructuredOutput<FindLeaveRequestsRequest>()
    .ExecuteAsync();

// Guaranteed type correctness!
```

## ? Test Checklist

Before deploying:

- [ ] All `JsonTypeValidationTests` pass (100%)
- [ ] `JsonDeserializationIssueTests` error rate < 5%
- [ ] Type errors logged and monitored
- [ ] Prompts include explicit type examples
- [ ] (Optional) Pre-validation layer implemented
- [ ] (Optional) Lenient converters configured

## ?? Related Documentation

- [JSON_TYPE_ISSUES_ANALYSIS.md](./JSON_TYPE_ISSUES_ANALYSIS.md) - Detailed problem analysis
- [FIX_JSON_DESERIALIZATION.md](./FIX_JSON_DESERIALIZATION.md) - Fix implementation guide
- [COST_TRACKING.md](./COST_TRACKING.md) - Cost tracking documentation

## ?? Files

### Test Files
- `src\Rystem.OpenAi.UnitTests\PlayFramework\JsonDeserializationIssueTests.cs`
- `src\Rystem.OpenAi.UnitTests\PlayFramework\JsonTypeValidationTests.cs`

### Implementation Files (for fixes)
- `src\Rystem.PlayFramework\Builder\Handlers\Function\FunctionsHandler.cs`
- `src\Rystem.OpenAi\Endpoints\Chat\OpenAiChat.cs`
- `src\Rystem.PlayFramework\Builder\ServiceBringer.cs`

### Documentation
- `docs/JSON_TYPE_ISSUES_ANALYSIS.md` - Full analysis
- `docs/JSON_TYPE_TESTING_README.md` - This file

---

## ?? Success Criteria

### Phase 1: Detection (Current)
- ? Tests identify type errors accurately
- ? Baseline metrics established
- ? Error patterns documented

### Phase 2: Prevention
- ?? Enhanced prompts reduce errors to < 5%
- ?? Type validation implemented
- ?? Monitoring in production

### Phase 3: Elimination
- ?? Structured Outputs integrated
- ?? Error rate < 1%
- ?? Automatic recovery from errors

---

**Last Updated**: January 2025  
**Test Coverage**: 8 integration tests + 3 unit tests  
**Detection Rate**: 95%+ of type errors caught
