using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace Architector.CommandLine
{
    internal class Program
    {
        const string programText =
            @"using System;
            using System.Collections;
            using System.Linq;
            using System.Text;

            namespace HelloWorld
            {
                class Program
                {
                    private string _name;
                    public string Name { get; set; }

                    static void Main(string[] args)
                    {
                        Console.WriteLine(""Hello, World!"");
                    }
                }
            }";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();


            // recursive search for all classes in the syntax tree root
            var classes = RecursiveSearchClasses(root).ToList();

            var s = new StringBuilder();

            s.AppendLine("```mermaid");
            s.AppendLine("---");
            s.AppendLine("title: Example");
            s.AppendLine("---");
            s.AppendLine("classDiagram");

            classes.ForEach(n =>
            {

                var test = ClassProperties.Create(n);
            s.AppendLine($"class {test.Identifier}{{");

            test.Fields.ForEach(fd => {
                s.AppendLine($"    -{fd.Declaration.Type} {fd.Declaration.Variables.Single().Identifier}");
            });

            test.Properties.ForEach(pd => {
                s.AppendLine($"    +{pd.Type} {pd.Identifier}");
            });


            test.Methods.ForEach(m => {
                s.AppendLine($"    +{m.Identifier}{m.ParameterList}");
            });

                s.AppendLine("}");
            });

            s.AppendLine("```");

            var result = s.ToString();
            Console.WriteLine(result);
        }

        private static IEnumerable<ClassDeclarationSyntax> RecursiveSearchClasses(SyntaxNode node)
        {
            List<ClassDeclarationSyntax> classes = new List<ClassDeclarationSyntax>();

            foreach (var child in node.ChildNodes())
            {
                if (child is ClassDeclarationSyntax classDeclaration)
                {
                    classes.Add(classDeclaration);
                }

                classes.AddRange(RecursiveSearchClasses(child));
            }

            return classes;
        }
    }
}
