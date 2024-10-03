using System;
using System.Runtime.CompilerServices;

namespace MagicOnnxRuntimeGenAi
{
    public class MagicResult
    {
        private MagicNativeMethods _MagicNativeMethods;
        public MagicResult(MagicNativeMethods magicNativeMethods)
        {
            _MagicNativeMethods = magicNativeMethods;
        }
        private string GetErrorMessage(IntPtr nativeResult)
        {
            return MagicStringUtils.FromUtf8(_MagicNativeMethods.OgaResultGetError(nativeResult));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void VerifySuccess(IntPtr nativeResult)
        {
            if (nativeResult != IntPtr.Zero)
            {
                try
                {
                    throw new Exception(GetErrorMessage(nativeResult));
                }
                finally
                {
                    _MagicNativeMethods.OgaDestroyResult(nativeResult);
                }
            }
        }
    }
}
