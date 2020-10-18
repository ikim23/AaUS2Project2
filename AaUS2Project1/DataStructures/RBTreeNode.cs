using System;

namespace DataStructures
{
    class RbTreeNode<TK, TV> : IComparable<RbTreeNode<TK, TV>> where TK : IComparable, IComparable<TK>
    {
        public TK Key { get; internal set; }
        public TV Value { get; internal set; }
        public Color Color { get; set; }
        public RbTreeNode<TK, TV> Parent { get; set; }
        public RbTreeNode<TK, TV> Brother => Parent?[Direction * -1];
        private RbTreeNode<TK, TV> _leftChild;
        public RbTreeNode<TK, TV> LeftChild
        {
            get => _leftChild;
            set
            {
                _leftChild = value;
                if (_leftChild != null) _leftChild.Parent = this;
            }
        }
        private RbTreeNode<TK, TV> _rightChild;
        public RbTreeNode<TK, TV> RightChild
        {
            get => _rightChild;
            set
            {
                _rightChild = value;
                if (_rightChild != null) _rightChild.Parent = this;
            }
        }
        public bool IsRoot => Parent == null;
        public bool IsLeaf => LeftChild == null && RightChild == null;
        public bool IsRemovable { get; set; }
        public int Direction => IsRoot ? int.MinValue : ReferenceEquals(Parent.LeftChild, this) ? -1 : 1;

        public RbTreeNode<TK, TV> this[int i]
        {
            get
            {
                if (i == -1) return LeftChild;
                if (i == 1) return RightChild;
                return null;
            }
            set
            {
                if (i == -1) LeftChild = value;
                else if (i == 1) RightChild = value;
            }
        }

        public RbTreeNode(TK key, TV value, Color color = Color.Red)
        {
            Key = key;
            Value = value;
            Color = color;
        }

        public void SetKeyValue(RbTreeNode<TK, TV> node)
        {
            Key = node.Key;
            Value = node.Value;
        }

        public int CompareTo(RbTreeNode<TK, TV> other) => Key.CompareTo(other.Key);

        public override string ToString()
        {
            if (IsRemovable) return Pad("null");
            switch (Color)
            {
                case Color.Red: return Pad($"{Key}");
                case Color.Black: return Pad($"({Key})");
                case Color.DoubleBlack: return Pad($"(({Key}))");
                default: return Pad("err");
            }
        }

        private string Pad(string s) => s.PadLeft(4);
    }
}
