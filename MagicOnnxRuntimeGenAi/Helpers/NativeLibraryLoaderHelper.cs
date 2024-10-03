using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MagicOnnxRuntimeGenAi.Helpers
{
    public static class NativeLibraryLoaderHelper
    {
        // Windows: LoadLibrary, GetProcAddress, FreeLibrary
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SetDllDirectory(string lpPathName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        // Unix: dlopen, dlsym, dlclose
        [DllImport("libdl", EntryPoint = "dlopen", SetLastError = true)]
        private static extern IntPtr dlopen(string fileName, int flags);

        [DllImport("libdl", EntryPoint = "dlsym", SetLastError = true)]
        private static extern IntPtr dlsym(IntPtr handle, string symbol);

        [DllImport("libdl", EntryPoint = "dlclose", SetLastError = true)]
        private static extern int dlclose(IntPtr handle);

        private const int RTLD_NOW = 2; // Flag for dlopen to load immediately

        public static IntPtr LoadNativeLibrary(string fullLibraryPath)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Extract directory and DLL name on Windows
                string directory = Path.GetDirectoryName(fullLibraryPath);
                string dllName = Path.GetFileName(fullLibraryPath);

                // Set the directory where the DLL is located
                SetDllDirectory(directory);

                // Load the DLL
                IntPtr handle = LoadLibrary(dllName);

                if (handle == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new InvalidOperationException($"Failed to load library '{fullLibraryPath}'. Error Code: {errorCode}");
                }

                // Reset the DLL directory after loading
                SetDllDirectory(null);

                return handle;
            }
            else
            {
                // Load the shared object (.so) on Unix-like platforms (Linux/macOS)
                IntPtr handle = dlopen(fullLibraryPath, RTLD_NOW);

                if (handle == IntPtr.Zero)
                {
                    throw new InvalidOperationException($"Failed to load library '{fullLibraryPath}' on Unix-based system.");
                }

                return handle;
            }
        }

        public static IntPtr GetNativeMethodPointer(IntPtr libraryHandle, string methodName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Retrieve the function pointer on Windows
                IntPtr methodPtr = GetProcAddress(libraryHandle, methodName);

                if (methodPtr == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new InvalidOperationException($"Failed to get method pointer for '{methodName}'. Error Code: {errorCode}");
                }

                return methodPtr;
            }
            else
            {
                // Retrieve the function pointer on Unix-based platforms (Linux/macOS)
                IntPtr methodPtr = dlsym(libraryHandle, methodName);

                if (methodPtr == IntPtr.Zero)
                {
                    throw new InvalidOperationException($"Failed to get method pointer for '{methodName}' on Unix-based system.");
                }

                return methodPtr;
            }
        }

        public static void FreeNativeLibrary(IntPtr libraryHandle)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Free the library on Windows
                if (!FreeLibrary(libraryHandle))
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new InvalidOperationException($"Failed to free library. Error Code: {errorCode}");
                }
            }
            else
            {
                // Free the library on Unix-based platforms (Linux/macOS)
                int result = dlclose(libraryHandle);

                if (result != 0)
                {
                    throw new InvalidOperationException($"Failed to close library on Unix-based system.");
                }
            }
        }
    }
}
