using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using System.Diagnostics.SymbolStore;
using System.Reflection;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Architector.CommandLine
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            // Create a MSBuild workspace.
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();

            // Open the solution.
            Solution solution = workspace.OpenSolutionAsync("K:\\Git\\Architector\\Architector.sln").Result;

            foreach ( var project in solution.Projects)
            {
                var compilation = await project.GetCompilationAsync();
                foreach (var syntaxTree in compilation.SyntaxTrees)
                {
                    var semanticModel = compilation.GetSemanticModel(syntaxTree);
                    var root = syntaxTree.GetRoot();
                    var classes = RecursiveSearchClasses(root).ToList();
                    foreach (var classDeclaration in classes)
                    {
                        var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);
                        // get references to the class
                        var references = await SymbolFinder.FindReferencesAsync(classSymbol, solution);

                        foreach (var reference in references)
                        {
                            foreach (var referenceLocation in reference.Locations)
                            {
                                var referenceSyntax = root.FindNode(referenceLocation.Location.SourceSpan);
                                Console.WriteLine(referenceSyntax);
                            }
                        }
                    }
                }
            }

            // recursive search for all classes in the syntax tree root
           // var classes = RecursiveSearchClasses(root).ToList();

            //var s = new StringBuilder();

            //s.AppendLine("```mermaid");
            //s.AppendLine("---");
            //s.AppendLine("title: Example");
            //s.AppendLine("---");
            //s.AppendLine("classDiagram");

            //classes.ForEach(n =>
            //{

            //    var test = ClassProperties.Create(n);
            //    s.AppendLine($"class {test.Identifier}{{");

            //    test.Fields.ForEach(fd => {
            //        s.AppendLine($"    -{fd.Declaration.Type} {fd.Declaration.Variables.Single().Identifier}");
            //    });

            //    test.Properties.ForEach(pd => {
            //        s.AppendLine($"    +{pd.Type} {pd.Identifier}");
            //    });


            //    test.Methods.ForEach(m => {
            //        s.AppendLine($"    +{m.Identifier}{m.ParameterList}");
            //    });

            //    // Get the symbol for the OtherClass type.
            //    INamedTypeSymbol otherClassSymbol = semanticModel.Compilation.GetTypeByMetadataName(test.Identifier);

            //    // Find all references to the OtherClass type in the code.
            //    IEnumerable<ReferencedSymbol> references = SymbolFinder.FindReferencesAsync(otherClassSymbol, solution).Result;

            //    // Print the locations of all references to the OtherClass type.
            //    foreach (ReferencedSymbol reference in references)
            //    {
            //        foreach (ReferenceLocation location in reference.Locations)
            //        {
            //            Console.WriteLine(location.Location.GetLineSpan());
            //        }
            //    }

            //    s.AppendLine("}");
            //});


            //s.AppendLine("```");

            //var result = s.ToString();
         //   Console.WriteLine(result);
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
