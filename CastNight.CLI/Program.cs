using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CastNight.CLI;

class Program
{
    static string AppPath => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.castnight/";

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        if (args.Length == 0)
        {
            print_usage();
            return;
        }

        parse_args(args);
    }

    static void parse_args(string[] args)
    {
        switch (args[0].ToLower())
        {
            case "create":
                create(args);
                break;

            case "install":
                install();
                break;

            default:
                print_usage();
                break;
        }
    }

    static void print_usage()
    {
        Console.WriteLine("Usage: " + Path.GetFileNameWithoutExtension(Environment.ProcessPath)!.ToLower() + " [options]");
        Console.WriteLine("\nOptions:");
        Console.WriteLine("  create [name] [type] <options> -> creates CastNight project");
        Console.WriteLine("  install                        -> checks integrity and installs CastNight");
    }

    static void create(string[] args)
    {
        DateTime start = DateTime.Now;
        int y = Console.GetCursorPosition().Top;
        Console.Write(" ⏳Checking..");
        int x = Console.GetCursorPosition().Left;
        if (Directory.Exists(args[1]))
        {
            Console.SetCursorPosition(0, y);
            for (int x2 = 0; x2 < Console.BufferWidth - 1; x2++)
                Console.Write(" ");
            Console.SetCursorPosition(0, y);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" ❌Checking: Directory already exists");
            Console.ResetColor();
            return;
        }
        if (args.Length == 3 && args[2].ToLower() != "desktop")
        {
            Console.SetCursorPosition(0, y);
            for (int x2 = 0; x2 < Console.BufferWidth - 1; x2++)
                Console.Write(" ");
            Console.SetCursorPosition(0, y);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" ❌Checking: Invalid project type (Valid options: 'Desktop')");
            Console.ResetColor();
            return;
        }
        Console.SetCursorPosition(0, y);
        for (int x2 = 0; x2 < Console.BufferWidth - 1; x2++)
            Console.Write(" ");
        Console.SetCursorPosition(0, y);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(" ✔ Checked");
        Console.ResetColor();

        Console.Write(" ⏳Creating project..");
        x = Console.GetCursorPosition().Left;
        y = Console.GetCursorPosition().Top;
        bool creation_status = false;
        if (args.Length == 2)
        {
            Console.Write("\n");
            creation_status = create_desktop(args[1]);
        }
        else
        {
            switch (args[2].ToLower())
            {
                case "desktop":
                    Console.Write("\n");
                    creation_status = create_desktop(args[1]);
                    break;
                default:
                    return;
            }
        }
        Console.SetCursorPosition(0, y);
        for (int x2 = 0; x2 < Console.BufferWidth - 1; x2++)
            Console.Write(" ");
        Console.SetCursorPosition(0, y);
        if (creation_status)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" ✔ Created project");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" ❌Creating project");
        }
        Console.ResetColor();
    }

    static bool create_desktop(string name)
    {
        Console.Write("   ⏳Creating dotnet project..");
        int x = Console.GetCursorPosition().Left;
        int y = Console.GetCursorPosition().Top;
        string dotnet_create_output = execute_shell_cmd("dotnet new console -o " + name);
        if (dotnet_create_output.StartsWith("Error: "))
        {
            Console.SetCursorPosition(0, y);
            for (int x2 = 0; x2 < Console.BufferWidth - 1; x2++)
                Console.Write(" ");
            Console.SetCursorPosition(0, y);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("   ❌Creating dotnet project: " + dotnet_create_output.Remove(0, "Error: ".Length));
            Console.ResetColor();
            return false;
        }
        Console.SetCursorPosition(0, y);
        for (int x2 = 0; x2 < Console.BufferWidth - 1; x2++)
            Console.Write(" ");
        Console.SetCursorPosition(0, y);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("   ✔ Created dotnet project");
        Console.ResetColor();

        Console.Write("   ⏳Installing CastNight package..");
        x = Console.GetCursorPosition().Left;
        y = Console.GetCursorPosition().Top;
        string dotnet_package_output = execute_shell_cmd("dotnet add package CastNight");
        if (dotnet_package_output.StartsWith("Error: "))
        {
            Console.SetCursorPosition(0, y);
            for (int x2 = 0; x2 < Console.BufferWidth - 1; x2++)
                Console.Write(" ");
            Console.SetCursorPosition(0, y);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("   ❌Installing CastNight package: " + dotnet_package_output.Remove(0, "Error: ".Length));
            Console.ResetColor();
            return false;
        }
        Console.SetCursorPosition(0, y);
        for (int x2 = 0; x2 < Console.BufferWidth - 1; x2++)
            Console.Write(" ");
        Console.SetCursorPosition(0, y);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("   ✔ Installed CastNight package");
        Console.ResetColor();

        Console.Write("   ⏳Using 'Desktop' template..");
        x = Console.GetCursorPosition().Left;
        y = Console.GetCursorPosition().Top;
        string csproj = File.ReadAllText(name + "/" + name + ".csproj").Trim();
        File.WriteAllText(name + "/" + name + ".csproj", csproj.Replace("</Project>",
        """
          <ItemGroup>
            <EmbeddedResource Include="**\*.cnxml" />
            <EmbeddedResource Include="**\*.cnss" />
          </ItemGroup>
        </Project>
        """));
        string program_cs = File.ReadAllText(AppPath + "/templates/desktop_app/Program.cs").Trim();
        File.WriteAllText(name + "/Program.cs", program_cs.Replace("%project_name%", name));
        string app_cnss = File.ReadAllText(AppPath + "/templates/desktop_app/App.cnss").Trim();
        File.WriteAllText(name + "/App.cnss", app_cnss.Replace("%project_name%", name));
        string mainwindow_cs = File.ReadAllText(AppPath + "/templates/desktop_app/MainWindow.cs").Trim();
        File.WriteAllText(name + "/MainWindow.cs", mainwindow_cs.Replace("%project_name%", name));
        string mainwindow_cnxml = File.ReadAllText(AppPath + "/templates/desktop_app/MainWindow.cnxml").Trim();
        File.WriteAllText(name + "/MainWindow.cnxml", mainwindow_cnxml.Replace("%project_name%", name));
        string mainwindow_cnss = File.ReadAllText(AppPath + "/templates/desktop_app/MainWindow.cnss").Trim();
        File.WriteAllText(name + "/MainWindow.cnss", mainwindow_cnss.Replace("%project_name%", name));

        Console.SetCursorPosition(0, y);
        for (int x2 = 0; x2 < Console.BufferWidth - 1; x2++)
            Console.Write(" ");
        Console.SetCursorPosition(0, y);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("   ✔ Applied 'Desktop' template");
        Console.ResetColor();

        return true;
    }

    static void install()
    {
        DateTime start = DateTime.Now;
        int y = Console.GetCursorPosition().Top;
        Console.Write(" ⏳Checking integrity..");
        int x = Console.GetCursorPosition().Left;
        if (!Directory.Exists(AppPath))
        {
            Console.SetCursorPosition(x - 2, y);
            for (int x2 = 0; x2 < Console.BufferWidth - 1 - x; x2++)
                Console.Write(" ");
            Console.SetCursorPosition(x - 2, y);
            Console.Write(": Creating app path");
            Directory.CreateDirectory(AppPath);
        }
        if (!Directory.Exists(AppPath + "/templates"))
        {
            Console.SetCursorPosition(x - 2, y);
            for (int x2 = 0; x2 < Console.BufferWidth - 1 - x; x2++)
                Console.Write(" ");
            Console.SetCursorPosition(x - 2, y);
            Console.Write(": Creating templates directory");
            Directory.CreateDirectory(AppPath + "/templates");
        }
        Console.SetCursorPosition(0, y);
        for (int x2 = 0; x2 < Console.BufferWidth - 1; x2++)
            Console.Write(" ");
        Console.SetCursorPosition(0, y);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(" ✔ Checked integrity");
        Console.ResetColor();

        Console.Write(" ⏳Checking templates..");
        x = Console.GetCursorPosition().Left;
        y = Console.GetCursorPosition().Top;
        if (!Directory.Exists(AppPath + "/templates/desktop_app"))
        {
            Console.SetCursorPosition(x - 2, y);
            for (int x2 = 0; x2 < Console.BufferWidth - 1 - x; x2++)
                Console.Write(" ");
            Console.SetCursorPosition(x - 2, y);
            Console.Write(": Creating desktop app template");
            Directory.CreateDirectory(AppPath + "/templates/desktop_app");
            File.WriteAllText(AppPath + "/templates/desktop_app/Program.cs",
            """
            using CastNight;
            using CastNight.Controls;
            using System.Reflection;

            namespace %project_name%;

            internal class Program
            {
                static void Main(string[] args)
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    App.Create("%project_name%", assembly);
                    App.MainWindow = (Window)StructureManager.Load<MainWindow>("MainWindow");
                    App.Run();
                }
            }
            """);
            File.WriteAllText(AppPath + "/templates/desktop_app/App.cnss",
            """
            
            """);
            File.WriteAllText(AppPath + "/templates/desktop_app/MainWindow.cs",
            """
            using CastNight;
            using CastNight.Controls;

            namespace %project_name%;

            public class MainWindow : Window
            {

            }
            """);
            File.WriteAllText(AppPath + "/templates/desktop_app/MainWindow.cnxml",
            """
            <MainWindow Title="%project_name% Application"
                    Size="1280 720">
            </MainWindow>
            """);
            File.WriteAllText(AppPath + "/templates/desktop_app/MainWindow.cnss",
            """
            MainWindow {
                BackgroundColor: White;
            }
            """);
        }
        Console.SetCursorPosition(0, y);
        for (int x2 = 0; x2 < Console.BufferWidth - 1; x2++)
            Console.Write(" ");
        Console.SetCursorPosition(0, y);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(" ✔ Checked templates");
        Console.ResetColor();

        Console.Write(" ⏳Checking tool integrity..");
        x = Console.GetCursorPosition().Left;
        y = Console.GetCursorPosition().Top;
        if (!File.Exists(AppPath + "/castnight"))
        {
            Console.SetCursorPosition(x - 2, y);
            for (int x2 = 0; x2 < Console.BufferWidth - 1 - x; x2++)
                Console.Write(" ");
            Console.SetCursorPosition(x - 2, y);
            Console.Write(": Copying CastNight CLI to app path");
            File.Copy(Environment.ProcessPath!, AppPath + "/castnight" + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : ""));
        }
        if (!File.Exists(AppPath + "/CastNight.CLI.dll"))
        {
            Console.SetCursorPosition(x - 2, y);
            for (int x2 = 0; x2 < Console.BufferWidth - 1 - x; x2++)
                Console.Write(" ");
            Console.SetCursorPosition(x - 2, y);
            Console.Write(": Copying CastNight CLI library to app path");
            File.Copy(Path.GetDirectoryName(Environment.ProcessPath!) + "/CastNight.CLI.dll", AppPath + "/CastNight.CLI.dll");
        }
        if (!File.Exists(AppPath + "/CastNight.CLI.runtimeconfig.json"))
        {
            Console.SetCursorPosition(x - 2, y);
            for (int x2 = 0; x2 < Console.BufferWidth - 1 - x; x2++)
                Console.Write(" ");
            Console.SetCursorPosition(x - 2, y);
            Console.Write(": Copying CastNight CLI config to app path");
            File.Copy(Path.GetDirectoryName(Environment.ProcessPath!) + "/CastNight.CLI.runtimeconfig.json", AppPath + "/CastNight.CLI.runtimeconfig.json");
        }
        Console.SetCursorPosition(0, y);
        for (int x2 = 0; x2 < Console.BufferWidth - 1; x2++)
            Console.Write(" ");
        Console.SetCursorPosition(0, y);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(" ✔ Checked tool integrity");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(" 🎉Installation was successfull");
        Console.ResetColor();
        Console.WriteLine($" ({DateTime.Now - start})");
    }

    static string execute_shell_cmd(string command)
    {
        string output = string.Empty;

        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/c " + command;
            }
            else
            {
                startInfo.FileName = "/bin/bash";
                startInfo.Arguments = $"-c \"{command}\"";
            }

            using (Process? process = Process.Start(startInfo))
            {
                output = process!.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
        }
        catch (Exception ex)
        {
            output = $"Error: {ex.Message}";
        }

        return output;
    }
}
