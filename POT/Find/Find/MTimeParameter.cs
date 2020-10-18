using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Find
{
    class MTimeParameter : Parameter
    {
        private readonly Func<FileInfo, bool> _filter;

        protected MTimeParameter(string value) : base(value)
        {
            try
            {
                var days = Int32.Parse(value);
                _filter = info => (DateTime.Now - info.LastWriteTime).Days <= days;
            }
            catch (Exception)
            {
                throw new ArgumentException($"Unsupported value: '{value}' of mtime parameter, only integer value supported");
            }
        }

        public static Parameter Creator(string value) => new MTimeParameter(value);

        public override IEnumerable<FileInfo> Execute(IEnumerable<FileInfo> items) => items.Where(_filter);
    }
}
