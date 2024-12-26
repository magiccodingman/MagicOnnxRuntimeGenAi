using MagicOnnxAi.Helpers;
using MagicOnnxAi.Models;
using MagicOnnxRuntimeGenAi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Test.MagicOnnxAi.Helpers;

namespace MagicOnnxAi
{
    public enum AiCategory
    {
        //Custom = 0, // Not available yet
        GenAiLLM = 1,
        // TextEmbedding = 3, // Not available yet
    }

    public enum GenAiModelType
    {
        //Custom = 0,
        Gemma = 1,
        Mistral = 2,
        Phi3 = 3,
        //Llama3 = 4,
        Llama31 = 5,
    }

    public class MagicGenAiPool //: IDisposable
    {
        private List<ServiceGenAiModel> _genAiServiceModels { get; set; } = new List<ServiceGenAiModel>();
        private readonly object _genAiLock = new object();
        private bool _disposed;
        private HardwareType? _hardwareType { get; set; } = null;

        private bool? _isGenAi;

        private readonly int DefaultAiModelTimeout;

        /// <summary>
        /// /// Use this when utilizing the GenAI protocol.
        /// Supported models currently: 
        /// Gemma 
        /// LlaMA 
        /// Mistral 
        /// Phi 
        /// </summary>
        /// <param name="_GenAiType"></param>
        /// <param name="_ModelName"></param>
        /// <param name="_ModelPath"></param>
        /// <param name="defaultAiModelTimeout">This is in minutes</param>
        /// <exception cref="Exception"></exception>
        public MagicGenAiPool(GenAiModelType _GenAiType, string _ModelName,
            string _ModelPath, int defaultAiModelTimeout = 5)
        {
            /*
             * In the genai_config.json we can determine the model type automatically 
             * by the type variable. So why not use this? Majorly because of Llama. 
             * Llama models change their chat template. Or it can potentially be changed 
             * via fine tuning. So, it's best to just specify it here. For example the 
             * chat template for llama 3 versus 3.1 is different.
             */

            if (defaultAiModelTimeout > 0)
                DefaultAiModelTimeout = defaultAiModelTimeout;
            else
                DefaultAiModelTimeout = 1;

            Category = AiCategory.GenAiLLM;
            ModelName = _ModelName;
            if (string.IsNullOrWhiteSpace(ModelName))
                throw new Exception("Model Name cannot be empty");

            ModelPath = _ModelPath;
            if (string.IsNullOrWhiteSpace(ModelPath))
                throw new Exception("Model path cannot be null or empty");
            if (!Directory.Exists(ModelPath))
                throw new Exception("Model path does not exist");

            GenAiType = _GenAiType;
            var isGenAI = IsGenAi;
            if (!isGenAI)
            {
                throw new Exception("Model does not have the genai_config.json file");
            }

            int loadContextLength = ContextLength;
        }

        private int? _contextLength;
        // Public property to access the cached context length
        public int ContextLength
        {
            get
            {
                if (_contextLength == null)
                {
                    _contextLength = LoadContextLength();
                }
                return _contextLength.Value;
            }
        }

        // Private method to read the JSON file and extract the context length
        private int LoadContextLength()
        {
            string configPath = Path.Combine(this.ModelPath, "genai_config.json");

            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException($"Configuration file not found at {configPath}");
            }

            string jsonContent = File.ReadAllText(configPath);
            JObject jsonObj = JObject.Parse(jsonContent);

            // Extract the context length
            int contextLength = (int)jsonObj["model"]["context_length"];

            return contextLength;
        }

        public GenAiModelType GenAiType { get; set; }


        /// <summary>
        /// Won't need to set any categories or anything yet. 
        /// As I'm only supporting the GenAI protocol right now. 
        /// This is for future code.
        /// Therefore, we can auto set the category.  
        /// </summary>
        /// <param name="_Category"></param>
        /// <param name="_ModelName"></param>
        /// <param name="_ModelPath"></param>
        /*public MagicAiGroup(AiCategory _Category,
            string _ModelName,
            string _ModelPath)
        {
            Category = _Category;
            ModelName = _ModelName;
            ModelPath = _ModelPath;
        }*/



        /// <summary>
        /// This is the model name and acts as the 
        /// unique identifier for models as well. It's 
        /// okay to have multiple of the same model with the 
        /// same name! This'll load balance for you!
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// Directory to where the model is
        /// </summary>
        public string ModelPath { get; set; }

        /// <summary>
        /// Category to let us know it's an LLM or not.
        /// </summary>
        public AiCategory Category { get; set; }

        /// <summary>
        /// Checks if the model is a generative AI model based on the presence of a configuration file.
        /// This value is cached after the first check to avoid repeated filesystem access.
        /// </summary>
        public bool IsGenAi
        {
            get
            {
                // Only check if we haven't already cached the result
                if (!_isGenAi.HasValue)
                {
                    // Set _isGenAi based on the conditions
                    if (Category == AiCategory.GenAiLLM)
                    {
                        // Check for the existence of the genai_config.json file
                        _isGenAi = File.Exists(Path.Combine(ModelPath, "genai_config.json"));
                    }
                    else
                    {
                        _isGenAi = false; // Not LLM, so it's false
                    }
                }

                return _isGenAi.Value;
            }
            set
            {
                // If there's a need to set it manually, reset the cache
                _isGenAi = value;
            }
        }

        public HardwareType GetHardwareType()
        {
            if (this.Category == AiCategory.GenAiLLM)
            {
                if (_genAiLock == null || !_genAiServiceModels.Any())
                {
                    throw new Exception("No Ai model is currently in ");
                }

                return _hardwareType??HardwareType.cpu;
            }

            throw new Exception("Hardware type can't be discovered due to bad logic!");
        }

        public void AddModelToMemory(int desiredAdditions = 1)
        {
            if (this.Category == AiCategory.GenAiLLM)
            {
                for (int i = 0; i < desiredAdditions; i++)
                {
                    var magicGenAi = new MagicGenAi(this.ModelPath);
                    if (_hardwareType == null)
                        _hardwareType = magicGenAi.Model.GetHardwareType();

                    var chatTemplate = GetGenAiDefaultChatTemplate();
                    _genAiServiceModels.Add(new ServiceGenAiModel(magicGenAi, chatTemplate));
                }
                return;
            }

            throw new Exception("Adding other models to memory not implemented yet.");
        }

        private Func<IEnumerable<AiMessage>, bool, string> GetGenAiDefaultChatTemplate()
        {
            if (this.GenAiType == GenAiModelType.Phi3)
                return ChatTemplate.ChatTemplatePhiMini3;
            else if (this.GenAiType == GenAiModelType.Gemma)
                return ChatTemplate.ChatTemplateGemma9b2;
            else if (this.GenAiType == GenAiModelType.Llama31)
                return ChatTemplate.ChatTemplateLlama31;

            throw new Exception("Chat template not implemented yet.");
        }

        public async Task<MagicLlmResponse> GenerateResponse(
            string systemPrompt, string userPrompt,
            List<(string userQuestion, string aiResponse)> conversationHistory,
            int tokenLimit = 4000, int outputTokenCutOff = 0)
        {
            ServiceGenAiModel serviceAiModel = AcquireNextFreeModel();
            MagicError error = new MagicError();
            MagicLlmResponse magicLlmResponse = new MagicLlmResponse();
            try
            {
                /*magicLlmResponse = await new GenAiLLmHelper().GenerateResponse(
                    serviceAiModel.Model,
                    GetGenAiDefaultChatTemplate(),
                    systemPrompt, userPrompt, conversationHistory, 
                    tokenLimit, outputTokenCutOff);*/

                /*
                 * Offload this task from the normal thread process. 
                 * TO securely make sure that concurrent requests are 
                 * handled without starvation.
                 */
                magicLlmResponse = await Task.Run(() =>
                    new GenAiLLmHelper().GenerateResponse(
                        serviceAiModel.Model,
                        GetGenAiDefaultChatTemplate(),
                        systemPrompt, userPrompt, conversationHistory,
                        tokenLimit, outputTokenCutOff));

            }
            catch (Exception ex)
            {
                // Handle the exception
                error = new MagicError();
                error.Error = true;
                error.Message = ex?.InnerException?.Message??ex.Message;
                magicLlmResponse.Error = error;
            }
            finally
            {
                // Code that should always execute
                ReleaseModel(serviceAiModel);
            }

            return magicLlmResponse;
        }
        /// <summary>
        /// Attempts to acquire the next available model.
        /// </summary>
        /// <returns>An available ServiceGenAiModel, or null if none are available.</returns>
        private ServiceGenAiModel AcquireNextFreeModel()
        {
            lock (_genAiLock)
            {
                foreach (var model in _genAiServiceModels)
                {
                    if (!model.IsInUse)
                    {
                        model.IsInUse = true;
                        model.StartRun = DateTime.UtcNow;
                        return model;
                    }
                    else if (model.IsInUse && model.StartRun.HasValue)
                    {
                        DateTime startTime = model.StartRun.Value;
                        if (DateTime.UtcNow - startTime >= TimeSpan.FromMinutes(DefaultAiModelTimeout))
                        {
                            /*
                             * If for some reason the AI model is stuck, we can 
                             * trash it if the timeout period designated is surpassed. 
                             * And then we can auto add it back to memory. 
                             * this is also a likely temporary location for this process. 
                             * There's better ways to handle this without slowing down the 
                             * acquire process. 
                             */
                            model.Model.Dispose();
                            AddModelToMemory(1);
                        }
                    }
                }
                return null; // No available model found
            }
        }


        /// <summary>
        /// Releases a model back into the pool, marking it as available.
        /// </summary>
        /// <param name="model">The model to release.</param>
        public void ReleaseModel(ServiceGenAiModel model)
        {
            lock (_genAiLock)
            {
                model.IsInUse = false;
                model.StartRun = null;
            }
        }

        /// <summary>
        /// Disposes of all models in the pool.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                if (_genAiLock != null && _genAiServiceModels.Any())
                {
                    lock (_genAiLock)
                    {
                        foreach (var sModel in _genAiServiceModels)
                        {
                            sModel.Model.Dispose();
                        }
                    }
                }
                _disposed = true;
            }
        }
    }

    public class ResourceManager
    {
        private readonly ConcurrentQueue<int> _cpuResources;
        private readonly object _lock = new object();

        public ResourceManager(int totalCpuCores)
        {
            _cpuResources = new ConcurrentQueue<int>();
            for (int i = 0; i < totalCpuCores; i++)
            {
                _cpuResources.Enqueue(i);
            }
        }

        public int? AllocateCpuResource()
        {
            lock (_lock)
            {
                if (_cpuResources.TryDequeue(out int cpuId))
                {
                    return cpuId;
                }
            }
            // Return null if no CPUs are available
            return null;
        }

        public void ReleaseCpuResource(int cpuId)
        {
            lock (_lock)
            {
                _cpuResources.Enqueue(cpuId);
            }
        }
    }
}
