using Azure.Core;

namespace Rystem.OpenAi
{
    public interface IOpenAiBase<out T, TModel>
        where TModel : ModelName
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
