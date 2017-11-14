namespace BPlusTree
{
    public interface IWritable
    {
        int ByteSize { get; }
        byte[] GetBytes();
        void FromBytes(byte[] bytes, int index = 0);
    }
}
