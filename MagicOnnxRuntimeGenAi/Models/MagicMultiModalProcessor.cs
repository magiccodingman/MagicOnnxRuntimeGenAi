using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace MagicOnnxRuntimeGenAi.Models
{
    public class MultiModalProcessor : IDisposable
    {
        private IntPtr _processorHandle;
        private bool _disposed = false;

        private MagicNativeMethods _MagicNativeMethods;
        private MagicModel _Model;
        public MultiModalProcessor(MagicModel model)
        {
            _Model = model;
            _MagicNativeMethods = model.GetMagicNativeMethods();
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaCreateMultiModalProcessor(model.Handle, out _processorHandle));
        }

        internal IntPtr Handle { get { return _processorHandle; } }

        public MagicNamedTensors ProcessImages(string prompt, MagicImages images)
        {
            IntPtr imagesHandle = images == null ? IntPtr.Zero : images.Handle;
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaProcessorProcessImages(_processorHandle, MagicStringUtils.ToUtf8(prompt),
                                                                         imagesHandle, out IntPtr namedTensorsHandle));
            return new MagicNamedTensors(namedTensorsHandle, _Model);
        }

        public string Decode(ReadOnlySpan<int> sequence)
        {
            IntPtr outStr = IntPtr.Zero;
            unsafe
            {
                fixed (int* sequencePtr = sequence)
                {
                    new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaProcessorDecode(_processorHandle, sequencePtr, (UIntPtr)sequence.Length, out outStr));
                }
            }
            try
            {
                return MagicStringUtils.FromUtf8(outStr);
            }
            finally
            {
                _MagicNativeMethods.OgaDestroyString(outStr);
            }
        }

        public MagicTokenizerStream CreateStream()
        {
            IntPtr tokenizerStreamHandle = IntPtr.Zero;
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaCreateTokenizerStreamFromProcessor(_processorHandle, out tokenizerStreamHandle));
            return new MagicTokenizerStream(tokenizerStreamHandle, _Model);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            _MagicNativeMethods.OgaDestroyMultiModalProcessor(_processorHandle);
            _processorHandle = IntPtr.Zero;
            _disposed = true;
        }
    }
}
