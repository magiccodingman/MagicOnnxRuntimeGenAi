using System;
using System.Text;

namespace MagicOnnxRuntimeGenAi
{
    public class MagicStringUtils
    {
        public static byte[] EmptyByteArray = new byte[1];

        public static byte[] ToUtf8(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return EmptyByteArray;
            }

            byte[] array = new byte[Encoding.UTF8.GetByteCount(str) + 1];
            Encoding.UTF8.GetBytes(str, 0, str.Length, array, 0);
            array[array.Length - 1] = 0; 
            return array;
        }

        public unsafe static string FromUtf8(IntPtr nativeUtf8)
        {
            int i;
            for (i = 0; *(bool*)((nint)nativeUtf8 + i); i++)
            {
            }

            if (i == 0)
            {
                return string.Empty;
            }

            return Encoding.UTF8.GetString((byte*)nativeUtf8, i);
        }
    }
}
