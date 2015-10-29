using System.Web;
using System.Web.Caching;
using BundleTransformer.BabelJS.Constants;
using BundleTransformer.BabelJS.Translators;
using BundleTransformer.Core;
using BundleTransformer.Core.Assets;
using BundleTransformer.Core.Configuration;
using BundleTransformer.Core.FileSystem;
using BundleTransformer.Core.HttpHandlers;
using BundleTransformer.Core.Transformers;

namespace BundleTransformer.BabelJS.HttpHandlers
{
    /// <summary>
    /// Debugging HTTP-handler that responsible for text output
    /// of translated EcmaScript2015-asset
    /// </summary>
    public sealed class EcmaScript2015AssetHandler : ScriptAssetHandlerBase
    {
        /// <summary>
        /// Gets a value indicating whether asset is static
        /// </summary>
        protected override bool IsStaticAsset
        {
            get { return false; }
        }


        /// <summary>
        /// Constructs a instance of the debugging EcmaScript2015 HTTP-handler
        /// </summary>
        public EcmaScript2015AssetHandler()
            : this(HttpContext.Current.Cache,
                BundleTransformerContext.Current.FileSystem.GetVirtualFileSystemWrapper(),
                BundleTransformerContext.Current.Configuration.GetCoreSettings().AssetHandler)
        { }

        /// <summary>
        /// Constructs a instance of the debugging EcmaScript2015 HTTP-handler
        /// </summary>
        /// <param name="cache">Server cache</param>
        /// <param name="virtualFileSystemWrapper">Virtual file system wrapper</param>
        /// <param name="assetHandlerConfig">Configuration settings of the debugging HTTP-handler</param>
        public EcmaScript2015AssetHandler(Cache cache,
            IVirtualFileSystemWrapper virtualFileSystemWrapper,
            AssetHandlerSettings assetHandlerConfig)
            : base(cache, virtualFileSystemWrapper, assetHandlerConfig)
        { }


        /// <summary>
        /// Translates a code of asset written on EcmaScript2015 to JS-code
        /// </summary>
        /// <param name="asset">Asset</param>
        /// <param name="transformer">Transformer</param>
        /// <param name="isDebugMode">Flag that web application is in debug mode</param>
        /// <returns>Translated asset</returns>
        protected override IAsset TranslateAsset(IAsset asset, ITransformer transformer, bool isDebugMode)
        {
            IAsset processedAsset = InnerTranslateAsset<BabelJsTranslator>(
                AssetTypeCode.EcmaScript2015, asset, transformer, isDebugMode);

            return processedAsset;
        }
    }
}