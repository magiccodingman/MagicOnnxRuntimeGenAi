using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace MagicOnnxRuntimeGenAi.Models
{
    public class MagicImages : IDisposable
    {
        private IntPtr _imagesHandle;
        private bool _disposed = false;
        private MagicNativeMethods _MagicNativeMethods;
        private MagicModel _Model;
        private MagicImages(MagicModel model, IntPtr imagesHandle)
        {
            _Model = model;
            _MagicNativeMethods = model.GetMagicNativeMethods();
            _imagesHandle = imagesHandle;
        }
        internal IntPtr Handle { get { return _imagesHandle; } }

        public MagicImages Load(string[] imagePaths)
        {
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaCreateStringArray(out IntPtr stringArray));
            foreach (string imagePath in imagePaths)
            {
                new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaStringArrayAddString(stringArray, MagicStringUtils.ToUtf8(imagePath)));
            }
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaLoadImages(stringArray, out IntPtr imagesHandle));
            _MagicNativeMethods.OgaDestroyStringArray(stringArray);
            return new MagicImages(_Model, imagesHandle);
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
