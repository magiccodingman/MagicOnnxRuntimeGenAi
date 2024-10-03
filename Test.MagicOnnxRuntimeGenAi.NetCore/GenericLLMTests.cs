using MagicOnnxRuntimeGenAi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.MagicAiLogic;
using Test.MagicAiLogic.Helpers;
using Xunit.Abstractions;

namespace Test.MagicOnnxRuntimeGenAi.NetCore
{
    public class GenericLLMTests : IAssemblyFixture<GlobalSetup>
    {
        private readonly ITestOutputHelper _output;
        public GenericLLMTests(ITestOutputHelper output)
        {
            // Set up the test environment
            _output = output;
        }

        /// <summary>
        /// CPU run
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Phi3MiniCpuResponse()
        {
            var model = new MagicModel(GlobalSetup.CpuModelPath);
            var tokenizer = new MagicTokenizer(model);

            string systemPrompt = @"You're a helpful AI assistant.";
            string userPrompt = @"Write a very short story about a goblin becoming a hero and saving the princess.";
            var aiResponse = await new CallAi().GenerateAIResponseV6(model, tokenizer, systemPrompt, userPrompt, null, 4000, ConsoleColor.Red);
            _output.WriteLine(aiResponse.UpdatedHistory.LastOrDefault().aiResponse);
            var endAiMessage = aiResponse.UpdatedHistory.LastOrDefault().aiResponse;
            Assert.True(string.IsNullOrWhiteSpace(endAiMessage) == false);
        }

        /// <summary>
        /// Windows only NPU/GPU run
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Phi3MiniDmlResponse()
        {
            var model = new MagicModel(GlobalSetup.DmlModelPath);
            var tokenizer = new MagicTokenizer(model);

            string systemPrompt = @"You're a helpful AI assistant.";
            string userPrompt = @"Write a very short story about a goblin becoming a hero and saving the princess.";
            var aiResponse = await new CallAi().GenerateAIResponseV6(model, tokenizer, systemPrompt, userPrompt, null, 4000, ConsoleColor.Red);
            _output.WriteLine(aiResponse.UpdatedHistory.LastOrDefault().aiResponse);
            var endAiMessage = aiResponse.UpdatedHistory.LastOrDefault().aiResponse;
            Assert.True(string.IsNullOrWhiteSpace(endAiMessage) == false);
        }

        /// <summary>
        /// Cuda run
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Phi3MiniCudaResponse()
        {
            var model = new MagicModel(GlobalSetup.CudaModelPath);
            var tokenizer = new MagicTokenizer(model);

            string systemPrompt = @"You're a helpful AI assistant.";
            string userPrompt = @"Write a very short story about a goblin becoming a hero and saving the princess.";
            var aiResponse = await new CallAi().GenerateAIResponseV6(model, tokenizer, systemPrompt, userPrompt, null, 4000, ConsoleColor.Red);
            _output.WriteLine(aiResponse.UpdatedHistory.LastOrDefault().aiResponse);
            var endAiMessage = aiResponse.UpdatedHistory.LastOrDefault().aiResponse;
            Assert.True(string.IsNullOrWhiteSpace(endAiMessage) == false);
        }

        /// <summary>
        /// 2 models running in parallel. One in the GPU/NPU the other on only the CPU.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Phi3MiniDmlAndCpuResponse()
        {
            var cpuModel = new MagicModel(GlobalSetup.CpuModelPath);
            var dmlModel = new MagicModel(GlobalSetup.DmlModelPath);
            var cpuTokenizer = new MagicTokenizer(cpuModel);
            var dmlTokenizer = new MagicTokenizer(dmlModel);

            string systemPrompt = @"You're a helpful AI assistant.";
            string userPrompt = @"Write a very short story about a goblin becoming a hero and saving the princess.";

            // Start the CPU model response task
            var cpuResponseTask = Task.Run(() =>
                new CallAi().GenerateAIResponseV6(cpuModel, cpuTokenizer, systemPrompt, userPrompt, null, 4000, ConsoleColor.Red)
            );

            // Start the DML model response task with a delay
            var dmlResponseTask = Task.Run(async () =>
            {
                await Task.Delay(6000); // Delay for DML response
                return await new CallAi().GenerateAIResponseV6(dmlModel, dmlTokenizer, systemPrompt, userPrompt, null, 4000, ConsoleColor.Red);
            });

            // Await both tasks
            var results = await Task.WhenAll(cpuResponseTask, dmlResponseTask);

            // Extract responses
            var cpuResponse = results[0].UpdatedHistory.LastOrDefault().aiResponse;
            var dmlResponse = results[1].UpdatedHistory.LastOrDefault().aiResponse;

            // Output responses for debugging
            _output.WriteLine(cpuResponse);
            _output.WriteLine(dmlResponse);

            // Assert that both responses are not null or whitespace
            Assert.True(!string.IsNullOrWhiteSpace(cpuResponse), "CPU model response should not be null or whitespace.");
            Assert.True(!string.IsNullOrWhiteSpace(dmlResponse), "DML model response should not be null or whitespace.");
        }
    }
}
