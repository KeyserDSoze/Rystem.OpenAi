using System.Text;

namespace Rystem.PlayFramework.Mcp.Server;

/// <summary>
/// Builds documentation for PlayFramework scenes to be exposed as MCP resources.
/// </summary>
public sealed class PlayFrameworkDocumentationBuilder
{
    /// <summary>
    /// Generates markdown documentation for a scene.
    /// </summary>
    public string BuildSceneDocumentation(SceneDocumentation scene)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"# {scene.SceneName}");
        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(scene.Description))
        {
            sb.AppendLine("## Description");
            sb.AppendLine(scene.Description);
            sb.AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(scene.SystemMessage))
        {
            sb.AppendLine("## System Behavior");
            sb.AppendLine(scene.SystemMessage);
            sb.AppendLine();
        }

        if (scene.AvailableTools.Count > 0)
        {
            sb.AppendLine("## Available Tools");
            foreach (var tool in scene.AvailableTools)
            {
                sb.AppendLine($"- `{tool}`");
            }
            sb.AppendLine();
        }

        if (scene.AvailableActors.Count > 0)
        {
            sb.AppendLine("## Available Actors");
            foreach (var actor in scene.AvailableActors)
            {
                sb.AppendLine($"- `{actor}`");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    /// <summary>
    /// Generates an overview documentation for the entire PlayFramework.
    /// </summary>
    public string BuildOverviewDocumentation(ExposedPlayFrameworkInfo info)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"# {info.Name} - PlayFramework Overview");
        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(info.Description))
        {
            sb.AppendLine("## Description");
            sb.AppendLine(info.Description);
            sb.AppendLine();
        }

        sb.AppendLine("## Available Scenes");
        sb.AppendLine();

        foreach (var scene in info.SceneDocumentations)
        {
            sb.AppendLine($"### {scene.SceneName}");
            if (!string.IsNullOrWhiteSpace(scene.Description))
            {
                sb.AppendLine(scene.Description);
            }
            sb.AppendLine();
        }

        sb.AppendLine("## Usage");
        sb.AppendLine();
        sb.AppendLine("Send a message to this PlayFramework and it will automatically route to the appropriate scene based on the content.");
        sb.AppendLine();
        sb.AppendLine("### Example");
        sb.AppendLine("```");
        sb.AppendLine($"tools/call with name=\"{info.Name}\" and arguments={{\"message\": \"your request here\"}}");
        sb.AppendLine("```");

        return sb.ToString();
    }
}
