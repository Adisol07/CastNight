using CastNight;
using System.Reflection;

namespace HelloWorld;

internal class Program
{
    static void Main(string[] args)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        App.Create("HelloWorld", assembly);
        App.MainWindow = new MainWindow();
        App.Run();
    }
}