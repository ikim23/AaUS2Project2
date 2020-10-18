using System;
using System.Collections.Generic;

namespace DataStructures
{
    public partial class RbTree<TK, TV> where TK : IComparable, IComparable<TK>
    {
        public int Count { get; internal set; }
        private RbTreeNode<TK, TV> _root;

        public void Clear()
        {
            _root = null;
            Count = 0;
        }

        public TV Find(TK key) => FindNodeOfThrow(key).Value;

        private RbTreeNode<TK, TV> FindNodeOfThrow(TK key)
        {
            var node = FindNode(key);
            if (node == null) throw new KeyNotFoundException();
            return node;
        }

        private RbTreeNode<TK, TV> FindNode(TK key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var node = _root;
            while (node != null)
            {
                var dir = key.CompareTo(node.Key);
                if (dir == 0) return node;
                node = node[dir];
            }
            return null;
        }

        public TV GetOrInsert(TK key, TV value)
        {
            var node = FindNode(key);
            if (node != null) return node.Value;
            Insert(key, value);
            return value;
        }

        public void Insert(TK key, TV value)
        {
            var node = BstInsert(key, value);
            while (!node.IsRoot)
            {
                var parent = node.Parent;
                var grandParent = parent.Parent;
                if (IsRed(node) && IsRed(parent))
                {
                    var parentBrother = parent.Brother;
                    if (IsRed(parentBrother))
                    {
                        SetColor(parent, Color.Black);
                        SetColor(parentBrother, Color.Black);
                        if (grandParent != null) grandParent.Color = grandParent.IsRoot ? Color.Black : Color.Red;
                    }
                    else
                    {
                        var nDir = node.Direction;
                        var pDir = parent.Direction;
                        var sorted = Sort(node, parent, grandParent);
                        var a = sorted[0];
                        var b = sorted[1];
                        var c = sorted[2];
                        Shift(a, b, c, nDir * -1);
                        if (nDir != pDir) Shift(a, b, c, nDir);
                    }
                }
                node = parent;
            }
            Count++;
        }

        private RbTreeNode<TK, TV> BstInsert(TK key, TV value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (_root == null) return _root = new RbTreeNode<TK, TV>(key, value, Color.Black);
            var node = _root;
            while (true)
            {
                var dir = key.CompareTo(node.Key);
                if (dir == 0) throw new ArgumentException($"Item with key: {key} already exists");
                if (node[dir] != null) node = node[dir];
                else return node[dir] = new RbTreeNode<TK, TV>(key, value);
            }
        }

        public void Remove(TK key)
        {
            var node = BstRemove(key);
            node.IsRemovable = true;
            if (node.IsRoot)
            {
                _root = null;
                return;
            }
            if (IsRed(node)) // red node removal does not violate constraints
            {
                RemoveNode(node);
                return;
            }
            SetColor(node, Color.DoubleBlack);
            while (IsDoubleBlack(node))
            {
                if (node.IsRoot)
                {
                    SetColor(node, Color.Black);
                    return;
                }
                var parent = node.Parent;
                var brother = node.Brother;
                var dir = node.Direction;
                if (IsBlack(brother) && IsBlack(brother[dir]) && IsBlack(brother[dir * -1])) // case 1
                {
                    RemoveNode(node);
                    SetColor(brother, Color.Red);
                    SetColor(parent, IsRed(parent) ? Color.Black : Color.DoubleBlack);
                    node = parent;
                }
                else if (IsBlack(brother) && IsRed(brother[dir * -1])) // case 2
                {
                    RemoveNode(node);
                    SetColor(brother[dir * -1], Color.Black);
                    SetColor(brother, parent.Color);
                    SetColor(parent, Color.Black);
                    Shift(parent, dir);
                }
                else if (IsBlack(brother) && IsRed(brother[dir]) && IsBlack(brother[dir * -1])) // case 3
                {
                    SetColor(brother, Color.Red);
                    SetColor(brother[dir], Color.Black);
                    Shift(brother, dir * -1);
                }
                else if (IsRed(brother)) // case 4
                {
                    SetColor(parent, Color.Red);
                    SetColor(brother, Color.Black);
                    Shift(parent, dir);
                }
                else throw new InvalidOperationException("Double black node wasn't processed by any case");
            }
            Count--;
        }

        /// <summary>
        /// Method executes BSTree node removal:
        /// 1. Locate desired node
        /// 2. If the node is leaf return node
        /// 3. If the node has only one child, update node's KV to child's KV and return child
        /// 4. If the node has both children, find its in order successor, update node's KV to successor's KV and return successor
        /// </summary>
        /// <param name="key">Key to be removed</param>
        /// <returns>Node to be removed. It can be double black node.</returns>
        private RbTreeNode<TK, TV> BstRemove(TK key)
        {
            var node = FindNodeOfThrow(key);
            if (node.IsLeaf) return node;
            if (node.LeftChild == null || node.RightChild == null)
            {
                var dir = node.LeftChild == null ? 1 : -1;
                node.SetKeyValue(node[dir]);
                return node[dir];
            }
            using (var inOrderEnumerator = InOrder(node.RightChild).GetEnumerator())
            {
                if (!inOrderEnumerator.MoveNext()) throw new InvalidOperationException($"Node {node} does not have in order successor");
                var successor = inOrderEnumerator.Current;
                if (successor == null) throw new NullReferenceException($"In order successor of node {node} is null");
                node.SetKeyValue(successor);
                return successor;
            }
        }

        private void RemoveNode(RbTreeNode<TK, TV> node)
        {
            if (node.IsRemovable)
            {
                var parent = node.Parent;
                if (parent == null) throw new InvalidOperationException("Tree root can not be removed");
                if (node.LeftChild != null && node.RightChild != null) throw new InvalidOperationException("Node with 2 children can not be removed");
                // replace node with its child
                parent[node.Direction] = node.LeftChild ?? node.RightChild;
            }
            if (IsDoubleBlack(node)) SetColor(node, Color.Black);
        }

        private void Shift(RbTreeNode<TK, TV> a, RbTreeNode<TK, TV> b, RbTreeNode<TK, TV> c, int dir)
        {
            Shift(dir < 0 ? a : c, dir);
            b.Color = Color.Black;
            a.Color = c.Color = Color.Red;
        }

        private void Shift(RbTreeNode<TK, TV> node, int dir)
        {
            var parent = node.Parent;
            var pDir = node.Direction;
            var child = node[dir * -1];
            node[dir * -1] = child[dir];
            child[dir] = node;
            if (parent == null)
            {
                _root = child;
                _root.Parent = null;
            }
            else parent[pDir] = child;
        }

        private List<RbTreeNode<TK, TV>> Sort(RbTreeNode<TK, TV> n1, RbTreeNode<TK, TV> n2, RbTreeNode<TK, TV> n3)
        {
            var list = new List<RbTreeNode<TK, TV>>(3) { n1, n2, n3 };
            list.Sort();
            return list;
        }

        private bool IsRed(RbTreeNode<TK, TV> node) => node != null && node.Color == Color.Red;

        private bool IsBlack(RbTreeNode<TK, TV> node) => node == null || node.Color == Color.Black;

        private bool IsDoubleBlack(RbTreeNode<TK, TV> node) => node != null && node.Color == Color.DoubleBlack;

        private void SetColor(RbTreeNode<TK, TV> node, Color color)
        {
            if (node != null) node.Color = color;
        }
    }
}
