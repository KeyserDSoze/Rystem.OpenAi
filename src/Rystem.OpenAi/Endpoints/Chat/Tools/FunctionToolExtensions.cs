using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System;

namespace Rystem.OpenAi
{
    public static class FunctionToolExtensions
    {
        /// <summary>
        /// Converts a method to a FunctionTool
        /// </summary>
        /// <param name="method"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static FunctionTool ToFunctionTool(this MethodInfo method, string? prefix = null)
        {
            var functionName = $"{prefix}{method.Name}";
            var description = method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(DescriptionAttribute)) as DescriptionAttribute;
            var jsonFunctionObject = new FunctionToolMainProperty();
            var jsonFunction = new FunctionTool
            {
                Name = functionName,
                Description = description?.Description ?? functionName,
                Parameters = jsonFunctionObject
            };
            foreach (var parameter in method.GetParameters())
            {
                if (parameter.ParameterType == typeof(CancellationToken))
                    continue;
                var parameterName = parameter.Name ?? parameter.ParameterType.Name;
                ToolPropertyHelper.Add(parameterName, parameter.ParameterType, jsonFunctionObject);
                if (!parameter.IsNullable())
                    jsonFunctionObject.AddRequired(parameterName);
            }
            return jsonFunction;
        }
    }
}
