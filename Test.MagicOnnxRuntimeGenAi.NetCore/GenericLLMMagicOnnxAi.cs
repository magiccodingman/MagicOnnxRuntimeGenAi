using MagicOnnxAi;
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
    public class GenericLLMMagicOnnxAi : IAssemblyFixture<GlobalSetup>
    {
        private readonly ITestOutputHelper _output;
        private readonly MagicGenAiPool _MagicAiPool_Cpu;
        private readonly MagicGenAiPool _MagicAiPool_DirectML;
        private readonly string Phi35MiniUnfiltered_Cpu = "Phi35MiniUnfiltered_Cpu";
        private readonly string Phi35MiniUnfiltered_DirectML = "Phi35MiniUnfiltered_DirectML";
        public GenericLLMMagicOnnxAi(ITestOutputHelper output)
        {
            // Set up the test environment
            _output = output;

            // DML must come first
            _MagicAiPool_DirectML = new MagicGenAiPool(GenAiModelType.Phi3, Phi35MiniUnfiltered_DirectML, GlobalSetup.DmlModelPath);
            _MagicAiPool_DirectML.AddModelToMemory(1);

            _MagicAiPool_Cpu = new MagicGenAiPool(GenAiModelType.Phi3, Phi35MiniUnfiltered_Cpu, GlobalSetup.CpuModelPath);
            _MagicAiPool_Cpu.AddModelToMemory(2);

            

            
        }
        [Fact]
        public async Task NoHistoryResponse_CPU()
        {
            string systemPrompt = @"You're a helpful AI assistant.";
            string userPrompt = @"Write a very short story about a goblin becoming a hero and saving the princess.";
            MagicLlmResponse response = await _MagicAiPool_Cpu.GenerateResponse(systemPrompt, userPrompt, null);
            var endAiMessage = response.UpdatedHistory.LastOrDefault().aiResponse;
            _output.WriteLine(endAiMessage);
            Assert.True(string.IsNullOrWhiteSpace(endAiMessage) == false);
        }

        /// <summary>
        /// DirectML run
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task NoHistoryResponse_DirectML()
        {
            string systemPrompt = @"You're a helpful AI assistant.";
            string userPrompt = @"Write a very short story about a goblin becoming a hero and saving the princess.";
            MagicLlmResponse response = await _MagicAiPool_DirectML.GenerateResponse(systemPrompt, userPrompt, null);
            var endAiMessage = response.UpdatedHistory.LastOrDefault().aiResponse;
            _output.WriteLine(endAiMessage);
            Assert.True(string.IsNullOrWhiteSpace(endAiMessage) == false);
        }

        /// <summary>
        /// CPU && DML run with multiple prompts in parallel. Showcasing multi thread 
        /// capabilities, ease, and queue.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task NoHistoryResponse_Parallel()
        {
            string systemPrompt = @"You're a helpful AI assistant.";

            // Define multiple user prompts
            string userPrompt1 = @"Write a very short story about a goblin becoming a hero and saving the princess.";
            string userPrompt2 = @"Tell a tale about a dragon that learns to bake cookies for the village.";
            string userPrompt3 = @"Write a short story about Christmas.";
            string userPrompt4 = @"Tell a short story about elves.";

            // Start the CPU model response tasks
            var cpuResponseTask1 = Task.Run(() =>
                _MagicAiPool_Cpu.GenerateResponse(systemPrompt, userPrompt1, null)
            );
             var cpuResponseTask2 = Task.Run(() =>
                 _MagicAiPool_Cpu.GenerateResponse(systemPrompt, userPrompt2, null)
             );
            /*var cpuResponseTask3 = Task.Run(() =>
                _MagicAiPool_Cpu.GenerateResponse(systemPrompt, userPrompt3, null)
            );
            var cpuResponseTask4 = Task.Run(() =>
                _MagicAiPool_Cpu.GenerateResponse(systemPrompt, userPrompt4, null)
            );

            // Start the DirectML model response tasks
            var dmlResponseTask1 = Task.Run(() =>
                _MagicAiPool_DirectML.GenerateResponse(systemPrompt, userPrompt1, null)
            );
            var dmlResponseTask2 = Task.Run(() =>
                _MagicAiPool_DirectML.GenerateResponse(systemPrompt, userPrompt2, null)
            );
            var dmlResponseTask3 = Task.Run(() =>
                _MagicAiPool_DirectML.GenerateResponse(systemPrompt, userPrompt3, null)
            );*/
            var dmlResponseTask4 = Task.Run(() =>
                _MagicAiPool_DirectML.GenerateResponse(systemPrompt, userPrompt4, null)
            );
                       

            var responses = await Task.WhenAll(cpuResponseTask1, cpuResponseTask2, dmlResponseTask4);

            // Loop through all responses, output and assert they are not null or whitespace
            for (int i = 0; i < responses.Length; i++)
            {
                try
                {
                    var aiMessage = responses[i].UpdatedHistory.LastOrDefault().aiResponse;
                    _output.WriteLine($"Response {i + 1}: {aiMessage}");
                    Assert.True(!string.IsNullOrWhiteSpace(aiMessage), $"Response {i + 1} should not be null or whitespace.");
                }
                catch(Exception ex)
                {
                    _output.WriteLine($"ERROR: {ex.Message}");
                }
            }
        }


    }
}
