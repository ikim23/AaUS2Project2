using System;
using System.Collections.Generic;
using System.Linq;

namespace Find
{
    public class FindParser
    {
        private readonly ParameterFactory _factory;

        public FindParser(ParameterFactory factory)
        {
            _factory = factory;
        }

        public Find Parse(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine(@"Wrong input. Usage example: Find.exe -name=bin -type=f -mtime=10 C:\Windows");
                Environment.Exit(1);
            }
            var path = args.Last();
            var chunks = args.Take(args.Length - 1);
            List<Parameter> parameters = new List<Parameter>();
            foreach (var chunk in chunks)
            {
                var pNameValue = chunk.Substring(1).Split('=');
                try
                {
                    var parameter = _factory.Build(pNameValue[0], pNameValue[1]);
                    parameters.Add(parameter);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine($"Wrong input format. Failure cause:\n{e.Message}");
                    Environment.Exit(2);
                }
                catch (KeyNotFoundException e)
                {
                    Console.WriteLine($"Wrong input format. Parameter with name: {pNameValue[0]} does not exist.");
                    Environment.Exit(2);
                }
            }
            return new Find(path, parameters);
        }
    }
}
