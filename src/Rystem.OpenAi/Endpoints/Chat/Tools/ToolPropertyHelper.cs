using System;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public static class ToolPropertyHelper
    {
        public static void Add(string? parameterName, Type type, FunctionToolNonPrimitiveProperty jsonFunction)
        {
            var description = type.GetCustomAttribute<DescriptionAttribute>();
            if (type.IsPrimitive())
            {
                jsonFunction.AddPrimitive(parameterName ?? type.Name, new FunctionToolPrimitiveProperty
                {
                    Type = type.IsNumeric() ? "number" : "string",
                    Description = description?.Description
                });
            }
            else
            {
                var innerFunction = new FunctionToolNonPrimitiveProperty();
                jsonFunction.AddObject(parameterName ?? type.Name, innerFunction);
                foreach (var innerParameter in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (innerParameter.GetCustomAttribute<JsonIgnoreAttribute>() is null)
                    {
                        Add(innerParameter.Name, innerParameter.PropertyType, innerFunction);
                    }
                }
            }
        }
    }
}
