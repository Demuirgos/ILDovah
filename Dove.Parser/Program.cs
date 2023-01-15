using System.Diagnostics.CodeAnalysis;
using ResourceDecl;

namespace Dove.Core;

public static class Parser {
    public static bool TryParse<T>(string source,[NotNullWhen(true)] out T result) where T : IDeclaration<T> {
        var parser = IDeclaration<T>.AsParser;
        int start = 0;
        if(parser(source, ref start, out result) /*&& start == source.Length*/) {
            return true;
        }
        return false;
    }

    public static T? Parse<T>(string source) where T : IDeclaration<T> {
        var parser = IDeclaration<T>.AsParser;
        int start = 0;
        if(parser(source, ref start, out var result) /*&& start == source.Length*/) {
            return result;
        }
        return default(T);
    }

    public static Task<T?> ParseAsync<T>(string source) where T : IDeclaration<T> {
        var parser = IDeclaration<T>.AsParser;
        int start = 0;
        return parser(source, ref start, out var result) //&& start == source.Length
            ? Task.FromResult(result)
            : Task.FromResult(default(T));
    } 
}