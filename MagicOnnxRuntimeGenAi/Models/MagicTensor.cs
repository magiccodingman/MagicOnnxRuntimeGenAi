using System;

namespace MagicOnnxRuntimeGenAi
{
    
    public enum ElementType : long
    {
        undefined,
        float32,
        uint8,
        int8,
        uint16,
        int16,
        int32,
        int64,
        string_t,
        bool_t,
        float16,
        float64,
        uint32,
        uint64
    }
    public class MagicTensor : IDisposable
    {
        private IntPtr _tensorHandle;

        private bool _disposed;

        internal IntPtr Handle => _tensorHandle;

        private MagicNativeMethods _MagicNativeMethods;

        public MagicTensor(IntPtr data, long[] shape, ElementType type, MagicModel model)
        {
            _MagicNativeMethods = model.GetMagicNativeMethods();
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaCreateTensorFromBuffer(data, shape, (nuint)shape.Length, type, out _tensorHandle));
        }

        ~MagicTensor()
        {
            Dispose(disposing: false);
        }

        public ElementType Type()
        {
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaTensorGetType(_tensorHandle, out var element_type));
            return element_type;
        }

        public long[] Shape()
        {
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaTensorGetShapeRank(_tensorHandle, out var rank));
            long[] array = new long[rank.ToUInt64()];
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaTensorGetShape(_tensorHandle, array, rank));
            return array;
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
                _MagicNativeMethods.OgaDestroyTensor(_tensorHandle);
                _tensorHandle = IntPtr.Zero;
                _disposed = true;
            }
        }
    }
}
