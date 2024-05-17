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
            Solution solution = await workspace.OpenSolutionAsync("K:\\Git\\Architector\\tst\\TestSolution\\TestSolution.sln");
            Console.WriteLine($"Solution {solution.FilePath} contains {solution.Projects.Count()} projects.");

            var stringBuilder = new StringBuilder();
            foreach (var project in solution.Projects)
            {
                stringBuilder.AppendLine("```mermaid");
                stringBuilder.AppendLine("---");
                stringBuilder.Append("title: ");
                stringBuilder.AppendLine(project.Name);
                stringBuilder.AppendLine("---");
                stringBuilder.AppendLine("classDiagram");

                var compilation = await project.GetCompilationAsync();

                foreach (var syntaxTree in compilation.SyntaxTrees)
                {
                    var semanticModel = compilation.GetSemanticModel(syntaxTree);

                    var surveyor = new Surveyor(semanticModel);
                    surveyor.Visit(syntaxTree.GetRoot());

                    Console.WriteLine(surveyor.ToString());



                    surveyor.Classes.ForEach(classDetails =>
                    {

                        stringBuilder.AppendLine($"class {classDetails.Identifier}{{");

                        classDetails.Fields.ForEach(fd =>
                        {
                            stringBuilder.AppendLine($"    -{fd.Declaration.Type} {fd.Declaration.Variables.Single().Identifier}");
                        });

                        classDetails.Properties.ForEach(pd =>
                        {
                            stringBuilder.AppendLine($"    +{pd.Type} {pd.Identifier}");
                        });


                        classDetails.Methods.ForEach(m =>
                        {
                            stringBuilder.AppendLine($"    +{m.Identifier}{m.ParameterList}");
                        });

                        stringBuilder.AppendLine("}");
                    });

                }

                stringBuilder.AppendLine("```");
            }

            Console.WriteLine(stringBuilder.ToString());
        }
    }
}
