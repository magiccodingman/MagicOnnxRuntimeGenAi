using MagicOnnxRuntimeGenAi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.MagicOnnxRuntimeGenAi.CLI._4Framework
{
    public class CallAi
    {
      

        public CallAi()
        {
        }

        public ulong CountTokens(MagicTokenizer tokenizer, string text)
        {
            var promptTokens = tokenizer.Encode(text);

            // Step 4: Correct token counting using model's vocab-based tokenizer
            // Use Sequences.NumSequences to get token count
            ulong tokenCount = 0;

            for (ulong i = 0; i < promptTokens.NumSequences; i++)
            {
                var tokenSpan = promptTokens[i];
                tokenCount += (ulong)tokenSpan.Length;  // Accumulate total token count from each sequence
            }

            return tokenCount;
        }
        public async Task<AiResponseModel> GenerateAIResponseV6(MagicModel model, MagicTokenizer tokenizer, string systemPrompt, string userPrompt, List<(string userQuestion, string aiResponse)> conversationHistory, int tokenLimit, ConsoleColor color, int optionalTimeWait = 0)
        {
            if (optionalTimeWait > 0)
            {
                await Task.Delay(optionalTimeWait);
            }
            var responseModel = new AiResponseModel();

            // Tokenize the user prompt and check if it exceeds the token limit

            var userPromptTokens = CountTokens(tokenizer, userPrompt);


            if (userPromptTokens > (ulong)tokenLimit) // Ensure proper token counting using the tokenizer's method
            {
                responseModel.AiError = new AiError
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


                var userQuestionTokens = CountTokens(tokenizer, userQuestion);
                var aiResponseTokens = CountTokens(tokenizer, aiResponse);

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
            //string fullPrompt = new ChatTemplate().ChatTemplateGemma9b2(messages, true);
            string fullPrompt = new ChatTemplate().ChatTemplatePhiMini3(messages, true);
            //string fullPrompt = new ChatTemplate().ChatTemplateLlama31(messages, true);


            var tokens = tokenizer.Encode(fullPrompt);

            // Set generator params (e.g., max_length)
            var generatorParams = new MagicGeneratorParams(model);
            //generatorParams.SetSearchOption("max_length", tokenLimit);  // Adjust to token limit
            //generatorParams.SetSearchOption("past_present_share_buffer", true);
            generatorParams.SetInputSequences(tokens);
            generatorParams.TryGraphCaptureWithMaxBatchSize(1);
            // Variable to hold the assistant's full response
            var fullResponse = new System.Text.StringBuilder();

            // Generate the response
            var generator = new MagicGenerator(model, generatorParams);
            int outputTokens = 0;

            // Output the response token by token as it's generated
            Console.WriteLine("Assistant: ");
            while (!generator.IsDone())
            {
                generator.ComputeLogits();  // Compute the next logits
                generator.GenerateNextToken();  // Generate the next token

                var outputTokensSequence = generator.GetSequence(0);
                var newToken = outputTokensSequence[outputTokensSequence.Length - 1];  // Get the last generated token
                var output = tokenizer.Decode(new ReadOnlySpan<int>(new int[] { newToken }));

                // Build the full response string incrementally
                fullResponse.Append(output);

                Console.ForegroundColor = color;
                // Print the current token (for real-time updates)
                Console.Write($"{output}");

                // Count the output tokens
                outputTokens++;
            }
            Console.ResetColor();
            // After generation, store the results in the response model
            responseModel.UpdatedHistory = new List<(string, string)>(conversationHistory)
     {
         (userPrompt, fullResponse.ToString())
     };
            responseModel.TotalInputTokens = (int)totalInputTokens;
            responseModel.TotalOutputTokens = outputTokens;

            Console.WriteLine("\n");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("AI TASK COMPLETE");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("AI TASK COMPLETE");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("AI TASK COMPLETE");
            Console.ResetColor();

            return responseModel;
        }
    }
}
