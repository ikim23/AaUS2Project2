using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Find
{
    public class Find
    {
        private readonly IList<Parameter> _parameters;
        private readonly string _path;

        public Find(string path, IList<Parameter> parameters)
        {
            _parameters = parameters;
            _path = path;
        }

        public IEnumerable<string> Execute()
        {
            if(!Directory.Exists(_path)) throw new Exception();
            var entries = Directory.GetFileSystemEntries(_path, "*", SearchOption.AllDirectories).Select(name => new FileInfo(name));
            foreach (var parameter in _parameters)
            {
                entries = parameter.Execute(entries);
            }
            var result = Format(entries);
            return result;
        }

        public IEnumerable<string> Format(IEnumerable<FileInfo> entries)
        {
            var formatters = entries.Select(e => new FileInfoFormatter(e)).ToList();
            var paddings = FileInfoFormatter.GetPadLengths(formatters);
            return formatters.Select(f => f.ToString(paddings));
        }
    }
}
