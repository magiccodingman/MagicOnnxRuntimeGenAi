
using System;

namespace MagicOnnxRuntimeGenAi
{
//GenAI Nuget Version: 0.4.0
public partial class MagicNativeMethods
{
    public MagicNativeMethods(HardwareType hardwareType)
    {
        _hardwareType = hardwareType;
        SetLibraryPath();
    }

    private delegate IntPtr /* const char* */ OgaResultGetErrorDelegate(IntPtr /* const OgaResult* */ result);
    public IntPtr /* const char* */ OgaResultGetError(IntPtr /* const OgaResult* */ result)
    {
        var method = GetNativeMethod<OgaResultGetErrorDelegate>("OgaResultGetError");
        return method(result);
    }

    private delegate IntPtr /* OgaResult */ OgaSetLogBoolDelegate(byte[] /* const char* */ name, bool value);
    public IntPtr /* OgaResult */ OgaSetLogBool(byte[] /* const char* */ name, bool value)
    {
        var method = GetNativeMethod<OgaSetLogBoolDelegate>("OgaSetLogBool");
        return method(name, value);
    }

    private delegate IntPtr /* OgaResult */ OgaSetLogStringDelegate(byte[] /* const char* */ name, byte[] /* const char* */ value);
    public IntPtr /* OgaResult */ OgaSetLogString(byte[] /* const char* */ name, byte[] /* const char* */ value)
    {
        var method = GetNativeMethod<OgaSetLogStringDelegate>("OgaSetLogString");
        return method(name, value);
    }

    private delegate void OgaDestroyResultDelegate(IntPtr /* OgaResult* */ result);
    public void OgaDestroyResult(IntPtr /* OgaResult* */ result)
    {
        var method = GetNativeMethod<OgaDestroyResultDelegate>("OgaDestroyResult");
        method(result);
    }

    private delegate IntPtr /* OgaResult* */ OgaCreateModelDelegate(byte[] /* const char* */ configPath, out IntPtr /* OgaModel** */ model);
    public IntPtr /* OgaResult* */ OgaCreateModel(byte[] /* const char* */ configPath, out IntPtr /* OgaModel** */ model)
    {
        var method = GetNativeMethod<OgaCreateModelDelegate>("OgaCreateModel");
        return method(configPath, out model);
    }

    private delegate void OgaDestroyModelDelegate(IntPtr /* OgaModel* */ model);
    public void OgaDestroyModel(IntPtr /* OgaModel* */ model)
    {
        var method = GetNativeMethod<OgaDestroyModelDelegate>("OgaDestroyModel");
        method(model);
    }

    private delegate IntPtr /* OgaResult* */ OgaCreateGeneratorParamsDelegate(IntPtr /* const OgaModel* */ model, out IntPtr /* OgaGeneratorParams** */ generatorParams);
    public IntPtr /* OgaResult* */ OgaCreateGeneratorParams(IntPtr /* const OgaModel* */ model, out IntPtr /* OgaGeneratorParams** */ generatorParams)
    {
        var method = GetNativeMethod<OgaCreateGeneratorParamsDelegate>("OgaCreateGeneratorParams");
        return method(model, out generatorParams);
    }

    private delegate void OgaDestroyGeneratorParamsDelegate(IntPtr /* OgaGeneratorParams* */ generatorParams);
    public void OgaDestroyGeneratorParams(IntPtr /* OgaGeneratorParams* */ generatorParams)
    {
        var method = GetNativeMethod<OgaDestroyGeneratorParamsDelegate>("OgaDestroyGeneratorParams");
        method(generatorParams);
    }

    private delegate IntPtr /* OgaResult* */ OgaGeneratorParamsSetSearchNumberDelegate(IntPtr /* OgaGeneratorParams* */ generatorParams, byte[] /* const char* */ searchOption, double value);
    public IntPtr /* OgaResult* */ OgaGeneratorParamsSetSearchNumber(IntPtr /* OgaGeneratorParams* */ generatorParams, byte[] /* const char* */ searchOption, double value)
    {
        var method = GetNativeMethod<OgaGeneratorParamsSetSearchNumberDelegate>("OgaGeneratorParamsSetSearchNumber");
        return method(generatorParams, searchOption, value);
    }

    private delegate IntPtr /* OgaResult* */ OgaGeneratorParamsSetSearchBoolDelegate(IntPtr /* OgaGeneratorParams* */ generatorParams, byte[] /* const char* */ searchOption, bool value);
    public IntPtr /* OgaResult* */ OgaGeneratorParamsSetSearchBool(IntPtr /* OgaGeneratorParams* */ generatorParams, byte[] /* const char* */ searchOption, bool value)
    {
        var method = GetNativeMethod<OgaGeneratorParamsSetSearchBoolDelegate>("OgaGeneratorParamsSetSearchBool");
        return method(generatorParams, searchOption, value);
    }

    private delegate IntPtr /* OgaResult* */ OgaGeneratorParamsTryGraphCaptureWithMaxBatchSizeDelegate(IntPtr /* OgaGeneratorParams* */ generatorParams, int /* int32_t */ maxBatchSize);
    public IntPtr /* OgaResult* */ OgaGeneratorParamsTryGraphCaptureWithMaxBatchSize(IntPtr /* OgaGeneratorParams* */ generatorParams, int /* int32_t */ maxBatchSize)
    {
        var method = GetNativeMethod<OgaGeneratorParamsTryGraphCaptureWithMaxBatchSizeDelegate>("OgaGeneratorParamsTryGraphCaptureWithMaxBatchSize");
        return method(generatorParams, maxBatchSize);
    }

    private unsafe delegate IntPtr /* OgaResult* */ OgaGeneratorParamsSetInputIDsDelegate(IntPtr /* OgaGeneratorParams* */ generatorParams, int* /* const int32_t* */ inputIDs, UIntPtr /* size_t */ inputIDsCount, UIntPtr /* size_t */ sequenceLength, UIntPtr /* size_t */ batchSize);
    public unsafe IntPtr /* OgaResult* */ OgaGeneratorParamsSetInputIDs(IntPtr /* OgaGeneratorParams* */ generatorParams, int* /* const int32_t* */ inputIDs, UIntPtr /* size_t */ inputIDsCount, UIntPtr /* size_t */ sequenceLength, UIntPtr /* size_t */ batchSize)
    {
        var method = GetNativeMethod<OgaGeneratorParamsSetInputIDsDelegate>("OgaGeneratorParamsSetInputIDs");
        return method(generatorParams, inputIDs, inputIDsCount, sequenceLength, batchSize);
    }

    private delegate IntPtr /* OgaResult* */ OgaGeneratorParamsSetInputSequencesDelegate(IntPtr /* OgaGeneratorParams* */ generatorParams, IntPtr /* const OgaSequences* */ sequences);
    public IntPtr /* OgaResult* */ OgaGeneratorParamsSetInputSequences(IntPtr /* OgaGeneratorParams* */ generatorParams, IntPtr /* const OgaSequences* */ sequences)
    {
        var method = GetNativeMethod<OgaGeneratorParamsSetInputSequencesDelegate>("OgaGeneratorParamsSetInputSequences");
        return method(generatorParams, sequences);
    }

    private delegate IntPtr /* OgaResult* */ OgaGeneratorParamsSetModelInputDelegate(IntPtr /* OgaGeneratorParams* */ generatorParams, byte[] /* const char* */ name, IntPtr /* const OgaTensor* */ tensor);
    public IntPtr /* OgaResult* */ OgaGeneratorParamsSetModelInput(IntPtr /* OgaGeneratorParams* */ generatorParams, byte[] /* const char* */ name, IntPtr /* const OgaTensor* */ tensor)
    {
        var method = GetNativeMethod<OgaGeneratorParamsSetModelInputDelegate>("OgaGeneratorParamsSetModelInput");
        return method(generatorParams, name, tensor);
    }

    private delegate IntPtr /* OgaResult* */ OgaGeneratorParamsSetInputsDelegate(IntPtr /* OgaGeneratorParams* */ generatorParams, IntPtr /* const OgaNamedTensors* */ namedTensors);
    public IntPtr /* OgaResult* */ OgaGeneratorParamsSetInputs(IntPtr /* OgaGeneratorParams* */ generatorParams, IntPtr /* const OgaNamedTensors* */ namedTensors)
    {
        var method = GetNativeMethod<OgaGeneratorParamsSetInputsDelegate>("OgaGeneratorParamsSetInputs");
        return method(generatorParams, namedTensors);
    }

    private delegate IntPtr /* OgaResult* */ OgaCreateGeneratorDelegate(IntPtr /* const OgaModel* */ model, IntPtr /* const OgaGeneratorParams* */ generatorParams, out IntPtr /* OgaGenerator** */ generator);
    public IntPtr /* OgaResult* */ OgaCreateGenerator(IntPtr /* const OgaModel* */ model, IntPtr /* const OgaGeneratorParams* */ generatorParams, out IntPtr /* OgaGenerator** */ generator)
    {
        var method = GetNativeMethod<OgaCreateGeneratorDelegate>("OgaCreateGenerator");
        return method(model, generatorParams, out generator);
    }

    private delegate void OgaDestroyGeneratorDelegate(IntPtr /* OgaGenerator* */ generator);
    public void OgaDestroyGenerator(IntPtr /* OgaGenerator* */ generator)
    {
        var method = GetNativeMethod<OgaDestroyGeneratorDelegate>("OgaDestroyGenerator");
        method(generator);
    }

    private delegate bool OgaGenerator_IsDoneDelegate(IntPtr /* const OgaGenerator* */ generator);
    public bool OgaGenerator_IsDone(IntPtr /* const OgaGenerator* */ generator)
    {
        var method = GetNativeMethod<OgaGenerator_IsDoneDelegate>("OgaGenerator_IsDone");
        return method(generator);
    }

    private delegate IntPtr /* OgaResult* */ OgaGenerator_ComputeLogitsDelegate(IntPtr /* OgaGenerator* */ generator);
    public IntPtr /* OgaResult* */ OgaGenerator_ComputeLogits(IntPtr /* OgaGenerator* */ generator)
    {
        var method = GetNativeMethod<OgaGenerator_ComputeLogitsDelegate>("OgaGenerator_ComputeLogits");
        return method(generator);
    }

    private delegate IntPtr /* OgaResult* */ OgaGenerator_GenerateNextTokenDelegate(IntPtr /* OgaGenerator* */ generator);
    public IntPtr /* OgaResult* */ OgaGenerator_GenerateNextToken(IntPtr /* OgaGenerator* */ generator)
    {
        var method = GetNativeMethod<OgaGenerator_GenerateNextTokenDelegate>("OgaGenerator_GenerateNextToken");
        return method(generator);
    }

    private delegate UIntPtr /* size_t */ OgaGenerator_GetSequenceCountDelegate(IntPtr /* const OgaGenerator* */ generator, UIntPtr /* size_t */ index);
    public UIntPtr /* size_t */ OgaGenerator_GetSequenceCount(IntPtr /* const OgaGenerator* */ generator, UIntPtr /* size_t */ index)
    {
        var method = GetNativeMethod<OgaGenerator_GetSequenceCountDelegate>("OgaGenerator_GetSequenceCount");
        return method(generator, index);
    }

    private delegate IntPtr /* const in32_t* */ OgaGenerator_GetSequenceDataDelegate(IntPtr /* const OgaGenerator* */ generator, UIntPtr /* size_t */ index);
    public IntPtr /* const in32_t* */ OgaGenerator_GetSequenceData(IntPtr /* const OgaGenerator* */ generator, UIntPtr /* size_t */ index)
    {
        var method = GetNativeMethod<OgaGenerator_GetSequenceDataDelegate>("OgaGenerator_GetSequenceData");
        return method(generator, index);
    }

    private delegate IntPtr /* OgaResult* */ OgaCreateSequencesDelegate(out IntPtr /* OgaSequences** */ sequences);
    public IntPtr /* OgaResult* */ OgaCreateSequences(out IntPtr /* OgaSequences** */ sequences)
    {
        var method = GetNativeMethod<OgaCreateSequencesDelegate>("OgaCreateSequences");
        return method(out sequences);
    }

    private delegate void OgaDestroySequencesDelegate(IntPtr /* OgaSequences* */ sequences);
    public void OgaDestroySequences(IntPtr /* OgaSequences* */ sequences)
    {
        var method = GetNativeMethod<OgaDestroySequencesDelegate>("OgaDestroySequences");
        method(sequences);
    }

    private delegate UIntPtr OgaSequencesCountDelegate(IntPtr /* const OgaSequences* */ sequences);
    public UIntPtr OgaSequencesCount(IntPtr /* const OgaSequences* */ sequences)
    {
        var method = GetNativeMethod<OgaSequencesCountDelegate>("OgaSequencesCount");
        return method(sequences);
    }

    private delegate UIntPtr OgaSequencesGetSequenceCountDelegate(IntPtr /* const OgaSequences* */ sequences, UIntPtr /* size_t */ sequenceIndex);
    public UIntPtr OgaSequencesGetSequenceCount(IntPtr /* const OgaSequences* */ sequences, UIntPtr /* size_t */ sequenceIndex)
    {
        var method = GetNativeMethod<OgaSequencesGetSequenceCountDelegate>("OgaSequencesGetSequenceCount");
        return method(sequences, sequenceIndex);
    }

    private delegate IntPtr /* const int32_t* */ OgaSequencesGetSequenceDataDelegate(IntPtr /* const OgaSequences* */ sequences, UIntPtr /* size_t */ sequenceIndex);
    public IntPtr /* const int32_t* */ OgaSequencesGetSequenceData(IntPtr /* const OgaSequences* */ sequences, UIntPtr /* size_t */ sequenceIndex)
    {
        var method = GetNativeMethod<OgaSequencesGetSequenceDataDelegate>("OgaSequencesGetSequenceData");
        return method(sequences, sequenceIndex);
    }

    private delegate IntPtr /* OgaResult* */ OgaGenerateDelegate(IntPtr /* const OgaModel* */ model, IntPtr /* const OgaGeneratorParams* */ generatorParams, out IntPtr /* OgaSequences** */ sequences);
    public IntPtr /* OgaResult* */ OgaGenerate(IntPtr /* const OgaModel* */ model, IntPtr /* const OgaGeneratorParams* */ generatorParams, out IntPtr /* OgaSequences** */ sequences)
    {
        var method = GetNativeMethod<OgaGenerateDelegate>("OgaGenerate");
        return method(model, generatorParams, out sequences);
    }

    private delegate IntPtr /* OgaResult* */ OgaCreateTokenizerDelegate(IntPtr /* const OgaModel* */ model, out IntPtr /* OgaTokenizer** */ tokenizer);
    public IntPtr /* OgaResult* */ OgaCreateTokenizer(IntPtr /* const OgaModel* */ model, out IntPtr /* OgaTokenizer** */ tokenizer)
    {
        var method = GetNativeMethod<OgaCreateTokenizerDelegate>("OgaCreateTokenizer");
        return method(model, out tokenizer);
    }

    private delegate void OgaDestroyTokenizerDelegate(IntPtr /* OgaTokenizer* */ tokenizer);
    public void OgaDestroyTokenizer(IntPtr /* OgaTokenizer* */ tokenizer)
    {
        var method = GetNativeMethod<OgaDestroyTokenizerDelegate>("OgaDestroyTokenizer");
        method(tokenizer);
    }

    private delegate IntPtr /* OgaResult* */ OgaTokenizerEncodeDelegate(IntPtr /* const OgaTokenizer* */ tokenizer, byte[] /* const char* */ strings, IntPtr /* OgaSequences* */ sequences);
    public IntPtr /* OgaResult* */ OgaTokenizerEncode(IntPtr /* const OgaTokenizer* */ tokenizer, byte[] /* const char* */ strings, IntPtr /* OgaSequences* */ sequences)
    {
        var method = GetNativeMethod<OgaTokenizerEncodeDelegate>("OgaTokenizerEncode");
        return method(tokenizer, strings, sequences);
    }

    private unsafe delegate IntPtr /* OgaResult* */ OgaTokenizerDecodeDelegate(IntPtr /* const OgaTokenizer* */ tokenizer, int* /* const int32_t* */ sequence, UIntPtr /* size_t */ sequenceLength, out IntPtr /* const char** */ outStr);
    public unsafe IntPtr /* OgaResult* */ OgaTokenizerDecode(IntPtr /* const OgaTokenizer* */ tokenizer, int* /* const int32_t* */ sequence, UIntPtr /* size_t */ sequenceLength, out IntPtr /* const char** */ outStr)
    {
        var method = GetNativeMethod<OgaTokenizerDecodeDelegate>("OgaTokenizerDecode");
        return method(tokenizer, sequence, sequenceLength, out outStr);
    }

    private delegate void OgaDestroyStringDelegate(IntPtr /* const char* */ str);
    public void OgaDestroyString(IntPtr /* const char* */ str)
    {
        var method = GetNativeMethod<OgaDestroyStringDelegate>("OgaDestroyString");
        method(str);
    }

    private delegate IntPtr /* OgaResult* */ OgaCreateTokenizerStreamDelegate(IntPtr /* const OgaTokenizer* */ tokenizer, out IntPtr /* OgaTokenizerStream** */ tokenizerStream);
    public IntPtr /* OgaResult* */ OgaCreateTokenizerStream(IntPtr /* const OgaTokenizer* */ tokenizer, out IntPtr /* OgaTokenizerStream** */ tokenizerStream)
    {
        var method = GetNativeMethod<OgaCreateTokenizerStreamDelegate>("OgaCreateTokenizerStream");
        return method(tokenizer, out tokenizerStream);
    }

    private delegate IntPtr /* OgaResult* */ OgaCreateTokenizerStreamFromProcessorDelegate(IntPtr /* const OgaMultiModalProcessor* */ processor, out IntPtr /* OgaTokenizerStream** */ tokenizerStream);
    public IntPtr /* OgaResult* */ OgaCreateTokenizerStreamFromProcessor(IntPtr /* const OgaMultiModalProcessor* */ processor, out IntPtr /* OgaTokenizerStream** */ tokenizerStream)
    {
        var method = GetNativeMethod<OgaCreateTokenizerStreamFromProcessorDelegate>("OgaCreateTokenizerStreamFromProcessor");
        return method(processor, out tokenizerStream);
    }

    private delegate void OgaDestroyTokenizerStreamDelegate(IntPtr /* OgaTokenizerStream* */ tokenizerStream);
    public void OgaDestroyTokenizerStream(IntPtr /* OgaTokenizerStream* */ tokenizerStream)
    {
        var method = GetNativeMethod<OgaDestroyTokenizerStreamDelegate>("OgaDestroyTokenizerStream");
        method(tokenizerStream);
    }

    private delegate IntPtr /* OgaResult* */ OgaTokenizerStreamDecodeDelegate(IntPtr /* const OgaTokenizerStream* */ tokenizerStream, int /* int32_t */ token, out IntPtr /* const char** */ outStr);
    public IntPtr /* OgaResult* */ OgaTokenizerStreamDecode(IntPtr /* const OgaTokenizerStream* */ tokenizerStream, int /* int32_t */ token, out IntPtr /* const char** */ outStr)
    {
        var method = GetNativeMethod<OgaTokenizerStreamDecodeDelegate>("OgaTokenizerStreamDecode");
        return method(tokenizerStream, token, out outStr);
    }

    private delegate IntPtr /* OgaResult* */ OgaCreateTensorFromBufferDelegate(IntPtr /* data* */ data, long[] shapeDims, UIntPtr shapeDimsCount, ElementType elementType, out IntPtr /* OgaTensor** */ tensor);
    public IntPtr /* OgaResult* */ OgaCreateTensorFromBuffer(IntPtr /* data* */ data, long[] shapeDims, UIntPtr shapeDimsCount, ElementType elementType, out IntPtr /* OgaTensor** */ tensor)
    {
        var method = GetNativeMethod<OgaCreateTensorFromBufferDelegate>("OgaCreateTensorFromBuffer");
        return method(data, shapeDims, shapeDimsCount, elementType, out tensor);
    }

    private delegate void OgaDestroyTensorDelegate(IntPtr /* OgaTensor * */ tensor);
    public void OgaDestroyTensor(IntPtr /* OgaTensor * */ tensor)
    {
        var method = GetNativeMethod<OgaDestroyTensorDelegate>("OgaDestroyTensor");
        method(tensor);
    }

    private delegate IntPtr /* OgaResult* */ OgaTensorGetTypeDelegate(IntPtr /* OgaTensor * */ tensor, out ElementType elementType);
    public IntPtr /* OgaResult* */ OgaTensorGetType(IntPtr /* OgaTensor * */ tensor, out ElementType elementType)
    {
        var method = GetNativeMethod<OgaTensorGetTypeDelegate>("OgaTensorGetType");
        return method(tensor, out elementType);
    }

    private delegate IntPtr /* OgaResult* */ OgaTensorGetShapeRankDelegate(IntPtr /* OgaTensor * */ tensor, out UIntPtr rank);
    public IntPtr /* OgaResult* */ OgaTensorGetShapeRank(IntPtr /* OgaTensor * */ tensor, out UIntPtr rank)
    {
        var method = GetNativeMethod<OgaTensorGetShapeRankDelegate>("OgaTensorGetShapeRank");
        return method(tensor, out rank);
    }

    private delegate IntPtr /* OgaResult* */ OgaTensorGetShapeDelegate(IntPtr /* OgaTensor * */ tensor, long[] shapeDims, UIntPtr /* size_t */ shapeDimsCount);
    public IntPtr /* OgaResult* */ OgaTensorGetShape(IntPtr /* OgaTensor * */ tensor, long[] shapeDims, UIntPtr /* size_t */ shapeDimsCount)
    {
        var method = GetNativeMethod<OgaTensorGetShapeDelegate>("OgaTensorGetShape");
        return method(tensor, shapeDims, shapeDimsCount);
    }

    private delegate IntPtr /* OgaResult* */ OgaTensorGetDataDelegate(IntPtr /* OgaTensor * */ tensor, out IntPtr /* void* */ data);
    public IntPtr /* OgaResult* */ OgaTensorGetData(IntPtr /* OgaTensor * */ tensor, out IntPtr /* void* */ data)
    {
        var method = GetNativeMethod<OgaTensorGetDataDelegate>("OgaTensorGetData");
        return method(tensor, out data);
    }

    private delegate IntPtr /* OgaResult* */ OgaSetCurrentGpuDeviceIdDelegate(int /* int32_t */ deviceId);
    public IntPtr /* OgaResult* */ OgaSetCurrentGpuDeviceId(int /* int32_t */ deviceId)
    {
        var method = GetNativeMethod<OgaSetCurrentGpuDeviceIdDelegate>("OgaSetCurrentGpuDeviceId");
        return method(deviceId);
    }

    private delegate IntPtr /* OgaResult* */ OgaGetCurrentGpuDeviceIdDelegate(out IntPtr /* int32_t */ deviceId);
    public IntPtr /* OgaResult* */ OgaGetCurrentGpuDeviceId(out IntPtr /* int32_t */ deviceId)
    {
        var method = GetNativeMethod<OgaGetCurrentGpuDeviceIdDelegate>("OgaGetCurrentGpuDeviceId");
        return method(out deviceId);
    }

    private delegate void OgaShutdownDelegate();
    public void OgaShutdown()
    {
        var method = GetNativeMethod<OgaShutdownDelegate>("OgaShutdown");
        method();
    }

    private delegate IntPtr /* OgaResult* */ OgaCreateMultiModalProcessorDelegate(IntPtr /* const OgaModel* */ model, out IntPtr /* OgaMultiModalProcessor** */ processor);
    public IntPtr /* OgaResult* */ OgaCreateMultiModalProcessor(IntPtr /* const OgaModel* */ model, out IntPtr /* OgaMultiModalProcessor** */ processor)
    {
        var method = GetNativeMethod<OgaCreateMultiModalProcessorDelegate>("OgaCreateMultiModalProcessor");
        return method(model, out processor);
    }

    private delegate void OgaDestroyMultiModalProcessorDelegate(IntPtr /* OgaMultiModalProcessor* */ processor);
    public void OgaDestroyMultiModalProcessor(IntPtr /* OgaMultiModalProcessor* */ processor)
    {
        var method = GetNativeMethod<OgaDestroyMultiModalProcessorDelegate>("OgaDestroyMultiModalProcessor");
        method(processor);
    }

    private delegate IntPtr /* OgaResult* */ OgaProcessorProcessImagesDelegate(IntPtr /* const OgaMultiModalProcessor* */ processor, byte[] /* const char* */ prompt, IntPtr /* const Images* */ images, out IntPtr /* OgaNamedTensors** */ namedTensors);
    public IntPtr /* OgaResult* */ OgaProcessorProcessImages(IntPtr /* const OgaMultiModalProcessor* */ processor, byte[] /* const char* */ prompt, IntPtr /* const Images* */ images, out IntPtr /* OgaNamedTensors** */ namedTensors)
    {
        var method = GetNativeMethod<OgaProcessorProcessImagesDelegate>("OgaProcessorProcessImages");
        return method(processor, prompt, images, out namedTensors);
    }

    private unsafe delegate IntPtr /* OgaResult* */ OgaProcessorDecodeDelegate(IntPtr /* const OgaMultiModalProcessor* */ processor, int* /* const int32_t* */ sequence, UIntPtr /* size_t */ sequenceLength, out IntPtr /* const char** */ outStr);
    public unsafe IntPtr /* OgaResult* */ OgaProcessorDecode(IntPtr /* const OgaMultiModalProcessor* */ processor, int* /* const int32_t* */ sequence, UIntPtr /* size_t */ sequenceLength, out IntPtr /* const char** */ outStr)
    {
        var method = GetNativeMethod<OgaProcessorDecodeDelegate>("OgaProcessorDecode");
        return method(processor, sequence, sequenceLength, out outStr);
    }

    private delegate IntPtr /* OgaResult* */ OgaLoadImagesDelegate(IntPtr /* const OgaStringArray* */ imagePaths, out IntPtr /* const OgaImages** */ images);
    public IntPtr /* OgaResult* */ OgaLoadImages(IntPtr /* const OgaStringArray* */ imagePaths, out IntPtr /* const OgaImages** */ images)
    {
        var method = GetNativeMethod<OgaLoadImagesDelegate>("OgaLoadImages");
        return method(imagePaths, out images);
    }

    private delegate void OgaDestroyImagesDelegate(IntPtr /* OgaImages* */ images);
    public void OgaDestroyImages(IntPtr /* OgaImages* */ images)
    {
        var method = GetNativeMethod<OgaDestroyImagesDelegate>("OgaDestroyImages");
        method(images);
    }

    private delegate void OgaDestroyNamedTensorsDelegate(IntPtr /* OgaNamedTensors* */ namedTensors);
    public void OgaDestroyNamedTensors(IntPtr /* OgaNamedTensors* */ namedTensors)
    {
        var method = GetNativeMethod<OgaDestroyNamedTensorsDelegate>("OgaDestroyNamedTensors");
        method(namedTensors);
    }

    private delegate IntPtr /* OgaResult* */ OgaCreateStringArrayDelegate(out IntPtr /* OgaStringArray** */ stringArray);
    public IntPtr /* OgaResult* */ OgaCreateStringArray(out IntPtr /* OgaStringArray** */ stringArray)
    {
        var method = GetNativeMethod<OgaCreateStringArrayDelegate>("OgaCreateStringArray");
        return method(out stringArray);
    }

    private delegate IntPtr /* OgaResult* */ OgaStringArrayAddStringDelegate(IntPtr /* OgaStringArray* */ stringArray, byte[] /* const char* */ str);
    public IntPtr /* OgaResult* */ OgaStringArrayAddString(IntPtr /* OgaStringArray* */ stringArray, byte[] /* const char* */ str)
    {
        var method = GetNativeMethod<OgaStringArrayAddStringDelegate>("OgaStringArrayAddString");
        return method(stringArray, str);
    }

    private delegate void OgaDestroyStringArrayDelegate(IntPtr /* OgaStringArray* */ stringArray);
    public void OgaDestroyStringArray(IntPtr /* OgaStringArray* */ stringArray)
    {
        var method = GetNativeMethod<OgaDestroyStringArrayDelegate>("OgaDestroyStringArray");
        method(stringArray);
    }
}
}