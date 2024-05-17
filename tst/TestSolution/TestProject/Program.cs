namespace TestProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var model = new Model("Test");

            Console.WriteLine(model.Name);
        }
    }
}
