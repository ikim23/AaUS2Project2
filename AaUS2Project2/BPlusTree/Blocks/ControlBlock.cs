using System;
using System.Windows.Controls;
using BPlusTree.Writables;

namespace BPlusTree.Blocks
{
    internal class ControlBlock : IBlock
    {
        public static readonly int StringLength = 250;
        public static char Type => 'C';

        public long Address { get; set; }
        public int ByteSize { get; }
        public Type KeyType
        {
            get => _keyType;
            set
            {
                _keyType = value;
                _keyTypeStr.Value = value.AssemblyQualifiedName;
            }
        }
        public Type ValueType
        {
            get => _valueType;
            set
            {
                _valueType = value;
                _valueTypeStr.Value = value.AssemblyQualifiedName;
            }
        }
        public int DataBlockRecordSize
        {
            get => _dataBlockRecordSize.Value;
            set => _dataBlockRecordSize.Value = value;
        }
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
        private Type _keyType;
        private Type _valueType;
        private readonly WritableChar _type = new WritableChar(Type);
        private readonly WritableString _keyTypeStr = new WritableString(StringLength);
        private readonly WritableString _valueTypeStr = new WritableString(StringLength);
        private readonly WritableInt _dataBlockRecordSize = new WritableInt();
        private readonly WritableLong _rootAddr = new WritableLong();
        private readonly WritableLong _emptyAddr = new WritableLong();

        public ControlBlock()
        {
            Address = 0;
            ByteSize = ByteUtils.ByteSize(_type, _keyTypeStr, _valueTypeStr, _dataBlockRecordSize, _rootAddr, _emptyAddr);
            RootAddr = long.MinValue;
            EmptyAddr = ByteSize;
        }

        public byte[] GetBytes() => ByteUtils.Join(_type, _keyTypeStr, _valueTypeStr, _dataBlockRecordSize, _rootAddr, _emptyAddr);

        public void FromBytes(byte[] bytes, int index = 0)
        {
            ByteUtils.FromBytes(bytes, index + _type.ByteSize, _keyTypeStr, _valueTypeStr, _dataBlockRecordSize, _rootAddr, _emptyAddr);
            _keyType = System.Type.GetType(_keyTypeStr.Value);
            _valueType = System.Type.GetType(_valueTypeStr.Value);
        }

        public override string ToString() => $"Type: {Type} Addr: {Address} Empty: {EmptyAddr} Root: {RootAddr}";

        public Grid CreateGrid() => UiUtils.CreateGrid(this);
    }
}
