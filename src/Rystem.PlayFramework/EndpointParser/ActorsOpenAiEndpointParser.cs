using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi;

namespace Rystem.PlayFramework
{
    internal sealed class ActorsOpenAiEndpointParser
    {
        private readonly PlayHandler _playHandler;
        private readonly FunctionsHandler _functionHandler;

        public ActorsOpenAiEndpointParser(PlayHandler playHandler, FunctionsHandler functionHandler)
        {
            _playHandler = playHandler;
            _functionHandler = functionHandler;
        }
        public void MapOpenApiAi(IServiceProvider serviceProvider)
        {
            var services = serviceProvider.GetServices<EndpointDataSource>();
            var endpoints = services
            .SelectMany(es => es.Endpoints)
            .OfType<RouteEndpoint>();
            foreach (var endpoint in endpoints)
            {
                if (endpoint.RoutePattern.RawText == null)
                    continue;
                var jsonFunctionObject = new FunctionToolMainProperty();
                var relativePath = endpoint.RoutePattern.RawText.Trim('/');
                var regexToNormalizePath = new Regex($"\\{{[a-zA-Z0-9_@]+\\??(:[a-zA-Z0-9]+)?\\}}");
                var basedFunctionName = endpoint.RoutePattern.RawText;
                if (regexToNormalizePath.IsMatch(basedFunctionName))
                {
                    var toNormalize = regexToNormalizePath.Match(basedFunctionName).Value;
                    var nameToNormalize = toNormalize.Trim('{').Trim('}').Split('?').First().Split(':').First();
                    basedFunctionName = regexToNormalizePath.Replace(basedFunctionName, nameToNormalize);
                }
                basedFunctionName = basedFunctionName.Trim('/').Replace("/", "_");
                var allowedMethods = GetAllowedHttpMethods(endpoint.Metadata).ToList();
                foreach (var method in allowedMethods)
                {
                    var functionName = $"{(allowedMethods.Count > 1 ? $"{method}_" : string.Empty)}{basedFunctionName}";
                    var jsonFunction = new FunctionTool
                    {
                        Name = functionName,
                        Description = endpoint.DisplayName ?? functionName,
                        Parameters = jsonFunctionObject
                    };
                    var function = _functionHandler[functionName];
                    var hasAddedAtLeastOne = false;
                    foreach (var scene in _playHandler.ChooseRightPath(relativePath).Distinct())
                    {
                        if (!_playHandler[scene].Functions.Contains(functionName))
                            _playHandler[scene].Functions.Add(functionName);
                        if (!function.Scenes.Contains(scene))
                            function.Scenes.Add(scene);
                        hasAddedAtLeastOne = true;
                    }
                    if (hasAddedAtLeastOne)
                    {
                        function.Chooser = x => x.AddFunctionTool(jsonFunction);
                        function.HttpRequest = new()
                        {
                            Uri = relativePath,
                            Call = (httpBringer) =>
                            {
                                httpBringer.Method = method;
                                return ValueTask.CompletedTask;
                            }
                        };
                        var actionDescriptor = endpoint.Metadata.FirstOrDefault(x => x is ControllerActionDescriptor);
                        if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor && controllerActionDescriptor is not null)
                        {
                            foreach (var parameter in controllerActionDescriptor.Parameters)
                            {
                                if (parameter is ControllerParameterDescriptor controllerParameterDescriptor)
                                {
                                    if (CheckParameter(parameter.Name, controllerParameterDescriptor.ParameterInfo, parameter.ParameterType))
                                        SetParameter(parameter.Name, controllerParameterDescriptor.ParameterInfo, parameter.ParameterType);
                                }
                            }
                        }
                        else if (endpoint.Metadata.Any(x => x is IParameterBindingMetadata))
                        {
                            foreach (var parameter in endpoint.Metadata.Where(x => x is IParameterBindingMetadata))
                            {
                                if (parameter is IParameterBindingMetadata bindingMetadata)
                                    if (CheckParameter(bindingMetadata.Name, bindingMetadata.ParameterInfo, bindingMetadata.ParameterInfo.ParameterType))
                                        SetParameter(bindingMetadata.Name, bindingMetadata.ParameterInfo, bindingMetadata.ParameterInfo.ParameterType);
                            }
                        }
                        bool CheckParameter(string name, ParameterInfo parameterInfo, Type type)
                        {
                            if (type != typeof(CancellationToken) && !parameterInfo.CustomAttributes.Any(x => x.AttributeType == typeof(FromServicesAttribute) || x.AttributeType == typeof(FromFormAttribute)))
                            {
                                ToolPropertyHelper.Add(name, type, jsonFunctionObject);
                                if (!parameterInfo.IsNullable())
                                    jsonFunctionObject.AddRequired(name);
                                return true;
                            }
                            return false;
                        }
                        bool SetParameter(string name, ParameterInfo parameterInfo, Type type)
                        {
                            var methodAsString = method.ToLower();
                            var bodyAsDefault = methodAsString == "post" || methodAsString == "put" || methodAsString == "patch";
                            if (parameterInfo.CustomAttributes.Any(x => x.AttributeType == typeof(FromHeaderAttribute)))
                            {
                                function.HttpRequest.Actions.Add(name, (value, httpBringer) =>
                                {
                                    if (!httpBringer.Headers.Contains(name))
                                        httpBringer.Headers.Add(name, value[name]);
                                    else
                                    {
                                        httpBringer.Headers.Remove(name);
                                        httpBringer.Headers.Add(name, value[name]);
                                    }
                                    return ValueTask.CompletedTask;
                                });
                            }
                            else if (parameterInfo.CustomAttributes.Any(x => x.AttributeType == typeof(FromRouteAttribute)))
                            {
                                var regexForUriReplace = new Regex($"\\{{{name}\\??(:[a-zA-Z0-9]+)?\\}}");
                                function.HttpRequest.Actions.Add(name, (value, httpBringer) =>
                                {
                                    httpBringer.RewrittenUri = regexForUriReplace.Replace(relativePath, value[name]);
                                    return ValueTask.CompletedTask;
                                });
                            }
                            else if (!bodyAsDefault || parameterInfo.CustomAttributes.Any(x => x.AttributeType == typeof(FromQueryAttribute)))
                            {
                                function.HttpRequest.Actions.Add(name, (value, httpBringer) =>
                                {
                                    httpBringer.Query ??= new();
                                    if (httpBringer.Query.Length > 0)
                                        httpBringer.Query.Append('&');
                                    httpBringer.Query.Append($"{name}={value[name]}");
                                    return ValueTask.CompletedTask;
                                });
                            }
                            else if (bodyAsDefault || parameterInfo.CustomAttributes.Any(x => x.AttributeType == typeof(FromBodyAttribute)))
                            {
                                function.HttpRequest.Actions.Add(name, (value, httpBringer) =>
                                {
                                    httpBringer.BodyAsJson = value[name];
                                    return ValueTask.CompletedTask;
                                });
                            }
                            return true;
                        }
                    }
                }
            }
            IEnumerable<string> GetAllowedHttpMethods(EndpointMetadataCollection metadataList)
            {
                foreach (var metadata in metadataList)
                {
                    if (metadata is HttpMethodMetadata httpMethodMetadata)
                    {
                        foreach (var method in httpMethodMetadata.HttpMethods)
                            yield return method;
                    }
                }
            }
        }
    }
}
