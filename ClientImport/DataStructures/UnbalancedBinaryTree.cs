using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace ClientImport.DataStructures
{
    public partial class UnbalancedBinaryTree<TKey, TValue> : IEnumerable<BinaryTreeNode<TKey, TValue>>
        where TKey : IComparable
    {
        public BinaryTreeNode<TKey, TValue> Root;
        private readonly IComparer<TKey> comparer;

        private readonly StringComparison defaultStringComparison = StringComparison.InvariantCulture;

        public UnbalancedBinaryTree()
        {
            comparer = GetComparer();
        }

        public UnbalancedBinaryTree(StringComparison defaultStringComparison)
        {
            this.defaultStringComparison = defaultStringComparison;
            comparer = GetComparer();
        }

        private IComparer<TKey> GetComparer()
        {
            if (typeof(IComparable<TKey>).IsAssignableFrom(typeof(TKey)) || typeof(IComparable).IsAssignableFrom(typeof(TKey)))
            {
                if (typeof(IComparable<TKey>).IsAssignableFrom(typeof(string)))
                {
                    switch(defaultStringComparison)
                    {
                        case StringComparison.Ordinal: return (IComparer<TKey>)StringComparer.Ordinal;
                        case StringComparison.OrdinalIgnoreCase: return (IComparer<TKey>)StringComparer.OrdinalIgnoreCase;
                        case StringComparison.InvariantCulture: return (IComparer<TKey>)StringComparer.InvariantCulture;
                        case StringComparison.InvariantCultureIgnoreCase: return (IComparer<TKey>)StringComparer.InvariantCultureIgnoreCase;
                        case StringComparison.CurrentCulture: return (IComparer<TKey>)StringComparer.CurrentCulture;
                        case StringComparison.CurrentCultureIgnoreCase: return (IComparer<TKey>)StringComparer.CurrentCultureIgnoreCase;
                    }
                }
                return Comparer<TKey>.Default;
            }
            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The type {0} cannot be compared. It must implement IComparable<T>.", typeof(TKey).FullName));
        }

        private void Add(TKey key, TValue value)
        {
            var child = new BinaryTreeNode<TKey, TValue> { KeyValue = new KeyValuePair<TKey, TValue>(key, value) };

            if (Root == null)
                Root = child;
            else
            {
                var current = Root;
                while (true)
                {
                    var compareResult = comparer.Compare(key, current.KeyValue.Key);
                    if (compareResult == 0)
                        throw new ArgumentException("Duplicate key found.");

                    if (compareResult < 0)
                        if (current.Left != null)
                            current = current.Left;
                        else
                        {
                            current.Left = child;
                            child.Parent = current;
                            break;
                        }
                    else if (compareResult > 0)
                        if (current.Right != null)
                            current = current.Right;
                        else
                        {
                            current.Right = child;
                            child.Parent = current;
                            break;
                        }
                }

            }
        }

        public BinaryTreeNode<TKey, TValue> Find(TKey key)
        {
            var currentNode = Root;
            while (currentNode != null)
            {
                var compareResult = comparer.Compare(key, currentNode.KeyValue.Key);

                if (compareResult == 0) return currentNode;
                if (compareResult < 0)
                {
                    currentNode = currentNode.Left;
                    continue;
                }
                currentNode = currentNode.Right;
            }
            return null;
        }

        private static BinaryTreeNode<TKey, TValue> ReadLastLeftNode(BinaryTreeNode<TKey, TValue> start)
        {
            var node = start;
            while (true)
            {
                if (node.Left != null)
                {
                    node = node.Left;
                    continue;
                }
                break;
            }
            return node;
        }

        public TValue this[TKey key]
        {
            get
            {
                var node = Find(key);
                return node.KeyValue.Value;
            }
            set
            {
                var node = Find(key);
                if (node == null)
                {
                    Add(key, value);
                }
                else
                {
                    node.KeyValue = new KeyValuePair<TKey, TValue>(key, value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public IEnumerator<BinaryTreeNode<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }
    }
}