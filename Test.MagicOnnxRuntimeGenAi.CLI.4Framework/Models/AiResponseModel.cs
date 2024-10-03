using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.MagicOnnxRuntimeGenAi.CLI._4Framework
{
    public class AiResponseModel
    {
        public List<(string userQuestion, string aiResponse)> UpdatedHistory { get; set; } = new List<(string, string)>();
        public int TotalInputTokens { get; set; } = 0;
        public int TotalOutputTokens { get; set; } = 0;
        public AiError AiError { get; set; }
        public int? Seed { get; set; }
    }
}
