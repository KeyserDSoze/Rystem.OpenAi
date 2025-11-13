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
                // Handle different primitive types explicitly for correct JSON Schema
                string typeName;
                if (type == typeof(bool) || type == typeof(bool?))
                {
                    typeName = "boolean";
                }
                else if (type.IsInteger())  // int, long, short, uint, etc.
                {
                    typeName = "integer";
                }
                else if (type.IsNumber())  // float, double, decimal
                {
                    typeName = "number";
                }
                else
                {
                    typeName = "string";  // char, string, DateTime, etc.
                }

                jsonFunction.AddPrimitive(name, new FunctionToolPrimitiveProperty
                {
                    Type = typeName,
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
                if (!type.IsPrimitive())
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
