namespace Rystem.PlayFramework
{
    /// <summary>
    /// Defines the planner for creating multi-scene execution plans.
    /// </summary>
    public interface IPlanner
    {
        /// <summary>
        /// Creates an execution plan based on the user request and available scenes/tools.
        /// </summary>
        /// <param name="context">The scene context containing request information</param>
        /// <param name="requestSettings">The request settings</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The execution plan</returns>
        Task<ExecutionPlan> CreatePlanAsync(SceneContext context, SceneRequestSettings requestSettings, CancellationToken cancellationToken);
    }
}
