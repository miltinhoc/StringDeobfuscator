using StringDeobfuscator.Logging;
using StringDeobfuscator.Manager;
using System;
using System.IO;
using System.Linq;

namespace StringDeobfuscator
{
    public class Program
    {
        private static AssemblyManager _assemblyManager;
        private static DeobfuscationManager _deobfuscationManager;

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                ShowHelp();
                Environment.Exit(0);
            }

            Header.Draw();

            string filepath = args[0];

            _assemblyManager = new AssemblyManager(filepath);
            _assemblyManager.Start();

            if (_assemblyManager.GetMethods().Count == 0)
                Environment.Exit(0);

            _deobfuscationManager = new DeobfuscationManager(
                filepath,
                _assemblyManager.GetMethods().First().Type,
                _assemblyManager.GetMethods().First().Name);

            _deobfuscationManager.FindObfuscatedMethods();

            string filename = $"output_{Path.GetFileName(filepath)}";

            _deobfuscationManager.SaveDll(filename);

            Logger.Print($"[*] Saved deobfuscated assembly to '{Path.Combine(Environment.CurrentDirectory, filename)}'", LogType.INFO);

            Console.WriteLine("[*] Press any key to exit...");
            Console.Read();
        }

        private static void ShowHelp()
        {
            string c = @"Usage: StringDeobfuscator.exe [-options]

options:
	<assembly path>		your .net assembly path";

            Console.WriteLine(c);
        }
    }
}
