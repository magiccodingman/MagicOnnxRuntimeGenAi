using System;

namespace MagicOnnxRuntimeGenAi
{
    public class MagicTokenizerStream : IDisposable
    {
        
        private IntPtr _tokenizerStreamHandle;

        private bool _disposed;

        internal IntPtr Handle => _tokenizerStreamHandle;

        private MagicNativeMethods _MagicNativeMethods;

        internal MagicTokenizerStream(IntPtr tokenizerStreamHandle, MagicModel model)
        {
            _MagicNativeMethods = model.GetMagicNativeMethods();
            _tokenizerStreamHandle = tokenizerStreamHandle;
        }

        public string Decode(int token)
        {
            IntPtr outStr = IntPtr.Zero;
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaTokenizerStreamDecode(_tokenizerStreamHandle, token, out outStr));
            return MagicStringUtils.FromUtf8(outStr);
        }

        ~MagicTokenizerStream()
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
                _MagicNativeMethods.OgaDestroyTokenizerStream(_tokenizerStreamHandle);
                _tokenizerStreamHandle = IntPtr.Zero;
                _disposed = true;
            }
        }
    }
}
