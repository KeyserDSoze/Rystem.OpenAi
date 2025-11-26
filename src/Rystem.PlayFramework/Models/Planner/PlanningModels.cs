using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Rystem.PlayFramework
{
    /// <summary>
    /// Request for creating an execution plan
    /// </summary>
    public sealed class PlanningRequest
    {
        [JsonPropertyName("user_request")]
        [Description("The user's original request")]
        public required string UserRequest { get; init; }

        [JsonPropertyName("available_scenes")]
        [Description("List of scenes available for execution")]
        public List<AvailableScene>? AvailableScenes { get; init; }

        [JsonPropertyName("conversation_summary")]
        [Description("Summary of previous conversation history")]
        public string? ConversationSummary { get; init; }
    }

    /// <summary>
    /// Available scene information for planning
    /// </summary>
    public sealed class AvailableScene
    {
        [JsonPropertyName("name")]
        [Description("Name of the scene")]
        public required string Name { get; init; }

        [JsonPropertyName("description")]
        [Description("Description of what the scene does")]
        public required string Description { get; init; }

        [JsonPropertyName("available_tools")]
        [Description("List of tool names available in this scene")]
        public List<string>? AvailableTools { get; init; }
    }

    /// <summary>
    /// Response from planning tool containing the execution plan
    /// </summary>
    public sealed class PlanningResponse
    {
        [JsonPropertyName("needs_execution")]
        [Description("True if scenes/tools need to be executed, false if you can answer directly")]
        public bool NeedsExecution { get; init; }

        [JsonPropertyName("reasoning")]
        [Description("Explanation of why execution is or isn't needed")]
        public string? Reasoning { get; init; }

        [JsonPropertyName("steps")]
        [Description("List of steps to execute (empty array if needs_execution is false)")]
        public List<PlannedStep>? Steps { get; init; }
    }

    /// <summary>
    /// A single planned step in the execution plan
    /// </summary>
    public sealed class PlannedStep
    {
        [JsonPropertyName("step_number")]
        [Description("Step execution order starting from 1")]
        public int StepNumber { get; init; }

        [JsonPropertyName("scene_name")]
        [Description("Name of the scene to execute")]
        public string SceneName { get; init; } = string.Empty;

        [JsonPropertyName("purpose")]
        [Description("What this step accomplishes")]
        public string Purpose { get; init; } = string.Empty;

        [JsonPropertyName("expected_tools")]
        [Description("Names of tools expected to be called in this scene")]
        public List<string>? ExpectedTools { get; init; }

        [JsonPropertyName("depends_on_step")]
        [Description("Step number this depends on, or null if independent")]
        public int? DependsOnStep { get; init; }
    }

    /// <summary>
    /// Request to check if execution should continue
    /// </summary>
    public sealed class ContinuationCheckRequest
    {
        [JsonPropertyName("user_request")]
        [Description("The user's original request")]
        public required string UserRequest { get; init; }

        [JsonPropertyName("available_scenes")]
        [Description("List of scene names still available")]
        public List<string>? AvailableScenes { get; init; }
    }

    /// <summary>
    /// Information about an executed scene
    /// </summary>
    public sealed class ExecutedSceneInfo
    {
        [JsonPropertyName("scene_name")]
        [Description("Name of the executed scene")]
        public string SceneName { get; init; } = string.Empty;

        [JsonPropertyName("tools_called")]
        [Description("Names of tools that were called")]
        public List<string>? ToolsCalled { get; init; }

        [JsonPropertyName("result_summary")]
        [Description("Summary of results from this scene")]
        public string? ResultSummary { get; init; }
    }

    /// <summary>
    /// Response indicating if execution should continue
    /// </summary>
    public sealed class ContinuationCheckResponse
    {
        [JsonPropertyName("should_continue")]
        [Description("True if more scenes/tools should be executed")]
        public bool ShouldContinue { get; init; }

        [JsonPropertyName("reasoning")]
        [Description("Explanation of the decision")]
        public string? Reasoning { get; init; }

        [JsonPropertyName("can_answer_now")]
        [Description("True if enough information has been gathered to answer the user")]
        public bool CanAnswerNow { get; init; }

        [JsonPropertyName("missing_information")]
        [Description("What information is still needed if should_continue is true")]
        public string? MissingInformation { get; init; }
    }
}
