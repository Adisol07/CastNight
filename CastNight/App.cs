using System.Reflection;
using CastNight.Controls;

namespace CastNight;

public static class App
{
    public static Assembly? Assembly;
    public static string? NameSpace { get; private set; }
    public static string? Path { get; private set; }
    public static Window? MainWindow { get; set; }

    public static void Create(string nameSpace, Assembly assembly)
    {
        Path = AppDomain.CurrentDomain.BaseDirectory;

        Assembly = assembly;
        NameSpace = nameSpace;
    }

    public static void Run()
    {
        MainWindow?.Show();
    }

    public static void Stop()
    {
        MainWindow?.Close();
    }

    public static string ReadResource(string name)
    {
        string resource = NameSpace + "." + name;
        using (Stream? resource_stream = Assembly?.GetManifestResourceStream(resource))
        {
            if (resource_stream != null)
            {
                using (StreamReader reader = new StreamReader(resource_stream))
                {
                    return reader.ReadToEnd();
                }
            }
            else
            {
                throw new Exception("Resource '" + resource + "' does not exist");
            }
        }
    }
}
