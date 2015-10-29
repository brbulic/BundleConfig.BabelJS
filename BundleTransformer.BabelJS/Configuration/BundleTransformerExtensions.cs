using System;
using System.Configuration;
using BundleTransformer.Core.Configuration;

namespace BundleTransformer.BabelJS.Configuration
{
    public static class ConfigurationContextExtensions
    { 
        private static readonly Lazy<BabelJsSettings> _babelJsConfig =
            new Lazy<BabelJsSettings>(() => (BabelJsSettings)ConfigurationManager.GetSection("bundleTransformer/babelJs"));
 
        public static BabelJsSettings GetBabelJsSettings(this IConfigurationContext context)
        {
            return _babelJsConfig.Value;
        }
    }
}