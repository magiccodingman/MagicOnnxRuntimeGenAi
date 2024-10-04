using MagicOnnxRuntimeGenAi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicOnnxRuntimeGenAi.Helpers;


namespace Test.MagicOnnxRuntimeGenAi.CLI
{
    internal class Startup
    {
        public async Task Main()
        {
            var modelPath = @"C:\Source\MagicOnnxRuntimeGenAi\Test.MagicOnnxRuntimeGenAi.NetCore\bin\Debug\net8.0\Ai_Models\cuda";
            var model = new MagicModel(modelPath);
            var tokenizer = new MagicTokenizer(model);

            string systemPrompt = @"You're a helpful AI assistant.";
            string userPrompt = @"Write me a very short story about a goblin saving the princess.";
            //await new CallAi().GenerateAIResponseV6(model, tokenizer, systemPrompt, userPrompt, null, 4000, ConsoleColor.Red);
        }
    }
}
