using System;
using BundleTransformer.BabelJS.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace BundleTransformer.BabelJS
{
    internal sealed class BabelJsCompilationOptions
    {
        public bool Comments { get; set; }
        public bool Compact { get; set; }
        public bool HighlightCode { get; set; }
        public SourceMaps? SourceMaps { get; set; }

        public BabelJsCompilationOptions()
        {
            Comments = false;
            Compact = false;
            HighlightCode = false;
            SourceMaps = null;
        }

        public JProperty SourceMapConfig()
        {
            const string SourceMapConfigName = "sourceMaps";

            switch (SourceMaps)
            {
                case Configuration.SourceMaps.Property:
                    return new JProperty(SourceMapConfigName, true);
                case Configuration.SourceMaps.Inline:
                    return new JProperty(SourceMapConfigName, "inline");
                case Configuration.SourceMaps.Both:
                    return new JProperty(SourceMapConfigName, "both");
                case null:
                    return new JProperty(SourceMapConfigName, false);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public JObject ToJObject()
        {
            return new JObject(
                new JProperty("comments", this.Comments),
                new JProperty("highlightCode", this.HighlightCode),
                new JProperty("compact", this.Compact),
                this.SourceMapConfig()
            );
        }
    }
}
