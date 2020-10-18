using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Find
{
    public class TypeParameter : Parameter
    {
        private readonly Func<FileInfo, bool> _filter;

        protected TypeParameter(string value) : base(value)
        {
            if (value == "f") _filter = info => !info.Attributes.HasFlag(FileAttributes.Directory);
            else if (value == "d") _filter = info => info.Attributes.HasFlag(FileAttributes.Directory);
            else throw new ArgumentException($"Unsupported value: '{value}' of type parameter");
        }

        public static Parameter Creator(string value) => new TypeParameter(value);

        public override IEnumerable<FileInfo> Execute(IEnumerable<FileInfo> items) => items.Where(_filter);
    }
}
