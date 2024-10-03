using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.MagicAiLogic.Helpers;

namespace Test.MagicOnnxRuntimeGenAi.NetCore
{
    public class GlobalSetup : IDisposable
    {
        public static string CpuModelPath { get; private set; }
        public static string DmlModelPath { get; private set; }
        public static string CudaModelPath { get; private set; }

        public GlobalSetup()
        {
            // Call the async initialization method and block until completion
            InitializeAsync().GetAwaiter().GetResult();
        }

        private async Task InitializeAsync()
        {
            // Async initialization logic (e.g., downloading models)
            CpuModelPath = await HuggingFaceDownloader.GetPhi3MiniCPU();
            DmlModelPath = await HuggingFaceDownloader.GetPhi3MiniDml();
            //CudaModelPath = await HuggingFaceDownloader.GetPhi3MiniCuda();
            CudaModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ai_Models", "cuda");

            Console.WriteLine($"CPU Model Path: {CpuModelPath}");
            Console.WriteLine($"DML Model Path: {DmlModelPath}");
        }

        public void Dispose()
        {
            // Optional cleanup logic after all tests
            Console.WriteLine("Global teardown after all tests.");
        }
    }

}
