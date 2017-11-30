using System;
using System.Collections.Generic;
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

        public TV Find(TK key)
        {
            var block = _factory.GetRoot();
            if (block == null) throw new KeyNotFoundException();
            IndexBlock<TK> parentBlock = null;
            while (block is IndexBlock<TK>)
            {
                parentBlock = (IndexBlock<TK>)block;
                var childAddress = parentBlock.GetChildAddress(key);
                block = _factory.ReadBlock(childAddress);
            }
            var dataBlock = (DataBlock<TK, TV>)block;
            Console.Write(key + ": ");
            var vals = dataBlock.ToKeyValueArray().Select(kv => kv.Item2);
            Console.WriteLine(string.Join(",", vals));
            return dataBlock.Find(key);
        }

        public void Insert(TK key, TV value)
        {
            var block = _factory.GetRoot() ?? new DataBlock<TK, TV>(_factory.DataBlockRecordSize);
            IndexBlock<TK> parentBlock = null;
            while (block is IndexBlock<TK>)
            {
                parentBlock = (IndexBlock<TK>)block;
                var childAddress = parentBlock.GetChildAddress(key);
                block = _factory.ReadBlock(childAddress);
            }
            var dataBlock = (DataBlock<TK, TV>)block;
            if (!dataBlock.IsFull())
            {
                dataBlock.Insert(key, value);
                // root hack
                if (dataBlock.Address == long.MinValue)
                {
                    dataBlock.Address = _factory.GetFreeAddress();
                    _factory.SetRoot(dataBlock.Address);
                }
                _factory.WriteBlock(dataBlock, dataBlock.Address);
            }
            else
            { // block is full = split block
                var leftBlock = dataBlock;
                var rightBlock = leftBlock.Split(key, value, out var middleKey);
                rightBlock.Address = _factory.GetFreeAddress();
                leftBlock.NextBlock = rightBlock.Address;
                if (parentBlock == null) // update root
                {
                    parentBlock = new IndexBlock<TK>(_factory.BlockByteSize);
                    parentBlock.Address = _factory.GetFreeAddress();
                    leftBlock.Parent = rightBlock.Parent = parentBlock.Address;
                    _factory.SetRoot(parentBlock.Address);
                }
                if (!parentBlock.IsFull()) // if parent is not full add middle key
                {
                    parentBlock.Insert(middleKey, leftBlock.Address, rightBlock.Address);
                    // must store in exact order as addresses were allocated
                    _factory.WriteBlock(leftBlock, leftBlock.Address);
                    _factory.WriteBlock(rightBlock, rightBlock.Address); // 0??
                    _factory.WriteBlock(parentBlock, parentBlock.Address);
                }
                else // parent full = split block
                {
                    var leftIndex = parentBlock;
                    var rightIndex = leftIndex.Split(middleKey, out var middleParentKey, leftBlock.Address, rightBlock.Address); // split sets: parentAddress
                    rightIndex.Address = _factory.GetFreeAddress();
                    // zisti parent addresu datovych blokov
                    if (rightIndex.ContainsChild(leftBlock.Address)) // order is important!!
                    {
                        leftBlock.Parent = rightIndex.Address;
                    }
                    else if (leftIndex.ContainsChild(leftBlock.Address))
                    {
                        leftBlock.Parent = leftIndex.Address;
                    }
                    else throw new InvalidOperationException();
                    if (rightIndex.ContainsChild(rightBlock.Address))
                    {
                        rightBlock.Parent = rightIndex.Address;
                    }
                    else if (leftIndex.ContainsChild(rightBlock.Address))
                    {
                        rightBlock.Parent = leftIndex.Address;
                    }
                    else throw new InvalidOperationException();
                    // zapis data bloky
                    _factory.WriteBlock(leftBlock, leftBlock.Address);
                    _factory.WriteBlock(rightBlock, rightBlock.Address);

                    if (leftIndex.Parent != rightIndex.Parent) throw new InvalidOperationException("blbost");

                    // zisti ci left index blok ma rodica
                    if (leftIndex.Parent == long.MinValue) // vytvor novy koren
                    {
                        var root = new IndexBlock<TK>(_factory.BlockByteSize);
                        root.Insert(middleParentKey, leftIndex.Address, rightIndex.Address);
                        root.Address = _factory.GetFreeAddress();
                        _factory.SetRoot(root.Address);

                        leftIndex.Parent = rightIndex.Parent = root.Address;

                        _factory.WriteBlock(leftIndex, leftIndex.Address);
                        _factory.WriteBlock(rightIndex, rightIndex.Address);
                        _factory.WriteBlock(root, root.Address);
                    }
                    else // left index ma parenta, idem o uroven vyssie
                    {
                        // nacitaj parenta
                        var parentAddress = leftIndex.Parent;
                        while (parentAddress != long.MinValue)
                        {
                            var parentIndex = (IndexBlock<TK>)_factory.ReadBlock(parentAddress);
                            // vloz middle do parenta ak je parent volny
                            if (!parentIndex.IsFull())
                            {
                                parentIndex.Insert(middleParentKey, leftIndex.Address, rightIndex.Address);
                                leftIndex.Parent = rightIndex.Parent = parentIndex.Address;

                                _factory.WriteBlock(leftIndex, leftIndex.Address);
                                _factory.WriteBlock(rightIndex, rightIndex.Address);
                                _factory.WriteBlock(parentIndex, parentIndex.Address);
                                return; // bolo volne miesto, koniec....
                            }
                            else
                            {
                                // parent je plny, = split
                                var parentIndexLeft = parentIndex;
                                var parentIndexRight = parentIndexLeft.Split(middleParentKey, out var newMiddle, leftIndex.Address, rightIndex.Address);
                                parentIndexRight.Address = _factory.GetFreeAddress();
                                // zisti parent addresu index blokov
                                if (parentIndexRight.ContainsChild(leftIndex.Address)) // order is important!!
                                {
                                    leftIndex.Parent = parentIndexRight.Address;
                                }
                                else if (parentIndexLeft.ContainsChild(leftIndex.Address))
                                {
                                    leftIndex.Parent = parentIndexLeft.Address;
                                }
                                else throw new InvalidOperationException();
                                if (parentIndexRight.ContainsChild(rightIndex.Address))
                                {
                                    rightIndex.Parent = parentIndexRight.Address;
                                }
                                else if (parentIndexLeft.ContainsChild(rightIndex.Address))
                                {
                                    rightIndex.Parent = parentIndexLeft.Address;
                                }
                                else throw new InvalidOperationException();
                                // zapis index bloky
                                _factory.WriteBlock(leftIndex, leftIndex.Address);
                                _factory.WriteBlock(rightIndex, rightIndex.Address);

                                // set variables for next round
                                middleParentKey = newMiddle;
                                leftIndex = parentIndexLeft;
                                rightIndex = parentIndexRight;
                                parentAddress = leftIndex.Parent;
                            }
                        }
                        // copyied end with root node...
                        var root = new IndexBlock<TK>(_factory.BlockByteSize);
                        root.Insert(middleParentKey, leftIndex.Address, rightIndex.Address);
                        root.Address = _factory.GetFreeAddress();
                        _factory.SetRoot(root.Address);

                        leftIndex.Parent = rightIndex.Parent = root.Address;

                        _factory.WriteBlock(leftIndex, leftIndex.Address);
                        _factory.WriteBlock(rightIndex, rightIndex.Address);
                        _factory.WriteBlock(root, root.Address);
                    }
                }
            }
        }

        public void Dispose() => _factory.Dispose();
    }
}
