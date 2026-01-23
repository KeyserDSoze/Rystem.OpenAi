using Xunit;

namespace Rystem.PlayFramework.Test
{
    /// <summary>
    /// Integration test that demonstrates MCP tool usage with Rystem docs
    /// to generate UserManager code using Repository Pattern via PlayFramework.
    /// The test requests AI to generate code, then validates it using AI review.
    /// </summary>
    public sealed class McpRystemCodeGenerationTest
    {
        private readonly ISceneManager _sceneManager;

        public McpRystemCodeGenerationTest(ISceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        [Fact]
        public async ValueTask GenerateUserManagerViaPlayFrameworkAsync()
        {
            // Arrange - Define the User class as reference
            var userClassDefinition = @"
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}";

            // Arrange - Create the prompt for code generation using MCP tools
            var prompt = $@"
Based on this User class:
```csharp
{userClassDefinition}
```

Generate a complete C# implementation with:

1. IUserRepository interface with these methods:
   - GetAsync(Guid id)
   - GetAllAsync()
   - AddAsync(User user)
   - UpdateAsync(User user)
   - DeleteAsync(Guid id)

2. UserManager class that:
   - Takes IUserRepository as constructor dependency
   - Implements these public methods:
     * CreateUserAsync(string name, string email)
     * GetUserAsync(Guid id)
     * GetAllUsersAsync()
     * UpdateUserAsync(Guid id, string name, string email)
     * DeleteUserAsync(Guid id)
   - Each method should proxy to the repository
   - Include error handling for invalid inputs
   - Use async/await with ValueTask
   - Include XML documentation

Use Rystem framework best practices and patterns.";

            // Act - Execute through PlayFramework scene manager
            var responses = new List<AiSceneResponse>();
            var response = _sceneManager.ExecuteAsync(
                prompt,
                null,
                TestContext.Current.CancellationToken);

            await foreach (var item in response)
            {
                responses.Add(item);
            }

            // Assert - Verify response is not empty
            Assert.NotEmpty(responses);

            var finalResponse = responses.LastOrDefault();
            Assert.NotNull(finalResponse);
            Assert.NotNull(finalResponse.Message);

            var generatedCode = finalResponse.Message!;

            // Assert - Verify generated code structure
            Assert.Contains("interface IUserRepository", generatedCode, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("class UserManager", generatedCode, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("GetUserAsync", generatedCode);
            Assert.Contains("CreateUserAsync", generatedCode);
            Assert.Contains("UpdateUserAsync", generatedCode);
            Assert.Contains("DeleteUserAsync", generatedCode);
            Assert.Contains("GetAllUsersAsync", generatedCode);
            Assert.Contains("IUserRepository", generatedCode);
            Assert.Contains("async", generatedCode);
            Assert.Contains("await", generatedCode);
        }

        [Fact]
        public async ValueTask GenerateAndValidateUserManagerAsync()
        {
            // Arrange - Step 1: Generate the code
            var userClassDefinition = @"
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}";

            var generationPrompt = $@"
Generate C# Repository Pattern implementation for this User class:
```csharp
{userClassDefinition}
```

Create IUserRepository interface and UserManager class.
Include async/await, error handling, and XML documentation.
Return ONLY the C# code.";

            var generateResponses = new List<AiSceneResponse>();
            var generateResponse = _sceneManager.ExecuteAsync(
                generationPrompt,
                null,
                TestContext.Current.CancellationToken);

            await foreach (var item in generateResponse)
            {
                generateResponses.Add(item);
            }

            var generatedCode = generateResponses.LastOrDefault()?.Message ?? "";
            Assert.NotEmpty(generatedCode);

            // Act - Step 2: Validate the generated code with AI
            var validationPrompt = $@"
Review this generated C# code and validate it:

```csharp
{generatedCode}
```

Check for:
1. Repository Pattern correctness
2. Proper async/await usage
3. Error handling
4. C# best practices
5. Logical correctness

Provide a validation report with a score (1-10) and any issues found.";

            var validationResponses = new List<AiSceneResponse>();
            var validationResponse = _sceneManager.ExecuteAsync(
                validationPrompt,
                null,
                TestContext.Current.CancellationToken);

            await foreach (var item in validationResponse)
            {
                validationResponses.Add(item);
            }

            var validationReport = validationResponses.LastOrDefault()?.Message ?? "";

            // Assert - Validation should contain positive indicators
            Assert.NotEmpty(validationReport);
            Assert.Contains("Repository", validationReport, StringComparison.OrdinalIgnoreCase);
            
            // Check for positive validation indicators
            var hasPositiveIndicators = 
                validationReport.Contains("correct", StringComparison.OrdinalIgnoreCase) ||
                validationReport.Contains("good", StringComparison.OrdinalIgnoreCase) ||
                validationReport.Contains("proper", StringComparison.OrdinalIgnoreCase) ||
                validationReport.Contains("well", StringComparison.OrdinalIgnoreCase) ||
                validationReport.Contains("appropriate", StringComparison.OrdinalIgnoreCase) ||
                validationReport.Contains("follows", StringComparison.OrdinalIgnoreCase) ||
                validationReport.Contains("implements", StringComparison.OrdinalIgnoreCase);

            Assert.True(hasPositiveIndicators, "Validation should contain positive assessment of the generated code");
        }

        [Fact]
        public async ValueTask MultiSceneProgressiveCodeGenerationAsync()
        {
            // Arrange
            var userClass = @"
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}";

            var responses = new List<AiSceneResponse>();

            // Step 1: Generate User model documentation
            var step1Prompt = $@"
Create clear documentation for this User entity:
```csharp
{userClass}
```

Explain each property and its purpose. Describe the Repository Pattern context.";

            var step1Response = _sceneManager.ExecuteAsync(
                step1Prompt,
                null,
                TestContext.Current.CancellationToken);

            await foreach (var item in step1Response)
            {
                responses.Add(item);
            }

            var userDocumentation = responses.LastOrDefault()?.Message ?? "";
            Assert.NotEmpty(userDocumentation);
            Assert.Contains("User", userDocumentation);

            // Step 2: Generate repository interface based on documentation
            responses.Clear();
            var step2Prompt = $@"
Based on this User entity documentation:
{userDocumentation}

Generate an IUserRepository interface with CRUD methods.
Include XML documentation for each method.
Return ONLY the interface code.";

            var step2Response = _sceneManager.ExecuteAsync(
                step2Prompt,
                null,
                TestContext.Current.CancellationToken);

            await foreach (var item in step2Response)
            {
                responses.Add(item);
            }

            var repositoryCode = responses.LastOrDefault()?.Message ?? "";
            Assert.Contains("interface IUserRepository", repositoryCode, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("GetAsync", repositoryCode);

            // Step 3: Generate UserManager using both previous outputs
            responses.Clear();
            var step3Prompt = $@"
Based on the User class and IUserRepository interface:

User Class:
```csharp
{userClass}
```

Repository Interface (excerpt):
{repositoryCode}

Generate a complete UserManager class that:
- Takes IUserRepository as dependency
- Implements business logic methods
- Includes validation and error handling
- Uses async/await patterns
- Has XML documentation

Return ONLY the UserManager class code.";

            var step3Response = _sceneManager.ExecuteAsync(
                step3Prompt,
                null,
                TestContext.Current.CancellationToken);

            await foreach (var item in step3Response)
            {
                responses.Add(item);
            }

            var managerCode = responses.LastOrDefault()?.Message ?? "";

            // Assert - Complete implementation generated
            Assert.Contains("class UserManager", managerCode, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("IUserRepository", managerCode);
            Assert.Contains("async", managerCode);
            Assert.Contains("await", managerCode);
        }

        [Fact]
        public async ValueTask ValidateRepositoryPatternBestPracticesAsync()
        {
            // Arrange
            var prompt = @"
Explain the Repository Pattern in C# with these points:
1. Core concepts and benefits
2. How it separates concerns
3. Why it improves testability
4. Real-world example with User management
5. Best practices to follow

Provide a comprehensive explanation with code example if helpful.";

            // Act
            var responses = new List<AiSceneResponse>();
            var response = _sceneManager.ExecuteAsync(
                prompt,
                null,
                TestContext.Current.CancellationToken);

            await foreach (var item in response)
            {
                responses.Add(item);
            }

            var fullResponse = responses.LastOrDefault()?.Message ?? "";

            // Assert
            Assert.NotEmpty(fullResponse);
            Assert.Contains("Repository", fullResponse, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("Pattern", fullResponse, StringComparison.OrdinalIgnoreCase);
            
            // Verify it discusses best practices
            var hasQualityContent = 
                fullResponse.Contains("benefit", StringComparison.OrdinalIgnoreCase) ||
                fullResponse.Contains("advantage", StringComparison.OrdinalIgnoreCase) ||
                fullResponse.Contains("testable", StringComparison.OrdinalIgnoreCase) ||
                fullResponse.Contains("decouple", StringComparison.OrdinalIgnoreCase) ||
                fullResponse.Contains("separation", StringComparison.OrdinalIgnoreCase) ||
                fullResponse.Contains("abstraction", StringComparison.OrdinalIgnoreCase);

            Assert.True(hasQualityContent, "Should discuss Repository Pattern benefits and practices");
        }
    }
}
