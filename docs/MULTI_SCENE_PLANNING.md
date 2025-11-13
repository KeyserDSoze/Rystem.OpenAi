# Play Framework - Multi-Scene Planning & Summarization

## Overview

Il sistema PlayFramework ora supporta la **pianificazione multi-scena** e la **summarization automatica** dello storico della conversazione. Queste funzionalità permettono di:

1. **Creare piani d'azione** che orchestrano più scene in modo intelligente
2. **Riassumere automaticamente** lo storico quando diventa troppo grande usando OpenAI
3. **Eseguire workflow complessi** con più scene in sequenza

## Architettura

### Componenti Principali

#### 1. IPlanner
Interfaccia per la creazione di piani d'azione multi-scena.

```csharp
public interface IPlanner
{
    Task<ExecutionPlan> CreatePlanAsync(SceneContext context, SceneRequestSettings requestSettings, CancellationToken cancellationToken);
}
```

**Implementazione Default:** `DefaultPlanner`
- Analizza la richiesta dell'utente
- Identifica le scene disponibili e i loro tool
- Crea un piano strutturato con step ordinati
- Usa OpenAI per generare piani intelligenti

#### 2. ISummarizer
Interfaccia per la summarization dello storico.

```csharp
public interface ISummarizer
{
    Task<string> SummarizeAsync(List<AiSceneResponse> responses, CancellationToken cancellationToken);
    bool ShouldSummarize(List<AiSceneResponse> responses);
}
```

**Implementazione Default:** `DefaultSummarizer`
- Controlla se lo storico supera le soglie configurate
- **Usa OpenAI per creare riassunti intelligenti** dello storico
- Crea riassunti strutturati delle azioni precedenti con focus su:
  - Richieste dell'utente
  - Scene e tool utilizzati
  - Risultati chiave
  - Informazioni importanti scambiate
- Fallback a riassunto semplice se OpenAI non è disponibile
- Mantiene le informazioni essenziali per il contesto

#### 3. ExecutionPlan
Modello che rappresenta un piano d'azione.

```csharp
public sealed class ExecutionPlan
{
    public required List<PlanStep> Steps { get; init; }
    public bool IsValid { get; set; }
    public string? Reasoning { get; set; }
}

public sealed class PlanStep
{
    public required string SceneName { get; init; }
    public required string Purpose { get; init; }
    public List<string>? ExpectedTools { get; init; }
    public bool IsCompleted { get; set; }
    public int Order { get; set; }
}
```

### Nuovi Stati

Aggiunti nuovi stati in `AiResponseStatus`:

```csharp
public enum AiResponseStatus
{
    // ...existing states...
    Planning = 512,      // Creazione del piano
    Summarizing = 1024,  // Summarization in corso
}
```

## Configurazione

### Settings Base

```csharp
services.AddPlayFramework(scenes =>
{
    scenes.Configure(settings =>
    {
        settings.OpenAi.Name = "playframework";
        
        // Configurazione Planning
        settings.Planning.Enabled = true;
        settings.Planning.MaxScenesInPlan = 10;
        
        // Configurazione Summarization
        settings.Summarization.Enabled = true;
        settings.Summarization.ResponseThreshold = 50;
        settings.Summarization.CharacterThreshold = 10000;
    });
    
    // ...scene configuration...
});
```

### Configurazioni Disponibili

#### Planning Settings

| Proprietà | Tipo | Default | Descrizione |
|-----------|------|---------|-------------|
| `Enabled` | bool | true | Abilita/disabilita il planning automatico |
| `MaxScenesInPlan` | int | 10 | Numero massimo di scene da includere in un piano |

#### Summarization Settings

| Proprietà | Tipo | Default | Descrizione |
|-----------|------|---------|-------------|
| `Enabled` | bool | true | Abilita/disabilita la summarization automatica |
| `ResponseThreshold` | int | 50 | Numero di risposte prima di triggerare la summarization |
| `CharacterThreshold` | int | 10000 | Numero di caratteri prima di triggerare la summarization |

### Implementazioni Custom

#### Custom Planner

```csharp
public class CustomPlanner : IPlanner
{
    public async Task<ExecutionPlan> CreatePlanAsync(
        SceneContext context, 
        SceneRequestSettings requestSettings, 
        CancellationToken cancellationToken)
    {
        // Logica custom per creare il piano
        return new ExecutionPlan
        {
            Steps = [
                new PlanStep { SceneName = "Scene1", Purpose = "...", Order = 1 },
                new PlanStep { SceneName = "Scene2", Purpose = "...", Order = 2 }
            ],
            IsValid = true,
            Reasoning = "Piano personalizzato"
        };
    }
}

// Registrazione
services.AddPlayFramework(scenes =>
{
    scenes.AddCustomPlanner<CustomPlanner>();
});
```

#### Custom Summarizer

```csharp
public class CustomSummarizer : ISummarizer
{
    public bool ShouldSummarize(List<AiSceneResponse> responses)
    {
        // Logica custom per decidere quando riassumere
        return responses.Count > 100;
    }
    
    public async Task<string> SummarizeAsync(
        List<AiSceneResponse> responses, 
        CancellationToken cancellationToken)
    {
        // Logica custom per creare il riassunto
        // Il DefaultSummarizer usa OpenAI per riassunti intelligenti
        // Puoi anche creare la tua logica personalizzata
        return "Riassunto personalizzato...";
    }
}

// Registrazione
services.AddPlayFramework(scenes =>
{
    scenes.AddCustomSummarizer<CustomSummarizer>();
});
```

### Come Funziona il DefaultSummarizer

Il `DefaultSummarizer` utilizza OpenAI per creare riassunti intelligenti:

1. **Costruisce lo Storico**: Formatta tutte le interazioni precedenti in un formato leggibile
2. **Chiede a OpenAI**: Invia lo storico all'LLM con un prompt specifico per la summarization
3. **Ottiene il Riassunto**: L'LLM crea un riassunto conciso e strutturato
4. **Fallback**: Se OpenAI non è disponibile, usa un riassunto semplice basato su regole

**Prompt utilizzato**:
```
You are an expert at summarizing conversation histories. 
Create a concise but comprehensive summary of the following conversation.
Focus on:
1. What the user requested
2. Which scenes/tools were used
3. Key results and outcomes
4. Important data or information exchanged
```

## Flusso di Esecuzione

### Con Planning Abilitato

1. **Inizializzazione**
   - L'utente invia una richiesta
   - Il sistema controlla se lo storico deve essere riassunto
   - Se necessario, crea un summary dello storico precedente

2. **Planning**
   - Il planner analizza la richiesta
   - Identifica le scene disponibili e i loro tool
   - Crea un piano d'azione strutturato
   - Restituisce un `ExecutionPlan` con gli step ordinati

3. **Esecuzione del Piano**
   - Per ogni step del piano (in ordine):
     - Esegue la scena specificata
     - Utilizza i tool disponibili nella scena
     - Marca lo step come completato
   - Dopo tutti gli step, consulta il Director per verificare se serve altro

4. **Finalizzazione**
   - Salva tutto lo storico (incluso il summary se creato) nella cache
   - Restituisce i risultati all'utente

### Senza Planning (Comportamento Originale)

Se il planning è disabilitato, il sistema funziona come prima:
- Chiede all'LLM di scegliere una scena
- Esegue la scena scelta
- Il Director decide se continuare

## Esempi

### Esempio 1: Richiesta Multi-Scena

**Input:** "Voglio chiedere 3 giorni di ferie dal 15 al 17 marzo e poi vorrei sapere il meteo a Milano"

**Con Planning:**
```json
{
  "plan": {
    "steps": [
      {
        "sceneName": "Chiedi ferie o permessi",
        "purpose": "Gestire la richiesta di ferie per 3 giorni",
        "order": 1,
        "expectedTools": ["eseguire_richiesta_ferie_permessi", "prendi_approvatori_richiesta"]
      },
      {
        "sceneName": "Weather",
        "purpose": "Ottenere informazioni meteo per Milano",
        "order": 2,
        "expectedTools": ["WeatherForecast_Get", "Country_AddCity"]
      }
    ],
    "reasoning": "L'utente ha due richieste separate: ferie e meteo. Le eseguiremo in sequenza."
  }
}
```

### Esempio 2: Summarization

**Scenario:** Una conversazione con 60 interazioni precedenti

**Prima (senza summary):**
```
Between triple backtips you can find information...
```
1) - type of message: Request
- Message: voglio il meteo a Roma
2) - type of message: FunctionRequest
...
[60 interazioni dettagliate]
```

**Dopo (con summary creato da OpenAI):**
```
Conversation Summary:

The user made several requests during this session:

• Weather Information:
  - Requested weather forecast for Rome
  - System used WeatherInfo scene with tools:
    - Country_AddCity (added Rome with population 2,873,000)
    - WeatherForecast_Get
  - Result: Weather in Rome is sunny with 22°C

• User Identity:
  - Retrieved user information using UserIdentity scene
  - Tool used: GetUserName
  - Result: User identified as Mario Rossi

• Vacation Request:
  - User requested 3 days off (March 15-17)
  - Scene: VacationManagement
  - Tools: eseguire_richiesta_ferie_permessi, prendi_approvatori_richiesta
  - Result: Vacation request submitted successfully, pending approval

All requests were completed successfully with no errors.
```

**Vantaggi del summary con OpenAI**:
- ? Riassunto intelligente e contestuale
- ? Organizzazione logica delle informazioni
- ? Estrazione automatica dei punti chiave
- ? Linguaggio naturale e comprensibile
- ? Riduzione da ~15000 caratteri a ~500 caratteri (~97% di riduzione)

## Best Practices

### 1. Configurare Soglie Appropriate

```csharp
settings.Summarization.ResponseThreshold = 50;  // Default buono per la maggior parte dei casi
settings.Summarization.CharacterThreshold = 10000;  // Circa 1500-2000 token
```

### 2. Descrivere Bene le Scene

Il planner usa le descrizioni delle scene per creare piani intelligenti:

```csharp
scene.WithName("VacationRequest")
    .WithDescription("Gestisce richieste di ferie e permessi. Permette di richiedere giorni di assenza, verificare approvatori e controllare date festive.")
```

### 3. Nominare i Tool in Modo Chiaro

```csharp
builder.WithMethod(x => x.MakeRequest, 
    "eseguire_richiesta_ferie_permessi", 
    "Metodo che permette la richiesta di ferie o permessi")
```

### 4. Usare il Context per Condividere Dati

```csharp
// In una scena
context.SetProperty("userId", userId);

// In un'altra scena
var userId = context.GetProperty<string>("userId");
```

### 5. Testare con Planning Disabilitato

Per scenari semplici o debugging:

```csharp
settings.Planning.Enabled = false;
```

## Vantaggi

### Planning Multi-Scena

? **Orchestrazione Intelligente**: Il sistema pianifica l'intero workflow prima dell'esecuzione
? **Efficienza**: Riduce i round-trip con l'LLM
? **Trasparenza**: Il piano è visibile e tracciabile
? **Flessibilità**: Possibilità di implementare planner custom

### Summarization

? **Gestione Token**: Riduce il consumo di token con conversazioni lunghe
? **Performance**: Riassunti intelligenti creati da OpenAI
? **Qualità**: Riassunti contestuali e ben strutturati dall'LLM
? **Costi Ridotti**: Meno token = meno costi API (riduzione ~95-97%)
? **Context Management**: Mantiene il contesto essenziale senza sovraccarico
? **Fallback Automatico**: Se OpenAI non è disponibile, usa riassunto semplice

## Troubleshooting

### Il Piano Non Viene Creato

**Problema**: `ExecutionPlan.IsValid == false`

**Soluzioni**:
- Verifica che `settings.Planning.Enabled = true`
- Controlla che ci siano scene disponibili
- Assicurati che `CreateNewDefaultChatClient` sia configurato
- Verifica i log per errori dell'LLM

### Lo Storico Non Viene Riassunto

**Problema**: Continua a usare lo storico completo

**Soluzioni**:
- Verifica che `settings.Summarization.Enabled = true`
- Controlla se le soglie sono state raggiunte
- Verifica che `_cacheService` sia configurato
- Controlla che la richiesta non sia la prima (KeyHasStartedAsNull)

### Piano Troppo Generico

**Problema**: Il planner crea piani poco specifici

**Soluzioni**:
- Migliora le descrizioni delle scene
- Implementa un custom planner con logica specifica
- Riduci `MaxScenesInPlan` per focalizzare le opzioni
- Fornisci più context negli actor

## Riferimenti Codice

- **Interfaces**: 
  - `src/Rystem.PlayFramework/Interfaces/Planner/IPlanner.cs`
  - `src/Rystem.PlayFramework/Interfaces/Summarizer/ISummarizer.cs`
- **Implementazioni**: 
  - `src/Rystem.PlayFramework/DefaultServices/Planner/DefaultPlanner.cs`
  - `src/Rystem.PlayFramework/DefaultServices/Summarizer/DefaultSummarizer.cs`
- **Modelli**: 
  - `src/Rystem.PlayFramework/Models/Planner/ExecutionPlan.cs`
  - `src/Rystem.PlayFramework/Models/SceneManager/SceneManagerSettings.cs`
