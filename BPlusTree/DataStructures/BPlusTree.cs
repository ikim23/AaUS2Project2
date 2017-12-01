using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BPlusTree.Blocks;
using BPlusTree.Writables;

namespace BPlusTree.DataStructures
{
    public class BPlusTree<TK, TV> where TK : IComparable<TK>, IWritable, new() where TV : IWritable, new()
    {
        private readonly BlockFactory<TK, TV> _factory;

        public BPlusTree(int blockSize, string file = "bptree.bin")
        {
            _factory = new BlockFactory<TK, TV>(blockSize, file);
        }

        public IEnumerable<TV> InOrder()
        {
            var block = _factory.GetRoot();
            if (block == null) yield break;
            IndexBlock<TK> parentBlock = null;
            while (block is IndexBlock<TK>)
            {
                parentBlock = (IndexBlock<TK>)block;
                var childAddress = parentBlock.MinAddress();
                block = _factory.ReadBlock(childAddress);
            }

            long nextAddr = block.Address;
            do
            {
                DataBlock<TK, TV> dataBlock = (DataBlock<TK, TV>)_factory.ReadBlock(nextAddr);
                var kv = dataBlock.ToKeyValueArray();
                foreach (var tuple in kv)
                {
                    yield return tuple.Item2;
                }
                nextAddr = dataBlock.NextBlock;
            } while (nextAddr != long.MinValue);
        }

        public bool CheckInternalNodeOrder()
        {
            var block = _factory.GetRoot();
            IndexBlock<TK> parentBlock = null;
            while (block is IndexBlock<TK>)
            {
                parentBlock = (IndexBlock<TK>)block;
                var childAddress = parentBlock.MinAddress();
                block = _factory.ReadBlock(childAddress);
            }

            long nextAddr = block.Address;
            do
            {
                DataBlock<TK, TV> dataBlock = (DataBlock<TK, TV>)_factory.ReadBlock(nextAddr);
                var kv = dataBlock.ToKeyValueArray();
                Console.WriteLine(string.Join(" ", kv.Select(i => i.Item1)));
                nextAddr = dataBlock.NextBlock;
            } while (nextAddr != long.MinValue);
            return true;
        }

        public void Print()
        {
            using (var w = new StreamWriter(new FileStream("out.txt", FileMode.Create)))
            {
                var childrenOuter = new List<long>();
                childrenOuter.Add(_factory.GetRoot().Address);
                var children = new List<long>();
                while (childrenOuter.Count > 0)
                {
                    var keysLine = "";
                    var addressLine = "";
                    foreach (var addr in childrenOuter)
                    {
                        var block = _factory.ReadBlock(addr);
                        if (block is IndexBlock<TK>)
                        {
                            IndexBlock<TK> indexBlock = (IndexBlock<TK>)block;
                            var addresses = string.Join(" ", indexBlock._children.Value.Take(indexBlock._keys.Count + 1).Select(v => v.Value));
                            var keys = string.Join(" ", indexBlock._keys).PadRight(addresses.Length);
                            keysLine += keys + " | ";
                            addressLine += addresses + " | ";
                            children.AddRange(indexBlock._children.Value.Take(indexBlock._keys.Count + 1)
                                .Select(v => v.Value));
                        }
                        else return;
                    }
                    w.WriteLine(keysLine);
                    w.WriteLine(addressLine);
                    childrenOuter.Clear();
                    childrenOuter.AddRange(children);
                    children.Clear();
                }
            }
        }

        public TV Find(TK key)
        {
            var block = _factory.GetRoot();
            if (block == null) throw new KeyNotFoundException();
            IndexBlock<TK> parentBlock = null;
            while (block is IndexBlock<TK>)
            {
                parentBlock = (IndexBlock<TK>)block;
                //var childAddress = parentBlock.GetChildAddress(key);
                var childAddress = parentBlock.Find(key);
                block = _factory.ReadBlock(childAddress);
            }
            var dataBlock = (DataBlock<TK, TV>)block;
            //Console.Write(key + ": ");
            //var vals = dataBlock.ToKeyValueArray().Select(kv => kv.Item2);
            //Console.WriteLine(string.Join(",", vals));
            return dataBlock.Find(key);
        }

        public void Insert(TK key, TV value)
        {
            var block = _factory.GetRoot();
            if (block == null) // initialize data 1st block (1st block at all)
            {
                block = new DataBlock<TK, TV>(_factory.DataBlockRecordSize);
                block.Address = _factory.GetFreeAddress();
                _factory.SetRoot(block.Address);
            }
            var parentAddresses = new Stack<long>();
            while (block is IndexBlock<TK>)
            {
                var indexBlock = (IndexBlock<TK>)block;
                parentAddresses.Push(indexBlock.Address);
                var childAddress = indexBlock.GetChildAddress(key);
                block = _factory.ReadBlock(childAddress);
            }
            var dataBlock = (DataBlock<TK, TV>)block;
            if (!dataBlock.IsFull())
            {
                dataBlock.Insert(key, value);
                _factory.WriteBlock(dataBlock, dataBlock.Address);
            }
            else // data block is full = split to 2 data blocks
            {
                var leftDataBlock = dataBlock;
                var rightDataBlock = leftDataBlock.Split(key, value, out var middleDataKey);
                leftDataBlock.NextBlock = rightDataBlock.Address = _factory.GetFreeAddress();
                IndexBlock<TK> parentBlock = null;
                if (parentAddresses.Count == 0) // initialize 1st index block
                {
                    parentBlock = new IndexBlock<TK>(_factory.BlockByteSize);
                    parentBlock.Address = _factory.GetFreeAddress();
                    _factory.SetRoot(parentBlock.Address);
                }
                else
                {
                    var parentAddress = parentAddresses.Pop();
                    parentBlock = (IndexBlock<TK>)_factory.ReadBlock(parentAddress);
                }
                if (!parentBlock.IsFull())
                {
                    parentBlock.Insert(middleDataKey, leftDataBlock.Address, rightDataBlock.Address);
                    // blocks must be saved in order as free adressess were allocated, to avoid writing error
                    _factory.WriteBlock(leftDataBlock, leftDataBlock.Address);
                    _factory.WriteBlock(rightDataBlock, rightDataBlock.Address);
                    _factory.WriteBlock(parentBlock, parentBlock.Address);
                }
                else // parent block (index block) is full = split to 2 index blocks
                {
                    var leftIndexBlock = parentBlock;
                    var rightIndexBlock = leftIndexBlock.Split(middleDataKey, out var middleIndexKey, leftDataBlock.Address, rightDataBlock.Address);
                    rightIndexBlock.Address = _factory.GetFreeAddress();
                    // TOTO SA MOZE VYTIAHNUT O UROVEN VYSSIE
                    _factory.WriteBlock(leftDataBlock, leftDataBlock.Address);
                    _factory.WriteBlock(rightDataBlock, rightDataBlock.Address);
                    // store middleIndexKey in upper level
                    while (parentAddresses.Count > 0)
                    {
                        var parentAddress = parentAddresses.Pop();
                        var upperParentIndexBlock = (IndexBlock<TK>)_factory.ReadBlock(parentAddress);
                        if (!upperParentIndexBlock.IsFull())
                        {
                            upperParentIndexBlock.Insert(middleIndexKey, leftIndexBlock.Address, rightIndexBlock.Address);
                            _factory.WriteBlock(leftIndexBlock, leftIndexBlock.Address);
                            _factory.WriteBlock(rightIndexBlock, rightIndexBlock.Address);
                            _factory.WriteBlock(upperParentIndexBlock, upperParentIndexBlock.Address);
                            return; // no need to continue to upper level
                        }
                        // upperParentIndexBlock is full = split to 2 index blocks
                        var upperLeftIndexBlock = upperParentIndexBlock;
                        var upperRightIndexBlock = upperLeftIndexBlock.Split(middleIndexKey, out var upperMiddleIndexKey, leftIndexBlock.Address, rightIndexBlock.Address);
                        upperRightIndexBlock.Address = _factory.GetFreeAddress();
                        // write lower index block level
                        _factory.WriteBlock(leftIndexBlock, leftIndexBlock.Address);
                        _factory.WriteBlock(rightIndexBlock, rightIndexBlock.Address);
                        // set variables for next round
                        leftIndexBlock = upperLeftIndexBlock;
                        rightIndexBlock = upperRightIndexBlock;
                        middleIndexKey = upperMiddleIndexKey;
                    }
                    // loop ended in the root = must create new root index block
                    var newRootIndexBlock = new IndexBlock<TK>(_factory.BlockByteSize);
                    newRootIndexBlock.Insert(middleIndexKey, leftIndexBlock.Address, rightIndexBlock.Address);
                    newRootIndexBlock.Address = _factory.GetFreeAddress();
                    _factory.SetRoot(newRootIndexBlock.Address);

                    _factory.WriteBlock(leftIndexBlock, leftIndexBlock.Address);
                    _factory.WriteBlock(rightIndexBlock, rightIndexBlock.Address);
                    _factory.WriteBlock(newRootIndexBlock, newRootIndexBlock.Address);
                }
            }
        }

        public void Dispose() => _factory.Dispose();
    }
}
