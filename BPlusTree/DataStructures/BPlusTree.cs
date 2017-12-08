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
            var dataBlock = FindDataBlock(key);
            return dataBlock.Find(key);
        }

        private DataBlock<TK, TV> FindDataBlock(TK key)
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
            return dataBlock;
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

        public void Remove(TK key)
        {
            var block = _factory.GetRoot();
            if (block == null) throw new KeyNotFoundException();
            IndexBlock<TK> parent = null;
            var indexContainsKey = false;
            IndexBlock<TK> indexContaining = null;
            var parentAddresses = new Stack<long>();
            while (block is IndexBlock<TK>)
            {
                parent = (IndexBlock<TK>)block;
                parentAddresses.Push(parent.Address);
                if (!indexContainsKey && parent.Contains(key))
                {
                    indexContaining = parent;
                    indexContainsKey = true;
                }
                var childAddress = parent.Find(key);
                block = _factory.ReadBlock(childAddress);
            }
            var dataBlock = (DataBlock<TK, TV>)block;
            if (!dataBlock.Contains(key)) throw new KeyNotFoundException($"not found: {key}");
            dataBlock.Remove(key);
            if (!dataBlock.IsUnderFlow())
            {
                if (indexContaining != null)
                {
                    var minKey = dataBlock.MinKey();
                    var idx = indexContaining.KeyIndex(key); // indexOf
                    indexContaining.SetParentKey(idx, minKey);
                    _factory.WriteBlock(parent);
                }
                _factory.WriteBlock(dataBlock);
                return;
            }
            // borrow from sibling
            if (parent == null) return; // only data block as root
            var dataBlockIndex = parent.ChildIndex(key);
            var leftSiblingAddr = parent.GetChildAddress(dataBlockIndex - 1);
            var rightSiblingAddr = parent.GetChildAddress(dataBlockIndex + 1);
            DataBlock<TK, TV> leftSibling = null;
            DataBlock<TK, TV> rightSibling = null;
            if (leftSiblingAddr != long.MinValue) leftSibling = (DataBlock<TK, TV>)_factory.ReadBlock(leftSiblingAddr);
            if (rightSiblingAddr != long.MinValue) rightSibling = (DataBlock<TK, TV>)_factory.ReadBlock(rightSiblingAddr);
            if (indexContaining != null)
            {
                var minKey = dataBlock.MinKey();
                var idx = indexContaining.KeyIndex(key); // indexOf
                indexContaining.SetParentKey(idx, minKey);
            }
            var shift = Shift(parent, dataBlockIndex, ref dataBlock, ref leftSibling, ref rightSibling);
            if (!shift)
            {
                // shift not successful => merge
                Merge(parent, dataBlockIndex, ref dataBlock, ref leftSibling, ref rightSibling);
            }
            _factory.WriteBlock(parent);
            if (dataBlock != null) _factory.WriteBlock(dataBlock);
            if (leftSibling != null) _factory.WriteBlock(leftSibling);
            if (rightSibling != null) _factory.WriteBlock(rightSibling);
            if (shift) return;
            // continue in upper level
            var indexBlock = parent;
            IndexBlock<TK> indexBlockParent = null;
            IndexBlock<TK> leftIndexSibling = null;
            IndexBlock<TK> rightIndexSibling = null;
            if (parentAddresses.Count > 0)
            {
                parentAddresses.Pop();
            }
            while (true)
            {
                if (!indexBlock.IsUnderFlow())
                {
                    return;
                }
                if (parentAddresses.Count == 0) return;
                var parentAddr = parentAddresses.Pop();
                indexBlockParent = (IndexBlock<TK>)_factory.ReadBlock(parentAddr);
                var indexBlockIndex = indexBlockParent.ChildIndex(key);// co sa stane ked sa prebije index prvok a dojde sa sem (indexContaining)
                var leftIndexSiblingAddr = indexBlockParent.GetChildAddress(indexBlockIndex - 1);
                var rightIndextSiblingAddr = indexBlockParent.GetChildAddress(indexBlockIndex + 1);
                if (leftIndexSiblingAddr != long.MinValue)
                    leftIndexSibling = (IndexBlock<TK>) _factory.ReadBlock(leftIndexSiblingAddr);
                else leftIndexSibling = null;
                if (rightIndextSiblingAddr != long.MinValue)
                    rightIndexSibling = (IndexBlock<TK>) _factory.ReadBlock(rightIndextSiblingAddr);
                else rightIndexSibling = null;
                var indexShift = Shift(indexBlockParent, indexBlockIndex, ref indexBlock, ref leftIndexSibling, ref rightIndexSibling);
                if (!indexShift) Merge(indexBlockParent, indexBlockIndex, ref indexBlock, ref leftIndexSibling, ref rightIndexSibling);
                _factory.WriteBlock(indexBlockParent);
                if (indexBlock != null) _factory.WriteBlock(indexBlock);
                if (leftIndexSibling != null) _factory.WriteBlock(leftIndexSibling);
                if (rightIndexSibling != null) _factory.WriteBlock(rightIndexSibling);
                if (indexShift) return;
                // next round
                indexBlock = indexBlockParent;
            }
            //if (indexBlockParent != null) _factory.WriteBlock(indexBlockParent);
            //if (indexBlock != null) _factory.WriteBlock(indexBlock);
            //if (leftIndexSibling != null) _factory.WriteBlock(leftIndexSibling);
            //if (rightIndexSibling != null) _factory.WriteBlock(rightIndexSibling);

            // TREBA VYMYSLIET AKO UPRAVIT ROOT ADRESU... KED SA ZNIZI CELY STROM O JEDEN LEVEL
        }

        public static bool Shift(IndexBlock<TK> parent, int dataBlockIndex, ref DataBlock<TK, TV> dataBlock, ref DataBlock<TK, TV> leftSibling, ref DataBlock<TK, TV> rightSibling)
        {
            if (leftSibling != null && leftSibling.CanBorrow())
            {
                var middleKey = dataBlock.ShiftMaxFromLeft(leftSibling);
                parent.SetParentKey(dataBlockIndex - 1, middleKey);
                return true;
            }
            if (rightSibling != null && rightSibling.CanBorrow())
            {
                var middleKey = dataBlock.ShiftMinFromRight(rightSibling);
                parent.SetParentKey(dataBlockIndex, middleKey);
                return true;
            }
            return false;
        }

        public static bool Shift(IndexBlock<TK> indexBlockParent, int indexBlockIndex, ref IndexBlock<TK> indexBlock, ref IndexBlock<TK> leftIndexSibling, ref IndexBlock<TK> rightIndexSibling)
        {// index !! mozem vybrat zo stredu parenta... == SOLVED, not TESTED
            if (leftIndexSibling != null && leftIndexSibling.CanBorrow())
            {
                indexBlock.ShiftMaxFromLeft(indexBlockParent, indexBlockIndex - 1, leftIndexSibling);
                return true;
            }
            if (rightIndexSibling != null && rightIndexSibling.CanBorrow())
            {
                indexBlock.ShiftMinFromRight(indexBlockParent, indexBlockIndex, rightIndexSibling);
                return true;
            }
            return false;
        }

        public void Merge(IndexBlock<TK> parent, int dataBlockIndex, ref DataBlock<TK, TV> dataBlock, ref DataBlock<TK, TV> leftSibling, ref DataBlock<TK, TV> rightSibling)
        {
            if (leftSibling != null)
            {
                leftSibling.Merge(dataBlock);
                parent.MergeRemove(dataBlockIndex - 1, leftSibling.Address);
                _factory.RemoveBlock(dataBlock);
                // fix addressess
                leftSibling.NextBlock = dataBlock.NextBlock;
                dataBlock = null;
            }
            else if (rightSibling != null)
            {
                dataBlock.Merge(rightSibling);
                parent.MergeRemove(dataBlockIndex, dataBlock.Address);
                _factory.RemoveBlock(rightSibling);
                // fix addressess
                dataBlock.NextBlock = rightSibling.NextBlock;
                rightSibling = null;
            }
            else if (_factory.isRoot(dataBlock.Address)) throw new InvalidOperationException("index root??");
        }

        public void Merge(IndexBlock<TK> indexBlockParent, int indexBlockIndex, ref IndexBlock<TK> indexBlock, ref IndexBlock<TK> leftIndexSibling, ref IndexBlock<TK> rightIndexSibling)
        {
            if (leftIndexSibling != null)
            {
                leftIndexSibling.Merge(indexBlockParent, indexBlockIndex - 1, indexBlock);
                _factory.RemoveBlock(indexBlock);
                indexBlock = null;
            }
            else if (rightIndexSibling != null)
            {
                indexBlock.Merge(indexBlockParent, indexBlockIndex, rightIndexSibling);
                _factory.RemoveBlock(rightIndexSibling);
                rightIndexSibling = null;
            }
            else if (_factory.isRoot(indexBlock.Address)) throw new InvalidOperationException("root??");
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
                block = _factory.ReadBlock(nextAddress);
                var dataBlock = (DataBlock<TK, TV>)block;
                foreach (var value in dataBlock)
                    yield return value;
                nextAddress = dataBlock.NextBlock;
            } while (nextAddress != long.MinValue);
        }

        public void Dispose() => _factory.Dispose();
    }
}
