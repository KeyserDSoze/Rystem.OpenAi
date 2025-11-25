# Real vs Mock JSON Deserialization Tests

## ?? Differenza Cruciale

Abbiamo **due tipi** di test per la deserializzazione JSON:

### 1. **Mock Tests** (JsonDeserializationIssueTests.cs)
Test che usano **direttamente** `JsonSerializer.Deserialize`:

```csharp
// ? NON REALISTICO
[Fact]
public void IncorrectJsonTypes_ShouldFailDeserialization()
{
    var wrongJson = """{"page": 1, "onlyMine": "true"}""";
    
    // Usa JsonSerializer.Deserialize direttamente
    // NON passa attraverso i converter di PlayFramework!
    Assert.Throws<JsonException>(() => 
        JsonSerializer.Deserialize<TestFindRequest>(wrongJson));
}
```

**Problema**: Questi test NON usano i converter lenient registrati in `SceneBuilder.cs`!

### 2. **Real Tests** (RealJsonDeserializationTests.cs) ?
Test che usano il **vero flusso** di PlayFramework:

```csharp
// ? REALISTICO
[Fact]
public async Task DateOnly_WithTimeComponent_ShouldWorkThroughPipeline()
{
    // Esegue una richiesta vera attraverso SceneManager
    var userQuestion = "mostrami le richieste di ferie dal 15 gennaio 2024";
    var responses = await ExecuteTurnAsync(userQuestion, conversationKey);
    
    // Verifica che non ci siano errori di deserializzazione
    Assert.NotEmpty(responses);
}
```

**Vantaggio**: Questi test usano il vero sistema di conversione con tutti i converter!

---

## ?? Il Vero Flusso di Deserializzazione

### Step-by-Step

```
1. LLM genera JSON
   ?
2. FunctionsHandler riceve Arguments
   ?
3. SceneBuilder.parametersFiller viene chiamato
   ?
4. JsonSerializer.Deserialize con s_options  ? QUI ci sono i converter!
   ?
5. Parametri deserializzati passati al service
```

### Codice Reale (SceneBuilder.cs)

```csharp
// src/Rystem.PlayFramework/Builder/SceneBuilder.cs
private static readonly JsonSerializerOptions s_options = new()
{
    Converters =
    {
        new FlexibleEnumConverterFactory(),              // Enum: "Requested" ? 0
        new Converters.LenientDateOnlyConverter(),       // DateOnly: "2024-01-15T00:00:00Z" ? DateOnly
        new Converters.LenientNullableDateOnlyConverter(),
        new Converters.LenientTimeOnlyConverter(),       // TimeOnly: "14:30:00.000" ? TimeOnly
        new Converters.LenientNullableTimeOnlyConverter(),
    },
};

// Questo è dove avviene la VERA deserializzazione:
parametersFiller.Add(parameterName, (value, bringer) =>
{
    if (parameterType.IsPrimitive())
    {
        if (parameterType == typeof(DateTime))
            bringer.Parameters.Add(DateTime.Parse(value[parameterName]));
        else
            bringer.Parameters.Add(value[parameterName].Cast(parameterType));
    }
    else
        // ? QUI usa s_options con i converter!
        bringer.Parameters.Add(JsonSerializer.Deserialize(
            value[parameterName], 
            parameterType, 
            s_options)!);
    return ValueTask.CompletedTask;
});
```

---

## ?? Confronto Test

### JsonDeserializationIssueTests (Mock)

| Test | Cosa Testa | Realistico? |
|------|------------|-------------|
| `BooleanAsString_ShouldFailDeserialization` | LLM genera `"true"` invece di `true` | ? Mock |
| `EnumAsString_ShouldFailDeserialization` | LLM genera `"Requested"` invece di `0` | ? Mock |
| `IncorrectJsonTypes_ShouldFailDeserialization` | Tipi sbagliati falliscono | ? Mock |

**Problema**: Usano `JsonSerializer.Deserialize` diretto senza i converter!

### RealJsonDeserializationTests (Real) ?

| Test | Cosa Testa | Realistico? |
|------|------------|-------------|
| `DateOnly_WithTimeComponent_ShouldWorkThroughPipeline` | DateOnly con time component | ? Vero flusso |
| `Enum_FlexibleConversion_ShouldWorkThroughPipeline` | Enum string/number | ? Vero flusso |
| `TimeOnly_VariousFormats_ShouldWorkThroughPipeline` | TimeOnly formati vari | ? Vero flusso |
| `ComplexRequest_WithMultipleTypes_ShouldWorkThroughPipeline` | Scenario complesso | ? Vero flusso |

**Vantaggio**: Usano `SceneManager.ExecuteAsync` che passa per tutti i converter!

---

## ?? Quando Usare Quale Test

### Mock Tests (JsonDeserializationIssueTests)
? **Usa quando**:
- Vuoi verificare che i **converter stessi** funzionino
- Test unitari di `LenientDateOnlyConverter`, `FlexibleEnumConverter`, etc.
- Vuoi test veloci e isolati

? **Non usare per**:
- Verificare il comportamento end-to-end
- Testare l'integrazione completa

### Real Tests (RealJsonDeserializationTests) ?
? **Usa quando**:
- Vuoi verificare che il **sistema completo** funzioni
- Test di integrazione
- Validare scenari utente reali
- Verificare che i converter siano **effettivamente usati**

? **Non usare per**:
- Test unitari specifici di singoli converter

---

## ?? Esempio Pratico

### Scenario: DateOnly con Time Component

#### Mock Test (NON rileva il problema)
```csharp
// Test Mock - PASSA anche se non dovrebbe
[Fact]
public void DateOnly_WithTime_PassesWithoutConverter()
{
    var json = """{"date": "2024-01-15T00:00:00Z"}""";
    
    // Questo FALLISCE perché non usa i converter
    var result = JsonSerializer.Deserialize<DateContainer>(json);
    // JsonException: cannot convert to DateOnly ?
}
```

#### Real Test (Rileva VERA situazione)
```csharp
// Test Real - Verifica il VERO comportamento
[Fact]
public async Task DateOnly_WithTime_WorksThroughPipeline()
{
    var userQuestion = "mostrami le richieste dal 15 gennaio 2024";
    
    // Passa attraverso SceneManager ? FunctionsHandler ? SceneBuilder
    // SceneBuilder usa s_options con LenientDateOnlyConverter
    var responses = await ExecuteTurnAsync(userQuestion, conversationKey);
    
    // Deve completare senza errori ?
    Assert.NotEmpty(responses);
}
```

---

## ?? Coverage Comparison

### Before (Solo Mock Tests)
```
? Converter funzionano in isolamento
? Non verifica integrazione reale
? Non verifica che converter siano usati
? Non verifica flusso completo
```

### After (Mock + Real Tests)
```
? Converter funzionano in isolamento (Mock)
? Verifica integrazione reale (Real)
? Verifica che converter siano usati (Real)
? Verifica flusso completo (Real)
? Scenari utente realistici (Real)
```

---

## ?? Run Tests

### Solo Mock Tests
```bash
dotnet test --filter "FullyQualifiedName~JsonDeserializationIssueTests"
```

### Solo Real Tests ?
```bash
dotnet test --filter "FullyQualifiedName~RealJsonDeserializationTests"
```

### Tutti i JSON Tests
```bash
dotnet test --filter "FullyQualifiedName~Json"
```

---

## ? Raccomandazioni

### Per Sviluppo Converter
1. **Prima**: Scrivi mock test per il converter specifico
2. **Poi**: Scrivi real test per verificare integrazione

### Per Verificare Bug
1. **Prima**: Scrivi real test che riproduce il bug
2. **Poi**: Se necessario, scrivi mock test per isolare il problema

### Per Validazione Produzione
1. **Usa**: Real tests (RealJsonDeserializationTests)
2. **Perché**: Validano il comportamento effettivo del sistema

---

## ?? Test Priority

### Critical (MUST HAVE) ???
- **RealJsonDeserializationTests**: Verifica flusso completo
- **DateTimeConverterTests**: Verifica converter specifici

### Important (SHOULD HAVE) ??
- **JsonTypeValidationTests**: Validazione schema JSON
- **MultiScenePlanningTest**: Integration tests complessi

### Nice to Have (COULD HAVE) ?
- **JsonDeserializationIssueTests**: Test isolati (utili per debugging)

---

## ?? File Correlati

### Test Files
- **RealJsonDeserializationTests.cs** ? - Test vero flusso (NUOVO)
- **JsonDeserializationIssueTests.cs** - Test mock (esistente)
- **DateTimeConverterTests.cs** - Test converter specifici
- **JsonTypeValidationTests.cs** - Test validazione schema

### Implementation Files
- **SceneBuilder.cs** - Dove avviene la vera deserializzazione con s_options
- **LenientDateTimeConverters.cs** - Converter DateOnly/TimeOnly
- **SceneManager.cs** - Orchestrazione esecuzione scene

### Documentation
- **JSON_TYPE_ISSUES_ANALYSIS.md** - Analisi problemi JSON
- **DATEONLY_TIMEONLY_HANDLING.md** - Gestione DateOnly/TimeOnly
- **REAL_VS_MOCK_JSON_TESTS.md** - Questo documento

---

## ?? Conclusione

**Key Takeaway**: 

I **Real Tests** (RealJsonDeserializationTests) sono **essenziali** perché:
1. ? Testano il **vero flusso** di deserializzazione
2. ? Usano i **converter effettivi** registrati in SceneBuilder
3. ? Validano scenari **realistici** degli utenti
4. ? Rilevano problemi di **integrazione**

I **Mock Tests** (JsonDeserializationIssueTests) sono **utili** per:
1. ? Test rapidi e isolati
2. ? Debugging specifico di converter
3. ? Validazione logica converter

**Usa entrambi**, ma dai **priorità ai Real Tests** per validare il sistema! ??
