using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    internal sealed class JsonFunctionContainerManager
    {
        private readonly ConcurrentDictionary<string, JsonFunction> _functions = new ConcurrentDictionary<string, JsonFunction>();
        public static JsonFunctionContainerManager Instance { get; } = new JsonFunctionContainerManager();
        private JsonFunctionContainerManager() { }
        public JsonFunction GetFunction(IOpenAiChatFunction function)
        {
            if (!_functions.ContainsKey(function.Name))
            {
                var chatFunction = new JsonFunction()
                {
                    Name = function.Name,
                    Description = function.Description,
                    Parameters = new JsonFunctionNonPrimitiveProperty()
                };
                SetProperties(function.Input, chatFunction.Parameters);
                if (!_functions.ContainsKey(function.Name))
                {
                    _functions.TryAdd(function.Name, chatFunction);
                }
            }
            return _functions[function.Name];
        }
        private static void SetProperties(Type type, JsonFunctionNonPrimitiveProperty parameters)
        {
            foreach (var property in type.GetProperties())
            {
                var name = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name;
                var isRequired = property.GetCustomAttribute<JsonRequiredAttribute>() != null;
                var description = property.GetCustomAttribute<JsonPropertyDescriptionAttribute>()?.Description ?? name;
                var allowedValues = property.GetCustomAttribute<JsonPropertyAllowedValuesAttribute>();
                if (allowedValues != null)
                {
                    if (property.PropertyType != typeof(string))
                        throw new ArgumentException($"It's not possible to set {nameof(JsonPropertyAllowedValuesAttribute)} on property that is not a string. {property.Name} is {property.PropertyType.FullName}.");
                    SetAllowed(allowedValues.Values, name, description, isRequired, parameters);
                }
                else if (property.PropertyType.IsPrimitive())
                {
                    SetPrimitive(property, name, description, isRequired, parameters);
                }
                else if (property.PropertyType.IsArray)
                {
                    SetArray(property.PropertyType, name, description, isRequired, parameters);
                }
                else if (property.PropertyType.IsDictionary())
                {
                    SetDictionary(property.PropertyType, name, description, isRequired, parameters);
                }
                else if (property.PropertyType.IsEnumerable())
                {
                    SetEnumerable(property.PropertyType, name, description, isRequired, parameters);
                }
                else if (property.PropertyType.IsEnum)
                {
                    SetEnum(property.PropertyType, name, description, isRequired, parameters);
                }
                else
                {
                    SetObject(property.PropertyType, name, description, isRequired, parameters);
                }
            }
        }
        private static void SetRequired(string name, bool isRequired, JsonFunctionNonPrimitiveProperty parameters)
        {
            if (isRequired)
                parameters.AddRequired(name);
        }
        private static void SetPrimitive(PropertyInfo? property, string? name, string? description, bool isRequired, JsonFunctionNonPrimitiveProperty parameters, Type? overriddenType = null)
        {
            var type = property?.PropertyType ?? overriddenType;
            if (type != null && name != null)
            {
                var isInteger = type.IsInteger();
                var isNumber = type.IsNumber();
                if (isInteger || isNumber)
                {
                    var rangeAttribute = property.GetCustomAttribute<JsonPropertyRangeAttribute>();
                    double? minimum = null;
                    double? maximum = null;
                    bool? minimumExclusive = null;
                    bool? maximumExclusive = null;
                    double? multipleOf = null;
                    if (rangeAttribute != null)
                    {
                        minimum = rangeAttribute.Minimum;
                        maximum = rangeAttribute.Maximum;
                        minimumExclusive = rangeAttribute.Exclusive;
                        maximumExclusive = rangeAttribute.Exclusive;
                    }
                    var minimumAttribute = property.GetCustomAttribute<JsonPropertyMinimumAttribute>();
                    if (minimumAttribute != null)
                    {
                        minimum = minimumAttribute.Minimum;
                        minimumExclusive = minimumAttribute.Exclusive;
                    }
                    var maximumAttribute = property.GetCustomAttribute<JsonPropertyMaximumAttribute>();
                    if (maximumAttribute != null)
                    {
                        maximum = maximumAttribute.Maximum;
                        maximumExclusive = maximumAttribute.Exclusive;
                    }
                    var multipleOfProperty = property.GetCustomAttribute<JsonPropertyMultipleOfAttribute>();
                    if (multipleOfProperty != null)
                    {
                        multipleOf = multipleOfProperty.MultipleOf;
                    }
                    parameters.AddPrimitive(name, new JsonFunctionNumberProperty
                    {
                        Type = isInteger ? "integer" : "number",
                        Description = description,
                        Minimum = minimum,
                        Maximum = maximum,
                        ExclusiveMaximum = maximumExclusive,
                        ExclusiveMinimum = minimumExclusive,
                        MultipleOf = multipleOf
                    });
                }
                else
                {
                    parameters.AddPrimitive(name, new JsonFunctionProperty
                    {
                        Description = description,
                        Type = type.Name.ToLower(),
                    });
                }
                SetRequired(name, isRequired, parameters);
            }
        }
        private static void SetObject(Type type, string name, string description, bool isRequired, JsonFunctionNonPrimitiveProperty parameters)
        {
            var objectParameters = new JsonFunctionNonPrimitiveProperty
            {
                Description = description,
            };
            parameters.AddObject(name, objectParameters);
            SetRequired(name, isRequired, parameters);
            SetProperties(type, objectParameters);
        }
        private static void SetEnum(Type type, string name, string description, bool isRequired, JsonFunctionNonPrimitiveProperty parameters)
        {
            var chances = Enum.GetNames(type);
            parameters.AddEnum(name, new JsonFunctionEnumProperty
            {
                Description = description,
                Type = "string",
                Enums = chances.ToList()
            });
            SetRequired(name, isRequired, parameters);
        }
        private static void SetAllowed(string[] values, string name, string description, bool isRequired, JsonFunctionNonPrimitiveProperty parameters)
        {
            parameters.AddEnum(name, new JsonFunctionEnumProperty
            {
                Description = description,
                Type = "string",
                Enums = values.ToList()
            });
            SetRequired(name, isRequired, parameters);
        }
        private static void SetArray(Type arrayType, JsonFunctionArrayProperty objectParameters)
        {
            if (arrayType.IsPrimitive())
            {
                if (arrayType.IsNumber())
                    objectParameters.Items = new JsonFunctionProperty
                    {
                        Type = "number",
                    };
                else if (arrayType.IsInteger())
                    objectParameters.Items = new JsonFunctionProperty
                    {
                        Type = "integer",
                    };
                else
                    objectParameters.Items = new JsonFunctionProperty
                    {
                        Type = arrayType.Name.ToLower(),
                    };
            }
            else if (arrayType.IsEnum)
            {
                objectParameters.Items = new JsonFunctionEnumProperty
                {
                    Enums = Enum.GetNames(arrayType).ToList()
                };
            }
            else
            {
                var defaultObject = new JsonFunctionNonPrimitiveProperty();
                objectParameters.Items = defaultObject;
                SetProperties(arrayType, defaultObject);
            }
        }
        private static void SetArray(Type type, string name, string description, bool isRequired, JsonFunctionNonPrimitiveProperty parameters)
        {
            var objectParameters = new JsonFunctionArrayProperty
            {
                Description = description,
                Type = "array",
            };
            var arrayType = type.GetElementType();
            SetArray(arrayType, objectParameters);
            parameters.AddArray(name, objectParameters);
            SetRequired(name, isRequired, parameters);
        }
        private static void SetEnumerable(Type type, string name, string description, bool isRequired, JsonFunctionNonPrimitiveProperty parameters)
        {
            var objectParameters = new JsonFunctionArrayProperty
            {
                Description = description,
                Type = "array",
            };
            var arrayType = type.GetGenericArguments().First();
            SetArray(arrayType, objectParameters);
            parameters.AddArray(name, objectParameters);
            SetRequired(name, isRequired, parameters);
        }
        private static void SetDictionary(Type type, string name, string description, bool isRequired, JsonFunctionNonPrimitiveProperty parameters)
        {
            SetArray(type, name, description, isRequired, parameters);
        }
    }
}
