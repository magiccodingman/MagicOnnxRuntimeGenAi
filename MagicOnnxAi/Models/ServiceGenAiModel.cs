using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MagicOnnxAi.Models
{
    public class ServiceGenAiModel
    {
        // Define a delegate that accepts a Func for custom template processing
        public Func<IEnumerable<AiMessage>, bool, string> _chatTemplateFunc;
        public ServiceGenAiModel(MagicGenAi _Model, Func<IEnumerable<AiMessage>, bool, string> chatTemplateFunc)
        {
            Model = _Model;
            // Assign the custom function or default to ChatTemplateGemma9b2
            _chatTemplateFunc = chatTemplateFunc;
        }
        public MagicGenAi Model { get; set; }
        public bool IsInUse { get; set; } = false;
        public DateTime? StartRun { get; set; }
    }
}
