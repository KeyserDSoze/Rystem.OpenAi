using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    internal sealed class FunctionContainer
    {
        private readonly ConcurrentDictionary<string, ChatFunction> _functions = new ConcurrentDictionary<string, ChatFunction>();
        public static FunctionContainer Instance { get; } = new FunctionContainer();
        private FunctionContainer() { }
        public ChatFunction GetFunction(IOpenAiChatFunction function)
        {
            if (!_functions.ContainsKey(function.Name))
            {
                var chatFunction = new ChatFunction()
                {
                    Name = function.Name,
                    Description = function.Description,
                    Parameters = new ChatFunctionParameters()
                    {
                        Type = "object",
                        Properties = new Dictionary<string, ChatFunctionProperty>(),
                        Required = new List<string>()
                    },
                };
                SetProperties(function.Input, chatFunction.Parameters);
                if (!_functions.ContainsKey(function.Name))
                {
                    _functions.TryAdd(function.Name, chatFunction);
                }
            }
            return _functions[function.Name];
        }
        private static void SetProperties(Type type, ChatFunctionParameters parameters)
        {
            foreach (var property in type.GetProperties())
            {
                var name = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name;
                var typeName = property.PropertyType.Name.ToLower();
                var isRequired = property.GetCustomAttribute<JsonRequiredAttribute>() != null;
                var description = property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? name;
                var isNumber = property.IsNumeric();
                if (property.PropertyType.IsPrimitive())
                {
                    SetPrimitive(name, description, typeName, isRequired, isNumber, parameters);
                }
                else if (property.PropertyType.IsDictionary())
                {

                }
                else if (property.PropertyType.IsArray())
                {
                    SetArray(property.PropertyType, name, description, isRequired, parameters);
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
        private static void SetRequired(string name, bool isRequired, ChatFunctionParameters parameters)
        {
            if (isRequired)
            {
                parameters.Required ??= new List<string>();
                parameters.Required.Add(name);
            }
        }
        private static void SetPrimitive(string name, string description, string typeName, bool isRequired, bool isNumber, ChatFunctionParameters parameters)
        {
            parameters.Properties.Add(name, new ChatFunctionProperty
            {
                Description = description,
                Type = isNumber ? "numeric" : typeName.ToLower(),
            });
            SetRequired(name, isRequired, parameters);
        }
        private static void SetObject(Type type, string name, string description, bool isRequired, ChatFunctionParameters parameters)
        {
            var objectParameters = new ChatFunctionParameters
            {
                Description = description,
                Type = "object",
                Properties = new Dictionary<string, ChatFunctionProperty>(),
            };
            parameters.Properties.Add(name, objectParameters);
            SetRequired(name, isRequired, parameters);
            SetProperties(type, objectParameters);
        }
        private static void SetEnum(Type type, string name, string description, bool isRequired, ChatFunctionParameters parameters)
        {
            var chances = Enum.GetNames(type);
            parameters.Properties.Add(name, new ChatFunctionEnumProperty
            {
                Description = description,
                Type = "enum",
                Enums = chances.ToList()
            });
            SetRequired(name, isRequired, parameters);
        }
        private static void SetArray(Type type, string name, string description, bool isRequired, ChatFunctionParameters parameters)
        {
            var objectParameters = new ChatFunctionParameters
            {
                Description = description,
                Type = "array",
                Properties = new Dictionary<string, ChatFunctionProperty>(),
            };
            parameters.Properties.Add(name, objectParameters);
            SetRequired(name, isRequired, parameters);
            SetProperties(type, objectParameters);
        }
    }
}
