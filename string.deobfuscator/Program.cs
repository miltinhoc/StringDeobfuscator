﻿using StringDeobfuscator.Manager;
using StringDeobfuscator.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StringDeobfuscator
{
    public class Program
    {
        private readonly static string _usage = " [*] Usage: StringDeobfuscator [-options]";
        private static AssemblyManager _assemblyManager;
        private static DeobfuscationManager _deobfuscationManager;

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Logging.Logger.Print(_usage, Logging.LogType.ERROR);
                Environment.Exit(0);
            }

            string filepath = args[0];

            _assemblyManager = new AssemblyManager(filepath);
            _assemblyManager.Start();

            if (_assemblyManager.GetMethods().Count == 0)
            {
                Logging.Logger.Print("[*] No method found", Logging.LogType.WARN);
                Environment.Exit(0);
            }

            List<FoundValue> values = EnumerationManager.Enumerate(_assemblyManager.GetMethods().First());
            _assemblyManager.SetFoundValues(values);

            _deobfuscationManager = new DeobfuscationManager(
                filepath,
                _assemblyManager.GetMethods().First().Type,
                _assemblyManager.GetMethods().First().Name,
                _assemblyManager.GetFoundValues());

            _deobfuscationManager.FindObfuscatedMethods();
            _deobfuscationManager.SaveDll($"output_{Path.GetFileName(filepath)}");
        }
    }
}
