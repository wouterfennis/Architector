using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace Architector.CommandLine
{
    public class Surveyor : CSharpSyntaxWalker
    {
        private readonly SemanticModel _semanticModel;
        private readonly StringBuilder _stringBuilder;

        public List<ClassProperties> Classes { get; }

        public Surveyor(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
            _stringBuilder = new StringBuilder();
            Classes = new List<ClassProperties>();
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var classProperties = ClassProperties.Create(node);
            Classes.Add(classProperties);
        }

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }
    }
}
