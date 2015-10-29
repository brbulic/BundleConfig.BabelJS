using System;
using System.Globalization;
using System.Text;
using BundleTransformer.Core.Utilities;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Core.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CoreStrings = BundleTransformer.Core.Resources.Strings;

namespace BundleTransformer.BabelJS.Compilers
{
    internal sealed class BabelJsCompiler: IDisposable
    {
        private const string RESOURCES_NAMESPACE = "BundleTransformer.BabelJS.Resources";
        private const string BABELJS_LIBRARY_FILE_NAME = "browser.min.js";
        private const string BABELJS_TRANSPILER_HELPER_FILE_NAME = "babelHelper.js";
        private const string COMPILATION_FUNCTION_CALL_TEMPLATE = @"babelJsTranspiler.compile({0}, {1});";

        private bool _disposed;
        private IJsEngine _jsEngine;
        private bool _initialized;

        private readonly string _defaultOptionsString;
        private readonly object _compilationSynchronizer = new object();

        public BabelJsCompiler(Func<IJsEngine> createJsEngineInstance)
            :this(createJsEngineInstance, null)
        {
            _disposed = false;
        }

        public BabelJsCompiler(Func<IJsEngine> createJsEngineInstance, BabelJsCompilationOptions options)
        {
            _jsEngine = createJsEngineInstance();
            _defaultOptionsString = (options != null) ?
                ConvertCompilationOptionsToJson(options).ToString() : "null";
        }

        /// <summary>
        /// Converts a compilation options to JSON
        /// </summary>
        /// <param name="options">Compilation options</param>
        /// <returns>Compilation options in JSON format</returns>
        private static JObject ConvertCompilationOptionsToJson(BabelJsCompilationOptions options)
        {
            var optionsJson = new JObject(
                new JProperty("comments", options.Comments),
                new JProperty("highlightCode", options.HighlightCode),
                new JProperty("compact", options.Compact)
            );

            return optionsJson;
        }

        /// <summary>
        /// Initializes compiler
        /// </summary>
        private void Initialize()
        {
            if (!_initialized)
            {
                Type type = GetType();

                _jsEngine.ExecuteResource(RESOURCES_NAMESPACE + "." + BABELJS_LIBRARY_FILE_NAME, type);
                _jsEngine.ExecuteResource(RESOURCES_NAMESPACE + "." + BABELJS_TRANSPILER_HELPER_FILE_NAME, type);

                _initialized = true;
            }
        }

        public string Compile(string content, string path, BabelJsCompilationOptions options = null)
        {
            string newContent;
            string currentOptionsString = (options != null) ?
                ConvertCompilationOptionsToJson(options).ToString() : _defaultOptionsString;

            lock (_compilationSynchronizer)
            {
                Initialize();

                try
                {
                    var result = _jsEngine.Evaluate<string>(
                        string.Format(COMPILATION_FUNCTION_CALL_TEMPLATE,
                            JsonConvert.SerializeObject(content),
                            currentOptionsString));
                    var json = JObject.Parse(result);

                    var errors = json["errors"] as JArray;
                    if (errors != null && errors.Count > 0)
                    {
                        throw new BabelJsCompilerException(FormatErrorDetails(errors[0], content, path));
                    }

                    newContent = json.Value<string>("compiledCode");
                }
                catch (JsRuntimeException e)
                {
                    throw new Exception(JsRuntimeErrorHelpers.Format(e));
                }
            }

            return newContent;
        }

        /// <summary>
		/// Generates a detailed error message
		/// </summary>
		/// <param name="errorDetails">Error details</param>
		/// <param name="sourceCode">Source code</param>
		/// <param name="currentFilePath">Path to current EcmaScript2015-file</param>
		/// <returns>Detailed error message</returns>
		private static string FormatErrorDetails(JToken errorDetails, string sourceCode,
			string currentFilePath)
		{
			var message = errorDetails.Value<string>("message");
			string file = currentFilePath;
			var lineNumber = errorDetails.Value<int>("lineNumber");
			var columnNumber = errorDetails.Value<int>("columnNumber");
			string sourceFragment = SourceCodeNavigator.GetSourceFragment(sourceCode,
				new SourceCodeNodeCoordinates(lineNumber, columnNumber));

			var errorMessage = new StringBuilder();
			errorMessage.AppendFormatLine("{0}: {1}", CoreStrings.ErrorDetails_Message, message);
			if (!string.IsNullOrWhiteSpace(file))
			{
				errorMessage.AppendFormatLine("{0}: {1}", CoreStrings.ErrorDetails_File, file);
			}
			if (lineNumber > 0)
			{
				errorMessage.AppendFormatLine("{0}: {1}", CoreStrings.ErrorDetails_LineNumber,
					lineNumber.ToString(CultureInfo.InvariantCulture));
			}
			if (columnNumber > 0)
			{
				errorMessage.AppendFormatLine("{0}: {1}", CoreStrings.ErrorDetails_ColumnNumber,
					columnNumber.ToString(CultureInfo.InvariantCulture));
			}
			if (!string.IsNullOrWhiteSpace(sourceFragment))
			{
				errorMessage.AppendFormatLine("{1}:{0}{0}{2}", Environment.NewLine,
					CoreStrings.ErrorDetails_SourceError, sourceFragment);
			}

			return errorMessage.ToString();
		}

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                if (_jsEngine != null)
                {
                    _jsEngine.Dispose();
                    _jsEngine = null;
                }
            }
        }
    }
}
