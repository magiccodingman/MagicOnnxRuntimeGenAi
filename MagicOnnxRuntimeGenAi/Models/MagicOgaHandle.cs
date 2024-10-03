using System;
using System.Collections.Generic;
using System.Text;

namespace MagicOnnxRuntimeGenAi.Models
{
    public class MagicOgaHandle : IDisposable
    {
        private bool _disposed = false;
        private MagicNativeMethods _MagicNativeMethods;

        public MagicOgaHandle(MagicModel model)
        {
            _MagicNativeMethods = model.GetMagicNativeMethods();
        }

        ~MagicOgaHandle()
        {
            Dispose(false);
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
            _MagicNativeMethods.OgaShutdown();
            _disposed = true;
        }
    }
}
