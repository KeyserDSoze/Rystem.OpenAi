using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public static class FunctionToolExtensions
    {
        /// <summary>
        /// Converts a method to a FunctionTool
        /// </summary>
        /// <param name="method"></param>
        /// <param name="prefix"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public static FunctionTool ToFunctionTool(this MethodInfo method, string? prefix = null, bool? strict = null)
        {
            var functionName = $"{prefix}{method.Name}";
            var description = method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(DescriptionAttribute)) as DescriptionAttribute;
            var jsonFunctionObject = new FunctionToolMainProperty();
            var jsonFunction = new FunctionTool
            {
                Name = functionName,
                Description = description?.Description ?? functionName,
                Parameters = jsonFunctionObject,
                Strict = strict
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
        /// <summary>
        /// Converts a class to a FunctionTool
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public static FunctionTool ToFunctionTool(this Type type, string name, string? description, bool? strict = null)
        {
            var jsonFunctionObject = new FunctionToolMainProperty();
            var jsonFunction = new FunctionTool
            {
                Name = name,
                Description = description ?? name,
                Parameters = jsonFunctionObject,
                Strict = strict
            };
            foreach (var property in type.GetProperties())
            {
                var attribute = property.GetCustomAttribute<JsonPropertyNameAttribute>();
                var parameterName = attribute?.Name ?? property.Name ?? property.PropertyType.Name;
                ToolPropertyHelper.Add(parameterName, property.PropertyType, jsonFunctionObject);
                if (!property.IsNullable() || property.GetCustomAttribute<JsonRequiredAttribute>() != null)
                    jsonFunctionObject.AddRequired(parameterName);
            }
            return jsonFunction;
        }
    }
}
