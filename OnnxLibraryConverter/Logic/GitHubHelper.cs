using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnnxLibraryConverter.Logic
{
    public class GitHubHelper
    {
        public static void CloneGitHubRepo(string repoUrl, string folderName)
        {
            // Get the current directory where the application is running
            string currentDirectory = Directory.GetCurrentDirectory();

            // Define the folder name
            string gitHubFolder = Path.Combine(currentDirectory, folderName);

            // Check if the folder exists
            if (Directory.Exists(gitHubFolder))
            {
                Console.WriteLine("GitHubFiles folder found, attempting to delete it recursively...");
                TryDeleteDirectory(gitHubFolder);
            }

            // Recreate the folder as an empty directory
            Console.WriteLine("Recreating the GitHubFiles folder...");
            Directory.CreateDirectory(gitHubFolder);

            // Clone the repository into the folder
            Console.WriteLine("Cloning the repository into GitHubFiles...");
            try
            {
                Repository.Clone(repoUrl, gitHubFolder);
                Console.WriteLine("Repository cloned successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while cloning the repository: {ex.Message}");
            }
        }

        /// <summary>
        /// Attempts to delete a directory and its contents recursively.
        /// Continues deleting even if some files or directories cannot be deleted due to permission issues.
        /// </summary>
        /// <param name="directoryPath">The directory to delete</param>
        static void TryDeleteDirectory(string directoryPath)
        {
            try
            {
                // Delete all files in the directory
                foreach (string file in Directory.GetFiles(directoryPath))
                {
                    try
                    {
                        File.SetAttributes(file, FileAttributes.Normal); // In case the file is read-only
                        File.Delete(file);
                        Console.WriteLine($"Deleted file: {file}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to delete file {file}: {ex.Message}");
                    }
                }

                // Delete all subdirectories recursively
                foreach (string dir in Directory.GetDirectories(directoryPath))
                {
                    TryDeleteDirectory(dir); // Recursively delete subdirectories
                }

                // Now try to delete the directory itself
                Directory.Delete(directoryPath, false);
                Console.WriteLine($"Deleted directory: {directoryPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete directory {directoryPath}: {ex.Message}");
            }
        }
    }
}
