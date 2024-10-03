using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace MagicOnnxRuntimeGenAi.Models
{
    public class MagicImage : IDisposable
    {
        private IntPtr _imagesHandle;
        private bool _disposed = false;
        private MagicNativeMethods _MagicNativeMethods;
        private MagicModel _Model;
        private MagicImage(MagicModel model, IntPtr imagesHandle)
        {
            _Model = model;
            _MagicNativeMethods = model.GetMagicNativeMethods();
            _imagesHandle = imagesHandle;
        }

        internal IntPtr Handle { get { return _imagesHandle; } }

        public MagicImage Load(string imagePath)
        {
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaLoadImage(MagicStringUtils.ToUtf8(imagePath), out IntPtr imagesHandle));
            return new MagicImage(_Model, imagesHandle);
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
            _MagicNativeMethods.OgaDestroyImages(_imagesHandle);
            _imagesHandle = IntPtr.Zero;
            _disposed = true;
        }
    }
}
