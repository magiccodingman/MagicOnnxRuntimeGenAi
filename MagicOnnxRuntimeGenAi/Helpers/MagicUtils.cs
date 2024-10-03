using System;
using System.Collections.Generic;
using System.Text;

namespace MagicOnnxRuntimeGenAi.Helpers
{
    internal class MagicUtils
    {
        private MagicNativeMethods _MagicNativeMethods;
        public MagicUtils(MagicModel model)
        {
            _MagicNativeMethods = model.GetMagicNativeMethods();
        }

        public void SetCurrentGpuDeviceId(int device_id)
        {
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaSetCurrentGpuDeviceId(device_id));
        }

        public int GetCurrentGpuDeviceId()
        {
            IntPtr device_id = IntPtr.Zero;
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaGetCurrentGpuDeviceId(out device_id));
            return (int)device_id.ToInt64();
        }

        public void SetLogBool(string name, bool value)
        {
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaSetLogBool(MagicStringUtils.ToUtf8(name), value));
        }

        public void SetLogString(string name, string value)
        {
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaSetLogString(MagicStringUtils.ToUtf8(name), MagicStringUtils.ToUtf8(value)));
        }
    }
}
