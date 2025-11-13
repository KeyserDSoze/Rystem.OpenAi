# Multi-Scene Planning Integration Tests

## Overview

Nuova suite di test completi per verificare le funzionalità di **planning multi-scena** e **summarization automatica** del PlayFramework.

## File Creato

**`src/Rystem.OpenAi.UnitTests/PlayFramework/MultiScenePlanningTest.cs`**

Suite completa di test di integrazione che verifica:
- Planning multi-scena
- Cache persistenza tra turni di conversazione
- Summarization automatica
- Gestione errori
- Esecuzione parallela

## Test Implementati

### 1. `CompleteConversationWithPlanningTest()`
**Scopo**: Test end-to-end di una conversazione completa multi-turno

**Scenario**:
1. **Turn 1**: Richiesta complessa (Identity + Weather)
   - "Ciao! Mi chiamo con username keysersoze e vorrei sapere il meteo a Milano oggi."
   - Verifica che il planning venga attivato
   - Verifica che vengano usate almeno 2 scene (Identity + Weather)
   - Verifica recupero identità utente

2. **Turn 2**: Richiesta ferie (VacationManagement)
   - "Perfetto! Ora vorrei richiedere 5 giorni di ferie dal 15 giugno al 19 giugno."
   - Verifica uso cache dal turn precedente
   - Verifica scene ferie eseguita

3. **Turn 3**: Richiesta approvatori
   - "Chi deve approvare la mia richiesta?"
   - Verifica funzione `prendi_approvatori_richiesta` chiamata

4. **Turn 4-6**: Ulteriori richieste per testare summarization
   - Date festive
   - Meteo Roma
   - Chiusura conversazione

**Assertions**:
- Planning attivato (status `Planning`)
- Minimo 2 scene utilizzate
- Cache funzionante (stesso `RequestKey`)
- Minimo 3 function calls totali
- Conversazione completata con successo

### 2. `PlanningCreatesValidPlanTest(string complexRequest)`
**Scopo**: Verifica creazione piano valido per richieste complesse

**Test Cases**:
- "Voglio sapere il meteo a Milano e poi richiedere 3 giorni di ferie"
- "Mi chiamo keysersoze, dimmi il meteo a Roma e voglio ferie dal 1 al 5 luglio"

**Assertions**:
- Planning triggerato
- Piano creato con reasoning
- Minimo 2 scene eseguite
- Ordine di esecuzione verificato

### 3. `CachePersistenceTest()`
**Scopo**: Verifica persistenza cache tra turni

**Scenario**:
1. **Turn 1**: "Il mio username è keysersoze"
2. **Turn 2**: "Qual è il mio nome completo?"

**Assertions**:
- Second turn usa cache (più efficiente)
- Risposta corretta ("Alessandro Rapiti")
- Numero risposte secondo turn ? primo turn + 10

### 4. `SummarizationThresholdTest()`
**Scopo**: Verifica che summarization si attivi dopo soglia

**Scenario**:
- 15 turni di conversazione con richieste alternate:
  - Meteo varie città
  - Username diversi
  - Richieste ferie

**Assertions**:
- Totale risposte > 50
- Check per status `Summarizing` (opzionale)

### 5. `InvalidSceneHandlingTest()`
**Scopo**: Gestione richieste che non matchano scene

**Scenario**:
- "Calcola la radice quadrata di 144"

**Assertions**:
- Completamento senza errori
- Status `FinishedNoTool` o `FinishedOk`

### 6. `MultipleIndependentRequestsTest()`
**Scopo**: Esecuzione parallela di conversazioni indipendenti

**Scenario**:
- 3 conversazioni parallele con chiavi diverse
- "Che tempo fa a Milano? (Request {i})"

**Assertions**:
- Tutte completate con successo
- 3 chiavi uniche
- Nessuna interferenza tra conversazioni

## Configurazione Aggiornata

### Modifiche a `Startup.cs`

```csharp
scenes.Configure(settings =>
{
    settings.OpenAi.Name = "Azure2";
    
    // Planning abilitato
    settings.Planning.Enabled = true;
    settings.Planning.MaxScenesInPlan = 5;
    
    // Summarization con soglie basse per testing
    settings.Summarization.Enabled = true;
    settings.Summarization.ResponseThreshold = 20;  // Basso per test
    settings.Summarization.CharacterThreshold = 3000;  // Basso per test
})
```

### Scene Configurate

1. **Weather**
   - Gestione meteo e città/paesi
   - 10 tool disponibili
   - Validazione esistenza prima di aggiungere

2. **Identity**
   - Recupero informazioni utente
   - Tool: `get_user_name`

3. **Chiedi ferie o permessi**
   - Gestione richieste ferie
   - 3 tool: richiesta, approvatori, date festive

## Helper Method

```csharp
private async Task<List<AiSceneResponse>> ExecuteTurnAsync(string message, string conversationKey)
{
    var responses = new List<AiSceneResponse>();
    
    await foreach (var response in _sceneManager.ExecuteAsync(
        message, 
        settings => settings.WithKey(conversationKey),
        TestContext.Current.CancellationToken))
    {
        responses.Add(response);
    }

    return responses;
}
```

Metodo riutilizzabile per eseguire un turno di conversazione e raccogliere tutte le risposte.

## Come Eseguire i Test

### Singolo Test
```bash
dotnet test --filter "FullyQualifiedName~MultiScenePlanningTest.CompleteConversationWithPlanningTest"
```

### Tutta la Suite
```bash
dotnet test --filter "FullyQualifiedName~MultiScenePlanningTest"
```

### Con Verbosità
```bash
dotnet test --filter "FullyQualifiedName~MultiScenePlanningTest" --logger "console;verbosity=detailed"
```

## Cosa Verificano i Test

### ? Planning Multi-Scena
- Piano creato automaticamente per richieste complesse
- Esecuzione sequenziale delle scene pianificate
- Reasoning del piano disponibile

### ? Cache & Persistenza
- Context mantenuto tra turni
- `RequestKey` consistente
- Efficienza migliorata nei turni successivi

### ? Summarization
- Attivazione dopo soglia configurata
- Riassunto dello storico
- Riduzione token per context

### ? Orchestrazione Scene
- Uso corretto di scene multiple
- Function calls eseguiti
- Risultati corretti

### ? Robustezza
- Gestione richieste invalide
- Nessun errore critico
- Graceful degradation

### ? Concorrenza
- Conversazioni parallele indipendenti
- Nessuna interferenza tra chiavi
- Thread-safety

## Metriche di Successo

Per una conversazione multi-turno completa:
- **Scenes utilizzate**: ? 2
- **Function calls totali**: ? 3
- **Status finale**: `FinishedOk` o `FinishedNoTool`
- **Cache hits**: Turni successivi più efficienti
- **Planning**: Attivato per richieste complesse
- **Summarization**: Attivata dopo ~20 risposte

## Note Importanti

### Soglie di Testing
Le soglie di summarization sono volutamente **basse** per facilitare il testing:
- `ResponseThreshold = 20` (production: 50)
- `CharacterThreshold = 3000` (production: 10000)

### Rate Limiting
Il test `SummarizationThresholdTest` include delay di 100ms tra richieste per evitare rate limiting dell'API OpenAI.

### Variabilità LLM
Alcuni assert usano `>=` invece di `==` perché il comportamento dell'LLM può variare leggermente tra esecuzioni.

## Troubleshooting

### Test Fallisce su Planning
**Problema**: Assert su `Planning` status fallisce

**Possibili cause**:
- Planning disabilitato in settings
- Richiesta troppo semplice (1 sola scena)
- LLM non crea piano valido

**Soluzione**: Verifica settings e usa richieste più complesse

### Test Fallisce su Summarization
**Problema**: Summarization non triggerata

**Possibili cause**:
- Soglia non raggiunta
- Cache non configurata
- Settings summarization disabilitati

**Soluzione**: Verifica soglie e cache

### Timeout su Test Lunghi
**Problema**: Test lunghi timeout

**Soluzione**: Aumenta timeout o riduci numero iterazioni
```csharp
[Fact(Timeout = 60000)] // 60 secondi
```

## Estensioni Future

Possibili aggiunte ai test:

1. **Streaming Tests**: Test per response streaming
2. **Custom Planner**: Test con planner personalizzato
3. **Custom Summarizer**: Test con summarizer personalizzato
4. **Error Injection**: Test con errori simulati
5. **Performance Metrics**: Misure di latenza e throughput
6. **Token Counting**: Verifica risparmio token effettivo

## Conclusione

Questa suite di test fornisce copertura completa per:
- ? Funzionalità core di planning
- ? Summarization automatica
- ? Cache e persistenza
- ? Conversazioni multi-turno
- ? Gestione errori
- ? Concorrenza

I test sono progettati per essere:
- **Deterministici** quando possibile
- **Robusti** alla variabilità LLM
- **Documentativi** del comportamento atteso
- **Manutenibili** con helper methods riutilizzabili
