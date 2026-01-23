using System.Collections.Concurrent;

namespace Rystem.PlayFramework.Mcp.Server;

/// <summary>
/// Registry that tracks all PlayFrameworks exposed as MCP servers.
/// </summary>
public sealed class PlayFrameworkMcpServerRegistry
{
    private readonly ConcurrentDictionary<string, ExposedPlayFrameworkInfo> _exposedFrameworks = new();

    /// <summary>
    /// Registers a PlayFramework to be exposed as an MCP server.
    /// </summary>
    public void Register(ExposedPlayFrameworkInfo info)
    {
        _exposedFrameworks[info.Name] = info;
    }

    /// <summary>
    /// Gets information about an exposed PlayFramework.
    /// </summary>
    public ExposedPlayFrameworkInfo? Get(string name)
    {
        _exposedFrameworks.TryGetValue(name, out var info);
        return info;
    }

    /// <summary>
    /// Gets all exposed PlayFrameworks.
    /// </summary>
    public IEnumerable<ExposedPlayFrameworkInfo> GetAll()
    {
        return _exposedFrameworks.Values;
    }

    /// <summary>
    /// Checks if a PlayFramework is registered.
    /// </summary>
    public bool IsRegistered(string name)
    {
        return _exposedFrameworks.ContainsKey(name);
    }

    /// <summary>
    /// Gets the authorization policy for a PlayFramework.
    /// </summary>
    public string? GetAuthorizationPolicy(string name)
    {
        return Get(name)?.AuthorizationPolicy;
    }
}
