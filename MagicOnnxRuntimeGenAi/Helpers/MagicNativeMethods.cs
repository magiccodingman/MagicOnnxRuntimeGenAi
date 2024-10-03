using MagicOnnxRuntimeGenAi.Helpers;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MagicOnnxRuntimeGenAi
{
    public class MagicNativeMethods
    {
        private string _libraryPath;
        private HardwareType _hardwareType;


        public MagicNativeMethods(HardwareType hardwareType)
        {
            _hardwareType = hardwareType;
            // Initialize with the default hardware type (assuming CPU as default)
            //SetLibraryPath(HardwareType.cpu);
            SetLibraryPath();
        }

        // Set the library path based on the hardware type and platform
        public void SetLibraryPath()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string runtimeFolder = string.Empty;

            switch (_hardwareType)
            {
                case HardwareType.cpu:
                    runtimeFolder = NativeLibraryLoader.GetNativeDllPath(Path.Combine(baseDirectory, "cpu"));
                    break;
                case HardwareType.cuda:
                    runtimeFolder = NativeLibraryLoader.GetNativeDllPath(Path.Combine(baseDirectory, "cuda"));
                    break;
                case HardwareType.dml:
                    runtimeFolder = NativeLibraryLoader.GetNativeDllPath(Path.Combine(baseDirectory, "dml"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_hardwareType), "Unsupported hardware type");
            }

            // Detect OS platform
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _libraryPath = Path.Combine(runtimeFolder, "onnxruntime-genai.dll");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _libraryPath = Path.Combine(runtimeFolder, "libonnxruntime-genai.so");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                _libraryPath = Path.Combine(runtimeFolder, "libonnxruntime-genai.dylib");
            }
#if __ANDROID__
else
{
    _libraryPath = Path.Combine(runtimeFolder, "onnxruntime.aar"); // Handle .aar differently as it's a package
}
#elif __IOS__
else
{
    _libraryPath = Path.Combine(runtimeFolder, "onnxruntime.xcframework"); // Handle .xcframework differently
}
#else
            else
            {
                throw new PlatformNotSupportedException("Unsupported platform for onnxruntime-genai");
            }
#endif

            if (!File.Exists(_libraryPath))
            {
                throw new DllNotFoundException($"Native library not found at {_libraryPath}");
            }
        }

        // Declare a delegate for dynamically loading native methods
        private delegate IntPtr OgaTokenizerEncodeDelegate(IntPtr tokenizer, byte[] strings, IntPtr sequences);
        public unsafe delegate IntPtr OgaTokenizerDecodeDelegate(IntPtr tokenizer, int* sequence, UIntPtr sequenceLength, out IntPtr outStr);
        private delegate void OgaDestroyStringDelegate(IntPtr str);
        private delegate IntPtr OgaCreateTokenizerStreamDelegate(IntPtr tokenizer, out IntPtr tokenizerStream);
        private delegate IntPtr OgaCreateTokenizerStreamFromProcessorDelegate(IntPtr tprocessor, out IntPtr tokenizerStream);
        private delegate void OgaDestroyTokenizerStreamDelegate(IntPtr tokenizerStream);
        private delegate IntPtr OgaTokenizerStreamDecodeDelegate(IntPtr tokenizerStream, int token, out IntPtr outStr);
        private delegate IntPtr OgaCreateTensorFromBufferDelegate(IntPtr data, long[] shape_dims, UIntPtr shape_dims_count, ElementType element_Type, out IntPtr tensor);
        private delegate void OgaDestroyTensorDelegate(IntPtr tensor);
        private delegate IntPtr OgaTensorGetTypeDelegate(IntPtr tensor, out ElementType element_type);
        private delegate IntPtr OgaTensorGetShapeRankDelegate(IntPtr tensor, out UIntPtr rank);
        private delegate IntPtr OgaTensorGetShapeDelegate(IntPtr tensor, long[] shape_dims, UIntPtr shape_dims_count);
        private delegate IntPtr OgaTensorGetDataDelegate(IntPtr tensor, out IntPtr data);
        private delegate IntPtr OgaSetCurrentGpuDeviceIdDelegate(int device_id);
        private delegate IntPtr OgaGetCurrentGpuDeviceIdDelegate(out IntPtr device_id);
        private delegate void OgaShutdownDelegate();
        private delegate IntPtr OgaCreateMultiModalProcessorDelegate(IntPtr model, out IntPtr processor);
        private delegate void OgaDestroyMultiModalProcessorDelegate(IntPtr processor);
        private delegate IntPtr OgaProcessorProcessImagesDelegate(IntPtr processor, byte[] prompt, IntPtr images, out IntPtr named_tensors);
        public unsafe delegate IntPtr OgaProcessorDecodeDelegate(IntPtr processor, int* sequence, UIntPtr sequenceLength, out IntPtr outStr);
        private delegate IntPtr OgaLoadImageDelegate(byte[] image_path, out IntPtr images);
        private delegate void OgaDestroyImagesDelegate(IntPtr images);
        private delegate void OgaDestroyNamedTensorsDelegate(IntPtr named_tensors);
#if NET8_0_OR_GREATER
private T GetNativeMethod<T>(string methodName) where T : Delegate
{
    IntPtr libraryHandle = NativeLibrary.Load(_libraryPath);
    IntPtr methodPtr = NativeLibrary.GetExport(libraryHandle, methodName);
    return Marshal.GetDelegateForFunctionPointer<T>(methodPtr);
}
#else
        public T GetNativeMethod<T>(string methodName) where T : Delegate
        {
            // Load the library (keep it loaded for the entire app lifecycle, or track when to free it)
            IntPtr libraryHandle = NativeLibraryLoaderHelper.LoadNativeLibrary(_libraryPath);

            if (libraryHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException($"Failed to load library: {_libraryPath}");
            }

            // Get the function pointer for the native method
            IntPtr methodPtr = NativeLibraryLoaderHelper.GetNativeMethodPointer(libraryHandle, methodName);

            if (methodPtr == IntPtr.Zero)
            {
                NativeLibraryLoaderHelper.FreeNativeLibrary(libraryHandle);
                throw new MissingMethodException($"Failed to find method: {methodName}");
            }

            // Convert the function pointer to a delegate of type T
            T methodDelegate = Marshal.GetDelegateForFunctionPointer<T>(methodPtr);

            // Don't free the library here, because the delegate will still rely on the library being loaded
            // Free it only when the application is done using it, or manage the lifetime separately

            return methodDelegate;
        }
#endif

        public IntPtr OgaTokenizerEncode(IntPtr tokenizer, byte[] strings, IntPtr sequences)
        {
            var method = GetNativeMethod<OgaTokenizerEncodeDelegate>("OgaTokenizerEncode");
            return method(tokenizer, strings, sequences);
        }

        public unsafe IntPtr OgaTokenizerDecode(IntPtr tokenizer, int* sequence, UIntPtr sequenceLength, out IntPtr outStr)
        {
            var method = GetNativeMethod<OgaTokenizerDecodeDelegate>("OgaTokenizerDecode");
            return method(tokenizer, sequence, sequenceLength, out outStr);
        }

        public void OgaDestroyString(IntPtr str)
        {
            var method = GetNativeMethod<OgaDestroyStringDelegate>("OgaDestroyString");
            method(str);
        }

        public IntPtr OgaCreateTokenizerStream(IntPtr tokenizer, out IntPtr tokenizerStream)
        {
            var method = GetNativeMethod<OgaCreateTokenizerStreamDelegate>("OgaCreateTokenizerStream");
            return method(tokenizer, out tokenizerStream);
        }

        public IntPtr OgaCreateTokenizerStreamFromProcessor(IntPtr processor, out IntPtr tokenizerStream)
        {
            var method = GetNativeMethod<OgaCreateTokenizerStreamFromProcessorDelegate>("OgaCreateTokenizerStreamFromProcessor");
            return method(processor, out tokenizerStream);
        }

        public void OgaDestroyTokenizerStream(IntPtr tokenizerStream)
        {
            var method = GetNativeMethod<OgaDestroyTokenizerStreamDelegate>("OgaDestroyTokenizerStream");
            method(tokenizerStream);
        }

        public IntPtr OgaTokenizerStreamDecode(IntPtr tokenizerStream, int token, out IntPtr outStr)
        {
            var method = GetNativeMethod<OgaTokenizerStreamDecodeDelegate>("OgaTokenizerStreamDecode");
            return method(tokenizerStream, token, out outStr);
        }

        public IntPtr OgaCreateTensorFromBuffer(IntPtr data, long[] shape_dims, UIntPtr shape_dims_count, ElementType element_Type, out IntPtr tensor)
        {
            var method = GetNativeMethod<OgaCreateTensorFromBufferDelegate>("OgaCreateTensorFromBuffer");
            return method(data, shape_dims, shape_dims_count, element_Type, out tensor);
        }

        public void OgaDestroyTensor(IntPtr tensor)
        {
            var method = GetNativeMethod<OgaDestroyTensorDelegate>("OgaDestroyTensor");
            method(tensor);
        }

        public IntPtr OgaTensorGetType(IntPtr tensor, out ElementType element_type)
        {
            var method = GetNativeMethod<OgaTensorGetTypeDelegate>("OgaTensorGetType");
            return method(tensor, out element_type);
        }

        public IntPtr OgaTensorGetShapeRank(IntPtr tensor, out UIntPtr rank)
        {
            var method = GetNativeMethod<OgaTensorGetShapeRankDelegate>("OgaTensorGetShapeRank");
            return method(tensor, out rank);
        }

        public IntPtr OgaTensorGetShape(IntPtr tensor, long[] shape_dims, UIntPtr shape_dims_count)
        {
            var method = GetNativeMethod<OgaTensorGetShapeDelegate>("OgaTensorGetShape");
            return method(tensor, shape_dims, shape_dims_count);
        }

        public IntPtr OgaTensorGetData(IntPtr tensor, out IntPtr data)
        {
            var method = GetNativeMethod<OgaTensorGetDataDelegate>("OgaTensorGetData");
            return method(tensor, out data);
        }

        public IntPtr OgaSetCurrentGpuDeviceId(int device_id)
        {
            var method = GetNativeMethod<OgaSetCurrentGpuDeviceIdDelegate>("OgaSetCurrentGpuDeviceId");
            return method(device_id);
        }

        public IntPtr OgaGetCurrentGpuDeviceId(out IntPtr device_id)
        {
            var method = GetNativeMethod<OgaGetCurrentGpuDeviceIdDelegate>("OgaGetCurrentGpuDeviceId");
            return method(out device_id);
        }

        public void OgaShutdown()
        {
            var method = GetNativeMethod<OgaShutdownDelegate>("OgaShutdown");
            method();
        }

        public IntPtr OgaCreateMultiModalProcessor(IntPtr model, out IntPtr processor)
        {
            var method = GetNativeMethod<OgaCreateMultiModalProcessorDelegate>("OgaCreateMultiModalProcessor");
            return method(model, out processor);
        }

        public void OgaDestroyMultiModalProcessor(IntPtr processor)
        {
            var method = GetNativeMethod<OgaDestroyMultiModalProcessorDelegate>("OgaDestroyMultiModalProcessor");
            method(processor);
        }

        public IntPtr OgaProcessorProcessImages(IntPtr processor, byte[] prompt, IntPtr images, out IntPtr named_tensors)
        {
            var method = GetNativeMethod<OgaProcessorProcessImagesDelegate>("OgaProcessorProcessImages");
            return method(processor, prompt, images, out named_tensors);
        }

        public unsafe IntPtr OgaProcessorDecode(IntPtr processor, int* sequence, UIntPtr sequenceLength, out IntPtr outStr)
        {
            var method = GetNativeMethod<OgaProcessorDecodeDelegate>("OgaProcessorDecode");
            return method(processor, sequence, sequenceLength, out outStr);
        }

        public IntPtr OgaLoadImage(byte[] image_path, out IntPtr images)
        {
            var method = GetNativeMethod<OgaLoadImageDelegate>("OgaLoadImage");
            return method(image_path, out images);
        }

        public void OgaDestroyImages(IntPtr images)
        {
            var method = GetNativeMethod<OgaDestroyImagesDelegate>("OgaDestroyImages");
            method(images);
        }

        public void OgaDestroyNamedTensors(IntPtr named_tensors)
        {
            var method = GetNativeMethod<OgaDestroyNamedTensorsDelegate>("OgaDestroyNamedTensors");
            method(named_tensors);
        }

        private delegate IntPtr OgaResultGetErrorDelegate(IntPtr result);

        public IntPtr OgaResultGetError(IntPtr result)
        {
            var method = GetNativeMethod<OgaResultGetErrorDelegate>("OgaResultGetError");
            return method(result);
        }

        private delegate IntPtr OgaSetLogBoolDelegate(byte[] name, bool value);

        public IntPtr OgaSetLogBool(byte[] name, bool value)
        {
            var method = GetNativeMethod<OgaSetLogBoolDelegate>("OgaSetLogBool");
            return method(name, value);
        }

        private delegate IntPtr OgaSetLogStringDelegate(byte[] name, byte[] value);

        public IntPtr OgaSetLogString(byte[] name, byte[] value)
        {
            var method = GetNativeMethod<OgaSetLogStringDelegate>("OgaSetLogString");
            return method(name, value);
        }


        private delegate void OgaDestroyResultDelegate(IntPtr result);

        public void OgaDestroyResult(IntPtr result)
        {
            var method = GetNativeMethod<OgaDestroyResultDelegate>("OgaDestroyResult");
            method(result);
        }
        private delegate void OgaDestroyModelDelegate(IntPtr model);

        public void OgaDestroyModel(IntPtr model)
        {
            var method = GetNativeMethod<OgaDestroyModelDelegate>("OgaDestroyModel");
            method(model);
        }
        private delegate IntPtr OgaCreateGeneratorParamsDelegate(IntPtr model, out IntPtr generatorParams);

        public IntPtr OgaCreateGeneratorParams(IntPtr model, out IntPtr generatorParams)
        {
            var method = GetNativeMethod<OgaCreateGeneratorParamsDelegate>("OgaCreateGeneratorParams");
            return method(model, out generatorParams);
        }

        private delegate void OgaDestroyGeneratorDelegate(IntPtr generatorParams);

        public void OgaDestroyGenerator(IntPtr generatorParams)
        {
            var method = GetNativeMethod<OgaDestroyGeneratorDelegate>("OgaDestroyGenerator");
            method(generatorParams);
        }

        private delegate void OgaDestroyGeneratorParamsDelegate(IntPtr generatorParams);

        public void OgaDestroyGeneratorParams(IntPtr generatorParams)
        {
            var method = GetNativeMethod<OgaDestroyGeneratorParamsDelegate>("OgaDestroyGeneratorParams");
            method(generatorParams);
        }

        private delegate IntPtr OgaGeneratorParamsSetSearchNumberDelegate(IntPtr generatorParams, byte[] searchOption, double value);

        public IntPtr OgaGeneratorParamsSetSearchNumber(IntPtr generatorParams, byte[] searchOption, double value)
        {
            var method = GetNativeMethod<OgaGeneratorParamsSetSearchNumberDelegate>("OgaGeneratorParamsSetSearchNumber");
            return method(generatorParams, searchOption, value);
        }

        private delegate IntPtr OgaGeneratorParamsSetSearchBoolDelegate(IntPtr generatorParams, byte[] searchOption, bool value);

        public IntPtr OgaGeneratorParamsSetSearchBool(IntPtr generatorParams, byte[] searchOption, bool value)
        {
            var method = GetNativeMethod<OgaGeneratorParamsSetSearchBoolDelegate>("OgaGeneratorParamsSetSearchBool");
            return method(generatorParams, searchOption, value);
        }
        private delegate IntPtr OgaGeneratorParamsTryGraphCaptureWithMaxBatchSizeDelegate(IntPtr generatorParams, int maxBatchSize);

        public IntPtr OgaGeneratorParamsTryGraphCaptureWithMaxBatchSize(IntPtr generatorParams, int maxBatchSize)
        {
            var method = GetNativeMethod<OgaGeneratorParamsTryGraphCaptureWithMaxBatchSizeDelegate>("OgaGeneratorParamsTryGraphCaptureWithMaxBatchSize");
            return method(generatorParams, maxBatchSize);
        }

        private unsafe delegate IntPtr OgaGeneratorParamsSetInputIDsDelegate(IntPtr generatorParams, int* inputIDs, UIntPtr inputIDsCount, UIntPtr sequenceLength, UIntPtr batchSize);

        public unsafe IntPtr OgaGeneratorParamsSetInputIDs(IntPtr generatorParams, int* inputIDs, UIntPtr inputIDsCount, UIntPtr sequenceLength, UIntPtr batchSize)
        {
            var method = GetNativeMethod<OgaGeneratorParamsSetInputIDsDelegate>("OgaGeneratorParamsSetInputIDs");
            return method(generatorParams, inputIDs, inputIDsCount, sequenceLength, batchSize);
        }

        private delegate IntPtr OgaCreateModelDelegate(byte[] configPath, out IntPtr model);

        public IntPtr OgaCreateModel(byte[] configPath, out IntPtr model)
        {
            var method = GetNativeMethod<OgaCreateModelDelegate>("OgaCreateModel");
            return method(configPath, out model);
        }
        private delegate IntPtr OgaGeneratorParamsSetInputSequencesDelegate(IntPtr generatorParams, IntPtr sequences);

        public IntPtr OgaGeneratorParamsSetInputSequences(IntPtr generatorParams, IntPtr sequences)
        {
            var method = GetNativeMethod<OgaGeneratorParamsSetInputSequencesDelegate>("OgaGeneratorParamsSetInputSequences");
            return method(generatorParams, sequences);
        }

        private delegate IntPtr OgaGeneratorParamsSetModelInputDelegate(IntPtr generatorParams, byte[] name, IntPtr tensor);

        public IntPtr OgaGeneratorParamsSetModelInput(IntPtr generatorParams, byte[] name, IntPtr tensor)
        {
            var method = GetNativeMethod<OgaGeneratorParamsSetModelInputDelegate>("OgaGeneratorParamsSetModelInput");
            return method(generatorParams, name, tensor);
        }

        private delegate IntPtr OgaGeneratorParamsSetInputsDelegate(IntPtr generatorParams, IntPtr named_tensors);

        public IntPtr OgaGeneratorParamsSetInputs(IntPtr generatorParams, IntPtr named_tensors)
        {
            var method = GetNativeMethod<OgaGeneratorParamsSetInputsDelegate>("OgaGeneratorParamsSetInputs");
            return method(generatorParams, named_tensors);
        }
        private delegate IntPtr OgaCreateGeneratorDelegate(IntPtr model, IntPtr generatorParams, out IntPtr generator);

        public IntPtr OgaCreateGenerator(IntPtr model, IntPtr generatorParams, out IntPtr generator)
        {
            var method = GetNativeMethod<OgaCreateGeneratorDelegate>("OgaCreateGenerator");
            return method(model, generatorParams, out generator);
        }

        private delegate bool OgaGenerator_IsDoneDelegate(IntPtr generator);

        public bool OgaGenerator_IsDone(IntPtr generator)
        {
            var method = GetNativeMethod<OgaGenerator_IsDoneDelegate>("OgaGenerator_IsDone");
            return method(generator);
        }

        private delegate IntPtr OgaGenerator_ComputeLogitsDelegate(IntPtr generator);

        public IntPtr OgaGenerator_ComputeLogits(IntPtr generator)
        {
            var method = GetNativeMethod<OgaGenerator_ComputeLogitsDelegate>("OgaGenerator_ComputeLogits");
            return method(generator);
        }
        private delegate IntPtr OgaGenerator_GenerateNextTokenDelegate(IntPtr generator);

        public IntPtr OgaGenerator_GenerateNextToken(IntPtr generator)
        {
            var method = GetNativeMethod<OgaGenerator_GenerateNextTokenDelegate>("OgaGenerator_GenerateNextToken");
            return method(generator);
        }
        private delegate UIntPtr OgaGenerator_GetSequenceCountDelegate(IntPtr generator, UIntPtr index);

        public UIntPtr OgaGenerator_GetSequenceCount(IntPtr generator, UIntPtr index)
        {
            var method = GetNativeMethod<OgaGenerator_GetSequenceCountDelegate>("OgaGenerator_GetSequenceCount");
            return method(generator, index);
        }
        private delegate IntPtr OgaGenerator_GetSequenceDataDelegate(IntPtr generator, UIntPtr index);

        public IntPtr OgaGenerator_GetSequenceData(IntPtr generator, UIntPtr index)
        {
            var method = GetNativeMethod<OgaGenerator_GetSequenceDataDelegate>("OgaGenerator_GetSequenceData");
            return method(generator, index);
        }
        private delegate IntPtr OgaCreateSequencesDelegate(out IntPtr sequences);

        public IntPtr OgaCreateSequences(out IntPtr sequences)
        {
            var method = GetNativeMethod<OgaCreateSequencesDelegate>("OgaCreateSequences");
            return method(out sequences);
        }
        private delegate void OgaDestroySequencesDelegate(IntPtr sequences);

        public void OgaDestroySequences(IntPtr sequences)
        {
            var method = GetNativeMethod<OgaDestroySequencesDelegate>("OgaDestroySequences");
            method(sequences);
        }
        private delegate UIntPtr OgaSequencesCountDelegate(IntPtr sequences);

        public UIntPtr OgaSequencesCount(IntPtr sequences)
        {
            var method = GetNativeMethod<OgaSequencesCountDelegate>("OgaSequencesCount");
            return method(sequences);
        }
        private delegate UIntPtr OgaSequencesGetSequenceCountDelegate(IntPtr sequences, UIntPtr sequenceIndex);

        public UIntPtr OgaSequencesGetSequenceCount(IntPtr sequences, UIntPtr sequenceIndex)
        {
            var method = GetNativeMethod<OgaSequencesGetSequenceCountDelegate>("OgaSequencesGetSequenceCount");
            return method(sequences, sequenceIndex);
        }
        private delegate IntPtr OgaSequencesGetSequenceDataDelegate(IntPtr sequences, UIntPtr sequenceIndex);

        public IntPtr OgaSequencesGetSequenceData(IntPtr sequences, UIntPtr sequenceIndex)
        {
            var method = GetNativeMethod<OgaSequencesGetSequenceDataDelegate>("OgaSequencesGetSequenceData");
            return method(sequences, sequenceIndex);
        }
        private delegate IntPtr OgaGenerateDelegate(IntPtr model, IntPtr generatorParams, out IntPtr sequences);

        public IntPtr OgaGenerate(IntPtr model, IntPtr generatorParams, out IntPtr sequences)
        {
            var method = GetNativeMethod<OgaGenerateDelegate>("OgaGenerate");
            return method(model, generatorParams, out sequences);
        }

        private delegate IntPtr OgaCreateTokenizerDelegate(IntPtr model, out IntPtr tokenizer);

        public IntPtr OgaCreateTokenizer(IntPtr model, out IntPtr tokenizer)
        {
            var method = GetNativeMethod<OgaCreateTokenizerDelegate>("OgaCreateTokenizer");
            return method(model, out tokenizer);
        }
        private delegate void OgaDestroyTokenizerDelegate(IntPtr tokenizer);

        public void OgaDestroyTokenizer(IntPtr tokenizer)
        {
            var method = GetNativeMethod<OgaDestroyTokenizerDelegate>("OgaDestroyTokenizer");
            method(tokenizer);
        }

    }
}
