using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnnxLibraryConverter.Logic
{
    public class NugetHelper
    {
        private string outputDirectory;
        private const string versionFileName = "version.txt";

        public NugetHelper()
        {
            this.outputDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public void GetOriginalOnnx(string version)
        {
            // Define the folder names that need to be cleaned and recreated
            string[] folders = { "cuda_original_nuget", "dml_original_nuget", "cpu_original_nuget" };

            // Loop through each folder, check if the version is already present
            foreach (var folder in folders)
            {
                string fullFolderPath = Path.Combine(outputDirectory, folder);
                string versionFilePath = Path.Combine(fullFolderPath, versionFileName);

                // Check if the folder exists and contains the correct version
                if (Directory.Exists(fullFolderPath) && File.Exists(versionFilePath))
                {
                    string existingVersion = File.ReadAllText(versionFilePath);

                    if (existingVersion == version)
                    {
                        Console.WriteLine($"Folder '{folder}' is already up-to-date with version {version}. Skipping download.");
                        continue; // Skip this folder as it's already up-to-date
                    }
                    else
                    {
                        // The folder exists but the version is outdated; we need to delete and recreate
                        Console.WriteLine($"Folder '{folder}' contains an outdated version. Cleaning up.");
                        DeleteDirectoryContents(fullFolderPath);
                        Directory.Delete(fullFolderPath);
                    }
                }

                // Recreate the folder
                Directory.CreateDirectory(fullFolderPath);
            }

            // Full path to nuget.exe within the output directory
            string nugetExePath = Path.Combine(outputDirectory, "nuget.exe");

            // Install the packages into their respective folders
            if (!CheckAndInstallNugetPackage(nugetExePath, "Microsoft.ML.OnnxRuntimeGenAI", version, Path.Combine(outputDirectory, "cpu_original_nuget")))
                return;

            if (!CheckAndInstallNugetPackage(nugetExePath, "Microsoft.ML.OnnxRuntimeGenAI.Cuda", version, Path.Combine(outputDirectory, "cuda_original_nuget")))
                return;

            if (!CheckAndInstallNugetPackage(nugetExePath, "Microsoft.ML.OnnxRuntimeGenAI.DirectML", version, Path.Combine(outputDirectory, "dml_original_nuget")))
                return;
        }

        private void DeleteDirectoryContents(string folderPath)
        {
            // Recursively delete all contents of the folder
            DirectoryInfo di = new DirectoryInfo(folderPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete(); // Delete each file
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true); // Recursively delete subdirectories
            }
        }

        private bool CheckAndInstallNugetPackage(string nugetExePath, string packageName, string version, string outputPath)
        {
            try
            {
                // Construct the command line arguments for nuget.exe
                string arguments = $"install {packageName} -Version {version} -OutputDirectory \"{outputPath}\"";

                // Set up the process info for nuget.exe execution
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = nugetExePath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Start the process and capture real-time output
                using (Process process = new Process())
                {
                    process.StartInfo = processInfo;

                    // Event handlers for real-time output to the console
                    process.OutputDataReceived += (sender, e) => { if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine(e.Data); };
                    process.ErrorDataReceived += (sender, e) => { if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine("ERROR: " + e.Data); };

                    process.Start();

                    // Start reading the output streams asynchronously
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    // Wait for the process to exit
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        Console.WriteLine($"Error installing package {packageName}. Process exited with code {process.ExitCode}.");
                        return false; // Return false if the installation failed
                    }
                    else
                    {
                        Console.WriteLine($"Successfully installed {packageName} version {version} in {outputPath}");

                        // Write the version number to the version.txt file in the output folder
                        string versionFilePath = Path.Combine(outputPath, versionFileName);
                        File.WriteAllText(versionFilePath, version); // Save the version number
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to install package {packageName}: {ex.Message}");
                return false;
            }

            return true; // Return true if everything was successful
        }
    }
}
