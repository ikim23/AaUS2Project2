using System;

namespace Find
{
    class Program
    {
        static void Main(string[] args)
        {
            ParameterFactory factory = new ParameterFactory();
            factory.RegisterParameter("name", NameParameter.Creator);
            factory.RegisterParameter("type", TypeParameter.Creator);
            factory.RegisterParameter("mtime", MTimeParameter.Creator);

            FindParser parser = new FindParser(factory);
            var find = parser.Parse(args);

            var results = find.Execute();
            foreach (var item in results)
            {
                Console.WriteLine(item);
            }
        }
    }
}
