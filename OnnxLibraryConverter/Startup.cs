using OnnxLibraryConverter.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnnxLibraryConverter
{
    internal class Startup
    {
        public async Task Main()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Be sure that Git LFS is installed or this will fail!");
            Console.ResetColor();

            string version = "0.4.0";
            new NugetHelper().GetOriginalOnnx(version);
            new RuntimeBuilder().BuildRuntimeFolders();
            new RuntimeBuilder().AddRuntimeToOutputLocation("cpu", "MagicOnnxRuntimeGenAi.Cpu");
            new RuntimeBuilder().AddRuntimeToOutputLocation("cuda", "MagicOnnxRuntimeGenAi.Cuda");
            new RuntimeBuilder().AddRuntimeToOutputLocation("dml", "MagicOnnxRuntimeGenAi.DirectML");

            //string nativeMethodsOriginal = File.ReadAllText(@"C:\Source\MagicOnnxRuntimeGenAi\OnnxLibraryConverter\NativeMethods.txt");

            string genAiFolderGithub = "GitHubGenAi";
            GitHubHelper.CloneGitHubRepo("https://github.com/microsoft/onnxruntime-genai", genAiFolderGithub);

            string genAiFullGithubFolderPath = Path.Combine(Directory.GetCurrentDirectory(), genAiFolderGithub);

            string genAiNativeMethodsPath = Path.Combine(genAiFullGithubFolderPath, "src", "csharp", "NativeMethods.cs");
            string nativeMethodsOriginal = File.ReadAllText(genAiNativeMethodsPath);

            //string nativeMethodsOriginal = File.ReadAllText(@"C:\Source\MagicOnnxRuntimeGenAi\OnnxLibraryConverter\OnnxNativeMethods.txt");

            string VersionComment = $"//GenAI Nuget Version: {version}";
            string MagicNativeMethods = new CodeTransformer().TransformDllImports(nativeMethodsOriginal);
            MagicNativeMethods = VersionComment + Environment.NewLine + MagicNativeMethods;

            string fileReplace = @$"
using System;

namespace MagicOnnxRuntimeGenAi
{{
{MagicNativeMethods}
}}";

            UpdateMagicNativeMethods(fileReplace);

        }

        private void UpdateMagicNativeMethods(string newContent)
        {
            // Start at the current directory
            string currentDirectory = Directory.GetCurrentDirectory();

            // Go up 4 folder levels
            for (int i = 0; i < 4; i++)
            {
                currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
                if (currentDirectory == null)
                {
                    throw new DirectoryNotFoundException("Unable to find the parent directory 4 levels up.");
                }
            }

            // Now find the "MagicOnnxRuntimeGenAi" folder
            string magicFolder = Path.Combine(currentDirectory, "MagicOnnxRuntimeGenAi");

            if (!Directory.Exists(magicFolder))
            {
                throw new DirectoryNotFoundException($"MagicOnnxRuntimeGenAi folder not found at: {magicFolder}");
            }

            // Find the "Helpers" folder inside "MagicOnnxRuntimeGenAi"
            string helpersFolder = Path.Combine(magicFolder, "Helpers");

            if (!Directory.Exists(helpersFolder))
            {
                throw new DirectoryNotFoundException($"Helpers folder not found in: {magicFolder}");
            }

            // Find the "MagicNativeMethods.cs" file inside the "Helpers" folder
            string filePath = Path.Combine(helpersFolder, "MagicNativeMethods.cs");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"MagicNativeMethods.cs not found in: {helpersFolder}");
            }

            // Replace the content of the file with the new content
            File.WriteAllText(filePath, newContent);

            Console.WriteLine($"Successfully updated {filePath}");
        }
    }
}
