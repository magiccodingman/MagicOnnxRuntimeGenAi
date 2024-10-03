using MagicOnnxRuntimeGenAi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.MagicOnnxRuntimeGenAi.CLI._4Framework
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var modelPath = @"C:\Ai_Models\Phi-3.5-mini-instruct_Uncensored_int4";

            var model = new MagicModel(modelPath);
            var tokenizer = new MagicTokenizer(model);

            string systemPrompt = @"You're a helpful AI assistant.";
            string userPrompt = @"Write me a very short story about a goblin saving the princess.";
            await new CallAi().GenerateAIResponseV6(model, tokenizer, systemPrompt, userPrompt, null, 4000, ConsoleColor.Red);
        }
    }
}
