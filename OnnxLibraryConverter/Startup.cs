using OnnxLibraryConverter.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnnxLibraryConverter
{
    internal class Startup
    {
        public async Task Main()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Be sure that Git LFS is installed or this will fail!");
            Console.ResetColor();

            string version = "0.4.0";
            new NugetHelper().GetOriginalOnnx(version);
        }
    }
}
