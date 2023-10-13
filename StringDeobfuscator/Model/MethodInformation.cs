namespace StringDeobfuscator.Model
{
    public class MethodInformation
    {
        public string Name;
        public string Type;
        public string DllPath;

        public MethodInformation(string name, string type, string dllPath)
        {
            Name = name;
            Type = type;
            DllPath = dllPath;
        }
    }
}
