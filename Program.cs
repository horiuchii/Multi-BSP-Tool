using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MultiBSPTool
{
    internal class Program
    {
        static void ThrowError(string message)
        {
            Console.WriteLine("Fatal Error: " + message);
            Console.ReadLine();
            Environment.Exit(0);
        }

        static readonly List<String> allowed_extensions = new List<string> { "mp3", "wav", "vmt", "vtf", "nut", "nuc", "txt", "pcf", "res", "ain", "nav", "vvd", "mdl", "ani", "vtx", "phy", "jpg", "rad", "vbsp" };
        static readonly string filelist_name = Path.GetTempPath() + "\\FileList.txt";

        static string current_bspzip = null;
        static string current_game = null;
        static string current_bsp = null;
        static string output_bsp = null;

        static bool first_operation = true;
        
        static string GetCurrentBSPPath()
        {
            return current_game + "maps\\" + (first_operation ? current_bsp : output_bsp);
        }

        static string GetOutputBSPPath()
        {
            return current_game + "maps\\" + (output_bsp == null ? current_bsp : output_bsp);
        }

        static void RunBSPZIP(string args)
        {
            if (!File.Exists(current_bspzip))
                ThrowError("BSPZIP has not been set or is an invalid directory. Aborting!");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    FileName = current_bspzip,
                    Arguments = args
                }
            };
            process.Start();
            process.WaitForExit();
        }

        static void Main(string[] args)
        {
            string filename = "commands.txt";

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-commands" && i + 1 < args.Length)
                {
                    filename = args[i + 1];
                    break;
                }
            }

            IterateCommandsFile(filename);
            Console.WriteLine("Finished executing commands.");
            Console.ReadLine();
            Environment.Exit(0);
        }

        static void IterateCommandsFile(string filename)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string command;
                    while ((command = reader.ReadLine()) != null)
                    {
                        // Ignore comments
                        if (command.TrimStart().StartsWith("#") || command.TrimStart().StartsWith("//"))
                            continue;

                        string[] parts = command.Split(new char[] { '=' }, 2);

                        if (parts.Length != 2)
                            continue;

                        HandleCommand(parts[0].Trim(), parts[1].Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                ThrowError($"Error parsing commands file: {ex.Message}");
            }
        }

        static void HandleCommand(string key, string value)
        {
            switch(key)
            {
                case "bspzip": current_bspzip = value.Replace('/', Path.DirectorySeparatorChar); break;
                case "game":
                    {
                        string game_path = value.Replace('/', Path.DirectorySeparatorChar);
                        
                        current_game = game_path.EndsWith("\\") ? game_path : game_path + Path.DirectorySeparatorChar;
                        break;
                    }
                case "map": current_bsp = value + ".bsp"; output_bsp = null; first_operation = true; break;
                case "new_map": output_bsp = value + ".bsp"; break;

                case "folder": PackFolder(value); break;
                case "custom_folder": PackFolder(current_game + "custom\\" + value); break;

                case "repack": RepackBSP(bool.Parse(value)); break;

                default:
                    Console.WriteLine($"Found Unsupported Command! key: {key}, value: {value}");
                    break;
            }
        }

        static void PackFolder(string path)
        {
            if (!Directory.Exists(current_game))
                ThrowError("Game Directory has not been set or is an invalid directory. Aborting!");

            if (!File.Exists(GetCurrentBSPPath()))
                ThrowError($"Map has not been set or is an invalid directory. Aborting! Map: {GetCurrentBSPPath()}");

            List<string> lines = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(path);

            //create filelist.txt for bspzip
            foreach (string extension in allowed_extensions)
            {
                foreach (FileInfo file in dir.GetFiles($"*.{extension}", SearchOption.AllDirectories))
                {
                    string relativePath = file.FullName.Substring(path.Length).TrimStart(Path.DirectorySeparatorChar);
                    lines.Add(relativePath.Replace(Path.DirectorySeparatorChar, '/'));
                    lines.Add(file.FullName);
                }
            }

            File.WriteAllLines(filelist_name, lines);

            RunBSPZIP($"-addlist \"{GetCurrentBSPPath()}\"" + " " + $"\"{filelist_name}\"" + " " + $"\"{GetOutputBSPPath()}\"");
            File.Delete(filelist_name);
            first_operation = false;
        }

        static void RepackBSP(bool compress)
        {
            if (!Directory.Exists(current_game))
                ThrowError("Game Directory has not been set or is an invalid directory. Aborting!");

            if (!File.Exists(GetCurrentBSPPath()))
                ThrowError($"Map has not been set or is an invalid directory. Aborting! Map: {GetCurrentBSPPath()}");

            if (first_operation && output_bsp != null)
            {
                first_operation = false;
                File.WriteAllBytes(GetOutputBSPPath(), File.ReadAllBytes(current_game + "maps\\" + current_bsp));
            }

            RunBSPZIP($"-repack" + (compress ? " -compress " : " ") + $"\"{GetCurrentBSPPath()}\"");
            first_operation = false;
        }
    }
}
