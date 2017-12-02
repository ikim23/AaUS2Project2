using System;
using System.Collections.Generic;
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

        public TV Find(TK key)
        {
            var block = _factory.GetRoot();
            if (block == null) throw new KeyNotFoundException();
            while (block is IndexBlock<TK>)
            {
                var indexBlock = (IndexBlock<TK>)block;
                var childAddress = indexBlock.Find(key);
                block = _factory.ReadBlock(childAddress);
            }
            var dataBlock = (DataBlock<TK, TV>)block;
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
                _factory.WriteBlock(dataBlock);
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
                    _factory.WriteBlock(leftDataBlock);
                    _factory.WriteBlock(rightDataBlock);
                    _factory.WriteBlock(parentBlock);
                }
                else // parent block (index block) is full = split to 2 index blocks
                {
                    var leftIndexBlock = parentBlock;
                    var rightIndexBlock = leftIndexBlock.Split(middleDataKey, out var middleIndexKey, leftDataBlock.Address, rightDataBlock.Address);
                    rightIndexBlock.Address = _factory.GetFreeAddress();
                    // TOTO SA MOZE VYTIAHNUT O UROVEN VYSSIE
                    _factory.WriteBlock(leftDataBlock);
                    _factory.WriteBlock(rightDataBlock);
                    // store middleIndexKey in upper level
                    while (parentAddresses.Count > 0)
                    {
                        var parentAddress = parentAddresses.Pop();
                        var upperParentIndexBlock = (IndexBlock<TK>)_factory.ReadBlock(parentAddress);
                        if (!upperParentIndexBlock.IsFull())
                        {
                            upperParentIndexBlock.Insert(middleIndexKey, leftIndexBlock.Address, rightIndexBlock.Address);
                            _factory.WriteBlock(leftIndexBlock);
                            _factory.WriteBlock(rightIndexBlock);
                            _factory.WriteBlock(upperParentIndexBlock);
                            return; // no need to continue to upper level
                        }
                        // upperParentIndexBlock is full = split to 2 index blocks
                        var upperLeftIndexBlock = upperParentIndexBlock;
                        var upperRightIndexBlock = upperLeftIndexBlock.Split(middleIndexKey, out var upperMiddleIndexKey, leftIndexBlock.Address, rightIndexBlock.Address);
                        upperRightIndexBlock.Address = _factory.GetFreeAddress();
                        // write lower index block level
                        _factory.WriteBlock(leftIndexBlock);
                        _factory.WriteBlock(rightIndexBlock);
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

                    _factory.WriteBlock(leftIndexBlock);
                    _factory.WriteBlock(rightIndexBlock);
                    _factory.WriteBlock(newRootIndexBlock);
                }
            }
        }

        public IEnumerable<TV> InOrder()
        {
            var block = _factory.GetRoot();
            if (block == null) yield break;
            while (block is IndexBlock<TK>)
            {
                var indexBlock = (IndexBlock<TK>)block;
                var childAddress = indexBlock.MinAddress();
                block = _factory.ReadBlock(childAddress);
            }
            var nextAddress = block.Address;
            do
            {
                var dataBlock = (DataBlock<TK, TV>)_factory.ReadBlock(nextAddress);
                foreach (var value in dataBlock)
                    yield return value;
                nextAddress = dataBlock.NextBlock;
            } while (nextAddress != long.MinValue);
        }
        
        public void Dispose() => _factory.Dispose();
    }
}
