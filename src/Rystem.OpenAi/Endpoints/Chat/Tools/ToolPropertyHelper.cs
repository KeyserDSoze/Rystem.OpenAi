using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public static class ToolPropertyHelper
    {
        public static void Add(string? parameterName, Type type, FunctionToolNonPrimitiveProperty jsonFunction, string? forceDescription)
        {
            var name = type.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? parameterName ?? type.Name;
            var description = forceDescription ?? type.GetCustomAttribute<DescriptionAttribute>()?.Description ?? parameterName;
            if (type.IsPrimitive())
            {
                jsonFunction.AddPrimitive(name, new FunctionToolPrimitiveProperty
                {
                    Type = type.IsNumeric() ? "number" : "string",
                    Description = description ?? "parameter"
                });
            }
            else if (type.IsEnumerable())
            {
                var theArrayObject = new FunctionToolNonPrimitiveProperty();
                var arrayFunction = new FunctionToolArrayProperty
                {
                    Description = description,
                    Type = "array",
                    Items = theArrayObject
                };
                jsonFunction.AddArray(name, arrayFunction);
                type = type.GetGenericArguments().First();
                foreach (var innerParameter in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (innerParameter.GetCustomAttribute<JsonIgnoreAttribute>() is null)
                    {
                        var propertyName = innerParameter.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? innerParameter.Name;
                        var propertyDescription = innerParameter.GetCustomAttribute<DescriptionAttribute>()?.Description;
                        Add(propertyName, innerParameter.PropertyType, theArrayObject, propertyDescription);
                    }
                }
            }
            else if (type.IsClass)
            {
                var innerFunction = new FunctionToolNonPrimitiveProperty();
                jsonFunction.AddObject(name, innerFunction);
                foreach (var innerParameter in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (innerParameter.GetCustomAttribute<JsonIgnoreAttribute>() is null)
                    {
                        var propertyName = innerParameter.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? innerParameter.Name;
                        var propertyDescription = innerParameter.GetCustomAttribute<DescriptionAttribute>()?.Description;
                        Add(propertyName, innerParameter.PropertyType, innerFunction, propertyDescription);
                    }
                }
            }
        }
    }
}
