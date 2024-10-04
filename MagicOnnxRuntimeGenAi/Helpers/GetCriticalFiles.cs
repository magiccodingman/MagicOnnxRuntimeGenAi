using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MagicOnnxRuntimeGenAi.Helpers
{
    public class GetCriticalFiles
    {
        private static bool _cudaFilesDownloaded = false; // Flag for successful download
        private static readonly object _lock = new object(); // Lock for synchronization
        private static TaskCompletionSource<bool> _downloadTaskCompletionSource = null; // Sync across threads

        public void DownloadCriticalCudaFiles()
        {
            // Early exit if download already completed (outside lock for efficiency)
            if (_cudaFilesDownloaded) return;

            lock (_lock) // Synchronize the start of the download process
            {
                // Double-check after acquiring lock
                if (_cudaFilesDownloaded) return;

                // If no download is in progress, create a new TaskCompletionSource to track this download
                if (_downloadTaskCompletionSource == null)
                {
                    _downloadTaskCompletionSource = new TaskCompletionSource<bool>();

                    // Start the download in the current thread
                    try
                    {
                        // Detect if it's Windows or Linux
                        bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                        // Construct the folder paths based on the OS
                        string targetDirectory;
                        string downloadUrl;
                        string fileName;

                        if (isWindows)
                        {
                            targetDirectory = Path.Combine(baseDirectory, "cuda", "runtimes", "win-x64", "native");
                            downloadUrl = "https://huggingface.co/datasets/magiccodingman/MagicOnnxRuntimeGenAI/resolve/main/cuda_providers/0.4.0/onnxruntime_providers_cuda.dll";
                            fileName = "onnxruntime_providers_cuda.dll";
                        }
                        else
                        {
                            targetDirectory = Path.Combine(baseDirectory, "cuda", "runtimes", "linux-x64", "native");
                            downloadUrl = "https://huggingface.co/datasets/magiccodingman/MagicOnnxRuntimeGenAI/resolve/main/cuda_providers/0.4.0/libonnxruntime_providers_cuda.so";
                            fileName = "libonnxruntime_providers_cuda.so";
                        }

                        // Ensure the directory exists or create it
                        if (!Directory.Exists(targetDirectory))
                        {
                            Directory.CreateDirectory(targetDirectory);
                        }

                        // Full path to the file
                        string filePath = Path.Combine(targetDirectory, fileName);

                        // Download the file if it doesn't exist or is incomplete
                        if (!File.Exists(filePath) || !IsFileComplete(filePath, downloadUrl))
                        {
                            using (var client = new WebClient())
                            {
                                client.DownloadFile(downloadUrl, filePath);
                            }
                        }

                        // Mark download as successful and update state
                        _cudaFilesDownloaded = true;
                        _downloadTaskCompletionSource.SetResult(true); // Notify waiting threads of success
                    }
                    catch (Exception ex)
                    {
                        _downloadTaskCompletionSource.SetException(ex); // Notify waiting threads of failure
                        throw;
                    }
                }
            }

            // Wait for the download to complete if it was already in progress by another thread
            if (!_cudaFilesDownloaded)
            {
                _downloadTaskCompletionSource.Task.Wait();
            }
        }

        // Check if the file is completely downloaded and valid
        private bool IsFileComplete(string filePath, string downloadUrl)
        {
            // Check if the file exists on disk
            if (!File.Exists(filePath))
            {
                return false; // File does not exist
            }

            // Get the size of the local file
            FileInfo fileInfo = new FileInfo(filePath);
            long localFileSize = fileInfo.Length;

            try
            {
                // Make a HEAD request to the download URL to get the file size
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(downloadUrl);
                request.Method = "HEAD"; // Only fetch headers, not the entire file
                request.AllowAutoRedirect = true; // Follow redirects

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Extract the content length (file size) from the headers
                    long remoteFileSize = response.ContentLength;

                    // Compare local file size with the remote file size
                    return localFileSize == remoteFileSize;
                }
            }
            catch (Exception ex)
            {
                // In case of any error, consider the file incomplete
                Console.WriteLine($"Error checking file completeness: {ex.Message}");
                return false;
            }
        }

    }

}
