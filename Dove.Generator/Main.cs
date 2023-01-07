using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

[Generator]
public class ParserGenerator : ISourceGenerator
{
    private IEnumerable<RecordDeclarationSyntax> GetAllMarkedClasses(Compilation context, string attributeName)
            => context.SyntaxTrees
                .SelectMany(st => st.GetRoot()
                .DescendantNodes()
                .OfType<RecordDeclarationSyntax>()
                .Where(r => r.AttributeLists
                    .SelectMany(al => al.Attributes)
                    .Any(a => a.Name.GetText().ToString().StartsWith(attributeName.Replace("Attribute", String.Empty)))));

    private IEnumerable<RecordDeclarationSyntax> GetAllTargetClasses(Compilation context, RecordDeclarationSyntax baseClass)
        => context.SyntaxTrees
            .SelectMany(st => st.GetRoot()
            .DescendantNodes()
            .OfType<RecordDeclarationSyntax>()
            .Where(c => c.BaseList?.Types.Any(t => t.ToString() == baseClass.Identifier.ToString()) ?? false))
            .Where(c => GetNamespace(c) == GetNamespace(baseClass) || baseClass.Identifier.ToString() == "Declaration"); // find a more general way to do this from attribute side

    private RecordDeclarationSyntax GetAllWrapperClasses(Compilation context, RecordDeclarationSyntax baseName)
    {
        var typeNamee = baseName.AttributeLists
            .SelectMany(al => al.Attributes)
            .Where(a => a.Name.GetText().ToString().StartsWith("WrapParser"))
            .SelectMany(a => a.DescendantNodes())
            .OfType<GenericNameSyntax>()
            .Select(par => par.TypeArgumentList?.Arguments[0].GetText())
            .FirstOrDefault();

        return context.SyntaxTrees
            .SelectMany(st => st.GetRoot()
                 .DescendantNodes()
                 .OfType<RecordDeclarationSyntax>()
                 .Where(c => c.Identifier.ToString() == typeNamee.ToString())
            )
            .FirstOrDefault();
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

        // get all classes that inherit from this class
        var children = GetAllTargetClasses(context, classDef);

        string body = String.Join(",\n        ", children.Select(c =>
        {
            var cacheName = GetFullPathName(c, new List<string>());
            return $"Lazy(() => Cast<{className}, {cacheName}>(IDeclaration<{cacheName}>.AsParser))";
        }));

        return (GetFullPathName(classDef, new List<string>()), $$$"""
            using static Core;
            using static ExtraTools.Extensions;
            using RootDecl;

            {{{(namespaceName != String.Empty ? $"namespace {namespaceName}" : String.Empty)}}};

            public partial record {{{className}}} : IDeclaration<{{{className}}}> {
                public static Parser<{{{className}}}> AsParser => TryRun(
                    converter: Core.Id,
                    {{{body}}}
                );
            }
            """);
    }

    public (string, string) HandleWrapping(Compilation context, RecordDeclarationSyntax classDef)
    {
        var className = classDef.Identifier.ToString();
        var namespaceName = GetNamespace(classDef);

        var targetName = GetFullPathName(GetAllWrapperClasses(context, classDef), new List<string>());


        return (GetFullPathName(classDef, new List<string>()), $$$"""
            using static Core;
            using static ExtraTools.Extensions;
            using RootDecl;
            
            {{{(namespaceName != String.Empty ? $"namespace {namespaceName}" : String.Empty)}}};

            public partial record {{{className}}}({{{targetName}}} Value) 
            {
                public override string ToString() => Value.ToString();
                public static Parser<{{{className}}}> AsParser => Map(
                    converter: value => new {{{className}}}(value),
                    IDeclaration<{{{targetName}}}>.AsParser
                );
            }
            """);
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var compilation = context.Compilation;
        var markedClasses = GetAllMarkedClasses(compilation, "GenerateParserAttribute");
        foreach (var classDef in markedClasses)
        {
            (string className, string classCode) = HandleUnionGeneration(compilation, classDef);
            context.AddSource($"{className}.p.g.cs", classCode);
        }


        var markedForWrap = GetAllMarkedClasses(compilation, "WrapParserAttribute");
        foreach (var classDef in markedForWrap)
        {
            (string className, string classCode) = HandleWrapping(compilation, classDef);
            context.AddSource($"{className}.w.g.cs", classCode);
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