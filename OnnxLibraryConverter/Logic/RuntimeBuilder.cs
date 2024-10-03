using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnnxLibraryConverter.Logic
{
    public class RuntimeBuilder
    {
        private string outputDirectory;

        public RuntimeBuilder()
        {
            // Get the output directory based on the current AppDomain base directory
            this.outputDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public void AddRuntimeToOutputLocation(string folderName, string GenAiFoldername)
        {
            // Step 1: Start from the current directory and go up 4 levels
            string currentDir = Directory.GetCurrentDirectory();
            string targetDir = currentDir;
            for (int i = 0; i < 4; i++)
            {
                targetDir = Directory.GetParent(targetDir)?.FullName;
                if (targetDir == null)
                {
                    Console.WriteLine("Unable to navigate up 4 levels from the current directory.");
                    return;
                }
            }

            // Step 2: Find the "MagicOnnxRuntimeGenAi" folder
            string magicOnnxDir = Path.Combine(targetDir, GenAiFoldername);
            if (!Directory.Exists(magicOnnxDir))
            {
                Console.WriteLine($"The directory '{GenAiFoldername}' could not be found at: {magicOnnxDir}");
                return;
            }

            // Step 3: Find the "cpu" folder inside "MagicOnnxRuntimeGenAi"
            string outputCpuDir = Path.Combine(magicOnnxDir, folderName);
            if (!Directory.Exists(outputCpuDir))
            {
                Console.WriteLine($"The {folderName} directory could not be found at: {outputCpuDir}");
                return;
            }

            // Step 4: Recursively delete everything inside the "cpu" folder but not the folder itself
            DirectoryInfo cpuDirInfo = new DirectoryInfo(outputCpuDir);
            foreach (FileInfo file in cpuDirInfo.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in cpuDirInfo.GetDirectories())
            {
                dir.Delete(true); // true means recursive delete
            }

            // Step 5: Copy the "runtimes" folder from "end_dlls/cpu" to the output CPU folder
            string sourceCpuDir = Path.Combine(currentDir, "end_dlls", folderName, "runtimes");
            if (!Directory.Exists(sourceCpuDir))
            {
                Console.WriteLine($"The source runtimes directory could not be found at: {sourceCpuDir}");
                return;
            }

            string destinationRuntimesDir = Path.Combine(outputCpuDir, "runtimes");
            CopyDirectory(sourceCpuDir, destinationRuntimesDir);

            Console.WriteLine("Operation completed successfully.");
        }

        // Helper method to copy a directory and its contents recursively
    static void CopyDirectory(string sourceDir, string destinationDir)
    {
        Directory.CreateDirectory(destinationDir);

        // Copy all files
        foreach (string filePath in Directory.GetFiles(sourceDir))
        {
            string destFile = Path.Combine(destinationDir, Path.GetFileName(filePath));
            File.Copy(filePath, destFile, true);
        }

        // Copy all subdirectories
        foreach (string dirPath in Directory.GetDirectories(sourceDir))
        {
            string destDir = Path.Combine(destinationDir, Path.GetFileName(dirPath));
            CopyDirectory(dirPath, destDir);
        }
    }

        public void BuildRuntimeFolders()
        {
            // Define the end_dlls directory
            string endDllsPath = Path.Combine(outputDirectory, "end_dlls");

            // If end_dlls exists, delete it and recreate the structure
            if (Directory.Exists(endDllsPath))
            {
                DeleteDirectoryContents(endDllsPath);
                Directory.Delete(endDllsPath);
            }

            Directory.CreateDirectory(endDllsPath);
            Directory.CreateDirectory(Path.Combine(endDllsPath, "cpu"));
            Directory.CreateDirectory(Path.Combine(endDllsPath, "cuda"));
            Directory.CreateDirectory(Path.Combine(endDllsPath, "dml"));

            // Define the original nuget directories
            string[] originalNugetFolders = { "cuda_original_nuget", "dml_original_nuget", "cpu_original_nuget" };
            string[] targetFolders = { "cuda", "dml", "cpu" };

            // Iterate over each of the original nuget folders
            for (int i = 0; i < originalNugetFolders.Length; i++)
            {
                string originalNugetFolder = Path.Combine(outputDirectory, originalNugetFolders[i]);
                string targetFolder = Path.Combine(endDllsPath, targetFolders[i]);

                if (Directory.Exists(originalNugetFolder))
                {
                    // Copy the runtime folders
                    CopyRuntimeFolders(originalNugetFolder, targetFolder);

                    // Copy the Microsoft.ML.OnnxRuntime.Managed lib folder
                    //CopyManagedLibFolder(originalNugetFolder, targetFolder, "Microsoft.ML.OnnxRuntime.Managed");

                    // Copy and merge the Microsoft.ML.OnnxRuntimeGenAI.Managed lib folder
                    //MergeGenAILibFolder(originalNugetFolder, targetFolder, "Microsoft.ML.OnnxRuntimeGenAI.Managed");
                }
            }
        }

        private void RenameFolder(string targetDirectory)
        {
            try
            {
                // Path to the folder you want to rename
                string oldFolderPath = Path.Combine(targetDirectory, "net6.0");
                string newFolderPath = Path.Combine(targetDirectory, "net8.0");

                // Check if "net6.0" folder exists
                if (Directory.Exists(oldFolderPath))
                {
                    // Ensure "net8.0" folder doesn't already exist
                    if (!Directory.Exists(newFolderPath))
                    {
                        // Rename the folder
                        Directory.Move(oldFolderPath, newFolderPath);
                        Console.WriteLine($"Folder renamed from 'net6.0' to 'net8.0' successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Folder 'net8.0' already exists.");
                    }
                }
                else
                {
                    Console.WriteLine($"Folder 'net6.0' not found in the target directory.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void DeleteDirectoryContents(string folderPath)
        {
            DirectoryInfo di = new DirectoryInfo(folderPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private void CopyRuntimeFolders(string originalNugetFolder, string targetFolder)
        {
            // Look for folders starting with Microsoft.ML.OnnxRuntime and containing a "runtime" folder
            foreach (var subDir in Directory.GetDirectories(originalNugetFolder, "Microsoft.ML.OnnxRuntime*"))
            {
                string runtimePath = Path.Combine(subDir, "runtimes");

                if (Directory.Exists(runtimePath))
                {
                    // Copy the runtime folder to the respective target folder
                    string destinationRuntimePath = Path.Combine(targetFolder, "runtimes");
                    DirectoryCopy(runtimePath, destinationRuntimePath, true);
                }
            }
        }

        private void CopyManagedLibFolder(string originalNugetFolder, string targetFolder, string packageName)
        {
            // Look for the Microsoft.ML.OnnxRuntime.Managed folder
            foreach (var subDir in Directory.GetDirectories(originalNugetFolder, $"{packageName}*"))
            {
                string libPath = Path.Combine(subDir, "lib");

                if (Directory.Exists(libPath))
                {
                    // Copy the entire lib folder to the target folder
                    DirectoryCopy(libPath, targetFolder, true);
                }
            }
        }

        /*private void MergeGenAILibFolder(string originalNugetFolder, string targetFolder, string packageName)
        {
            RenameFolder(targetFolder);
            // Look for the Microsoft.ML.OnnxRuntimeGenAI.Managed folder
            foreach (var subDir in Directory.GetDirectories(originalNugetFolder, $"{packageName}*"))
            {
                string genAILibPath = Path.Combine(subDir, "lib");

                if (Directory.Exists(genAILibPath))
                {
                    // Get all subfolders in the GenAI lib folder
                    foreach (var genAISubFolder in Directory.GetDirectories(genAILibPath))
                    {
                        string genAISubFolderName = Path.GetFileName(genAISubFolder);

                        // Check if a folder with the same name exists in the target folder
                        string correspondingTargetSubFolder = Path.Combine(targetFolder, genAISubFolderName);
                        if (Directory.Exists(correspondingTargetSubFolder))
                        {
                            // Merge the contents of the GenAI lib folder into the corresponding folder in the target
                            DirectoryCopy(genAISubFolder, correspondingTargetSubFolder, true);
                        }
                    }
                }
            }
        }*/

        // Helper method to copy directories
        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {sourceDirName}");
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, true);
            }

            // If copying subdirectories, copy them and their contents to new location
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
    }
}
