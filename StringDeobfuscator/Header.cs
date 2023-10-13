using System;
using System.Collections.Generic;
using System.Reflection;

namespace StringDeobfuscator
{
    public class Header
    {
        private static readonly Dictionary<string, ConsoleColor> logo = new Dictionary<string, ConsoleColor>();
        private static bool isInit;
        private static readonly ConsoleColor color = ConsoleColor.Yellow;

        public static void Init()
        {
            isInit = true;
            logo.Add(@"", Console.ForegroundColor);
            logo.Add(@"Art by Hayley Jane Wakenshaw", color);
            logo.Add(@"  ooo,    .---.", color);
            logo.Add(@" o`  o   /    |\________________", color);
            logo.Add(@"o`   'oooo()  | ________   _   _)", color);
            logo.Add(@"`oo   o` \    |/        | | | |", color);
            logo.Add(@"  `ooo'   `---'         ""-"" |_|", color);
            logo.Add(@"                                .net", color);
            logo.Add($" [miltinh0c] (v{Assembly.GetExecutingAssembly().GetName().Version})\n", ConsoleColor.White);
        }

        public static void Draw()
        {
            if (!isInit)
            {
                Init();
            }
            Console.BackgroundColor = ConsoleColor.Black;
            ConsoleColor startColor = ConsoleColor.White;

            foreach (KeyValuePair<string, ConsoleColor> keyValue in logo)
            {
                if (Console.ForegroundColor != keyValue.Value)
                    Console.ForegroundColor = keyValue.Value;

                Console.WriteLine(keyValue.Key);
            }

            Console.ForegroundColor = startColor;
        }
    }
}
