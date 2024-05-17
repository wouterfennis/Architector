using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System.Text;

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

            foreach (var project in solution.Projects)
            {
                var compilation = await project.GetCompilationAsync();

                foreach (var syntaxTree in compilation.SyntaxTrees)
                {
                    var semanticModel = compilation.GetSemanticModel(syntaxTree);

                    var surveyor = new Surveyor(semanticModel);
                    surveyor.Visit(syntaxTree.GetRoot());

                    Console.WriteLine(surveyor.ToString());

                    var s = new StringBuilder();

                    s.AppendLine("```mermaid");
                    s.AppendLine("---");
                    s.AppendLine("title: Example");
                    s.AppendLine("---");
                    s.AppendLine("classDiagram");

                    surveyor.Classes.ForEach(classDetails =>
                    {

                        s.AppendLine($"class {classDetails.Identifier}{{");

                        classDetails.Fields.ForEach(fd =>
                        {
                            s.AppendLine($"    -{fd.Declaration.Type} {fd.Declaration.Variables.Single().Identifier}");
                        });

                        classDetails.Properties.ForEach(pd =>
                        {
                            s.AppendLine($"    +{pd.Type} {pd.Identifier}");
                        });


                        classDetails.Methods.ForEach(m =>
                        {
                            s.AppendLine($"    +{m.Identifier}{m.ParameterList}");
                        });

                        s.AppendLine("}");
                    });

                    Console.WriteLine(s.ToString());
                }
            }

        }
    }
}
