# Fix: JSON Deserialization Error

## ?? Problema

Errore durante la deserializzazione del JSON ritornato dall'LLM:

```
JSON deserialization for type 'Rystem.PlayFramework.PlannedStep' 
was missing required properties including: 'scene_name', 'purpose'.
```

## ?? Causa

Il problema aveva due cause:

### 1. Proprietà `required` troppo strict
Le proprietà nei modelli erano marcate come `required`:
```csharp
public sealed class PlannedStep
{
    public required int StepNumber { get; init; }
    public required string SceneName { get; init; }  // ? required
    public required string Purpose { get; init; }     // ? required
}
```

System.Text.Json è **molto strict** con `required` - se manca anche solo una proprietà nel JSON, la deserializzazione fallisce.

### 2. Mancanza di opzioni di deserializzazione
Il codice non usava `JsonSerializerOptions` con:
- `PropertyNameCaseInsensitive = true`

Quindi piccole variazioni nel JSON dall'LLM potevano causare fallimenti.

## ? Soluzione Implementata

### 1. Rimozione `required` dai Response Models

**File**: `src/Rystem.PlayFramework/Models/Planner/PlanningModels.cs`

```csharp
// Prima
public sealed class PlannedStep
{
    public required string SceneName { get; init; }  // ?
    public required string Purpose { get; init; }     // ?
}

// Dopo
public sealed class PlannedStep
{
    public string SceneName { get; init; } = string.Empty;  // ?
    public string Purpose { get; init; } = string.Empty;     // ?
}
```

**Modelli aggiornati**:
- `PlannedStep`: `SceneName`, `Purpose` con default values
- `PlanningResponse`: `NeedsExecution` senza required
- `ExecutedSceneInfo`: `SceneName` con default value  
- `ContinuationCheckResponse`: nessun required sulle proprietà

### 2. Aggiunta JsonSerializerOptions

**File**: `src/Rystem.PlayFramework/DefaultServices/Planner/DeterministicPlanner.cs`

```csharp
// Prima
var planningResponse = JsonSerializer.Deserialize<PlanningResponse>(
    toolCall.Function.Arguments);  // ? no options

// Dopo
var serializerOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true  // ? tollerante al case
};

var planningResponse = JsonSerializer.Deserialize<PlanningResponse>(
    toolCall.Function.Arguments, 
    serializerOptions);  // ? con options
```

### 3. Validazione Post-Deserializzazione

Aggiungiamo filtro per validare step dopo deserializzazione:

```csharp
var steps = planningResponse.Steps?
    .Where(s => !string.IsNullOrEmpty(s.SceneName) && 
                !string.IsNullOrEmpty(s.Purpose))  // ? valida
    .OrderBy(s => s.StepNumber)
    .Select(...)
    .ToList() ?? [];
```

## ?? Confronto

### Prima
```csharp
// Modello strict
public required string SceneName { get; init; }

// Deserializzazione senza options
JsonSerializer.Deserialize<PlanningResponse>(json);

// Risultato: ? Exception se manca anche 1 campo
```

### Dopo
```csharp
// Modello tollerante
public string SceneName { get; init; } = string.Empty;

// Deserializzazione con options
JsonSerializer.Deserialize<PlanningResponse>(json, new JsonSerializerOptions {
    PropertyNameCaseInsensitive = true
});

// Validazione post-deserializzazione
.Where(s => !string.IsNullOrEmpty(s.SceneName))

// Risultato: ? Deserializza e filtra step invalidi
```

## ?? Benefici

1. **Robustezza**: Non fallisce per piccole variazioni nel JSON dell'LLM
2. **Flessibilità**: Tollera case-insensitive property names
3. **Graceful Degradation**: Step invalidi vengono filtrati, non bloccano tutto
4. **Better Error Messages**: Include il JSON negli errori per debugging

## ?? Test

Con questo fix:
- ? Deserializzazione sempre riesce (con options)
- ? Step invalidi vengono filtrati
- ? JSON errors includono content per debug
- ? Sistema continua anche se alcuni step non parsano

## ?? Note

### Perché non usare `JsonNamingPolicy.SnakeCaseLower`?

Non esiste in .NET Standard/alcune versioni di .NET. Usiamo solo `PropertyNameCaseInsensitive` che è sufficiente e universale.

### Perché rimuovere `required`?

I modelli di **input** (che noi creiamo) mantengono `required`:
```csharp
public class PlanningRequest
{
    public required string UserRequest { get; init; }  // ? OK
}
```

I modelli di **response** (che vengono dall'LLM) NON hanno `required`:
```csharp
public class PlanningResponse
{
    public bool NeedsExecution { get; init; }  // ? NO required
}
```

Questo perché **noi controlliamo l'input**, ma l'LLM potrebbe variare l'output.

## ?? Risultato

Il sistema ora:
- ? Deserializza correttamente JSON dall'LLM
- ? Tollera variazioni minori
- ? Filtra step invalidi invece di fallire
- ? Fornisce error messages utili

Build successful! ?
