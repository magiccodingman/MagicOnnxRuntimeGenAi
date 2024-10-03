using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace MagicOnnxRuntimeGenAi
{
    public class MagicModel : IDisposable
    {
        private IntPtr _modelHandle;

        private bool _disposed;

        internal IntPtr Handle => _modelHandle;

        public HardwareType hardwareType { get; set; }
        public string modelPath { get; set; }
        private MagicNativeMethods _MagicNativeMethods;

        public MagicModel(string _modelPath)
        {
            modelPath = _modelPath;
            hardwareType = GetHardwareType();
            _MagicNativeMethods = new MagicNativeMethods(hardwareType);
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaCreateModel(MagicStringUtils.ToUtf8(_modelPath), out _modelHandle));
        }
        public MagicNativeMethods GetMagicNativeMethods()
        {
            return _MagicNativeMethods;
        }

        public HardwareType GetHardwareType()
        {
            // Path to the genai_config.json file
            string filePath = Path.Combine(modelPath, "genai_config.json");

            // Check if the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The genai_config.json file was not found in the directory.");
            }

            // Read the file content
            string jsonContent = File.ReadAllText(filePath);

            // Parse the JSON content using Newtonsoft.Json.Linq
            JObject json = JObject.Parse(jsonContent);

            // Extract the provider options
            var providerOptions = json["model"]?["decoder"]?["session_options"]?["provider_options"];

            // Determine the hardware type
            if (providerOptions != null && providerOptions.HasValues)
            {
                var firstProviderOption = providerOptions.First as JObject;

                if (firstProviderOption.ContainsKey("dml"))
                {
                    return HardwareType.dml;
                }
                else if (firstProviderOption.ContainsKey("cuda"))
                {
                    return HardwareType.cuda;
                }
            }

            // If provider_options is empty or null, assume CPU
            return HardwareType.cpu;
        }

        public MagicSequences Generate(MagicGeneratorParams generatorParams)
        {
            IntPtr sequences = IntPtr.Zero;
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaGenerate(_modelHandle, generatorParams.Handle, out sequences));
            return new MagicSequences(sequences, this);
        }

        ~MagicModel()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_modelHandle != IntPtr.Zero)
                {
                    _MagicNativeMethods.OgaDestroyModel(_modelHandle);
                    _modelHandle = IntPtr.Zero;
                }

                _disposed = true;
            }
        }
    }
}
