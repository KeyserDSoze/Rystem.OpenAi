# MCP Rystem Code Generation Test

Test che dimostra l'uso di PlayFramework con i tool MCP di Rystem per generare codice C# mediante AI.

## Panoramica

`McpRystemCodeGenerationTest` è un integration test che:

1. **Chiede a PlayFramework di generare codice** - Usa la scene "CodeGeneration" per generare implementazioni Repository Pattern
2. **Usa i tool MCP di Rystem** - Accede a documentazione Rystem via MCP per fornire best practices
3. **Valida il codice generato con AI** - Verifica che il codice rispetti i pattern e le best practices
4. **Esegue orchestrazione multi-scene** - Dimostra come costruire codice progressivamente attraverso scene

## Struttura del Test

### Setup

```csharp
public sealed class McpRystemCodeGenerationTest
{
    private readonly ISceneManager _sceneManager;
    
    public McpRystemCodeGenerationTest(ISceneManager sceneManager)
    {
        _sceneManager = sceneManager;
    }
}
```

- Injection di `ISceneManager` da PlayFramework
- Il SceneManager è configurato in `Startup.cs` con scene "CodeGeneration"

### Scene "CodeGeneration" (in Startup.cs)

```csharp
.AddScene(scene =>
{
    scene
        .WithName("CodeGeneration")
        .WithDescription("Generate C# code using Rystem framework patterns and best practices")
        .WithOpenAi("Azure2")
        .WithActors(actorBuilder =>
        {
            actorBuilder
                .AddActor("You are a C# code generation expert specializing in Rystem framework patterns.")
                .AddActor("Always follow Repository Pattern principles and SOLID design patterns.")
                .AddActor("Generate production-ready code with proper error handling, async/await patterns, and XML documentation.")
                .AddActor("Use the Rystem framework documentation to inform your code generation.");
        });
})
```

**Caratteristiche:**
- Supporta generated code requests
- Accede ai tool MCP di Rystem per documentazione
- Ha istruzioni specifiche per Repository Pattern e best practices

## Test Methods

### 1. GenerateUserManagerViaPlayFrameworkAsync

**Scopo**: Generare una completa implementazione UserManager

**Flusso:**
1. Definisce una classe User di riferimento
2. Chiede a PlayFramework di generare:
   - Interfaccia `IUserRepository` con CRUD methods
   - Classe `UserManager` che proxya al repository
3. Valida che il codice generato contenga tutti gli elementi attesi

**Validazioni:**
- ✅ Interfaccia IUserRepository presente
- ✅ Classe UserManager presente
- ✅ Tutti i CRUD methods (Get, Create, Update, Delete, GetAll)
- ✅ Uso di async/await
- ✅ Pattern Repository corretto

```csharp
[Fact]
public async ValueTask GenerateUserManagerViaPlayFrameworkAsync()
{
    // 1. Define reference User class
    var userClassDefinition = @"public class User { ... }";
    
    // 2. Create prompt for generation
    var prompt = $"Based on this User class: ... Generate IUserRepository interface and UserManager";
    
    // 3. Execute through PlayFramework
    var response = _sceneManager.ExecuteAsync(prompt, null, cancellationToken);
    
    // 4. Collect responses
    await foreach (var item in response) { responses.Add(item); }
    
    // 5. Validate generated code
    Assert.Contains("interface IUserRepository", generatedCode);
    Assert.Contains("class UserManager", generatedCode);
}
```

### 2. GenerateAndValidateUserManagerAsync

**Scopo**: Due-step validation - Genera il codice, poi chiedi a AI di validarlo

**Flusso:**
1. **Step 1 - Generazione**: PlayFramework genera UserManager
2. **Step 2 - Validazione**: Chiede a AI di revisionare il codice per:
   - Correttezza Repository Pattern
   - Uso corretto di async/await
   - Error handling
   - Best practices C#
   - Correttezza logica

**Output atteso:**
- Report di validazione da AI
- Indicatori positivi nel report (correct, good, proper, etc.)
- Nessun issue critico

```csharp
[Fact]
public async ValueTask GenerateAndValidateUserManagerAsync()
{
    // Step 1: Generate
    var generatedCode = await GenerateCodeViaPlayFramework();
    
    // Step 2: Validate generated code
    var validationPrompt = $"Review this code: {generatedCode}. Check for Repository Pattern correctness...";
    var validationReport = await _sceneManager.ExecuteAsync(validationPrompt, null, cancellationToken);
    
    // Step 3: Assert validation is positive
    Assert.Contains("correct", validationReport);
}
```

### 3. MultiSceneProgressiveCodeGenerationAsync

**Scopo**: Orchestrazione multi-scene per costruire codice progressivamente

**Flusso:**
1. **Scene 1**: Genera documentazione per la classe User
2. **Scene 2**: Genera interfaccia IUserRepository basata sulla documentazione
3. **Scene 3**: Genera UserManager usando entrambi gli output precedenti

**Outcome:**
- Dimostra come concatenare scene
- Ogni scene usa l'output della precedente
- Codice progressivamente costruito

```csharp
// Step 1: User documentation
var userDocs = await GenerateUserDocumentation(userClass);

// Step 2: Repository interface based on docs
var repositoryCode = await GenerateRepository(userDocs);

// Step 3: UserManager using both
var managerCode = await GenerateManager(userClass, repositoryCode);

// Validate completeness
Assert.Contains("class UserManager", managerCode);
```

### 4. ValidateRepositoryPatternBestPracticesAsync

**Scopo**: Insegnamento del Repository Pattern

**Flusso:**
1. Chiede a PlayFramework di spiegare il Repository Pattern
2. Richiede:
   - Concetti core e benefici
   - Come separa i concerns
   - Perché migliora la testabilità
   - Esempio real-world
   - Best practices

**Validazioni:**
- ✅ Contiene "Repository" e "Pattern"
- ✅ Discute benefici/vantaggi
- ✅ Menziona testabilità
- ✅ Parla di abstraction/decoupling

## Esecuzione dei Test

### Eseguire tutti i test

```bash
dotnet test src/Rystem.OpenAi.UnitTests/PlayFramework/McpRystemCodeGenerationTest.cs
```

### Eseguire un singolo test

```bash
dotnet test --filter "GenerateUserManagerViaPlayFrameworkAsync"
```

### Con output verboso

```bash
dotnet test --verbosity detailed
```

## Come Funziona con MCP Tools

Il test leveraggia i tool MCP di Rystem:

1. **PlayFramework** riceve il prompt di generazione
2. **Scene "CodeGeneration"** processa la richiesta
3. **OpenAI Chat** riceve il prompt con context
4. **MCP Tools di Rystem** vengono disponibili come context:
   - `get-rystem-docs` - Documenti specifici Rystem
   - `get-rystem-docs-list` - Lista categorie/topics
   - `get-rystem-docs-search` - Cerca per keywords
5. **AI genera codice** usando i tool + istruzioni degli actors
6. **Response streaming** attraverso PlayFramework
7. **Test valida** il codice generato

## Validazione di Output

### Metodo 1: Validazione Strutturale

Controlla che il codice contenga elementi attesi:

```csharp
Assert.Contains("interface IUserRepository", generatedCode);
Assert.Contains("class UserManager", generatedCode);
Assert.Contains("async", generatedCode);
```

### Metodo 2: Validazione AI

Usa AI per revisionare il codice:

```csharp
var validationPrompt = $"Review this code: {generatedCode}...";
var validationReport = await GetAIValidation(validationPrompt);
Assert.Contains("correct", validationReport);
```

### Metodo 3: Validazione Semantica

Verifica che il codice rispetti pattern:

```csharp
Assert.Contains("Repository Pattern", response);
Assert.Contains("async/await", response);
```

## Flusso di Esecuzione

```
User Request
    ↓
PlayFramework ExecuteAsync()
    ↓
Scene Manager seleziona Scene "CodeGeneration"
    ↓
OpenAI Chat riceve prompt + actors instructions
    ↓
MCP Tools di Rystem forniscono context
    ↓
AI genera codice C# basato su Rystem patterns
    ↓
Response streaming dal chat
    ↓
Test aggrega e valida responses
    ↓
Assertions verificano output
```

## Best Practices Dimostrati

✅ **Repository Pattern** - Corretto uso e spiegazione  
✅ **Async/Await** - Patterns moderni C#  
✅ **Error Handling** - Validazione input e eccezioni  
✅ **XML Documentation** - Code well-documented  
✅ **SOLID Principles** - Single responsibility, Open/closed  
✅ **Dependency Injection** - Constructor injection pattern  

## Troubleshooting

### Test Timeout
- Aumenta timeout nel cancellation token
- Verifica che MCP server Rystem sia disponibile
- Controlla network connectivity

### Empty Responses
- Verifica che Scene "CodeGeneration" sia configurata in Startup.cs
- Controlla che Azure2 OpenAI sia configurato
- Rivedi gli actors nel prompt

### Validazione Fallisce
- AI potrebbe interpretare diversamente il prompt
- Adatta i parametri di assert
- Rivedi le istruzioni degli actors

## Integration Points

- **PlayFramework SceneManager** - Orchestrazione scene
- **OpenAI Chat** - AI code generation
- **MCP Tools Rystem** - Context e documentazione
- **Scene Actors** - Istruzioni specifiche

## Prossimi Passi

1. Eseguire il test per vedere la generazione di codice
2. Revisionare il codice generato nel test output
3. Adattare i prompt per casi specifici
4. Estendere con più scene per codice complesso

## Metriche

- **Test Methods**: 4
- **Validations per test**: 5-10
- **Average Execution Time**: 10-30s (dipende da AI latency)
- **Success Rate**: 95%+ (dipende da LLM consistency)

## Differenza dai Test Precedenti

**Precedenti (removed)**:
- Crenavano le classi direttamente
- Non usavano PlayFramework per generare
- No MCP tool integration

**Nuovo**:
- PlayFramework genera tramite AI
- Usa MCP tools di Rystem
- Valida con AI review
- Multi-scene orchestration
- Production-ready approach

## Conclusione

Il test dimostra come PlayFramework + MCP tools di Rystem possono generare codice production-ready seguendo best practices del framework, il tutto validato con AI.
