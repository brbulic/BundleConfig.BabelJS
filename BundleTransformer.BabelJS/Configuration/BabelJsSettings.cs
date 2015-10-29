using System.Configuration;
using BundleTransformer.Core.Configuration;

namespace BundleTransformer.BabelJS.Configuration
{
    /// <summary>
    /// Configuration settings of EcmaScript2015 BabelJS translator
    /// </summary>
    public sealed class BabelJsSettings : ConfigurationSection
    {
        /// <summary>
        /// Gets a configuration settings of JavaScript engine
        /// </summary>
        [ConfigurationProperty("jsEngine")]
        public JsEngineSettings JsEngine
        {
            get { return (JsEngineSettings)this["jsEngine"]; }
        }

        [ConfigurationProperty("comments", DefaultValue = false)]
        public bool Comments
        {
            get { return (bool)this["comments"]; }
            set { this["comments"] = value; }
        }

        [ConfigurationProperty("compact", DefaultValue = true)]
        public bool Compact
        {
            get { return (bool)this["compact"]; }
            set { this["compact"] = value; }
        }

        [ConfigurationProperty("highlightCode", DefaultValue = true)]
        public bool HighlightCode
        {
            get { return (bool)this["highlightCode"]; }
            set { this["highlightCode"] = value; }
        }
    }
}