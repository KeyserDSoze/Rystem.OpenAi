# Documentation Index

Welcome to the Rystem documentation! This index helps you navigate all available documentation.

## Quick Navigation

### Getting Started
- **[MCP Quick Start](MCP_QUICK_START.md)** - Start here for MCP integration
- **[PlayFramework README](../src/Rystem.PlayFramework/README.md)** - PlayFramework overview
- **[Main README](../README.md)** - Project overview

### MCP Documentation
1. **[MCP Quick Start](MCP_QUICK_START.md)** - Entry point, quick links, common tasks
2. **[MCP Integration](MCP_INTEGRATION.md)** - Comprehensive guide with examples
3. **[MCP Filtering Reference](MCP_FILTERING_REFERENCE.md)** - Filter syntax quick reference
4. **[MCP Changelog](MCP_CHANGELOG.md)** - What's new, migration guide
5. **[README Update Summary](README_UPDATE_SUMMARY.md)** - Documentation update details

### Feature Documentation

#### PlayFramework Features
- **[Deterministic Planning](DETERMINISTIC_PLANNING.md)** - Planning system
- **[Multi-Scene Planning](MULTI_SCENE_PLANNING_SUMMARY.md)** - Multi-scene orchestration
- **[Cost Tracking](COST_TRACKING.md)** - Token and cost calculation
- **[Caching in PlayFramework](CACHE_*.md)** - Caching strategies

#### OpenAI Integration
- **[Cost Tracking Tests](COST_TRACKING_TESTS.md)** - Testing cost calculations
- **[OpenAI Models Update 2025](OPENAI_MODELS_UPDATE_2025.md)** - Latest model information
- **[Tool Skipped Status](TOOL_SKIPPED_STATUS.md)** - Tool execution status
- **[Summarizing Status](SUMMARIZING_STATUS.md)** - Summarization handling

#### Data Handling
- **[Token Summary in Responses](TOKEN_SUMMARY_IN_RESPONSES.md)** - Token tracking
- **[DateOnly/TimeOnly Handling](DATEONLY_TIMEONLY_HANDLING.md)** - Date/time types
- **[DateOnly/TimeOnly Summary](DATEONLY_TIMEONLY_SUMMARY.md)** - Date/time reference
- **[JSON Type Testing](JSON_TYPE_TESTING_README.md)** - JSON serialization
- **[JSON Type Issues Analysis](JSON_TYPE_ISSUES_ANALYSIS.md)** - JSON troubleshooting
- **[Real vs Mock JSON Tests](REAL_VS_MOCK_JSON_TESTS.md)** - JSON test approaches

### Code Fixes & Patterns
- **[Fix JSON Deserialization](FIX_JSON_DESERIALIZATION.md)** - JSON handling solutions
- **[Fix Final Response](FIX_FINAL_RESPONSE.md)** - Response formatting
- **[LLM Validated Tests](LLM_VALIDATED_TESTS.md)** - Testing with LLM validation
- **[Yield and Track Pattern](YIELD_AND_TRACK_PATTERN.md)** - Response tracking pattern
- **[Score Based Validation](SCORE_BASED_VALIDATION.md)** - Validation patterns

### Testing
- **[Multi-Scene Planning Test](../src/Rystem.OpenAi.UnitTests/PlayFramework/MultiScenePlanningTest.cs)** - Test examples
- **[Cost Tracking Test](../src/Rystem.OpenAi.UnitTests/PlayFramework/CostTrackingTest.cs)** - Cost calculation tests

## Documentation by Topic

### MCP Integration
- Quick start: [MCP_QUICK_START.md](MCP_QUICK_START.md)
- Full guide: [MCP_INTEGRATION.md](MCP_INTEGRATION.md)
- Filter reference: [MCP_FILTERING_REFERENCE.md](MCP_FILTERING_REFERENCE.md)
- What's new: [MCP_CHANGELOG.md](MCP_CHANGELOG.md)

### PlayFramework Features
- Overview: [../src/Rystem.PlayFramework/README.md](../src/Rystem.PlayFramework/README.md)
- Planning: [DETERMINISTIC_PLANNING.md](DETERMINISTIC_PLANNING.md)
- Multi-scene: [MULTI_SCENE_PLANNING_SUMMARY.md](MULTI_SCENE_PLANNING_SUMMARY.md)
- Costs: [COST_TRACKING.md](COST_TRACKING.md)

### OpenAI Integration
- Main SDK: [../README.md](../README.md)
- Models: [OPENAI_MODELS_UPDATE_2025.md](OPENAI_MODELS_UPDATE_2025.md)
- Pricing: [COST_TRACKING.md](COST_TRACKING.md)

### Common Issues & Solutions
- JSON handling: 
  - [JSON_TYPE_TESTING_README.md](JSON_TYPE_TESTING_README.md)
  - [JSON_TYPE_ISSUES_ANALYSIS.md](JSON_TYPE_ISSUES_ANALYSIS.md)
  - [FIX_JSON_DESERIALIZATION.md](FIX_JSON_DESERIALIZATION.md)
- Date/time types:
  - [DATEONLY_TIMEONLY_HANDLING.md](DATEONLY_TIMEONLY_HANDLING.md)
  - [DATEONLY_TIMEONLY_SUMMARY.md](DATEONLY_TIMEONLY_SUMMARY.md)

## File Organization

```
docs/
├── README.md (this file)
├── README_UPDATE_SUMMARY.md (documentation update details)
│
├── MCP Documentation
├── MCP_QUICK_START.md
├── MCP_INTEGRATION.md
├── MCP_FILTERING_REFERENCE.md
├── MCP_CHANGELOG.md
│
├── PlayFramework Documentation
├── DETERMINISTIC_PLANNING.md
├── MULTI_SCENE_PLANNING_SUMMARY.md
├── COST_TRACKING.md
├── COST_TRACKING_TESTS.md
├── TOKEN_SUMMARY_IN_RESPONSES.md
├── YIELD_AND_TRACK_PATTERN.md
├── TOOL_SKIPPED_STATUS.md
├── SUMMARIZING_STATUS.md
│
├── Data Handling
├── DATEONLY_TIMEONLY_HANDLING.md
├── DATEONLY_TIMEONLY_SUMMARY.md
├── JSON_TYPE_TESTING_README.md
├── JSON_TYPE_ISSUES_ANALYSIS.md
├── REAL_VS_MOCK_JSON_TESTS.md
├── FIX_JSON_DESERIALIZATION.md
├── FIX_FINAL_RESPONSE.md
├── LLM_VALIDATED_TESTS.md
│
├── Validation & Patterns
├── SCORE_BASED_VALIDATION.md
├── OPENAI_MODELS_UPDATE_2025.md
```

## Document Types

### Getting Started
- [MCP_QUICK_START.md](MCP_QUICK_START.md)
- [../src/Rystem.PlayFramework/README.md](../src/Rystem.PlayFramework/README.md)
- [../README.md](../README.md)

### Comprehensive Guides
- [MCP_INTEGRATION.md](MCP_INTEGRATION.md)
- [DETERMINISTIC_PLANNING.md](DETERMINISTIC_PLANNING.md)
- [MULTI_SCENE_PLANNING_SUMMARY.md](MULTI_SCENE_PLANNING_SUMMARY.md)

### Quick References
- [MCP_FILTERING_REFERENCE.md](MCP_FILTERING_REFERENCE.md)
- [COST_TRACKING.md](COST_TRACKING.md)
- [TOKEN_SUMMARY_IN_RESPONSES.md](TOKEN_SUMMARY_IN_RESPONSES.md)

### Troubleshooting
- [JSON_TYPE_ISSUES_ANALYSIS.md](JSON_TYPE_ISSUES_ANALYSIS.md)
- [FIX_JSON_DESERIALIZATION.md](FIX_JSON_DESERIALIZATION.md)
- [FIX_FINAL_RESPONSE.md](FIX_FINAL_RESPONSE.md)

### What's New
- [MCP_CHANGELOG.md](MCP_CHANGELOG.md)
- [OPENAI_MODELS_UPDATE_2025.md](OPENAI_MODELS_UPDATE_2025.md)
- [README_UPDATE_SUMMARY.md](README_UPDATE_SUMMARY.md)

## Search Guide

### Looking for...?

**"How do I set up MCP?"**
→ Start with [MCP_QUICK_START.md](MCP_QUICK_START.md)

**"How do I filter MCP tools?"**
→ See [MCP_FILTERING_REFERENCE.md](MCP_FILTERING_REFERENCE.md)

**"How does PlayFramework work?"**
→ Read [../src/Rystem.PlayFramework/README.md](../src/Rystem.PlayFramework/README.md)

**"How do I calculate costs?"**
→ Check [COST_TRACKING.md](COST_TRACKING.md)

**"How do I handle dates and times?"**
→ See [DATEONLY_TIMEONLY_HANDLING.md](DATEONLY_TIMEONLY_HANDLING.md)

**"What's new in this update?"**
→ Check [MCP_CHANGELOG.md](MCP_CHANGELOG.md) and [README_UPDATE_SUMMARY.md](README_UPDATE_SUMMARY.md)

**"How do I fix JSON issues?"**
→ Read [FIX_JSON_DESERIALIZATION.md](FIX_JSON_DESERIALIZATION.md)

**"What are the latest OpenAI models?"**
→ See [OPENAI_MODELS_UPDATE_2025.md](OPENAI_MODELS_UPDATE_2025.md)

## Learning Paths

### Path 1: MCP Integration (Beginner)
1. [MCP_QUICK_START.md](MCP_QUICK_START.md) - Getting started
2. [../README.md](../README.md) - PlayFramework overview
3. [../src/Rystem.PlayFramework/README.md](../src/Rystem.PlayFramework/README.md) - MCP section
4. Try a simple example

### Path 2: MCP Mastery (Intermediate)
1. [MCP_INTEGRATION.md](MCP_INTEGRATION.md) - Comprehensive guide
2. [MCP_FILTERING_REFERENCE.md](MCP_FILTERING_REFERENCE.md) - Filtering techniques
3. [MCP_CHANGELOG.md](MCP_CHANGELOG.md) - Architecture and performance
4. Build a multi-server scenario

### Path 3: PlayFramework Deep Dive (Advanced)
1. [DETERMINISTIC_PLANNING.md](DETERMINISTIC_PLANNING.md) - Planning system
2. [MULTI_SCENE_PLANNING_SUMMARY.md](MULTI_SCENE_PLANNING_SUMMARY.md) - Multi-scene scenarios
3. [MCP_INTEGRATION.md](MCP_INTEGRATION.md) - MCP integration
4. [COST_TRACKING.md](COST_TRACKING.md) - Cost optimization

### Path 4: OpenAI SDK Essentials (Beginner)
1. [../README.md](../README.md) - Overview
2. [OPENAI_MODELS_UPDATE_2025.md](OPENAI_MODELS_UPDATE_2025.md) - Available models
3. [COST_TRACKING.md](COST_TRACKING.md) - Pricing
4. Start coding!

### Path 5: Troubleshooting (All Levels)
- JSON issues: [JSON_TYPE_ISSUES_ANALYSIS.md](JSON_TYPE_ISSUES_ANALYSIS.md)
- Date/time: [DATEONLY_TIMEONLY_SUMMARY.md](DATEONLY_TIMEONLY_SUMMARY.md)
- Response issues: [FIX_FINAL_RESPONSE.md](FIX_FINAL_RESPONSE.md)
- MCP issues: [MCP_INTEGRATION.md](MCP_INTEGRATION.md#troubleshooting)

## Quick Links

- 🚀 [Quick Start (MCP)](MCP_QUICK_START.md)
- 📚 [Full PlayFramework Docs](../src/Rystem.PlayFramework/README.md)
- 💰 [Cost Tracking Guide](COST_TRACKING.md)
- 🔧 [MCP Integration Guide](MCP_INTEGRATION.md)
- 🎯 [MCP Filter Reference](MCP_FILTERING_REFERENCE.md)
- ❓ [Troubleshooting](MCP_INTEGRATION.md#troubleshooting)
- 📦 [What's New](MCP_CHANGELOG.md)

## Contributing

Found an issue in the docs? Have suggestions?

- Create an issue: [GitHub Issues](https://github.com/KeyserDSoze/Rystem.OpenAi/issues)
- Submit a PR: [GitHub PRs](https://github.com/KeyserDSoze/Rystem.OpenAi/pulls)
- Join the community: [Discord](https://discord.gg/wUh2fppr)

---

**Last Updated**: 2025 - MCP Documentation Complete
**Status**: ✅ Ready for use
