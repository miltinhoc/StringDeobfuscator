using StringDeobfuscator.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace StringDeobfuscator.Manager
{
    public class EnumerationManager
    {
        public static List<FoundValue> Enumerate(MethodInformation methodInformation)
        {
            if (!File.Exists(methodInformation.DllPath))
                return new List<FoundValue>();

            Assembly assembly = Assembly.LoadFrom(methodInformation.DllPath);

            Type type = assembly.GetType(methodInformation.Type, throwOnError: true, ignoreCase: false);
            MethodInfo method = type.GetMethod(methodInformation.Name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            return Bruteforce(ref method);
        }

        static List<FoundValue> Bruteforce(ref MethodInfo methodInfo)
        {
            List<FoundValue> values = new List<FoundValue>();

            for (int i = 0; i < 1000000; i++)
            {
                try
                {
                    string result = (string)methodInfo.Invoke(null, new object[] { i });

                    if (!string.IsNullOrEmpty(result))
                    {
                        values.Add(new FoundValue(i, result));
                    }
                }
                catch { }
            }
            return values;
        }
    }
}
