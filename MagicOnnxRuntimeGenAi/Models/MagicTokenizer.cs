using System;

namespace MagicOnnxRuntimeGenAi
{
    public class MagicTokenizer : IDisposable
    {
        private MagicNativeMethods _MagicNativeMethods;
        private IntPtr _tokenizerHandle;

        private bool _disposed;

        private MagicModel _model { get; set; }

        public MagicTokenizer(MagicModel model)
        {
            _model = model;
            _MagicNativeMethods = model.GetMagicNativeMethods();
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaCreateTokenizer(model.Handle, out _tokenizerHandle));
        }

        public MagicSequences EncodeBatch(string[] strings)
        {
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaCreateSequences(out var sequences));
            try
            {
                foreach (string str in strings)
                {
                    new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaTokenizerEncode(_tokenizerHandle, MagicStringUtils.ToUtf8(str), sequences));
                }

                return new MagicSequences(sequences, _model);
            }
            catch
            {
                _MagicNativeMethods.OgaDestroySequences(sequences);
                throw;
            }
        }

        public string[] DecodeBatch(MagicSequences sequences)
        {
            string[] array = new string[sequences.NumSequences];
            for (ulong num = 0uL; num < sequences.NumSequences; num++)
            {
                array[num] = Decode(sequences[num]);
            }

            return array;
        }

        public MagicSequences Encode(string str)
        {
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaCreateSequences(out var sequences));
            try
            {
                new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaTokenizerEncode(_tokenizerHandle, MagicStringUtils.ToUtf8(str), sequences));
                return new MagicSequences(sequences, _model);
            }
            catch
            {
                _MagicNativeMethods.OgaDestroySequences(sequences);
                throw;
            }
        }

        public unsafe string Decode(ReadOnlySpan<int> sequence)
        {
            IntPtr outStr = IntPtr.Zero;
            fixed (int* sequence2 = sequence)
            {
                new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaTokenizerDecode(_tokenizerHandle, sequence2, (nuint)sequence.Length, out outStr));
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
            IntPtr tokenizerStream = IntPtr.Zero;
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaCreateTokenizerStream(_tokenizerHandle, out tokenizerStream));
            return new MagicTokenizerStream(tokenizerStream, _model);
        }

        ~MagicTokenizer()
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
                _MagicNativeMethods.OgaDestroyTokenizer(_tokenizerHandle);
                _tokenizerHandle = IntPtr.Zero;
                _disposed = true;
            }
        }
    }
}
