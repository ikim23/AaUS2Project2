using System;
using System.Linq;
using BPlusTree.Writables;

namespace BPlusTree
{
    internal class ByteUtils
    {
        public static int ByteSize(params IWritable[] writables) => writables.Sum(w => w.ByteSize);

        public static byte[] Join(params IWritable[] writables)
        {
            var size = ByteSize(writables);
            var bytes = new byte[size];
            var dstIdx = 0;
            foreach (var w in writables)
            {
                Array.Copy(w.GetBytes(), 0, bytes, dstIdx, w.ByteSize);
                dstIdx += w.ByteSize;
            }
            return bytes;
        }
    }
}
