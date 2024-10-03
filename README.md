# MagicOnnxRuntimeGenAI

**MagicOnnxRuntimeGenAI** is an extension of the `Microsoft.ML.OnnxRuntimeGenAI` library that removes the limitations associated with hardware utilization and platform compatibility. It allows you to run multiple AI models on different hardware environments (CPU, CUDA, DirectML) simultaneously in a single instance, solving the original library’s constraint of choosing only one type of hardware at a time. The goal is to maintain code similarity with the original library, while enhancing flexibility and scalability.

### Nuget
CPU:
https://www.nuget.org/packages/MagicOnnxRuntimeGenAi.Cpu/0.4.0.1

DirectML:
https://www.nuget.org/packages/MagicOnnxRuntimeGenAi.DirectML/0.4.0.1

Cuda:
https://www.nuget.org/packages/MagicOnnxRuntimeGenAi.Cuda/0.4.0.1

## Features

- **Multi-hardware support**: Run CPU, CUDA, and DirectML versions in parallel, enabling better performance scaling across platforms.
- **Automatic library path handling**: Dynamically manage paths to hardware-specific DLLs, eliminating conflicts from shared DLL names.
- **Simple migration**: Maintain close compatibility with `Microsoft.ML.OnnxRuntimeGenAI`. Just add "Magic" to the class names to switch to the enhanced version.
- **Cross-platform AI scaling**: Utilize different hardware setups on platforms like Android, iOS, Windows, Linux, and Mac.
- **ASP.NET and client-side AI models**: Run AI models on different devices and environments without being restricted to server-side execution.
- **Automated DirectML setup**: Automatically adds the DirectML.dll to your output directory.
- **XUnit test support**: Includes test samples showcasing CPU and DirectML models running in parallel for better validation.

## Motivation

The original `OnnxRuntimeGenAI` library imposes limitations on using hardware acceleration across different platforms. For instance, running AI tasks on CUDA restricts you to NVIDIA GPUs, while DirectML is Windows-only. These restrictions make it difficult to scale AI solutions across platforms like mobile, web, and desktop applications. With **MagicOnnxRuntimeGenAI**, you can overcome these barriers and run different models on various hardware configurations (CPU, GPU, NPU) in parallel.

### Use Cases

- Running a text embedding LLM on one CPU thread while simultaneously running another LLM on another CPU thread, and utilizing DirectML for a larger LLM.
- Developing an AI-powered client-side application using a platform like MAUI Blazor, which can scale across different platforms.
- Creating an ASP.NET REST API that manages multiple AI models across various hardware environments.
- Maintaining flexibility and scalability while minimizing server-side dependencies, reducing latency, and improving control.

### Library Structure

The key libraries (`cpu`, `cuda`, `dml`) are separated into different folders, avoiding conflicts due to identical DLL names. This allows you to utilize all three in a single application. 

For DirectML users, **MagicOnnxRuntimeGenAI** automatically includes the required `DirectML.dll` in the output directory.

## Quickstart Example

Here are some examples showcasing how to use **MagicOnnxRuntimeGenAI**:

### CPU Model Example

```csharp
/// <summary>
/// Run a model using the CPU.
/// </summary>
/// <returns></returns>
[Fact]
public async Task Phi3MiniCpuResponse()
{
    var model = new MagicModel(GlobalSetup.CpuModelPath);
    var tokenizer = new MagicTokenizer(model);

    string systemPrompt = @"You're a helpful AI assistant.";
    string userPrompt = @"Write a very short story about a goblin becoming a hero and saving the princess.";

    var aiResponse = await new CallAi().GenerateAIResponseV6(model, tokenizer, systemPrompt, userPrompt, null, 4000, ConsoleColor.Red);
    _output.WriteLine(aiResponse.UpdatedHistory.LastOrDefault().aiResponse);

    var endAiMessage = aiResponse.UpdatedHistory.LastOrDefault().aiResponse;
    Assert.True(!string.IsNullOrWhiteSpace(endAiMessage));
}
```

### DirectML Model Example (Windows Only)

```csharp
/// <summary>
/// Run a model using DirectML (Windows-only).
/// </summary>
/// <returns></returns>
[Fact]
public async Task Phi3MiniDmlResponse()
{
    var model = new MagicModel(GlobalSetup.DmlModelPath);
    var tokenizer = new MagicTokenizer(model);

    string systemPrompt = @"You're a helpful AI assistant.";
    string userPrompt = @"Write a very short story about a goblin becoming a hero and saving the princess.";

    var aiResponse = await new CallAi().GenerateAIResponseV6(model, tokenizer, systemPrompt, userPrompt, null, 4000, ConsoleColor.Red);
    _output.WriteLine(aiResponse.UpdatedHistory.LastOrDefault().aiResponse);

    var endAiMessage = aiResponse.UpdatedHistory.LastOrDefault().aiResponse;
    Assert.True(!string.IsNullOrWhiteSpace(endAiMessage));
}
```

### Parallel Execution: CPU and DirectML Models

```csharp
/// <summary>
/// Run both CPU and DirectML models in parallel.
/// </summary>
/// <returns></returns>
[Fact]
public async Task Phi3MiniDmlAndCpuResponse()
{
    var cpuModel = new MagicModel(GlobalSetup.CpuModelPath);
    var dmlModel = new MagicModel(GlobalSetup.DmlModelPath);
    var cpuTokenizer = new MagicTokenizer(cpuModel);
    var dmlTokenizer = new MagicTokenizer(dmlModel);

    string systemPrompt = @"You're a helpful AI assistant.";
    string userPrompt = @"Write a very short story about a goblin becoming a hero and saving the princess.";

    // Start the CPU model response task
    var cpuResponseTask = Task.Run(() =>
        new CallAi().GenerateAIResponseV6(cpuModel, cpuTokenizer, systemPrompt, userPrompt, null, 4000, ConsoleColor.Red)
    );

    // Start the DML model response task with a delay
    var dmlResponseTask = Task.Run(async () =>
    {
        await Task.Delay(6000); // Delay for DML response
        return await new CallAi().GenerateAIResponseV6(dmlModel, dmlTokenizer, systemPrompt, userPrompt, null, 4000, ConsoleColor.Red);
    });

    // Await both tasks
    var results = await Task.WhenAll(cpuResponseTask, dmlResponseTask);

    // Extract and output responses
    var cpuResponse = results[0].UpdatedHistory.LastOrDefault().aiResponse;
    var dmlResponse = results[1].UpdatedHistory.LastOrDefault().aiResponse;

    _output.WriteLine(cpuResponse);
    _output.WriteLine(dmlResponse);

    Assert.True(!string.IsNullOrWhiteSpace(cpuResponse), "CPU model response should not be null or whitespace.");
    Assert.True(!string.IsNullOrWhiteSpace(dmlResponse), "DML model response should not be null or whitespace.");
}
```

## Future Development

- **CUDA Support**: Plans to extend the capabilities, but currently replicates what's in the original GenAI.Cuda
- **OnnxRuntime support**: There are plans to extend the Magic protocol to the larger `OnnxRuntime` library.
- **Automatic GenAI updates**: Automating the update process to newer versions of `OnnxRuntimeGenAI`.
- **New Projects**: Future projects will build on this, making AI easier to use with higher-level abstractions.

## Contributing

Contributions are welcome! If you wish to add features, please ensure you include relevant XUnit tests to make merging easier. The project’s goal is to remain as close to the original `OnnxRuntimeGenAI` as possible, with minimal changes.

### How to Contribute

1. Fork the repository.
2. Create a new branch (`feature/my-feature`).
3. Write your code and accompanying unit tests.
4. Submit a pull request.
