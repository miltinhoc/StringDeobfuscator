using dnlib.DotNet;
using dnlib.DotNet.Emit;
using StringDeobfuscator.Logging;
using System;
using System.Reflection;

namespace StringDeobfuscator.Manager
{
    public class DeobfuscationManager
    {
        private readonly string _dllPath;
        private readonly string _whiteListType = "dnlib.DotNet.MethodDefMD";
        private readonly string _deobfuscationTypeName;
        private readonly string _deobfuscationMethodName;
        private ModuleDefMD _module;

        private MethodInfo _methodInfo;
        private Type _type;

        public DeobfuscationManager(string dllPath, string typeName, string methodName)
        {
            _dllPath = dllPath;
            _deobfuscationTypeName = typeName;
            _deobfuscationMethodName = methodName;

            GetReferences();
        }

        private void GetReferences()
        {
            Assembly assembly = Assembly.LoadFile(_dllPath);
            _type = assembly.GetType(_deobfuscationTypeName, throwOnError: true, ignoreCase: false);
            _methodInfo = _type.GetMethod(_deobfuscationMethodName);
        }

        public void FindObfuscatedMethods()
        {
            ModuleContext modCtx = ModuleDef.CreateModuleContext();
            _module = ModuleDefMD.Load(_dllPath, modCtx);

            int count = 0;

            foreach (TypeDef type in _module.GetTypes())
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (method.Body == null) continue;

                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                    {
                        bool isValid = ValidateInstruction(method.Body.Instructions[i]);

                        if (isValid)
                        {
                            string v = FindValue(method.Body.Instructions[i - 1].Operand.ToString());

                            if (string.IsNullOrEmpty(v))
                                continue;

                            count += 1;
                            Instruction newInstruction = new Instruction(OpCodes.Ldstr, v);
                            method.Body.Instructions[i] = newInstruction;

                            Instruction removedInstruction = method.Body.Instructions[i - 1];

                            method.Body.Instructions.RemoveAt(i - 1);

                            UpdateReferences(method.Body, removedInstruction, newInstruction);
                        }
                    }
                }
            }

            Logger.Print($"[*] Patched {count.ToString("N0")} method calls.", LogType.INFO);
        }

        public void SaveDll(string path)
        {
            _module.Write(path);
            _module.Dispose();
        }

        private string FindValue(string key)
        {
            try
            {
                string result = (string)_methodInfo.Invoke(null, new object[] { Convert.ToInt32(key) });

                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
            }
            catch { }

            return string.Empty;
        }

        private void UpdateReferences(CilBody body, Instruction removedInstruction, Instruction newInstruction)
        {
            foreach (Instruction instruction in body.Instructions)
            {
                if (instruction.Operand == removedInstruction)
                {
                    instruction.Operand = newInstruction;
                }
                else if (instruction.Operand is Instruction[] targets)
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        if (targets[i] == removedInstruction)
                        {
                            targets[i] = newInstruction;
                        }
                    }
                }
            }

            foreach (ExceptionHandler eh in body.ExceptionHandlers)
            {
                if (eh.FilterStart == removedInstruction)
                {
                    eh.FilterStart = newInstruction;
                }

                if (eh.HandlerEnd == removedInstruction)
                {
                    eh.HandlerEnd = newInstruction;
                }

                if (eh.HandlerStart == removedInstruction)
                {
                    eh.HandlerStart = newInstruction;
                }

                if (eh.TryEnd == removedInstruction)
                {
                    eh.TryEnd = newInstruction;
                }

                if (eh.TryStart == removedInstruction)
                {
                    eh.TryStart = newInstruction;
                }
            }
        }

        private bool ValidateInstruction(Instruction instruction)
        {
            if (instruction.OpCode != OpCodes.Call) return false;

            if (instruction.Operand == null) return false;

            if (instruction.Operand.GetType().ToString() != _whiteListType) return false;

            IMethod calledMethod = instruction.Operand as IMethod;

            if (calledMethod == null)
                return false;

            if (calledMethod.DeclaringType.FullName != _deobfuscationTypeName) return false;

            if (calledMethod.Name != _deobfuscationMethodName) return false;

            return true;
        }
    }
}
