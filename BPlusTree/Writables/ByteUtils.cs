using System;
using System.Linq;

namespace BPlusTree.Writables
{
    public class ByteUtils
    {
        public static int ByteSize(params IWritable[] writables) => writables.Sum(w => w.ByteSize);

        public static byte[] Join(params IWritable[] writables)
        {
            var size = ByteSize(writables);
            return Join(size, writables);
        }

        public static byte[] Join(int size, params IWritable[] writables)
        {
            var bytes = new byte[size];
            var dstIdx = 0;
            foreach (var w in writables)
            {
                Array.Copy(w.GetBytes(), 0, bytes, dstIdx, w.ByteSize);
                dstIdx += w.ByteSize;
            }
            return bytes;
        }

        public static void FromBytes(byte[] bytes, int index, params IWritable[] writables)
        {
            var srcIdx = index;
            foreach (var writable in writables)
            {
                writable.FromBytes(bytes, srcIdx);
                srcIdx += writable.ByteSize;
            }
        }
    }
}
