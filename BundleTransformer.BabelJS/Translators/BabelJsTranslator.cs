using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using BundleTransformer.BabelJS.Compilers;
using BundleTransformer.BabelJS.Configuration;
using BundleTransformer.BabelJS.Constants;
using BundleTransformer.Core;
using BundleTransformer.Core.Assets;
using BundleTransformer.Core.Translators;
using JavaScriptEngineSwitcher.Core;
using CoreStrings = BundleTransformer.Core.Resources.Strings;

namespace BundleTransformer.BabelJS.Translators
{
    /// <summary>
    /// Translator that responsible for translation of EcmaScript2015-code to JS-code
    /// </summary>
    public sealed class BabelJsTranslator : ITranslator
    {
        /// <summary>
        /// Name of input code type
        /// </summary>
        const string INPUT_CODE_TYPE = "EcmaScript2015";

        /// <summary>
        /// Name of output code type
        /// </summary>
        const string OUTPUT_CODE_TYPE = "JS";

        /// <summary>
        /// Delegate that creates an instance of JavaScript engine
        /// </summary>
        private readonly Func<IJsEngine> _createJsEngineInstance;

        private readonly BabelJsSettings _babelJsConfig;

        public bool IsDebugMode { get; set; }

        /// <summary>
        /// Constructs a instance of EcmaScript2015-translator
        /// </summary>
        public BabelJsTranslator()
            : this(null, BundleTransformerContext.Current.Configuration.GetBabelJsSettings())
        { }

        /// <summary>
        /// Constructs a instance of EcmaScript2015-translator
        /// </summary>
        /// <param name="createJsEngineInstance">Delegate that creates an instance of JavaScript engine</param>
        /// <param name="babelJsConfig">Configuration settings of EcmaScript2015-translator</param>
        public BabelJsTranslator(Func<IJsEngine> createJsEngineInstance, BabelJsSettings babelJsConfig)
        {           
            if (createJsEngineInstance == null)
            {
                _babelJsConfig = babelJsConfig;

                var jsEngineName = _babelJsConfig.JsEngine.Name;
                if (string.IsNullOrWhiteSpace(jsEngineName))
                {
                    throw new ConfigurationErrorsException(
                        string.Format(CoreStrings.Configuration_JsEngineNotSpecified,
                            "babelJs",
                            @"
  * JavaScriptEngineSwitcher.Msie
  * JavaScriptEngineSwitcher.V8",
                            "MsieJsEngine")
                    );
                }

                createJsEngineInstance = (() =>
                    JsEngineSwitcher.Current.CreateJsEngineInstance(jsEngineName));
            }
            _createJsEngineInstance = createJsEngineInstance;
        }


        /// <summary>
        /// Translates a code of asset written on EcmaScript2015 to JS-code
        /// </summary>
        /// <param name="asset">Asset with code written on EcmaScript2015</param>
        /// <returns>Asset with translated code</returns>
        public IAsset Translate(IAsset asset)
        {
            if (asset == null)
            {
                throw new ArgumentException(CoreStrings.Common_ValueIsEmpty, "asset");
            }

            using (var babelJsCompiler = new BabelJsCompiler(_createJsEngineInstance))
            {
                InnerTranslate(asset, babelJsCompiler);
            }

            return asset;
        }

        /// <summary>
        /// Translates a code of assets written on EcmaScript2015 to JS-code
        /// </summary>
        /// <param name="assets">Set of assets with code written on EcmaScript2015</param>
        /// <returns>Set of assets with translated code</returns>
        public IList<IAsset> Translate(IList<IAsset> assets)
        {
            if (assets == null)
            {
                throw new ArgumentException(CoreStrings.Common_ValueIsEmpty, "assets");
            }

            if (assets.Count == 0)
            {
                return assets;
            }

            var assetsToProcessing = assets.Where(a => a.AssetTypeCode == AssetTypeCode.EcmaScript2015).ToList();
            if (assetsToProcessing.Count == 0)
            {
                return assets;
            }

            using (var babelJsCompiler = new BabelJsCompiler(_createJsEngineInstance))
            {
                foreach (var asset in assetsToProcessing)
                {
                    InnerTranslate(asset, babelJsCompiler);
                }
            }

            return assets;
        }

        private void InnerTranslate(IAsset asset, BabelJsCompiler babelJsCompiler)
        {
            string newContent;
            var assetVirtualPath = asset.VirtualPath;
            var options = CreateCompilationOptions(_babelJsConfig);

            try
            {
                newContent = babelJsCompiler.Compile(asset.Content, assetVirtualPath, options);
            }
            catch (BabelJsCompilerException e)
            {
                throw new AssetTranslationException(
                    string.Format(CoreStrings.Translators_TranslationSyntaxError,
                        INPUT_CODE_TYPE, OUTPUT_CODE_TYPE, assetVirtualPath, e.Message));
            }
            catch (Exception e)
            {
                throw new AssetTranslationException(
                    string.Format(CoreStrings.Translators_TranslationFailed,
                        INPUT_CODE_TYPE, OUTPUT_CODE_TYPE, assetVirtualPath, e.Message));
            }

            asset.Content = newContent;
        }

        private BabelJsCompilationOptions CreateCompilationOptions(BabelJsSettings settings)
        {
            return new BabelJsCompilationOptions()
            {
                Comments = settings.Comments,
                Compact = settings.Compact,
                HighlightCode = settings.HighlightCode,
                SourceMaps = settings.SourceMaps
            };
        }
    }
}