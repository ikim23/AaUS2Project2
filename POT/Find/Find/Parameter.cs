using System.Collections.Generic;
using System.IO;

namespace Find
{
    public abstract class Parameter
    {
        protected Parameter(string value)
        {
        }

        public abstract IEnumerable<FileInfo> Execute(IEnumerable<FileInfo> items);
    }
}
