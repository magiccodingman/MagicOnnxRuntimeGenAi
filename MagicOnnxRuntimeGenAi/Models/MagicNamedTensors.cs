using System;

namespace MagicOnnxRuntimeGenAi
{
    public class MagicNamedTensors : IDisposable
    {
        private IntPtr _namedTensorsHandle;

        private bool _disposed;

        internal IntPtr Handle => _namedTensorsHandle;
        private MagicNativeMethods _MagicNativeMethods;
        internal MagicNamedTensors(IntPtr namedTensorsHandle, MagicModel model)
        {
            _MagicNativeMethods = model.GetMagicNativeMethods();
            _namedTensorsHandle = namedTensorsHandle;
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
                _MagicNativeMethods.OgaDestroyNamedTensors(_namedTensorsHandle);
                _namedTensorsHandle = IntPtr.Zero;
                _disposed = true;
            }
        }
    }
}