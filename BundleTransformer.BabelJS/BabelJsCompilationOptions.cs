namespace BundleTransformer.BabelJS
{
    internal sealed class BabelJsCompilationOptions
    {
        public bool Comments { get; set; }
        public bool Compact { get; set; }
        public bool HighlightCode { get; set; }

        public BabelJsCompilationOptions()
        {
            Comments = false;
            Compact = false;
            HighlightCode = false;
        }
    }
}
