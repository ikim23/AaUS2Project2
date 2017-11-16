using System.Linq;

namespace BPlusTree
{
    public class StringUtils
    {
        public static string Pad(string str)
        {
            var lines = str.Split('\n');
            return string.Join("\n", lines.Select(line => $"    {line}"));
        }

        public static string PadArrayItem(string str)
        {
            var lines = str.Split('\n');
            return string.Join("\n", lines.Select((line, index) => $"  {(index == 0 ? '-' : ' ')} {line}"));
        }
    }
}
