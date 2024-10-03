using System;

namespace MagicOnnxRuntimeGenAi
{
    public class MagicGeneratorParams : IDisposable
    {
        private IntPtr _generatorParamsHandle;

        private bool _disposed;

        internal IntPtr Handle => _generatorParamsHandle;
        private MagicNativeMethods _MagicNativeMethods;
        public MagicGeneratorParams(MagicModel model)
        {
            _MagicNativeMethods = model.GetMagicNativeMethods();
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaCreateGeneratorParams(model.Handle, out _generatorParamsHandle));
        }

        public void SetSearchOption(string searchOption, double value)
        {
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaGeneratorParamsSetSearchNumber(_generatorParamsHandle, MagicStringUtils.ToUtf8(searchOption), value));
        }

        public void SetSearchOption(string searchOption, bool value)
        {
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaGeneratorParamsSetSearchBool(_generatorParamsHandle, MagicStringUtils.ToUtf8(searchOption), value));
        }

        public void TryGraphCaptureWithMaxBatchSize(int maxBatchSize)
        {
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaGeneratorParamsTryGraphCaptureWithMaxBatchSize(_generatorParamsHandle, maxBatchSize));
        }

        public unsafe void SetInputIDs(ReadOnlySpan<int> inputIDs, ulong sequenceLength, ulong batchSize)
        {
            fixed (int* inputIDs2 = inputIDs)
            {
                new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaGeneratorParamsSetInputIDs(_generatorParamsHandle, inputIDs2, (nuint)inputIDs.Length, (nuint)sequenceLength, (nuint)batchSize));
            }
        }

        public void SetInputSequences(MagicSequences sequences)
        {
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaGeneratorParamsSetInputSequences(_generatorParamsHandle, sequences.Handle));
        }

        public void SetModelInput(string name, MagicTensor value)
        {
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaGeneratorParamsSetModelInput(_generatorParamsHandle, MagicStringUtils.ToUtf8(name), value.Handle));
        }

        public void SetInputs(MagicNamedTensors namedTensors)
        {
            new MagicResult(_MagicNativeMethods).VerifySuccess(_MagicNativeMethods.OgaGeneratorParamsSetInputs(_generatorParamsHandle, namedTensors.Handle));
        }

        ~MagicGeneratorParams()
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
                _MagicNativeMethods.OgaDestroyGeneratorParams(_generatorParamsHandle);
                _generatorParamsHandle = IntPtr.Zero;
                _disposed = true;
            }
        }
    }
}
