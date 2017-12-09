using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using BPlusTree.DataStructures;
using BPlusTree.Writables;
using System.Linq;
using System.Windows.Media;

namespace BPlusTree.Blocks
{
    public class DataBlock<TK, TV> : IBlock, IEnumerable<TV> where TK : IComparable<TK>, IWritable, new() where TV : IWritable, new()
    {
        public static char Type => 'D';

        public long Address { get; set; }
        public int ByteSize => ByteUtils.ByteSize(_type, _nextBlock, _records);
        public int FillFactor => Math.Max((int)(Math.Ceiling((double)_records.MaxSize / 2) - 1), 1);
        public int Count => _records.Count;
        public int MaxSize => _records.MaxSize;
        public long NextBlock
        {
            get => _nextBlock.Value;
            set => _nextBlock.Value = value;
        }
        private readonly WritableChar _type = new WritableChar(Type);
        private readonly WritableLong _nextBlock = new WritableLong(long.MinValue);
        private SortedBlock<TK, TV> _records;

        public DataBlock(int size)
        {
            Address = long.MinValue;
            _records = new SortedBlock<TK, TV>(size);
        }

        public DataBlock(int size, IEnumerable<Tuple<TK, TV>> items)
        {
            Address = long.MinValue;
            _records = new SortedBlock<TK, TV>(size, items);
        }

        public DataBlock<TK, TV> Split(TK key, TV value, out TK middle)
        {
            var rightRecords = _records.Split(key, value, out middle);
            var rightBlock = new DataBlock<TK, TV>(MaxSize)
            {
                _records = rightRecords,
                NextBlock = NextBlock
            };
            return rightBlock;
        }

        public bool IsFull() => _records.IsFull();

        public bool IsUnderFlow() => _records.Count < FillFactor;

        public bool CanBorrow() => _records.Count > FillFactor;

        public void Insert(TK key, TV value) => _records.Insert(key, value);

        public TV Find(TK key) => _records.Find(key);

        public bool Contains(TK key) => _records.Contains(key);

        public void Remove(TK key) => _records.Remove(key);

        public TK ShiftMaxFromLeft(DataBlock<TK, TV> left) => _records.ShiftMaxFromLeft(left._records);

        public TK ShiftMinFromRight(DataBlock<TK, TV> right) => _records.ShiftMinFromRight(right._records);

        public void Merge(DataBlock<TK, TV> right)
        {
            _records.Merge(right._records);
        }

        public TK MinKey() => _records.MinKey();

        public byte[] GetBytes() => ByteUtils.Join(_type, _nextBlock, _records);

        public void FromBytes(byte[] bytes, int index = 0) => ByteUtils.FromBytes(bytes, index + _type.ByteSize, _nextBlock, _records);

        public IEnumerator<TV> GetEnumerator() => _records.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => $"Type: {Type} Addr: {Address} Next: {NextBlock} Records: {_records.Count}";

        public Grid CreateGrid()
        {
            var grid = new Grid();
            var colLeft = new ColumnDefinition();
            var colRight = new ColumnDefinition();
            grid.ColumnDefinitions.Add(colLeft);
            grid.ColumnDefinitions.Add(colRight);
            var colorSwaper = UiUtils.GetColor().GetEnumerator();
            colorSwaper.MoveNext();
            var rowIndex = 0;
            UiUtils.AddGridRow(grid, rowIndex++, "Type:", Type, colorSwaper.Current);
            UiUtils.AddGridRow(grid, rowIndex++, "ByteSize:", ByteSize, colorSwaper.Current);
            UiUtils.AddGridRow(grid, rowIndex++, "Address:", Address, colorSwaper.Current);
            UiUtils.AddGridRow(grid, rowIndex++, "NextBlock:", NextBlock, colorSwaper.Current);
            UiUtils.AddGridRow(grid, rowIndex++, "MaxSize:", MaxSize, colorSwaper.Current);
            UiUtils.AddGridRow(grid, rowIndex++, "Records:", null, colorSwaper.Current);
            var recordIndex = 0;
            foreach (var record in _records)
            {
                var recordType = record.GetType();
                var recordProps = recordType.GetProperties();
                colorSwaper.MoveNext();
                UiUtils.AddGridRow(grid, rowIndex++, $"Record {recordIndex++}:", null, Colors.Transparent);
                foreach (var prop in recordProps)
                {
                    var propType = prop.PropertyType;
                    if (!propType.IsGenericType)
                    {
                        var value = prop.GetValue(record);
                        UiUtils.AddGridRow(grid, rowIndex++, prop.Name, value, colorSwaper.Current, 25);
                    }
                    else
                    {
                        var propTypeInterfaces = propType.GetInterfaces();
                        var isEnumerable = propTypeInterfaces.Contains(typeof(IEnumerable));
                        if (isEnumerable)
                        {
                            UiUtils.AddGridRow(grid, rowIndex++, $"{prop.Name}:", null, colorSwaper.Current, 25);
                            var enumerable = (IEnumerable)prop.GetValue(record);
                            var index = 0;
                            foreach (var item in enumerable)
                            {
                                if (item == null) break;
                                var itemType = item.GetType();
                                var itemProps = itemType.GetProperties();
                                colorSwaper.MoveNext();
                                UiUtils.AddGridRow(grid, rowIndex++, $"{prop.Name} {index++}:", null, Colors.Transparent, 50);
                                foreach (var itemProp in itemProps)
                                {
                                    var value = itemProp.GetValue(item);
                                    UiUtils.AddGridRow(grid, rowIndex++, itemProp.Name, value, colorSwaper.Current, 50);
                                }
                            }
                        }
                    }
                }
            }
            colorSwaper.Dispose();
            return grid;
        }
    }
}
