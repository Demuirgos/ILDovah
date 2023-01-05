using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerator
{
    [Generator]
    public class HelloSourceGenerator : ISourceGenerator
    {
        private IEnumerable<ClassDeclarationSyntax> GetAllMarkedClasses(Compilation context)
        {
            IEnumerable<SyntaxNode> allNodes = context.SyntaxTrees.SelectMany(s => s.GetRoot().DescendantNodes());
            return allNodes
                .Where(d => d.IsKind(SyntaxKind.ClassDeclaration))
                .OfType<ClassDeclarationSyntax>()
                .Where(
                    classDef => classDef.AttributeLists.Any(
                        attrList => attrList.Attributes.Any(
                            attr => attr.Name.ToString() == nameof(GenerateParserAttribute).Replace("Attribute", String.Empty)
                        )
                    )
                );
        }

        public string HandleClass(ClassDeclarationSyntax classDef)
        {
            var className = classDef.Identifier.ToString();
            // get all classes that inherit from this class
            var children = classDef.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(c => c.BaseList?.Types.Any(t => t.ToString() == className) ?? false);

            return $$$"""
            using static Core;
            using static Extensions;

            public partial record {{{className}}} : IDeclaration<{{{className}}}> {{
                public int test = 5;
                public static Parser<{{{className}}}> AsParser => TryRun(
                    Converter: Core.Id,
                    {{{
                        String.Join(",\n", children.Select(c => $"Cast<{className}, {c.Identifier}>(IDeclaration<{c.Identifier}>.AsParser)"))
                    }}}
                )
            }}
            """;
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            var markedClasses = GetAllMarkedClasses(compilation);
            foreach (var classDef in markedClasses)
            {
                context.AddSource($"{classDef.Identifier}.cs", HandleClass(classDef));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
        }
#endif 
        }
    }
}