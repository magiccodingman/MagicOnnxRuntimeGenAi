using System;
using System.Runtime.InteropServices;

namespace MagicOnnxRuntimeGenAi
{
    public class MagicSequences
    {
        public IntPtr _sequencesHandle;
        private bool _disposed;
        private ulong _numSequences;
        private MagicNativeMethods _MagicNativeMethods;
        internal IntPtr Handle => _sequencesHandle;

        public ulong NumSequences => _numSequences;

        // Constructor
        internal MagicSequences(IntPtr sequencesHandle, MagicModel model)
        {
            _MagicNativeMethods = model.GetMagicNativeMethods();
            _sequencesHandle = sequencesHandle;
            _numSequences = _MagicNativeMethods.OgaSequencesCount(_sequencesHandle).ToUInt64();
        }

        public int[] this[ulong sequenceIndex]
        {
            get
            {
                if (sequenceIndex >= _numSequences)
                {
                    throw new ArgumentOutOfRangeException(nameof(sequenceIndex));
                }

                // Fetch the sequence count from the native method
                ulong num = _MagicNativeMethods.OgaSequencesGetSequenceCount(_sequencesHandle, (nuint)sequenceIndex).ToUInt64();

                // Get the sequence data pointer from the native method
                IntPtr dataPointer = _MagicNativeMethods.OgaSequencesGetSequenceData(_sequencesHandle, (nuint)sequenceIndex);

                // Create an array from the pointer (unsafe)
                int[] sequenceData = new int[num];
                Marshal.Copy(dataPointer, sequenceData, 0, (int)num);  // Use Marshal.Copy to convert IntPtr to array
                return sequenceData;
            }
        }

        // Dispose pattern
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _MagicNativeMethods.OgaDestroySequences(_sequencesHandle);
                _sequencesHandle = IntPtr.Zero;
                _disposed = true;
            }
        }
    }
}
