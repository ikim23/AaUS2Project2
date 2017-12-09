using System;
using BPlusTree.Blocks;

namespace BPlusTree.DataStructures
{
    public interface IBlockFactory : IDisposable
    {
        int ControlBlockByteSize { get; }
        int BlockByteSize { get; }
        int DataBlockRecordSize { get; }
        IBlock GetRoot();
        void SetRoot(long addr);
        bool IsRoot(long addr);
        void RemoveBlock(IBlock block);
        IBlock ReadBlock(long addr);
        void WriteBlock(IBlock block);
        long GetFreeAddress();
        long Length();
    }
}
