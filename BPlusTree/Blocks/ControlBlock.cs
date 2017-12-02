using System.Windows.Controls;
using BPlusTree.Writables;

namespace BPlusTree.Blocks
{
    internal class ControlBlock : IBlock
    {
        public static char Type => 'C';

        public long Address { get; set; }

        public int ByteSize => ByteUtils.ByteSize(_type, _rootAddr, _emptyAddr);
        public long RootAddr
        {
            get => _rootAddr.Value;
            set => _rootAddr.Value = value;
        }
        public long EmptyAddr
        {
            get => _emptyAddr.Value;
            set => _emptyAddr.Value = value;
        }
        private readonly WritableChar _type = new WritableChar(Type);
        private readonly WritableLong _rootAddr = new WritableLong();
        private readonly WritableLong _emptyAddr = new WritableLong();

        public ControlBlock()
        {
            Address = 0;
            RootAddr = long.MinValue;
            EmptyAddr = ByteSize;
        }

        public byte[] GetBytes() => ByteUtils.Join(_type, _rootAddr, _emptyAddr);

        public void FromBytes(byte[] bytes, int index = 0) => ByteUtils.FromBytes(bytes, index + _type.ByteSize, _rootAddr, _emptyAddr);

        public override string ToString() => $"Type: {Type} Addr: {Address} Empty: {EmptyAddr} Root: {RootAddr}";

        public Grid CreateGrid() => UiUtils.CreateGrid(this);
    }
}
