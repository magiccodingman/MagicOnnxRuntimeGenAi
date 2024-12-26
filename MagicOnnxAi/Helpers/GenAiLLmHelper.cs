using MagicOnnxAi.Helpers;
using MagicOnnxAi;
using MagicOnnxRuntimeGenAi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Test.MagicOnnxAi.Helpers
{
    public class GenAiLLmHelper
    {

        public async Task<MagicLlmResponse> GenerateResponse(MagicGenAi magicGenAi,
            Func<IEnumerable<AiMessage>, bool, string> chatTemplateFunc,
            string systemPrompt, string userPrompt, 
            List<(string userQuestion, string aiResponse)> conversationHistory, 
            int tokenLimit, int outputTokenCutOff = 0)
        {            
            if(conversationHistory == null)
            {
                conversationHistory = new List<(string userQuestion, string aiResponse)>();
            }
            var responseModel = new MagicLlmResponse();

            // Tokenize the user prompt and check if it exceeds the token limit
            var userPromptTokens = magicGenAi.CountTokens(userPrompt);


            if (userPromptTokens > (ulong)tokenLimit) // Ensure proper token counting using the tokenizer's method
            {
                responseModel.Error = new MagicError
                {
                    Error = true,
                    Message = "The prompt went over the maximum tokens allowed."
                };
                return responseModel;  // Early return in case of error
            }

            // Track total input tokens
            ulong totalInputTokens = userPromptTokens;  // Updated to use NumSequences

            List<AiMessage> messages = new List<AiMessage>();
            messages.Add(new AiMessage() { Role = "system", Content = systemPrompt });


            if (conversationHistory == null)
                conversationHistory = new List<(string userQuestion, string aiResponse)>();

            for (int i = conversationHistory.Count - 1; i >= 0; i--)
            {
                var (userQuestion, aiResponse) = conversationHistory[i];


                var userQuestionTokens = magicGenAi.CountTokens(userQuestion);
                var aiResponseTokens = magicGenAi.CountTokens(aiResponse);

                // Check if adding the userQuestion and aiResponse exceeds token limit
                if (totalInputTokens + userQuestionTokens + aiResponseTokens > (ulong)tokenLimit)
                {
                    break;
                }


                messages.Add(new AiMessage() { Role = "user", Content = userQuestion });

                messages.Add(new AiMessage() { Role = "assistant", Content = aiResponse });

                // Add to the total input tokens
                totalInputTokens += userQuestionTokens +  aiResponseTokens;
            }

            messages.Add(new AiMessage() { Role = "user", Content = userPrompt });

            // Tokenize the full prompt to send to the AI        
            string fullPrompt = chatTemplateFunc(messages, true);


            var tokens = magicGenAi.Tokenizer.Encode(fullPrompt);

            // Set generator params (e.g., max_length)
            var generatorParams = new MagicGeneratorParams(magicGenAi.Model);
            //generatorParams.SetSearchOption("max_length", tokenLimit);  // Adjust to token limit

            bool pastPresentShareBuffer = false;
            if (magicGenAi.Model.hardwareType != HardwareType.cpu)
                pastPresentShareBuffer = true;
            generatorParams.SetSearchOption("past_present_share_buffer", pastPresentShareBuffer);
            generatorParams.SetInputSequences(tokens);
            generatorParams.TryGraphCaptureWithMaxBatchSize(1);
            // Variable to hold the assistant's full response
            var fullResponse = new System.Text.StringBuilder();

            // Generate the response
            var generator = new MagicGenerator(magicGenAi.Model, generatorParams);
            int outputTokens = 0;

            // Output the response token by token as it's generated
            Console.WriteLine("Assistant: ");
            while (!generator.IsDone())
            {
                generator.ComputeLogits();  // Compute the next logits
                generator.GenerateNextToken();  // Generate the next token

                var outputTokensSequence = generator.GetSequence(0);
                var newToken = outputTokensSequence[outputTokensSequence.Length - 1];  // Get the last generated token
                var output = magicGenAi.Tokenizer.Decode(new ReadOnlySpan<int>(new int[] { newToken }));

                // Build the full response string incrementally
                fullResponse.Append(output);

                // Print the current token (for real-time updates)
                Console.Write($"{output}");

                // Count the output tokens
                outputTokens++;

                if (outputTokenCutOff > 0 && outputTokenCutOff >= outputTokens)
                    break;
            }
            
            // After generation, store the results in the response model
            responseModel.UpdatedHistory = new List<(string, string)>(conversationHistory)
     {
         (userPrompt, fullResponse.ToString())
     };
            responseModel.TotalInputTokens = (int)totalInputTokens;
            responseModel.TotalOutputTokens = outputTokens;


            return responseModel;
        }
    }
}
