using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Find
{
    class NameParameter : Parameter
    {
        private readonly Func<FileInfo, bool> _filter;

        protected NameParameter(string value) : base(value)
        {
            _filter = info => info.Name.Contains(value);
        }

        public static Parameter Creator(string value) => new NameParameter(value);

        public override IEnumerable<FileInfo> Execute(IEnumerable<FileInfo> items) => items.Where(_filter);
    }
}
