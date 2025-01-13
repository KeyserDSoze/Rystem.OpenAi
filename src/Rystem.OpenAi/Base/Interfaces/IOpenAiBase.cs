namespace Rystem.OpenAi
{
    public interface IOpenAiBase<out T>
        where T : class
    {
        /// <summary>
        /// Version of the API to use.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        T WithVersion(string version);
    }
    public interface IOpenAiBase<out T, TModel> : IOpenAiBase<T>
        where TModel : ModelName
        where T : class
    {
        /// <summary>
        /// ID of the model to use.
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns> <see cref="T"/></returns>
        T WithModel(TModel model);
        /// <summary>
        /// Force the value ID of the model to use.
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns> <see cref="TClass"/></returns>
        T ForceModel(string model);
        /// <summary>
        /// Calculate the cost for this request based on configurated price during startup.
        /// </summary>
        /// <returns></returns>
        decimal CalculateCost();
    }
}
