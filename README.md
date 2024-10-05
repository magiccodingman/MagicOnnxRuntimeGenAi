# MagicOnnxRuntimeGenAI

**MagicOnnxRuntimeGenAI** is an extension of the `Microsoft.ML.OnnxRuntimeGenAI` library that removes the limitations associated with hardware utilization and platform compatibility. It allows you to run multiple AI models on different hardware environments (CPU, CUDA, DirectML) simultaneously in a single instance, solving the original library’s constraint of choosing only one type of hardware at a time. The goal is to maintain code similarity with the original library, while enhancing flexibility and scalability.

## Setup
I will be making a setup guide in the near future. I've had a great deal of headache getting NuGet to work. Even when configured to add the necessary DLL's in the output directory. For the life of me, I cannot get the code to work exactly correct. Thing begin to fail when they should not. I have bugs that I cannot replicate at all within the native solution. For example. If I load a model with CPU first then DirectML utilizing the NuGet package. This will cause a massive error. But if you run DirectML first then CPU and then dispose and then do CPU then DirectML, the issue no longer persists. Yet I cannot replicate this issue at all in any unit test within the native solution environment. Not even mentioning the build package issues I'm having.

I'll be working this out, but I'll likely be making zipped versions of the projects you'd need to import or update manually as necessary. I'll work on getting a NuGet package working in time. I try to do this as a hobby and for fun. But good lord this process annoyed me more than any other. If you have ideas as to how to fix the issue or care to help. Please get in contact with me, I'm more than happy to recieve support.

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


### Nuget (Nuget version is bugged) DO NOT USE

When using the `MagicOnnxRuntimeGenAi` packages, you may need to manually configure the output settings in your project file (`.csproj`). The following instructions outline how to correctly set up the configurations for CPU, CUDA, and DirectML packages.

I'm still trying to figure out how to make this part deploy automatically but until then.

**NUGET ISSUES** are very real. I have no idea what's going on, but there's very odd quirks when using the code via nuget. Even when the environment should work identical, I'm having various issues that cannot be reproduced in the native solution environment. I'll work out the kinks over time, but if you want a more seamless experience. It's best you clone down the projects necessary to your project. Will be annoying to update them, but I apologize. I do this for free and as a hobby. I can only dedicate but so much time and effort to open sourcing. This annoys me as well.

#### 1. Setting Up the CPU Package

CPU:
https://www.nuget.org/packages/MagicOnnxRuntimeGenAi.Cpu/0.4.0.3

To set up the `MagicOnnxRuntimeGenAi.Cpu` package:

1. Locate the `PackageReference` for `MagicOnnxRuntimeGenAi.Cpu` in your `.csproj` file.
   ```xml
   <PackageReference Include="MagicOnnxRuntimeGenAi.Cpu" Version="VersionNumber" GeneratePathProperty="true" />
   ```
   Make sure that `GeneratePathProperty="true"` is included, as shown above.

2. Add the following `ItemGroup` to the bottom of your `.csproj` file to copy the required CPU files:
   ```xml
   <ItemGroup>
       <None Include="$(PkgMagiconnxruntimegenai_cpu)\contentFiles\any\any\cpu\**\*">
           <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
           <Link>cpu\%(RecursiveDir)%(FileName)%(Extension)</Link>
       </None>
   </ItemGroup>
   ```

#### 2. Setting Up the CUDA Package

Cuda:
https://www.nuget.org/packages/MagicOnnxRuntimeGenAi.Cuda/0.4.0.3

The main Cuda DLL's I'll be hosting on HuggingFace. These files will download in runtime due to free storage limitations. Plus maybe that's the better way anyways. It's the exact DLL's from Microsoft. You can use Microsofts official ones for the OnnxRuntimeGenAI library if you'd like, just go to their github:


Microsoft GenAI Github: https://github.com/microsoft/onnxruntime-genai

My HuggingFace Dataset hosted DLLs: https://huggingface.co/datasets/magiccodingman/MagicOnnxRuntimeGenAI


If using the CUDA package, follow these steps:

1. Locate the `PackageReference` for `MagicOnnxRuntimeGenAi.Cuda`.
   ```xml
   <PackageReference Include="MagicOnnxRuntimeGenAi.Cuda" Version="VersionNumber" GeneratePathProperty="true" />
   ```
   Again, make sure to include `GeneratePathProperty="true"` in the reference.

2. Add the following `ItemGroup` at the bottom of your `.csproj` file to copy the necessary CUDA files:
   ```xml
   <ItemGroup>
       <None Include="$(PkgMagiconnxruntimegenai_cuda)\contentFiles\any\any\cuda\**\*"> 
           <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
           <Link>cuda\%(RecursiveDir)%(FileName)%(Extension)</Link> 
       </None>
   </ItemGroup>
   ```

#### 3. Setting Up the DirectML Package

DirectML:
https://www.nuget.org/packages/MagicOnnxRuntimeGenAi.DirectML/0.4.0.3

For DirectML support:

1. Locate the `PackageReference` for `MagicOnnxRuntimeGenAi.DirectML` and ensure it includes `GeneratePathProperty="true"`.
   ```xml
   <PackageReference Include="MagicOnnxRuntimeGenAi.DirectML" Version="VersionNumber" GeneratePathProperty="true" />
   ```

2. Add the following `ItemGroup` elements to include the DirectML files:
   ```xml
   <ItemGroup>
       <None Include="$(PkgMagiconnxruntimegenai_directml)\contentFiles\any\any\dml\dml\**\*"> 
           <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
           <Link>dml\%(RecursiveDir)%(FileName)%(Extension)</Link> 
       </None>
   </ItemGroup>
   
   <ItemGroup>
       <None Include="$(PkgMagiconnxruntimegenai_directml)\contentFiles\any\any\D3D12Core.dll">
           <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
       </None>
   </ItemGroup>
   
   <ItemGroup>
       <None Include="$(PkgMagiconnxruntimegenai_directml)\contentFiles\any\any\DirectML.dll">
           <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
       </None>
   </ItemGroup>
   ```
