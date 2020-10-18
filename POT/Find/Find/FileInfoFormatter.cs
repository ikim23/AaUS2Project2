using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Find
{
    public class FileInfoFormatter
    {
        public string Type { get; }
        public string DateModified { get; }
        public string ByteSize { get; }
        public string Name { get; }
        public string ParentDirectory { get; }

        public static int[] GetPadLengths(IList<FileInfoFormatter> formatters)
        {
            int type = formatters.Count == 0 ? 0 : formatters.Max(f => f.Type.Length);
            int dateModified = formatters.Count == 0 ? 0 : formatters.Max(f => f.DateModified.Length);
            int byteSize = formatters.Count == 0 ? 0 : formatters.Max(f => f.ByteSize.Length);
            int name = formatters.Count == 0 ? 0 : formatters.Max(f => f.Name.Length);
            return new[] { type, dateModified, byteSize, name };
        }

        public FileInfoFormatter(FileInfo info)
        {
            Type = info.Attributes.HasFlag(FileAttributes.Directory) ? "Dir" : "File";
            DateModified = $"{info.LastWriteTime:dd.MM.yyyy hh:mm:ss}";
            ByteSize = info.Attributes.HasFlag(FileAttributes.Directory) ? "" : info.Length.ToString();
            Name = info.Name;
            ParentDirectory = info.DirectoryName;
        }

        public string ToString(int[] paddings) => $"{Type.PadRight(paddings[0])} {DateModified.PadRight(paddings[1])} {ByteSize.PadRight(paddings[2])} {Name.PadRight(paddings[3])} {ParentDirectory}";
    }
}
