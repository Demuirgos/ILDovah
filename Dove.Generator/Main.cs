using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AttributeResult = System.ValueTuple<int, string>;

public enum Order { First = 0, Middle = 1, Last = 2 }
[Generator]
public class ParserGenerator : ISourceGenerator
{
    private HashSet<int> WrapperCache = new();
    private IEnumerable<RecordDeclarationSyntax> GetAllMarkedClasses(Compilation context, string attributeName)
            => context.SyntaxTrees
                .SelectMany(st => st.GetRoot()
                .DescendantNodes()
                .OfType<RecordDeclarationSyntax>()
                .Where(r => GetMark(r, attributeName) is not null));
    private AttributeSyntax GetMark(RecordDeclarationSyntax classDeclaration, string attributeName)
        => classDeclaration.AttributeLists
            .SelectMany(al => al.Attributes)
            .Where(a => a.Name.GetText().ToString().StartsWith(attributeName.Replace("Attribute", String.Empty)))
            .FirstOrDefault();

    private IEnumerable<RecordDeclarationSyntax> GetAllTargetClasses(Compilation context, RecordDeclarationSyntax baseClass)
        => context.SyntaxTrees
            .SelectMany(st => st.GetRoot()
            .DescendantNodes()
            .OfType<RecordDeclarationSyntax>()
            .Where(c => c.BaseList?.Types.Any(t => t.ToString() == baseClass.Identifier.ToString()) ?? false))
            .Where(c => GetNamespace(c) == GetNamespace(baseClass) || baseClass.Identifier.ToString() == "Declaration"); // find a more general way to do this from attribute side

    private (AttributeResult attrRes, string[] result) GetAllWrapperClasses(Compilation context, RecordDeclarationSyntax baseName)
    {
        var typesNames = baseName.AttributeLists
            .SelectMany(al => al.Attributes)
            .Where(a => a.Name.GetText().ToString().StartsWith("WrapParser"))
            .SelectMany(a => a.DescendantNodes())
            .OfType<GenericNameSyntax>()
            .SelectMany(par => par.TypeArgumentList?.Arguments.Select(arg => arg.GetText()))
            .ToList();

        string attrCode = String.Empty;
        if (typesNames.Count > 0)
        {
            string generics = String.Join(", ", Enumerable.Range(0, typesNames.Count).Select(i => $"T{i}"));
            attrCode = $"public class WrapParserAttribute<{generics}> : System.Attribute {{}}";
        }
        else
        {
            throw new Exception("WrapParserAttribute must have at least one type argument");
        }

        var results = typesNames.Select(typeNameNode =>
        {
            var typeName = typeNameNode.ToString();
            var targetSubType = String.Empty;
            bool containsDot = false;
            if (typeName.Contains('.'))
            {
                containsDot = true;
                if (typeName.Substring(0, typeName.IndexOf('.')).EndsWith("Decl"))
                {
                    return typeName;
                }
                targetSubType = typeName.Substring(typeName.IndexOf(".") + 1);
                typeName = typeName.Substring(0, typeName.IndexOf("."));
            }

            var targetType = context.SyntaxTrees
                .SelectMany(st => st.GetRoot()
                        .DescendantNodes()
                        .OfType<RecordDeclarationSyntax>()
                        .Where(c => c.Identifier.ToString() == typeName)
                )
                .FirstOrDefault();
            if (containsDot)
            {
                return $"{(targetType is not null ? GetFullPathName(targetType, new List<string>()) : typeName)}.{targetSubType}";
            }
            else
            {
                return targetType is not null ? GetFullPathName(targetType, new List<string>()) : typeName;
            }
        }).ToArray();
        return ((typesNames.Count, attrCode), results);
    }

    public static string GetNamespace(SyntaxNode node)
    {
        if (node.Parent is FileScopedNamespaceDeclarationSyntax or NamespaceDeclarationSyntax)
            return node.Parent switch
            {
                FileScopedNamespaceDeclarationSyntax f => f.Name.ToString(),
                NamespaceDeclarationSyntax n => n.Name.ToString(),
                _ => throw new NotImplementedException(),
            };

        else
            return node.Parent is not null ? GetNamespace(node.Parent) : String.Empty;
    }

    public static string GetFullPathName(SyntaxNode node, List<string> sb)
    {
        List<T> Prepend<T>(List<T> list, T item)
        {
            list.Insert(0, item);
            return list;
        }

        if (node is BaseNamespaceDeclarationSyntax)
            return String.Join(".", Prepend(sb,
                node switch
                {
                    FileScopedNamespaceDeclarationSyntax fsns => fsns.Name.ToString(),
                    NamespaceDeclarationSyntax cbns => cbns.Name.ToString(),
                    _ => throw new Exception()
                }));

        else return node switch
        {
            RecordDeclarationSyntax r => GetFullPathName(node.Parent, Prepend(sb, r.Identifier.ToString())),
            ClassDeclarationSyntax c => GetFullPathName(node.Parent, Prepend(sb, c.Identifier.ToString())),
            _ => String.Join(".", sb)
        };
    }

    public (string, string) HandleUnionGeneration(Compilation context, RecordDeclarationSyntax classDef)
    {
        var className = classDef.Identifier.ToString();
        var namespaceName = GetNamespace(classDef);

        var children =
            GetAllTargetClasses(context, classDef).Select(r =>
            {
                if (GetMark(r, "GenerationOrderParserAttribute") is AttributeSyntax attrs)
                {
                    var arg = attrs?.ArgumentList?.Arguments.FirstOrDefault().Expression.ToString();
                    arg = arg.Substring(arg.LastIndexOf('.') + 1);
                    if (Enum.TryParse<Order>(arg, out var o))
                    {
                        return (r, (int)o);
                    }
                }
                return (r, 1);
            }).OrderBy(t => t.Item2);

        var body = children.Select(c => GetFullPathName(c.r, new List<string>()));
        string prefix = $$$"""
        using static Core;
        using static ExtraTools.Extensions;
        using RootDecl;

        {{{(namespaceName != String.Empty ? $"namespace {namespaceName}" : String.Empty)}}};
        """;

        return (GetFullPathName(classDef, new List<string>()), $"{prefix}\n{HandleUnionGeneration(className, body)}");
    }

    public string HandleUnionGeneration(string BaseClassName, IEnumerable<string> childClassNames)
    {
        string body = String.Join(",\n        ", childClassNames.Select(children =>
        {
            return $"Lazy(() => Cast<{BaseClassName}, {children}>(IDeclaration<{children}>.AsParser))";
        }));

        string casts = String.Join("\n    ", childClassNames.Select(child =>
        {
            string childTypeName = child.Substring(child.LastIndexOf('.') + 1);
            return $"public {child}? As{childTypeName}() => this is {child} c ? c : null;";
        }));

        return $$$"""
        public partial record {{{BaseClassName}}} : IDeclaration<{{{BaseClassName}}}> {
            {{{casts}}}
            public static Parser<{{{BaseClassName}}}> AsParser => TryRun(
                converter: Core.Id,
                {{{body}}}
            );
        }
        """;
    }

    public (AttributeResult, string, string) HandleWrapping(Compilation context, RecordDeclarationSyntax classDef)
    {

        var className = classDef.Identifier.ToString();
        var namespaceName = GetNamespace(classDef);

        var targetName = GetAllWrapperClasses(context, classDef);
        bool HasSuffix = targetName.result.Length > 1;

        string GetSuffix(string str) => str.Substring(str.IndexOf('.') + 1);
        string targetClassSubname(string targetName) => $"{className}{(HasSuffix ? $"_{GetSuffix(targetName)}" : String.Empty)}";

        string prefix = $$$"""
        using static Core;
        using static ExtraTools.Extensions;
        using RootDecl;
        
        {{{(namespaceName != String.Empty ? $"namespace {namespaceName};" : String.Empty)}}}
        {{{(HasSuffix ? HandleUnionGeneration(className, targetName.result.Select(targetName => targetClassSubname(targetName))) : String.Empty)}}}
        """;

        // emit types and hook inheritence tree if count > 1
        string body = String.Join("\n", targetName.result.Select(targetName =>
        {
            var targetClass = targetClassSubname(targetName);
            return $$$"""
            public partial record {{{targetClass}}}
                ({{{targetName}}} Value) : {{{(HasSuffix ? $"{className}, " : String.Empty)}}}IDeclaration<{{{targetClass}}}>
            {
                public override string ToString() => Value.ToString();
                public static Parser<{{{targetClass}}}> AsParser => Map(
                    converter: value => new {{{targetClass}}}(value),
                    IDeclaration<{{{targetName}}}>.AsParser
                );
            }

            """;
        }));

        return (targetName.attrRes, GetFullPathName(classDef, new List<string>()), $"{prefix}\n{body}");
    }

    public void Execute(GeneratorExecutionContext context)
    {
        WrapperCache.Clear();
        var compilation = context.Compilation;

        var markedForWrap = GetAllMarkedClasses(compilation, "WrapParserAttribute");
        foreach (var classDef in markedForWrap)
        {
            (var attrcode, string className, string classCode) = HandleWrapping(compilation, classDef);
            if (!WrapperCache.Contains(attrcode.Item1))
            {
                context.AddSource($"WrapAttribute.{attrcode.Item1}.w.g.cs", attrcode.Item2);
                WrapperCache.Add(attrcode.Item1);
            }
            context.AddSource($"{className}.w.g.cs", classCode);
        }

        var markedClasses = GetAllMarkedClasses(compilation, "GenerateParserAttribute");
        foreach (var classDef in markedClasses)
        {
            (string className, string classCode) = HandleUnionGeneration(compilation, classDef);
            context.AddSource($"{className}.p.g.cs", classCode);
        }

    }
    public void Initialize(GeneratorInitializationContext context)
    {
#if DEBUG
        // if (!Debugger.IsAttached)
        // {
        //     Debugger.Launch();
        // }
#endif 
    }
}