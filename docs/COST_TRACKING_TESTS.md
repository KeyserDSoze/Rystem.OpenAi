# Cost Tracking Unit Tests

## Overview

This document describes the comprehensive unit tests for the cost tracking feature in the PlayFramework.

## Test Files

### 1. `CostTrackingTest.cs`
**Purpose**: Comprehensive tests for cost tracking functionality across different scenarios.

#### Tests Included

##### `SingleSceneCostTrackingTest`
- **What it tests**: Basic cost tracking for simple single-scene requests
- **Verifies**:
  - At least one response has individual cost
  - All responses have totalCost populated
  - Total cost is monotonically increasing
  - Final total equals sum of individual costs

##### `MultiSceneCostTrackingTest`
- **What it tests**: Cost tracking when multiple scenes are executed
- **Verifies**:
  - Planning costs are tracked (if planning is enabled)
  - Scene execution costs are captured
  - Final total cost is calculated correctly

##### `MultiTurnCostAccumulationTest`
- **What it tests**: Cost accumulation across multiple conversation turns
- **Verifies**:
  - Turn 1 has cost
  - Turn 2 total > Turn 1 total
  - Turn 3 total > Turn 2 total
  - Each turn adds incremental cost

##### `IndependentConversationCostsTest`
- **What it tests**: Different conversations have independent cost tracking
- **Verifies**:
  - Each conversation tracks costs separately
  - Similar operations have similar costs (within 20% variance)

##### `PlanningCostTrackingTest`
- **What it tests**: Cost tracking when deterministic planning is enabled
- **Verifies**:
  - Planning operations have costs
  - Planning costs are included in total
  - Final message may include cost information

##### `NoDuplicateCostsForSkippedOperationsTest`
- **What it tests**: Skipped/cached operations don't add duplicate costs
- **Verifies**:
  - Skipped operations have `null` cost
  - TotalCost still tracked for skipped operations

##### `CostBreakdownByResponseTypeTest`
- **What it tests**: Cost distribution by response type (Planning, SceneRequest, etc.)
- **Verifies**:
  - Costs grouped by status type
  - All costs are positive
  - Average costs are reasonable

##### `CostTrackingWithErrorsTest`
- **What it tests**: Cost tracking works even with errors
- **Verifies**:
  - Errors don't break cost tracking
  - TotalCost always present

##### `CostPrecisionTest`
- **What it tests**: Cost values have reasonable precision
- **Verifies**:
  - Costs are non-negative
  - Single request cost < 1 unit
  - Total cost < 10 units for simple requests

##### `ParallelRequestsCostTrackingTest`
- **What it tests**: Parallel requests track costs independently
- **Verifies**:
  - All parallel requests have costs
  - Costs are similar for same operations (within 30% variance)

##### `FinalResponseIncludesCostTest`
- **What it tests**: Final response includes total cost
- **Verifies**:
  - Final response has TotalCost
  - Cost value is positive

##### `CachedConversationCostTrackingTest`
- **What it tests**: Cached conversations still track costs
- **Verifies**:
  - Cached turns may have lower incremental cost
  - Total cost never decreases

---

### 2. `PricingConfigurationTest.cs`
**Purpose**: Tests for pricing configuration and calculation accuracy.

#### Tests Included

##### `CostCalculationAccuracyTest`
- **What it tests**: Mathematical accuracy of cost calculations
- **Verifies**:
  - Sum of individual costs equals final total
  - Precision within 0.000001 (floating point tolerance)

##### `ConsistentCostForIdenticalRequestsTest`
- **What it tests**: Identical requests produce consistent costs
- **Verifies**:
  - Same question produces similar costs (< 20% variance)
  - Cost calculation is deterministic

##### `CostIncreasesWithConversationLengthTest`
- **What it tests**: Costs accumulate properly over conversation
- **Verifies**:
  - Costs are monotonically increasing
  - Each turn adds to total

##### `CostValuesWithinExpectedRangesTest`
- **What it tests**: Cost values are reasonable
- **Verifies**:
  - Individual request cost < 0.1
  - Total conversation cost < 1.0
  - All costs non-negative

##### `CostPrecisionSixDecimalPlacesTest`
- **What it tests**: Cost precision to 6 decimal places
- **Verifies**:
  - Can format to "F6" without loss
  - Rounding differences < 0.0000001

##### `NullCostHandlingTest`
- **What it tests**: Proper handling of null costs
- **Verifies**:
  - Some responses may have null Cost
  - TotalCost always present
  - TotalCost never negative

##### `CostBreakdownMessageFormatTest`
- **What it tests**: Cost messages don't include currency symbols
- **Verifies**:
  - No $ symbols
  - No € symbols
  - No £ symbols
  - Cost is numeric only

---

## Running the Tests

### Run all cost tracking tests:
```bash
dotnet test --filter "FullyQualifiedName~CostTrackingTest"
```

### Run pricing configuration tests:
```bash
dotnet test --filter "FullyQualifiedName~PricingConfigurationTest"
```

### Run specific test:
```bash
dotnet test --filter "FullyQualifiedName~CostTrackingTest.SingleSceneCostTrackingTest"
```

---

## Test Assertions Summary

### Cost Properties Verified
1. **Individual Cost** (`Cost`):
   - Can be `null` (for non-OpenAI operations)
   - When present, must be > 0
   - Should be < 0.1 for single requests

2. **Total Cost** (`TotalCost`):
   - Should always be present (at least from first OpenAI call)
   - Must be >= 0
   - Should be monotonically increasing
   - Must equal sum of all individual costs

### Cost Calculation Rules
1. **Accumulation**: Each OpenAI call adds to total
2. **Precision**: 6 decimal places (F6 format)
3. **Currency-neutral**: No currency symbols in messages
4. **Independence**: Each conversation tracks costs separately

### Expected Cost Ranges
- **Single request**: 0.00001 - 0.1
- **Simple conversation**: 0.0001 - 1.0
- **Multi-scene conversation**: 0.001 - 5.0

---

## Test Data

### Example Questions Used
- **Simple**: "Che tempo fa a Milano?" (weather query)
- **Multi-scene**: "Il mio username è keysersoze e vorrei sapere il meteo" (identity + weather)
- **Math**: "What is 2+2?" (simple calculation)
- **Multi-turn**: Multiple related questions in same conversation

### Conversation Key Management
- Each test uses `Guid.NewGuid().ToString()` for unique conversations
- Multi-turn tests reuse same key to test accumulation
- Parallel tests use different keys to test independence

---

## Common Failure Scenarios

### 1. Cost Sum Mismatch
**Symptom**: Sum of individual costs ? final total cost
**Cause**: Missing cost calculation somewhere in the flow
**Fix**: Ensure `CalculateCost()` called after every `ExecuteAsync()`

### 2. Negative Costs
**Symptom**: Cost or TotalCost < 0
**Cause**: Subtraction instead of addition somewhere
**Fix**: Check all `AddCost()` calls

### 3. Missing TotalCost
**Symptom**: TotalCost is `null` in responses
**Cause**: TotalCost not initialized or not propagated
**Fix**: Ensure all responses populate TotalCost from context

### 4. Non-Monotonic Total
**Symptom**: TotalCost decreases between responses
**Cause**: Using wrong cost value or resetting total
**Fix**: Verify accumulation logic in `SceneContext.AddCost()`

---

## Integration with CI/CD

These tests are part of the standard test suite and run on:
- Every pull request
- Every commit to main branch
- Nightly builds

### Test Performance
- **Average duration**: 2-5 seconds per test
- **Total suite duration**: ~1 minute (all cost tests)
- **Parallel execution**: Supported (tests use independent conversations)

---

## Future Enhancements

### Planned Test Additions
1. **Cost budget limits**: Test maximum cost thresholds
2. **Cost alerts**: Test notifications when cost exceeds threshold
3. **Cost optimization**: Test caching effectiveness
4. **Model-specific pricing**: Test different model costs
5. **Batch operations**: Test cost tracking for batch requests

### Planned Features to Test
1. **Cost prediction**: Estimate cost before execution
2. **Cost breakdown**: Detailed breakdown by operation type
3. **Cost comparison**: Compare costs across different approaches
4. **Cost analytics**: Aggregate costs over time

---

## Related Documentation

- [Cost Tracking Feature](../COST_TRACKING.md) - Main feature documentation
- [OpenAI Pricing](https://openai.com/pricing) - Current OpenAI rates
- [Rystem.OpenAI Cost](../../README.md#cost) - Pricing configuration guide

---

## Maintenance Notes

### Updating Cost Thresholds
If OpenAI changes pricing:
1. Update expected cost ranges in tests
2. Adjust assertion thresholds
3. Update pricing configuration in startup

### Adding New Tests
When adding new cost-related features:
1. Add test to appropriate test class
2. Follow existing naming conventions
3. Include XML documentation
4. Update this README with test description

---

## Support

For issues with cost tracking tests:
1. Check that pricing is configured in startup
2. Verify OpenAI client is properly initialized
3. Ensure test environment has valid API keys
4. Check that test data matches expected patterns
