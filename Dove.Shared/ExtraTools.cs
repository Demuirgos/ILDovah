using System;
using System.Linq;
namespace ExtraTools;
public class NotImplementedAttribute : System.Attribute { }
public static class Extensions
{
    public static string PadRSpaces<T>(this T word, int spaceCount = 1)
        => $"{word}{new string(' ', spaceCount)}";
    public static T Construct<T>(int argsCount, int argIndex, Object? argument)
    {
        var constructor = typeof(T).GetConstructors()
                                                    .Where(c => c.GetParameters().Length == argsCount)
                                                    .First();

        var arguments = new object[argsCount];
        for (int i = 0; i < argsCount; i++)
        {
            if (i == argIndex)
            {
                arguments[i] = argument;
            }
            else
            {
                arguments[i] = null;
            }
        }

        var instance = constructor.Invoke(arguments);
        return (T)instance;
    }
}