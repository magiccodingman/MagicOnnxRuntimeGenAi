using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using MagicOnnxRuntimeGenAi.Helpers;

namespace MagicOnnxRuntimeGenAi
{
    public partial class MagicNativeMethods
    {
        private string _libraryPath;
        private HardwareType _hardwareType;
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

#if NET8_0_OR_GREATER
        private T GetNativeMethod<T>(string methodName) where T : Delegate
        {
            IntPtr libraryHandle = NativeLibrary.Load(_libraryPath);
            IntPtr methodPtr = NativeLibrary.GetExport(libraryHandle, methodName);
            return Marshal.GetDelegateForFunctionPointer<T>(methodPtr);
        }
#elif NETSTANDARD2_0
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
    }
}
