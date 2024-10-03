using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.MagicAiLogic
{
    public class ChatTemplate
    {

        public string ChatTemplateGemma9b2(IEnumerable<AiMessage> messages, bool addGenerationPrompt = true, string bosToken = "<|startoftext|>")
        {
            var sb = new StringBuilder();

            // Start with the beginning-of-sequence token
            sb.Append(bosToken);

            // Track the alternating pattern (we start with expecting a user message first)
            bool expectUser = true;

            // Iterate through each message
            foreach (var message in messages)
            {
                // Determine if the current message should be from a user or assistant
                bool isUser = message.Role == "user" || message.Role == "system";

                // Adjust the alternation logic but allow flexibility for trailing user messages
                if (isUser != expectUser)
                {
                    if (expectUser)
                    {
                        // If we expected a user message but got something else, throw an error
                        throw new InvalidOperationException("Conversation roles must alternate user/assistant/user/assistant...");
                    }
                    else
                    {
                        // If we expected an assistant response but got a user message again, don't throw an error
                        // This allows for flexibility in case of missing assistant responses
                    }
                }

                // Assign the role to 'model' if it's 'assistant', otherwise keep the original role
                var role = message.Role == "assistant" ? "model" : message.Role;

                // Append the message content with <start_of_turn> and <end_of_turn> markers
                sb.Append("<start_of_turn>" + role + "\n" + message.Content.Trim() + "<end_of_turn>\n");

                // Toggle the expectation for the next message
                expectUser = !expectUser;
            }

            // Optionally add the generation prompt if required
            if (addGenerationPrompt)
            {
                sb.Append("<start_of_turn>model\n");
            }

            return sb.ToString();
        }

        public string ChatTemplatePhiMini3(IEnumerable<AiMessage> messages, bool addGenerationPrompt = true, string eosToken = "<|endoftext|>")
        {
            var sb = new StringBuilder();

            foreach (var message in messages)
            {
                if (message.Role == "system" && !string.IsNullOrEmpty(message.Content))
                {
                    sb.Append("<|system|>\n");
                    sb.Append(message.Content);
                    sb.Append("<|end|>\n");
                }
                else if (message.Role == "user")
                {
                    sb.Append("<|user|>\n");
                    sb.Append(message.Content);
                    sb.Append("<|end|>\n");
                }
                else if (message.Role == "assistant")
                {
                    sb.Append("<|assistant|>\n");
                    sb.Append(message.Content);
                    sb.Append("<|end|>\n");
                }
            }

            if (addGenerationPrompt)
            {
                sb.Append("<|assistant|>\n");
            }
            else
            {
                sb.Append(eosToken);
            }

            return sb.ToString();
        }

        public string ChatTemplateLlama31(
       List<AiMessage> messages,
       bool addGenerationPrompt = true,
       string bosToken = "bos_token",
       string dateString = "26 Jul 2024",
       bool? toolsInUserMessage = null)
        {
            var sb = new StringBuilder();

            // Set default values if not provided
            if (toolsInUserMessage == null)
                toolsInUserMessage = true;

            // Extract system message if it exists
            string systemMessage = "";
            if (messages.Count > 0 && messages[0].Role == "system")
            {
                systemMessage = messages[0].Content.Trim();
                messages = messages.Skip(1).ToList();
            }

            // Start building the output
            sb.Append($"{bosToken}\n");

            // System message header
            sb.Append("<|start_header_id|>system<|end_header_id|>\n\n");
            sb.Append("Cutting Knowledge Date: December 2023\n");
            sb.Append($"Today Date: {dateString}\n\n");

            sb.Append(systemMessage);
            sb.Append("<|eot_id|>");

            // Process remaining messages
            foreach (var message in messages)
            {
                sb.Append($"<|start_header_id|>{message.Role}<|end_header_id|>\n\n");
                sb.Append(message.Content.Trim() + "<|eot_id|>");
            }

            // Optionally add a generation prompt at the end
            if (addGenerationPrompt)
            {
                sb.Append("<|start_header_id|>assistant<|end_header_id|>\n\n");
            }

            return sb.ToString();
        }
    }
}
