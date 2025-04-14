using System.Reflection;

namespace LearningSemanticKernel.Helpers;

public static class DirectoryHandler
{
    public static string GetPluginDirectory()
    {
        return Path.Combine(GetRootPath(), "Plugins");
    }

    public static string GetDataDirectory()
    {
        return Path.Combine(GetRootPath(), "Data");
    }

    private static string GetRootPath()
    {
        string assemblyPath = Assembly.GetExecutingAssembly().Location;
        string projectDirectory = Path.GetDirectoryName(assemblyPath);
        return Directory.GetParent(projectDirectory).Parent.Parent.FullName;
    }
}
