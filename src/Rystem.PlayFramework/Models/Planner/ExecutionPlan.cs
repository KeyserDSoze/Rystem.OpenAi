namespace Rystem.PlayFramework
{
    /// <summary>
    /// Represents an execution plan for multi-scene orchestration.
    /// </summary>
    public sealed class ExecutionPlan
    {
        /// <summary>
        /// List of planned steps to execute.
        /// </summary>
        public required List<PlanStep> Steps { get; init; }

        /// <summary>
        /// Indicates if planning was successful.
        /// </summary>
        public bool IsValid { get; set; } = true;

        /// <summary>
        /// The reasoning behind the plan.
        /// </summary>
        public string? Reasoning { get; set; }
    }

    /// <summary>
    /// Represents a single step in the execution plan.
    /// </summary>
    public sealed class PlanStep
    {
        /// <summary>
        /// The scene to execute in this step.
        /// </summary>
        public required string SceneName { get; init; }

        /// <summary>
        /// The purpose of this step.
        /// </summary>
        public required string Purpose { get; init; }

        /// <summary>
        /// Expected tools/functions to use in this scene.
        /// </summary>
        public List<string>? ExpectedTools { get; init; }

        /// <summary>
        /// Whether this step has been completed.
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Order of execution.
        /// </summary>
        public int Order { get; set; }
    }
}
