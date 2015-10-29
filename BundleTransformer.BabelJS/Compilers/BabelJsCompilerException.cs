using System;

namespace BundleTransformer.BabelJS.Compilers
{
    internal sealed class BabelJsCompilerException: Exception
    {
        public BabelJsCompilerException(string message): base(message) {}

        public BabelJsCompilerException(string message, Exception innerException): base(message, innerException) {}
    }
}
