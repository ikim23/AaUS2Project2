using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace DataStructures
{
    public partial class RbTree<TK, TV> where TK : IComparable, IComparable<TK>
    {
        /* START Check methods */
        private void Check2Red(RbTreeNode<TK, TV> node)
        {
            if (node?.Parent == null) return;
            if (IsRed(node) && IsRed(node.Parent)) throw new Exception("Two red nodes found");
            Check2Red(node.LeftChild);
            Check2Red(node.RightChild);
        }

        public void Check2Red() => Check2Red(_root);

        private int CheckBlackHeight(RbTreeNode<TK, TV> node)
        {
            if (node == null) return 1;
            var leftHeight = CheckBlackHeight(node.LeftChild);
            var rightHeight = CheckBlackHeight(node.RightChild);
            if (leftHeight != rightHeight) throw new Exception("Black height does not match");
            return IsBlack(node) ? leftHeight + 1 : leftHeight;
        }

        public int CheckBlackHeight() => CheckBlackHeight(_root);

        public bool Check()
        {
            if (_root == null) return true;
            if (IsRed(_root)) throw new Exception("Root is not black");
            Check2Red();
            CheckBlackHeight();
            return true;
        }
        /* END Check methods */

        /* START ordering */
        private IEnumerable<RbTreeNode<TK, TV>> InOrder(RbTreeNode<TK, TV> node)
        {
            var stack = new Stack<RbTreeNode<TK, TV>>();
            while (node != null)
            {
                stack.Push(node);
                node = node.LeftChild;
            }
            while (stack.Count > 0)
            {
                node = stack.Pop();
                yield return node;
                if (node.RightChild == null) continue;
                node = node.RightChild;
                while (node != null)
                {
                    stack.Push(node);
                    node = node.LeftChild;
                }
            }
        }

        private IEnumerable<TV> InOrderValue(RbTreeNode<TK, TV> node)
        {
            var stack = new Stack<RbTreeNode<TK, TV>>();
            while (node != null)
            {
                stack.Push(node);
                node = node.LeftChild;
            }
            while (stack.Count > 0)
            {
                node = stack.Pop();
                yield return node.Value;
                if (node.RightChild == null) continue;
                node = node.RightChild;
                while (node != null)
                {
                    stack.Push(node);
                    node = node.LeftChild;
                }
            }
        }

        public IEnumerable<TV> InOrder() => InOrderValue(_root);

        public IEnumerable<Tuple<TK, TV>> InOrderTuple()
        {
            foreach (var node in InOrder(_root))
                yield return new Tuple<TK, TV>(node.Key, node.Value);
        }

        public IEnumerable<TV> LevelOrder()
        {
            if (_root == null) yield break;
            var node = _root;
            var queue = new Queue<RbTreeNode<TK, TV>>();
            queue.Enqueue(node);
            while (queue.Count > 0)
            {
                node = queue.Dequeue();
                yield return node.Value;
                if (node.LeftChild != null) queue.Enqueue(node.LeftChild);
                if (node.RightChild != null) queue.Enqueue(node.RightChild);
            }
        }

        public IEnumerable<TV> GetInterval(TK from, TK to)
        {
            if (from.CompareTo(to) > 0) throw new ArgumentException($"Starting key or interval is greater than end key.");
            var stack = new Stack<RbTreeNode<TK, TV>>();
            var node = _root;
            while (node != null)
            {
                var dir = from.CompareTo(node.Key);
                if (dir <= 0)
                {
                    stack.Push(node);
                    if (dir == 0) break;
                }
                node = node[dir];
            }
            while (stack.Count > 0)
            {
                node = stack.Pop();
                if (to.CompareTo(node.Key) < 0) yield break;
                yield return node.Value;
                if (node.RightChild == null) continue;
                node = node.RightChild;
                while (node != null)
                {
                    stack.Push(node);
                    node = node.LeftChild;
                }
            }
        }
        /* END ordering */

        /* START print tree */
        private void GetLevel(RbTreeNode<TK, TV> node, int level, ICollection<RbTreeNode<TK, TV>> list)
        {
            if (level == 1)
            {
                list.Add(node);
                return;
            }
            GetLevel(node?.LeftChild, level - 1, list);
            GetLevel(node?.RightChild, level - 1, list);
        }

        private List<List<RbTreeNode<TK, TV>>> GetLevels()
        {
            var rows = new List<List<RbTreeNode<TK, TV>>>();
            for (var level = 1; ; level++)
            {
                var row = new List<RbTreeNode<TK, TV>>();
                GetLevel(_root, level, row);
                var isEmpty = row.Count(n => n != null) == 0;
                if (isEmpty) return rows;
                rows.Add(row);
            }
        }

        public override string ToString()
        {
            if (_root == null) return "";
            var rows = GetLevels();
            rows.Reverse();
            var result = "";
            var startPad = 0;
            var midPad = 0;
            foreach (var row in rows)
            {
                var levelNodes = row.Select(node => node?.ToString() ?? " -- ");
                var line = string.Join("".PadLeft(midPad), levelNodes);
                result = $"\n{"".PadLeft(startPad)}{line}{result}";
                startPad = midPad + 2;
                midPad = startPad * 2;
            }
            return result;
        }

        public void Print() => Console.WriteLine(ToString());
        /* END print tree */

        /* START min/max */
        public TV Min()
        {
            if (_root == null) return default(TV);
            var node = _root;
            while (node.LeftChild != null)
                node = node.LeftChild;
            return node.Value;
        }

        public TV Max()
        {
            if (_root == null) return default(TV);
            var node = _root;
            while (node.RightChild != null)
                node = node.RightChild;
            return node.Value;
        }
        /* END min/max */
    }
}
