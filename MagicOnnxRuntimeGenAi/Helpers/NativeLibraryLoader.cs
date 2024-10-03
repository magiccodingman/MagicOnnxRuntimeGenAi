using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MagicOnnxRuntimeGenAi.Helpers
{
    public class NativeLibraryLoader
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetDllDirectory(string lpPathName);

        public static IDisposable UseDllDirectory(string dllPath)
        {
            string originalDllPath = AppDomain.CurrentDomain.BaseDirectory;

            if (!SetDllDirectory(dllPath))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }

            return new DllDirectoryResetter(originalDllPath);
        }

        private class DllDirectoryResetter : IDisposable
        {
            private readonly string _originalDllPath;

            public DllDirectoryResetter(string originalDllPath)
            {
                _originalDllPath = originalDllPath;
            }

            public void Dispose()
            {
                SetDllDirectory(_originalDllPath);
            }
        }

        // Function to dynamically determine the correct platform and architecture
        public static string GetNativeDllPath(string basePath)
        {
            // Detect the platform
            string platform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win"
                            : RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux"
                            : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx"
                            : RuntimeInformation.IsOSPlatform(OSPlatform.Create("IOS")) ? "ios"
                            : RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID")) ? "android"
                            : throw new PlatformNotSupportedException("Unsupported platform");

            // Detect the architecture
            string architecture = RuntimeInformation.OSArchitecture == Architecture.X64 ? "x64"
                              : RuntimeInformation.OSArchitecture == Architecture.X86 ? "x86"
                              : RuntimeInformation.OSArchitecture == Architecture.Arm64 ? "arm64"
                              : throw new PlatformNotSupportedException("Unsupported architecture");

            // If platform is iOS or Android, don't append the architecture
            if (platform == "ios" || platform == "android")
            {
                return Path.Combine(basePath, "runtimes", platform, "native");
            }

            // Construct the final path for the native DLL based on OS and architecture
            return Path.Combine(basePath, "runtimes", $"{platform}-{architecture}", "native");
        }
    }
}
