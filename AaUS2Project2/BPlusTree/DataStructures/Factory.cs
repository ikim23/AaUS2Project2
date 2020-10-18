using System;
using System.IO;
using BPlusTree.Blocks;
using BPlusTree.Writables;

namespace BPlusTree.DataStructures
{
    public class Factory : IBlockFactory
    {
        public static IBlockFactory Create(string file, int dataBlockRecordSize = 0, Type keyType = null, Type valueType = null)
        {
            if (File.Exists(file)) return new Factory(file);
            return new Factory(file, dataBlockRecordSize, keyType, valueType);
        }

        public int ControlBlockByteSize => _cb.ByteSize;
        public int BlockByteSize { get; }
        public int DataBlockRecordSize => _cb.DataBlockRecordSize;
        private readonly ControlBlock _cb = new ControlBlock();
        private readonly WritableChar _blockType = new WritableChar();
        private readonly FileStream _stream;
        private readonly byte[] _buffer;

        private Factory(string file)
        {
            // read control block
            _stream = new FileStream(file, FileMode.Open);
            var buffer = new byte[_cb.ByteSize];
            _stream.Seek(0, SeekOrigin.Begin);
            _stream.Read(buffer, 0, buffer.Length);
            _cb.FromBytes(buffer);
            BlockByteSize = DataBlock().ByteSize;
            _buffer = new byte[BlockByteSize];
        }

        private Factory(string file, int dataBlockRecordSize, Type keyType, Type valueType)
        {
            // create control block
            _cb.KeyType = keyType;
            _cb.ValueType = valueType;
            _cb.DataBlockRecordSize = dataBlockRecordSize;
            _stream = new FileStream(file, FileMode.Create);
            var buffer = _cb.GetBytes();
            _stream.Seek(0, SeekOrigin.Begin);
            _stream.Write(buffer, 0, buffer.Length);
            BlockByteSize = DataBlock().ByteSize;
            _buffer = new byte[BlockByteSize];
        }

        public IBlock GetRoot() => _cb.RootAddr == long.MinValue ? null : ReadBlock(_cb.RootAddr);

        public bool IsRoot(long addr) => _cb.RootAddr == addr;

        public void SetRoot(long addr)
        {
            _cb.RootAddr = addr;
            WriteBlock(_cb);
        }

        public void RemoveBlock(IBlock block)
        {
            //Console.WriteLine($"\nR: {block}");
            var lastAddress = _stream.Length - _buffer.Length;
            if (block.Address == lastAddress)
            {
                if (_stream.Length == _cb.EmptyAddr)
                {
                    _cb.EmptyAddr = lastAddress;
                    WriteBlock(_cb);
                }
                // remove data block at the end of file
                _stream.SetLength(lastAddress);
                // remove all following empty blocks
                lastAddress -= _buffer.Length;
                while (lastAddress > _cb.ByteSize)
                {
                    var lastBlock = ReadBlock(lastAddress);
                    if (lastBlock.GetType() != typeof(EmptyBlock)) break;
                    var lastFree = (EmptyBlock)lastBlock;
                    if (lastFree.PrevAddr != long.MinValue)
                    {
                        var prev = (EmptyBlock)ReadBlock(lastFree.PrevAddr);
                        prev.NextAddr = lastFree.NextAddr;
                        WriteBlock(prev);
                    }
                    if (lastFree.NextAddr != long.MinValue)
                    {
                        var next = (EmptyBlock)ReadBlock(lastFree.NextAddr);
                        next.PrevAddr = lastFree.PrevAddr;
                        WriteBlock(next);
                    }
                    if (lastFree.Address == _cb.EmptyAddr)
                    {
                        if (lastFree.PrevAddr != long.MinValue)
                        {
                            _cb.EmptyAddr = lastFree.PrevAddr;
                        }
                        else if (lastFree.NextAddr != long.MinValue)
                        {
                            _cb.EmptyAddr = lastFree.NextAddr;
                        }
                        else
                        { // no more empty blocks, new blocks will be appended to end
                            _cb.EmptyAddr = lastAddress;
                        }
                        WriteBlock(_cb);
                    }
                    _stream.SetLength(lastAddress);
                    lastAddress -= _buffer.Length;
                }
            }
            else
            {
                var newFree = new EmptyBlock { Address = block.Address };
                if (_cb.EmptyAddr == _stream.Length)
                {
                    _cb.EmptyAddr = newFree.Address;
                }
                else
                {
                    var blk = ReadBlock(_cb.EmptyAddr);
                    var free = (EmptyBlock)blk;
                    free.PrevAddr = newFree.Address;
                    newFree.NextAddr = free.Address;
                    _cb.EmptyAddr = newFree.Address;
                    WriteBlock(free);
                }
                WriteBlock(newFree);
                WriteBlock(_cb);
            }
            //Console.WriteLine($"\nE: {_cb}");
        }

        public IBlock ReadBlock(long addr = 0)
        {
            if (addr == 0)
            {
                var buff = new byte[_cb.ByteSize];
                _stream.Seek(addr, SeekOrigin.Begin);
                _stream.Read(buff, 0, buff.Length);
                var b = new ControlBlock();
                b.FromBytes(buff);
                return b;
            }
            _stream.Seek(addr, SeekOrigin.Begin);
            _stream.Read(_buffer, 0, _buffer.Length);
            _blockType.FromBytes(_buffer);
            var block = InitBlock(_buffer);
            block.FromBytes(_buffer);
            block.Address = addr;
            return block;
        }

        public void WriteBlock(IBlock block)
        {
            if (block is ControlBlock && _cb.EmptyAddr < 0)
            {
                throw new Exception("empty address lower than 0");
            }
            var buffer = block.GetBytes();
            _stream.Seek(block.Address, SeekOrigin.Begin);
            _stream.Write(buffer, 0, buffer.Length);
        }

        public long GetFreeAddress()
        {
            var freeAddr = _cb.EmptyAddr;
            if (freeAddr >= _stream.Length)
            {
                _cb.EmptyAddr += BlockByteSize;
            }
            else
            {
                var free = (EmptyBlock)ReadBlock(_cb.EmptyAddr);
                if (free.NextAddr != long.MinValue)
                {
                    _cb.EmptyAddr = free.NextAddr;
                }
                else if (free.PrevAddr != long.MinValue)
                {
                    _cb.EmptyAddr = free.PrevAddr;
                }
                else
                {
                    _cb.EmptyAddr = _stream.Length;
                }
            }
            WriteBlock(_cb);
            return freeAddr;
        }

        private IBlock InitBlock(byte[] buffer)
        {
            _blockType.FromBytes(buffer);
            if (_blockType.Value == 'I') return IndexBlock();
            if (_blockType.Value == 'D') return DataBlock();
            if (_blockType.Value == EmptyBlock.Type) return new EmptyBlock();
            if (_blockType.Value == ControlBlock.Type) return new ControlBlock();
            throw new ArgumentNullException($"Missing implementation of {nameof(IBlock)} with Type: {_blockType.Value}");
        }

        private IBlock DataBlock()
        {
            var blockType = typeof(DataBlock<,>).MakeGenericType(_cb.KeyType, _cb.ValueType);
            var dataBlock = (IBlock)Activator.CreateInstance(blockType, DataBlockRecordSize);
            return dataBlock;
        }

        private IBlock IndexBlock()
        {
            var blockType = typeof(IndexBlock<>).MakeGenericType(_cb.KeyType);
            var indexBlock = (IBlock)Activator.CreateInstance(blockType, BlockByteSize);
            return indexBlock;
        }

        public long Length() => _stream.Length;

        public void Dispose() => _stream.Dispose();
    }


}
