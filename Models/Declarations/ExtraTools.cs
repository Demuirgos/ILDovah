public static class ExtraFunctions {
    public static bool StartsWith(this string code, string[] pool, out String attribute) {
        var result = pool.Any(x => code.StartsWith(x));
        if(result) {
            attribute = pool.First(x => code.StartsWith(x));
        } else {
            attribute = null;
        }
        return result;
    } 

    public static bool ConsumeWord(this string code, ref int index, string word) {
        if(code[index..].StartsWith(word)) {
            index += word.Length;
            return true;
        }
        return false;
    }

    public static bool ConsumeCount(this string code, ref int index, int count, out string result) {
        if(code.Length < index + count) {
            result = code[index..(index + count)];
            index += count;
            return true;
        } else {
            result = null;
            return false;
        }
    }

    public static bool ConsumeUntil(this string code, ref int index, Func<string, bool> predicate) {
        while(!predicate(code[index..])) {
            index++;
        }
        return true;
    }
}