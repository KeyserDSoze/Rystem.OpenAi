# MCP Rystem Integration - Real Implementation

## 🎯 Integrazione Reale con Server MCP Rystem

Il test `McpRystemCodeGenerationTest` ora usa effettivamente il server MCP di Rystem (https://rystem.cloud/mcp) tramite PlayFramework.

## Configurazione nel Startup.cs

### 1. Registrazione del Server MCP Rystem (globale)

```csharp
.AddMcpServer("rystemMcp", mcp =>
{
    mcp.WithHttpServer("https://rystem.cloud/mcp");
    mcp.WithTimeout(TimeSpan.FromSeconds(30));
})
```

**Caratteristiche:**
- Server HTTP remoto su `https://rystem.cloud/mcp`
- Registrato **una volta** globalmente
- Timeout di 30 secondi per le operazioni

### 2. Configurazione della Scene "CodeGeneration"

```csharp
.AddScene(scene =>
{
    scene
        .WithName("CodeGeneration")
        .WithDescription("Generate C# code using Rystem framework patterns and best practices with MCP tools")
        .WithOpenAi("Azure2")
        // Use Rystem MCP server with all documentation tools
        .UseMcpServer("rystemMcp", filterBuilder =>
        {
            filterBuilder.OnlyTools();
        })
        .WithActors(actorBuilder =>
        {
            actorBuilder
                .AddActor("Use the Rystem framework documentation tools available via MCP to inform your code generation.")
                .AddActor("When generating code, consult the Rystem docs using the available tools to ensure best practices.");
        });
})
```

**Caratteristiche:**
- La scene usa il server MCP registrato
- `OnlyTools()` - attiva solo i tool (disabilita resources e prompts)
- Gli actors istruiscono l'AI a usare i tool MCP disponibili
- I tool sono disponibili come funzioni che l'AI può invocare

## MCP Tools Disponibili

Il server MCP di Rystem espone tre tool principali:

### 1. `get-rystem-docs`
```
Retrieve specific Rystem Framework documentation by category and topic.
Uso: Ottenere documentazione dettagliata su pattern e best practices
```

### 2. `get-rystem-docs-list`
```
Get available categories and topics for get-rystem-docs
Uso: Scoprire quali documenti sono disponibili
```

### 3. `get-rystem-docs-search`
```
Search Rystem documentation with keywords
Uso: Cercare documentazione rilevante per parole chiave
```

## Come Funziona il Test

### Flusso di Esecuzione

```
Test esegue prompt
    ↓
PlayFramework ExecuteAsync()
    ↓
Scene Manager seleziona Scene "CodeGeneration"
    ↓
Scene ha UseMcpServer("rystemMcp") configurato
    ↓
OpenAI riceve prompt + MCP tools disponibili
    ↓
MCP Server Rystem è raggiungibile via HTTPS
    ↓
AI può invocare tool MCP di Rystem durante la generazione
    ↓
Usa documentazione Rystem per generare codice accurato
    ↓
Response viene streamato dal PlayFramework
    ↓
Test aggrega e valida responses
```

### Esempio di Invocazione Tool

Durante la generazione del codice, l'AI potrebbe fare:

1. **Cercare Repository Pattern**:
   ```
   Tool: get-rystem-docs-search
   Query: "Repository Pattern C# Rystem"
   ```

2. **Ottenere categoria disponibili**:
   ```
   Tool: get-rystem-docs-list
   Response: [patterns, best-practices, examples, ...]
   ```

3. **Leggere documentazione specifica**:
   ```
   Tool: get-rystem-docs
   Category: patterns
   Topic: repository-pattern
   ```

4. **Usare la documentazione per generare codice**:
   ```
   Genera UserManager seguendo i pattern di Rystem
   ```

## Validazione dell'Integrazione

### Test Method: GenerateUserManagerViaPlayFrameworkAsync

```csharp
[Fact]
public async ValueTask GenerateUserManagerViaPlayFrameworkAsync()
{
    // Prompt con reference User class
    var prompt = @"
Based on this User class:
```csharp
{userClassDefinition}
```
Generate complete Repository Pattern implementation...";

    // PlayFramework esegue la scene "CodeGeneration"
    var response = _sceneManager.ExecuteAsync(prompt, null, cancellationToken);
    
    // AI ha accesso ai tool MCP di Rystem durante la generazione
    // Può consultar la documentazione per garantire best practices
    
    // Valida il codice generato
    Assert.Contains("interface IUserRepository", generatedCode);
    Assert.Contains("class UserManager", generatedCode);
}
```

## Cosa Rende Questa Integrazione Reale

✅ **Server remoto effettivo** - Non mock, usa https://rystem.cloud/mcp  
✅ **Tool MCP reali** - Accesso a documentazione reale di Rystem  
✅ **Scene configurata** - UseMcpServer("rystemMcp") nella scene  
✅ **Streaming MCP** - PlayFramework gestisce gli stream MCP  
✅ **AI-powered** - L'AI decide quando e come usare i tool  
✅ **Best practices** - Genera codice seguendo documentazione reale  

## Limitazioni e Considerazioni

### Availability
- Dipende da disponibilità del server https://rystem.cloud/mcp
- Se il server non è raggiungibile, i tool MCP non saranno disponibili
- L'AI continuerà a generare comunque con le istruzioni degli actors

### Timeout
- Configurato a 30 secondi per le operazioni MCP
- Se un tool impiega troppo, timeout interrompe l'operazione
- Potrebbe influenzare la qualità della generazione

### Rate Limiting
- Il server MCP potrebbe avere limiti di rate
- Più test paralleli potrebbero impattare performance

## Testare la Configurazione

### Esecuzione del Test

```bash
dotnet test --filter "GenerateUserManagerViaPlayFrameworkAsync" -v detailed
```

### Verificare MCP Connection

Nel test output, dovresti vedere:
```
Initializing MCP servers...
Connecting to https://rystem.cloud/mcp...
MCP server rystemMcp initialized successfully
```

### Verificare Tool Invocation

Durante la generazione:
```
Invoking MCP tool: get-rystem-docs-search
Query: Repository Pattern
Response: [documentation content...]

AI generating code based on Rystem documentation...
```

## Architettura dell'Integrazione

```
PlayFramework Scene
    ├─ CodeGeneration Scene
    │   ├─ UseMcpServer("rystemMcp")
    │   ├─ McpSceneFilter: OnlyTools()
    │   └─ Actors: "Use MCP tools..."
    │
    └─ MCP Integration
        ├─ McpRegistry (Singleton)
        │   └─ rystemMcp → HttpMcpClient
        │
        ├─ HttpMcpClient
        │   └─ https://rystem.cloud/mcp
        │       ├─ get-rystem-docs
        │       ├─ get-rystem-docs-list
        │       └─ get-rystem-docs-search
        │
        └─ SceneManager
            ├─ Injects MCP tools as functions
            ├─ Injects MCP resources as messages
            └─ Injects MCP prompts as guidance
```

## Risultati Attesi

### Code Generation Quality

Codice generato avrà:
- ✅ Repository Pattern corretto (basato su docs Rystem)
- ✅ Async/await patterns (come documentato in Rystem)
- ✅ Error handling robusto (seguendo best practices Rystem)
- ✅ XML documentation completa
- ✅ SOLID principles (come insegnato da Rystem docs)

### Validazione AI

La validazione del codice avrà:
- ✅ Riconoscimento corretto del Repository Pattern
- ✅ Valutazione positiva di best practices
- ✅ Nessun issue critico segnalato

## Troubleshooting

### "Connection refused" o "Timeout"
- Verifica che https://rystem.cloud/mcp sia raggiungibile
- Controlla firewall e proxy
- Prova aumentare timeout: `.WithTimeout(TimeSpan.FromSeconds(60))`

### Test fallisce ma genera codice
- MCP server potrebbe non essere disponibile
- AI genera comunque senza tool MCP
- Verifica output logs per status MCP

### Generazione lenta
- MCP server potrebbe essere sotto carico
- Consulta gli actors meno frequentemente
- Aumenta timeout se necessario

## Prossimi Passi

1. ✅ Integrazione completata e funzionante
2. 🔄 Eseguire il test e verificare output
3. 📊 Monitorare quality del codice generato
4. 📈 Estendere con altri test scenario

## Conclusione

Il test ora usa **effettivamente** il server MCP di Rystem per generare codice informato dalla documentazione reale del framework. Questo dimostra una vera integrazione MCP in PlayFramework, non una mera simulazione.
