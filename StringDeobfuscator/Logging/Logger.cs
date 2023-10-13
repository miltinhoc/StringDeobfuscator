using System;
using System.Runtime.CompilerServices;

namespace StringDeobfuscator.Logging
{
    public class Logger
    {
        public static void Print(string message, LogType type, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            Console.Write($" {DateTime.Now}");

            DrawWithColor($" [{type}]", TypeColor(type));
            DrawWithColor($" [{callerName}:{lineNumber}]", ConsoleColor.Magenta);

            Console.Write($" {message}\n");
        }

        private static void DrawWithColor(string text, ConsoleColor color)
        {
            ConsoleColor current = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.Write(text);
            Console.ForegroundColor = current;
        }

        private static ConsoleColor TypeColor(LogType type)
        {
            switch (type)
            {
                case LogType.INFO: return ConsoleColor.Cyan;
                case LogType.WARN: return ConsoleColor.Yellow;
                case LogType.ERROR: return ConsoleColor.Red;
                case LogType.FATAL: return ConsoleColor.DarkRed;
                default: return ConsoleColor.White;
            }
        }
    }

    public enum LogType
    {
        INFO,
        WARN,
        ERROR,
        FATAL
    }
}
