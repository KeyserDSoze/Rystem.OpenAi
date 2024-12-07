using System.Text.RegularExpressions;

namespace Rystem.PlayFramework
{
    public interface IScenePathBuilder
    {
        IScenePathBuilder Map(Regex regex);
        IScenePathBuilder Map(string startsWith);
    }
}
