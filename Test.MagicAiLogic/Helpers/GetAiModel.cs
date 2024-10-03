using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Test.MagicAiLogic.Helpers
{
    public class HuggingFaceDownloader
    {
        public static async Task<string> GetPhi3MiniDml()
        {
            // Define the base folder
            string baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ai_Models", "dml");

            // Ensure the directory exists
            EnsureDirectoryExists(baseDirectory);

            // List of URLs to download
            string[] urls = new string[]
            {
                "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/directml/directml-int4-awq-block-128/added_tokens.json",
                "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/directml/directml-int4-awq-block-128/config.json",
                "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/directml/directml-int4-awq-block-128/configuration_phi3.py",
                "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/directml/directml-int4-awq-block-128/genai_config.json",
                "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/directml/directml-int4-awq-block-128/model.onnx",
                "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/directml/directml-int4-awq-block-128/model.onnx.data",
                "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/directml/directml-int4-awq-block-128/special_tokens_map.json",
                "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/directml/directml-int4-awq-block-128/tokenizer.json",
                "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/directml/directml-int4-awq-block-128/tokenizer.model",
                "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/directml/directml-int4-awq-block-128/tokenizer_config.json"
            };

            // Download each file to the specified folder
            foreach (var url in urls)
            {
                await DownloadFileIfNotExists(url, baseDirectory);
            }

            return baseDirectory;
        }

        public static async Task<string> GetPhi3MiniCPU()
        {
            // Define the base folder
            string baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ai_Models", "cpu");

            // Ensure the directory exists
            EnsureDirectoryExists(baseDirectory);

            // List of URLs to download
            string[] urls = new string[]
{
    "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/cpu_and_mobile/cpu-int4-rtn-block-32-acc-level-4/added_tokens.json",
    "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/cpu_and_mobile/cpu-int4-rtn-block-32-acc-level-4/config.json",
    "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/cpu_and_mobile/cpu-int4-rtn-block-32-acc-level-4/configuration_phi3.py",
    "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/cpu_and_mobile/cpu-int4-rtn-block-32-acc-level-4/genai_config.json",
    "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/cpu_and_mobile/cpu-int4-rtn-block-32-acc-level-4/phi3-mini-4k-instruct-cpu-int4-rtn-block-32-acc-level-4.onnx",
    "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/cpu_and_mobile/cpu-int4-rtn-block-32-acc-level-4/phi3-mini-4k-instruct-cpu-int4-rtn-block-32-acc-level-4.onnx.data",
    "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/cpu_and_mobile/cpu-int4-rtn-block-32-acc-level-4/special_tokens_map.json",
    "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/cpu_and_mobile/cpu-int4-rtn-block-32-acc-level-4/tokenizer.json",
    "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/cpu_and_mobile/cpu-int4-rtn-block-32-acc-level-4/tokenizer.model",
    "https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/resolve/main/cpu_and_mobile/cpu-int4-rtn-block-32-acc-level-4/tokenizer_config.json"
};

            // Download each file to the specified folder
            foreach (var url in urls)
            {
                await DownloadFileIfNotExists(url, baseDirectory);
            }

            return baseDirectory;
        }

        // Method to ensure the directory exists
        static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Console.WriteLine($"Created directory: {path}");
            }
        }

        // Method to download the file if it doesn't exist or if it's incomplete
        static async Task DownloadFileIfNotExists(string url, string outputDirectory)
        {
            string fileName = Path.GetFileName(new Uri(url).LocalPath);
            string outputPath = Path.Combine(outputDirectory, fileName);

            // Check if the file exists
            if (File.Exists(outputPath))
            {
                Console.WriteLine($"File {fileName} already exists. Checking size...");

                // Check if the file size matches the remote file size
                long localFileSize = new FileInfo(outputPath).Length;
                long remoteFileSize = await GetRemoteFileSize(url);

                if (localFileSize == remoteFileSize)
                {
                    Console.WriteLine($"File {fileName} is already fully downloaded. Skipping download.");
                    return;
                }
                else
                {
                    Console.WriteLine($"File {fileName} is incomplete (local size: {localFileSize}, remote size: {remoteFileSize}). Downloading again.");
                }
            }

            // Download the file by streaming
            await DownloadLargeFile(url, outputPath);
        }

        // Method to download large files by streaming
        static async Task DownloadLargeFile(string url, string outputPath)
        {
            using HttpClient client = new HttpClient();

            try
            {
                Console.WriteLine($"Downloading {Path.GetFileName(outputPath)} from {url}...");

                // Send the request and get the response (streaming)
                using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                // Open the file stream and download the file chunk by chunk
                using Stream contentStream = await response.Content.ReadAsStreamAsync();
                using FileStream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);

                await contentStream.CopyToAsync(fileStream);
                Console.WriteLine($"Downloaded and saved file to: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to download {url}. Error: {ex.Message}");
            }
        }

        // Method to get the size of a remote file
        static async Task<long> GetRemoteFileSize(string url)
        {
            using HttpClient client = new HttpClient();
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, url); // Use HEAD method to get headers only
                HttpResponseMessage response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                // Get the content length (file size)
                long fileSize = response.Content.Headers.ContentLength ?? 0;
                return fileSize;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get remote file size for {url}. Error: {ex.Message}");
                return -1; // Return -1 if there's an error
            }
        }
    }
}
