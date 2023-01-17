using System.Diagnostics.CodeAnalysis;
using ResourceDecl;

namespace Dove.Core;

public static class Parser {
    public static bool IsStrict { get; set; } = false; // to be removed
    public static bool TryParse<T>(string source,[NotNullWhen(true)] out T result, out string error) where T : IDeclaration<T> {
        var parser = IDeclaration<T>.AsParser;
        int start = 0;
        if(parser(source, ref start, out result, out error) && (IsStrict ? start == source.Length : true)) {
            return true;
        }
        return false;
    }

    public static T? Parse<T>(string source) where T : IDeclaration<T> {
        var parser = IDeclaration<T>.AsParser;
        int start = 0;
        if(parser(source, ref start, out var result, out string error) && (IsStrict ? start == source.Length : true)) {
            return result;
        }
        throw new Exception(error);
    }

    public static Task<T?> ParseAsync<T>(string source) where T : IDeclaration<T> {
        var parser = IDeclaration<T>.AsParser;
        int start = 0;
        return parser(source, ref start, out var result, out string error)  && (IsStrict ? start == source.Length : true)
            ? Task.FromResult(result)
            : throw new Exception(error);
    } 
}