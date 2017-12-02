using System.Windows.Controls;
using BPlusTree.Writables;

namespace BPlusTree.Blocks
{
    public interface IBlock : IWritable
    {
        long Address { get; set; }
        Grid CreateGrid();
    }
}
