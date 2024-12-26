using MagicOnnxRuntimeGenAi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicOnnxAi
{
    public partial class MagicGenAi : IDisposable
    {
        private MagicModel _model;
        private MagicTokenizer _tokenizer;
        private bool _disposed = false; // Track whether Dispose has been called

        public MagicGenAi(string modelPath)
        {
            _model = new MagicModel(modelPath);
            _tokenizer = new MagicTokenizer(_model);
        }

        public MagicModel Model
        {
            get => _model;
        }

        public MagicTokenizer Tokenizer
        {
            get => _tokenizer;
        }

        public ulong CountTokens(string text)
        {
            var promptTokens = this.Tokenizer.Encode(text);

            // Step 4: Correct token counting using model's vocab-based tokenizer
            // Use Sequences.NumSequences to get token count
            ulong tokenCount = 0;

            for (ulong i = 0; i < promptTokens.NumSequences; i++)
            {
                var tokenSpan = promptTokens[i];
                tokenCount += (ulong)tokenSpan.Length;  // Accumulate total token count from each sequence
            }

            return tokenCount;
        }

        // Implement IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Prevents the finalizer from running
        }

        // Protected implementation of Dispose pattern
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Dispose managed resources here
                if (_tokenizer is IDisposable disposableTokenizer)
                {
                    disposableTokenizer.Dispose();
                }
                if (_model is IDisposable disposableModel)
                {
                    disposableModel.Dispose();
                }
            }

            // Free unmanaged resources here, if any

            _disposed = true;
        }

        // Destructor
        ~MagicGenAi()
        {
            Dispose(false);
        }
    }

}
