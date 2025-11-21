# OpenAI Models and Pricing Update

## Date: January 2025
**Source**: OpenAI Pricing Page (Standard Tier)

---

## ?? Summary of Changes

### New Model Series Added

#### 1. **GPT-5 Series** (Latest Generation)
- `gpt-5.1`, `gpt-5` - Full models
- `gpt-5-mini` - Medium model
- `gpt-5-nano` - Smallest model
- `gpt-5-pro` - Professional/premium model
- `gpt-5.1-chat-latest`, `gpt-5-chat-latest` - Latest chat versions
- `gpt-5.1-codex`, `gpt-5-codex` - Code-specialized versions
- `gpt-5.1-codex-mini` - Code-specialized mini
- `gpt-5-search-api` - Search-optimized

**Pricing (Standard - per 1M tokens)**:
- GPT-5.1/5: $1.25 input, $0.125 cached, $10.00 output
- GPT-5-mini: $0.25 input, $0.025 cached, $2.00 output
- GPT-5-nano: $0.05 input, $0.005 cached, $0.40 output
- GPT-5-pro: $15.00 input, $120.00 output

#### 2. **GPT-4.5 Series**
- `gpt-4.5-preview`
- `gpt-4.5-preview-2025-02-27`

#### 3. **O3/O4 Series** (Reasoning Models)
- `o3`, `o3-2025-04-16` - Standard reasoning
- `o3-pro` - Premium reasoning
- `o3-deep-research` - Deep research mode
- `o3-mini`, `o3-mini-2025-01-31` - Lightweight reasoning
- `o4-mini`, `o4-mini-2025-04-16` - Next-gen mini reasoning
- `o4-mini-deep-research` - Mini deep research

**Pricing (Standard - per 1M tokens)**:
- O3: $2.00 input, $0.50 cached, $8.00 output
- O3-pro: $20.00 input, $80.00 output
- O3-deep-research: $10.00 input, $2.50 cached, $40.00 output
- O3-mini: $1.10 input, $0.55 cached, $4.40 output
- O4-mini: $1.10 input, $0.275 cached, $4.40 output

#### 4. **Realtime/Audio Models**
- `gpt-realtime`, `gpt-realtime-mini` - Real-time processing
- `gpt-audio`, `gpt-audio-mini` - Audio processing
- Updated audio previews with latest versions

**Pricing (Standard - per 1M tokens)**:
- Text tokens: $4.00 input, $0.40 cached, $16.00 output (gpt-realtime)
- Audio tokens: $32.00 input, $0.40 cached, $64.00 output (gpt-realtime)

#### 5. **Transcription/Speech Models**
- `gpt-4o-transcribe` - Transcription
- `gpt-4o-transcribe-diarize` - Transcription with speaker identification
- `gpt-4o-mini-transcribe` - Mini transcription
- `gpt-4o-mini-tts` - Text-to-speech

**Pricing (Standard)**:
- Text tokens: $2.50 input, $10.00 output (gpt-4o-transcribe)
- Audio tokens: $6.00 input (gpt-4o-transcribe)

#### 6. **Image Models**
- `gpt-image-1` - Full image model
- `gpt-image-1-mini` - Mini image model

**Pricing (Standard - per 1M tokens)**:
- GPT-Image-1: $5.00 input, $1.25 cached (image tokens)
- GPT-Image-1-Mini: $2.00 input, $0.20 cached

#### 7. **Search Models**
- `gpt-4o-search-preview`
- `gpt-4o-search-preview-2025-03-11`
- `gpt-4o-mini-search-preview`
- `gpt-4o-mini-search-preview-2025-03-11`

#### 8. **Computer Use**
- `computer-use-preview`
- `computer-use-preview-2025-03-11`

**Pricing**: $3.00 input, $12.00 output

#### 9. **Codex**
- `codex-mini-latest`

**Pricing**: $1.50 input, $0.375 cached, $6.00 output

#### 10. **Legacy Models**
- `chatgpt-4o-latest`
- `gpt-3.5-turbo` series (multiple versions)
- `davinci-002`, `babbage-002`

---

## ?? Updated Pricing for Existing Models

### GPT-4.1 Series (NEW)
- **gpt-4.1**: $2.00 input, $0.50 cached, $8.00 output
- **gpt-4.1-mini**: $0.40 input, $0.10 cached, $1.60 output
- **gpt-4.1-nano**: $0.10 input, $0.025 cached, $0.40 output

### GPT-4o Series (UPDATED)
- **gpt-4o**: $2.50 input, $1.25 cached, $10.00 output
- **gpt-4o-mini**: $0.15 input, $0.075 cached, $0.60 output
- **gpt-4o-2024-05-13**: $5.00 input, $15.00 output

### O1 Series (UPDATED)
- **o1**: $15.00 input, $7.50 cached, $60.00 output
- **o1-pro**: $150.00 input, $600.00 output
- **o1-mini**: $1.10 input, $0.55 cached, $4.40 output

### GPT-3.5 Turbo (Legacy)
- **gpt-3.5-turbo**: $0.50 input, $1.50 output
- **gpt-3.5-turbo-0125**: $0.50 input, $1.50 output
- **gpt-3.5-turbo-16k-0613**: $3.00 input, $4.00 output

---

## ?? Model Naming Conventions

### Naming Pattern
- **Base models**: `gpt-5`, `gpt-4o`, `o3`
- **Dated versions**: `gpt-4o-2024-08-06`, `o3-2025-04-16`
- **Size variants**: `-mini`, `-nano`, `-pro`
- **Specialized**: `-codex`, `-search-api`, `-audio`, `-realtime`
- **Preview/Latest**: `-preview`, `-latest`, `-chat-latest`

### Examples
```csharp
// Latest generation
ChatModelName.Gpt_5_1
ChatModelName.Gpt_5_mini
ChatModelName.Gpt_5_pro

// Reasoning models
ChatModelName.O3
ChatModelName.O3_mini
ChatModelName.O4_mini

// Audio/Realtime
ChatModelName.Gpt_realtime
ChatModelName.Gpt_audio
ChatModelName.Gpt_4o_audio_preview

// Specialized
ChatModelName.Gpt_5_codex
ChatModelName.Gpt_4o_transcribe
ChatModelName.Computer_use_preview
```

---

## ?? Cost Calculation Examples

### GPT-5 (Standard)
```csharp
// 1M input tokens: $1.25
// 1M output tokens: $10.00
// 1M cached input: $0.125

// Example: 100K input + 50K output
Cost = (100,000 / 1,000,000 * 1.25) + (50,000 / 1,000,000 * 10.00)
     = $0.125 + $0.50
     = $0.625
```

### O3 (Reasoning)
```csharp
// 1M input tokens: $2.00
// 1M output tokens: $8.00
// 1M cached input: $0.50

// Example: 50K input + 200K output (reasoning)
Cost = (50,000 / 1,000,000 * 2.00) + (200,000 / 1,000,000 * 8.00)
     = $0.10 + $1.60
     = $1.70
```

### GPT-Realtime (Audio)
```csharp
// Text: $4.00 input, $16.00 output
// Audio: $32.00 input, $64.00 output

// Example: 10K text input + 5K audio output
Cost = (10,000 / 1,000,000 * 4.00) + (5,000 / 1,000,000 * 64.00)
     = $0.04 + $0.32
     = $0.36
```

---

## ?? Usage in Code

### Using New Models
```csharp
services.AddOpenAi(settings =>
{
    settings.ApiKey = apiKey;
    
    // Use GPT-5
    settings.DefaultRequestConfiguration.Chat = chatClient =>
    {
        chatClient.WithModel(ChatModelName.Gpt_5_mini);
    };
});
```

### Manual Pricing Configuration
```csharp
services.AddOpenAi(settings =>
{
    settings.ApiKey = apiKey;
    
    // Custom pricing for GPT-5-pro
    settings.PriceBuilder
        .AddModel(ChatModelName.Gpt_5_pro,
            new OpenAiCost { 
                Kind = KindOfCost.Input, 
                UnitOfMeasure = UnitOfMeasure.Tokens, 
                Units = 0.000015m 
            },
            new OpenAiCost { 
                Kind = KindOfCost.Output, 
                UnitOfMeasure = UnitOfMeasure.Tokens, 
                Units = 0.00012m 
            });
});
```

### Cost Tracking
```csharp
var openAiApi = _openAiFactory.Create();
var results = await openAiApi.Chat
    .AddMessage(new ChatMessage { Role = ChatRole.User, Content = "Hello!" })
    .WithModel(ChatModelName.Gpt_5_mini)
    .ExecuteAsync();

// Calculate cost
var cost = openAiApi.Chat.CalculateCost();
Console.WriteLine($"Request cost: {cost:F6}");
```

---

## ?? Important Notes

### 1. **Cached Input Pricing**
Many models now support prompt caching with reduced costs:
- GPT-5: 90% reduction (10% of input cost)
- GPT-4o: 50% reduction
- O3: 75% reduction

### 2. **Audio Token Costs**
Audio models have separate pricing for audio vs text tokens:
- Text tokens: Lower cost
- Audio tokens: Higher cost (typically 10-20x text cost)

### 3. **Reasoning Token Costs**
O-series models use reasoning tokens (not visible via API):
- Reasoning tokens are billed as output tokens
- Can significantly increase costs for complex reasoning

### 4. **Legacy Model Deprecation**
Some older models may be deprecated:
- Consider migrating from GPT-3.5 to GPT-4o-mini
- GPT-4 ? GPT-4o for better performance/cost ratio
- Check OpenAI deprecation schedule

### 5. **Batch vs Standard Pricing**
All prices listed are for **Standard tier**:
- Batch tier: 50% discount
- Flex tier: Variable discount
- Priority tier: 70-100% markup

---

## ?? Recommendations

### Cost Optimization
1. **Use Cached Input** where possible (50-90% savings)
2. **Choose Right Size**: 
   - Simple tasks ? nano/mini models
   - Complex tasks ? standard models
   - Critical tasks ? pro models
3. **Batch Processing**: 50% discount for non-urgent requests
4. **Monitor Costs**: Use built-in cost tracking

### Model Selection Guide
| Use Case | Recommended Model | Cost/Performance |
|----------|------------------|------------------|
| Simple chat | gpt-5-nano | Best value |
| General chat | gpt-5-mini | Balanced |
| Complex tasks | gpt-5 | High quality |
| Reasoning | o3-mini | Efficient |
| Code generation | gpt-5-codex | Specialized |
| Audio/Voice | gpt-realtime-mini | Real-time |
| Search | gpt-5-search-api | Optimized |

---

## ?? Migration Guide

### From GPT-3.5 to GPT-4o-mini
```csharp
// Before
.WithModel(ChatModelName.Gpt_3_5_turbo)

// After (better performance, similar cost)
.WithModel(ChatModelName.Gpt_4o_mini)
```

### From GPT-4 to GPT-5
```csharp
// Before
.WithModel(ChatModelName.Gpt_4_turbo)

// After (latest generation)
.WithModel(ChatModelName.Gpt_5_mini)
```

### From O1 to O3
```csharp
// Before
.WithModel(ChatModelName.O1_mini)

// After (better reasoning)
.WithModel(ChatModelName.O3_mini)
```

---

## ?? References

- [OpenAI Pricing Page](https://openai.com/pricing)
- [OpenAI Models Documentation](https://platform.openai.com/docs/models)
- [Cost Tracking Documentation](./COST_TRACKING.md)
- [Rystem.OpenAI GitHub](https://github.com/KeyserDSoze/Rystem.OpenAi)

---

## ? Checklist for Update

- [x] Added all GPT-5 series models
- [x] Added O3/O4 reasoning models
- [x] Added Realtime/Audio models
- [x] Added Transcription/Speech models
- [x] Added Image models (gpt-image-1)
- [x] Added Search preview models
- [x] Added Computer Use models
- [x] Added Legacy models (GPT-3.5, etc.)
- [x] Updated all pricing to Standard tier
- [x] Added cached input pricing
- [x] Added audio token pricing
- [x] Documented model naming conventions
- [x] Provided migration guide
- [x] Added cost calculation examples

---

**Last Updated**: January 2025  
**Pricing Tier**: Standard (non-batch, non-priority)  
**Source**: OpenAI Official Pricing Page
