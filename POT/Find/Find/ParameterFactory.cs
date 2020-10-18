
using System.Collections.Generic;

namespace Find
{
    public delegate Parameter ParameterCreator(string value);

    public class ParameterFactory
    {
        private Dictionary<string, ParameterCreator> _creators = new Dictionary<string, ParameterCreator>();

        public void RegisterParameter(string name, ParameterCreator creator)
        {
            _creators[name] = creator;
        }

        public Parameter Build(string name, string value)
        {
            var creator = _creators[name];
            return creator.Invoke(value);
        }
    }
}
