using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MagicOnnxAi
{
    public class MagicLlmResponse
    {
        public List<(string userQuestion, string aiResponse)> UpdatedHistory { get; set; } = new List<(string, string)>();
        public int TotalInputTokens { get; set; } = 0;
        public int TotalOutputTokens { get; set; } = 0;
        public MagicError Error { get; set; }
    }
}
