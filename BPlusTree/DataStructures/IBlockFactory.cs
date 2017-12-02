using System;
using BPlusTree.Blocks;

namespace BPlusTree.DataStructures
{
    public interface IBlockFactory : IDisposable
    {
        IBlock GetRoot();
        void SetRoot(long addr);
        long GetFreeAddress();
        IBlock ReadBlock(long addr);
        void WriteBlock(IBlock block);
        void RemoveBlock(IBlock block);
        long Length();
    }
}
