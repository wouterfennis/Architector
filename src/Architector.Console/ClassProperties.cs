using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Architector.CommandLine
{
    public sealed class ClassProperties
    {
        public string Identifier { get; private set; }
        public List<FieldDeclarationSyntax> Fields { get; private set; }
        public List<PropertyDeclarationSyntax> Properties { get; private set; }
        public List<MethodDeclarationSyntax> Methods { get; private set; }

        private ClassProperties() { }

        public static ClassProperties Create(ClassDeclarationSyntax classDeclaration)
        {
            var fields = classDeclaration.Members.OfType<FieldDeclarationSyntax>().ToList();
            var properties = classDeclaration.Members.OfType<PropertyDeclarationSyntax>().ToList();
            var methods = classDeclaration.Members.OfType<MethodDeclarationSyntax>().ToList();

            return new ClassProperties
            {
                Identifier = classDeclaration.Identifier.Text,
                Fields = fields,
                Properties = properties,
                Methods = methods
            };
        }
    }
}
