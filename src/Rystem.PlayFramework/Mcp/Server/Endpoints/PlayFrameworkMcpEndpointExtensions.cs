using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.PlayFramework.Mcp.Server;

/// <summary>
/// Extension methods for mapping PlayFramework MCP endpoints.
/// </summary>
public static class PlayFrameworkMcpEndpointExtensions
{
    /// <summary>
    /// Maps MCP endpoints for all exposed PlayFrameworks.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <param name="routePrefix">The route prefix for MCP endpoints (default: "/mcp").</param>
    /// <returns>A builder for further configuration.</returns>
    public static PlayFrameworkMcpEndpointBuilder MapPlayFrameworkMcpEndpoints(
        this IEndpointRouteBuilder endpoints,
        string routePrefix = "/mcp")
    {
        var registry = endpoints.ServiceProvider.GetRequiredService<PlayFrameworkMcpServerRegistry>();

        // Map endpoint for each exposed PlayFramework
        foreach (var info in registry.GetAll())
        {
            var route = $"{routePrefix.TrimEnd('/')}/{info.Name}";

            var endpointBuilder = endpoints.MapPost(route, async (HttpContext context) =>
            {
                var router = context.RequestServices.GetRequiredService<McpMethodRouter>();

                McpJsonRpcRequest? request;
                try
                {
                    request = await context.Request.ReadFromJsonAsync<McpJsonRpcRequest>(context.RequestAborted);
                    if (request == null)
                    {
                        var errorResponse = McpJsonRpcResponse.Failure(null, McpJsonRpcErrorCodes.ParseError, "Failed to parse request");
                        return Results.Json(errorResponse);
                    }
                }
                catch (JsonException ex)
                {
                    var errorResponse = McpJsonRpcResponse.Failure(null, McpJsonRpcErrorCodes.ParseError, ex.Message);
                    return Results.Json(errorResponse);
                }

                var response = await router.RouteAsync(info.Name, request, context.RequestAborted);
                return Results.Json(response);
            });

            // Apply authorization if configured
            if (!string.IsNullOrWhiteSpace(info.AuthorizationPolicy))
            {
                endpointBuilder.RequireAuthorization(info.AuthorizationPolicy);
            }
            else
            {
                endpointBuilder.AllowAnonymous();
            }

            endpointBuilder.WithName($"MCP_{info.Name}")
                          .WithDisplayName($"MCP Endpoint for {info.Name}")
                          .WithTags("MCP", "PlayFramework");
        }

        return new PlayFrameworkMcpEndpointBuilder(endpoints, registry);
    }
}

/// <summary>
/// Builder for PlayFramework MCP endpoint configuration.
/// </summary>
public sealed class PlayFrameworkMcpEndpointBuilder
{
    private readonly IEndpointRouteBuilder _endpoints;
    private readonly PlayFrameworkMcpServerRegistry _registry;

    internal PlayFrameworkMcpEndpointBuilder(
        IEndpointRouteBuilder endpoints,
        PlayFrameworkMcpServerRegistry registry)
    {
        _endpoints = endpoints;
        _registry = registry;
    }

    /// <summary>
    /// Gets the endpoint route builder.
    /// </summary>
    public IEndpointRouteBuilder Endpoints => _endpoints;

    /// <summary>
    /// Gets the registry of exposed PlayFrameworks.
    /// </summary>
    public PlayFrameworkMcpServerRegistry Registry => _registry;
}
