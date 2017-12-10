using System.Collections;
using System.Collections.Generic;

namespace PersonalHealthRecord.Generator
{
    internal class ArrayEnumerator<T> : IEnumerator<T>
    {
        public int Index;
        public T[] Array;
        public T Current => Array[Index];

        public ArrayEnumerator(T[] array)
        {
            Index = 0;
            Array = array;
        }

        public bool MoveNext()
        {
            Index++;
            if (Index >= Array.Length) Index = 0;
            return true;
        }

        public void Dispose()
        {
        }

        public void Reset() => Index = 0;

        object IEnumerator.Current => Current;
    }
}
