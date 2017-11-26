using BPlusTree.Writables;

namespace BPlusTree.Blocks
{
    public class EmptyBlock : IBlock
    {
        public static char Type => 'E';

        public long Address { get; set; }
        public int ByteSize => ByteUtils.ByteSize(_type, _prevEmptyBlock, _nextEmptyBlock);
        public long PrevAddr
        {
            get => _prevEmptyBlock.Value;
            set => _prevEmptyBlock.Value = value;
        }
        public long NextAddr
        {
            get => _nextEmptyBlock.Value;
            set => _nextEmptyBlock.Value = value;
        }
        private readonly WritableChar _type = new WritableChar(Type);
        private readonly WritableLong _prevEmptyBlock = new WritableLong(long.MinValue);
        private readonly WritableLong _nextEmptyBlock = new WritableLong(long.MinValue);

        public bool HasPrev() => PrevAddr != long.MinValue;

        public bool HasNext() => NextAddr != long.MinValue;

        public byte[] GetBytes() => ByteUtils.Join(_type, _prevEmptyBlock, _nextEmptyBlock);

        public void FromBytes(byte[] bytes, int index = 0) => ByteUtils.FromBytes(bytes, index + _type.ByteSize, _prevEmptyBlock, _nextEmptyBlock);

        public override string ToString() => $"A: {Address}, P: {PrevAddr}, N: {NextAddr}";
    }
}
