using Mono.Cecil;
using Mono.Cecil.Cil;
using StringDeobfuscator.Logging;
using StringDeobfuscator.Model;
using System.Collections.Generic;

namespace StringDeobfuscator.Manager
{
    public class AssemblyManager
    {
        private readonly string _dllPath;
        private readonly List<MethodInformation> _methods;
        private List<FoundValue> _foundValues;
        private bool _found;

        public AssemblyManager(string dllPath)
        {
            _dllPath = dllPath;
            _methods = new List<MethodInformation>();
        }

        public void SetFoundValues(List<FoundValue> values) => _foundValues = values;

        public List<FoundValue> GetFoundValues() => _foundValues;

        public void Start()
        {
            Logger.Print("[*] trying to find obfuscation method...", LogType.INFO);

            SearchDLLForReference(_dllPath);
            _found = (_methods.Count > 0) ? true : false;

            Logger.Print($"[*] obfuscation method {((_found) ? "found!" : "not found!")}", LogType.INFO);
        }

        public bool Found() => _found;

        public List<MethodInformation> GetMethods() => _methods;

        private bool IsTargetMethod(MethodDefinition method)
        {
            if (method.Parameters.Count != 1)
                return false;

            if (method.Parameters[0].ParameterType.FullName != "System.Int32")
                return false;

            if (method.ReturnType.FullName != "System.String")
                return false;

            return true;
        }

        public void SearchDLLForReference(string pathToDll)
        {
            var module = ModuleDefinition.ReadModule(pathToDll);

            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.HasBody)
                    {
                        if (!IsTargetMethod(method)) continue;

                        foreach (var instruction in method.Body.Instructions)
                        {
                            if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt)
                            {
                                var methodReference = instruction.Operand as MethodReference;
                                if (methodReference != null)
                                {
                                    if (methodReference.DeclaringType.FullName == "System.AppDomain" && methodReference.Name == "GetData")
                                    {
                                        module.Dispose();
                                        _methods.Add(new MethodInformation(method.Name, type.FullName, pathToDll));
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            module.Dispose();
        }
    }
}
