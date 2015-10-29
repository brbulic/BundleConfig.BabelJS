/*global babel*/
var babelJsTranspiler = function (babelJsTranspiler, undefined) {
    "use strict";
    var transpilerProto = {};

    function compile(code, options) {
        var compiledCode = "",
			result = {},
			parseErrors = [],
			message,
			lineNumber,
			columnNumber,
			location;
        try {
            compiledCode = babelJsTranspiler.transform(code, options).code;
        } catch (e) {
            if (e instanceof SyntaxError) {
                message = e.message;
                lineNumber = 0;
                columnNumber = 0;

                if (typeof e.location !== 'undefined') {
                    location = e.location;
                    if (typeof location.first_line !== 'undefined') {
                        lineNumber = location.first_line + 1;
                    }
                    if (typeof location.first_column !== 'undefined') {
                        columnNumber = location.first_column + 1;
                    }
                }

                parseErrors.push({
                    'message': message,
                    'lineNumber': lineNumber,
                    'columnNumber': columnNumber
                });
            }
            else {
                throw (e);
            }
        }

        result.compiledCode = compiledCode;
        if (parseErrors.length > 0) {
            result.errors = parseErrors;
        }

        return JSON.stringify(result);
    }
    transpilerProto.compile = compile;
    return transpilerProto;
}(babel)