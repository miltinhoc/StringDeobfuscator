# string.deobfuscator

Some .NET assemblies employ a unique form of obfuscation by storing their strings in a Hashtable within the application domain's data. 

This is achieved using the AppDomain.CurrentDomain.GetData method. This approach involves storing and retrieving data using non-descriptive method names and integer keys, which adds a layer of obscurity and makes the code harder to reverse engineer.

I believe this is achieved with some type of obfuscator, as I have seen this in other assemblies, but not sure which one yet.

![image](https://github.com/miltinhoc/StringDeobfuscator/assets/26238419/3e368628-c9db-43cb-9417-c6372f4df0f1)
![image](https://github.com/miltinhoc/StringDeobfuscator/assets/26238419/234cf25b-9a26-4c2c-b669-63b1bfd3ab7c)

This tool is designed to automate the process of extracting and replacing strings from the assemblies. The process is as follows:
1. Scanning the assembly for a method with a specific signature that leverages the ``AppDomain.CurrentDomain.GetData`` method.
2. Once identified, the tool invokes this method iteratively to enumerate and retrieve the stored strings.
3. The tool then replaces the identified method calls in the code with the actual strings retrieved from step 2.
4. It then compiles the modified code and saves the patched assembly to disk.

## Example

Before:
![image](https://github.com/miltinhoc/StringDeobfuscator/assets/26238419/f9cef9c7-aed2-4ff9-b9f8-eca54a101b9f)

After:
![image](https://github.com/miltinhoc/StringDeobfuscator/assets/26238419/7557a779-13cf-43ed-98b7-8655712714a6)

## Todo
1. Instead of brute-forcing a set of possibilities, a better way would be to understand all the numbers used to fetch strings and then only try those. This is possible to implement right now since all the necessary code is kinda there
