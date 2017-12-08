using System;
using System.IO;
using BPlusTree.Blocks;
using BPlusTree.Writables;

namespace BPlusTree.DataStructures
{
    public class BlockFactory<TK, TV> : IBlockFactory where TK : IComparable<TK>, IWritable, new() where TV : IWritable, new()
    {
        public int BlockByteSize { get; }
        public int DataBlockRecordSize { get; }
        private readonly ControlBlock _cb = new ControlBlock();
        private readonly WritableChar _blockType = new WritableChar();
        private readonly FileStream _stream;
        private readonly byte[] _buffer;

        public BlockFactory(int dataBlockRecordSize, string file)
        {
            BlockByteSize = new DataBlock<TK, TV>(dataBlockRecordSize).ByteSize;
            DataBlockRecordSize = dataBlockRecordSize;
            _buffer = new byte[BlockByteSize];
            var exists = File.Exists(file);
            _stream = new FileStream(file, FileMode.OpenOrCreate);
            if (!exists) WriteBlock(_cb);
            else _cb = (ControlBlock)ReadBlock();
        }

        public IBlock GetRoot() => _cb.RootAddr == long.MinValue ? null : ReadBlock(_cb.RootAddr);

        public void RemoveBlock(IBlock block)
        {
            //Console.WriteLine("Start:");
            //Console.WriteLine($"Remove: {block}");
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
                while (lastAddress > 0)
                {
                    var lastBlock = ReadBlock(lastAddress);
                    if (lastBlock.GetType() != typeof(EmptyBlock)) break;
                    var lastFree = (EmptyBlock)lastBlock;
                    //Console.WriteLine($"Loop remove: {lastFree}");
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
            //Console.WriteLine($"Cntrol: {_cb}");
            //Console.WriteLine($"Lenght: {_stream.Length}\nEnd\n");
        }

        public IBlock ReadBlock(long addr = 0)
        {
            _stream.Seek(addr, SeekOrigin.Begin);
            _stream.Read(_buffer, 0, _buffer.Length);
            _blockType.FromBytes(_buffer);
            var block = InitBlock(_buffer);
            block.FromBytes(_buffer);
            block.Address = addr;
            return block;
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
                _cb.EmptyAddr = free.NextAddr;
            }
            WriteBlock(_cb);
            return freeAddr;
        }

        public void WriteBlock(IBlock block)
        {
            var buffer = block.GetBytes();
            _stream.Seek(block.Address, SeekOrigin.Begin);
            _stream.Write(buffer, 0, buffer.Length);
        }

        public void SetRoot(long addr)
        {
            _cb.RootAddr = addr;
            WriteBlock(_cb);
        }

        public bool isRoot(long addr) => _cb.RootAddr == addr;

        public long Length() => _stream.Length;

        private IBlock InitBlock(byte[] buffer)
        {
            _blockType.FromBytes(buffer);
            if (_blockType.Value == IndexBlock<TK>.Type) return new IndexBlock<TK>(BlockByteSize);
            if (_blockType.Value == DataBlock<TK, TV>.Type) return new DataBlock<TK, TV>(DataBlockRecordSize);
            if (_blockType.Value == EmptyBlock.Type) return new EmptyBlock();
            if (_blockType.Value == ControlBlock.Type) return new ControlBlock();
            throw new ArgumentNullException($"Missing implementation of {nameof(IBlock)} with Type: {_blockType.Value}");
        }

        public void Dispose() => _stream.Dispose();
    }
}
