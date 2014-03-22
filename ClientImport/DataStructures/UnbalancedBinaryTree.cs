using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace ClientImport.DataStructures
{
    internal enum NodeType {LeafeNode, HasOneChild, HasTwoChildren}
    internal enum NodeLinkToParentAs {Left, Right}
    public enum InOrderNode {Successor, Predecessor}

    public partial class UnbalancedBinaryTree<TKey, TValue> : IEnumerable<BinaryTreeNode<TKey, TValue>>
        where TKey : IComparable
    {
        public BinaryTreeNode<TKey, TValue> Root;

        /// <summary>
        /// Consistently using the in-order successor or the in-order predecessor for every instance of the two-child case can lead to an unbalanced tree, so some implementations select one or the other at different times.
        /// </summary>
        public InOrderNode? ForceDeleteType = null;

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

        public void Delete (TKey key )
        {
            var node = Find(key);
            if (node == null)
                return;

            var nodeType = GetNodeType(node);

            if(nodeType == NodeType.LeafeNode)
            {
                if(NodeLinkedToParentAs(node) == NodeLinkToParentAs.Right)
                    node.Parent.Right = null;
                else
                    node.Parent.Left = null;
            }
            if(nodeType == NodeType.HasOneChild)
            {
                var theChild = node.Left ?? node.Right;
                theChild.Parent = node.Parent;
                if (NodeLinkedToParentAs(node) == NodeLinkToParentAs.Right)
                    node.Parent.Right = theChild;
                else
                    node.Parent.Left = theChild;
            }
            if (nodeType != NodeType.HasTwoChildren) return;
            var replaceInOrderType = RollDiceForSuccessorOrPredecessor();

            if (replaceInOrderType == InOrderNode.Successor)
                UseSuccessor(node);
            else
                UsePredecessor(node);
        }

        private static void UsePredecessor(BinaryTreeNode<TKey, TValue> node)
        {
            var replaceWith = ReadLastRightNode(node.Left);
            var deletedNodeParent = node.Parent;
            replaceWith.Parent = deletedNodeParent;
            node.KeyValue = replaceWith.KeyValue;

            node.Left = replaceWith.Left;

            if (node.Left != null) node.Left.Parent = node;
            if (node.Right != null) node.Right.Parent = node;
        }

        private static void UseSuccessor(BinaryTreeNode<TKey, TValue> node)
        {
            var replaceWith = ReadLastLeftNode(node.Right);
            var deletedNodeParent = node.Parent;
            replaceWith.Parent = deletedNodeParent;
            node.KeyValue = replaceWith.KeyValue;

            node.Right = replaceWith.Right;

            if (node.Left != null) node.Left.Parent = node;
            if (node.Right != null) node.Right.Parent = node;
        }

        private InOrderNode RollDiceForSuccessorOrPredecessor()
        {
            if (ForceDeleteType.HasValue)
                return ForceDeleteType.Value;

            var random = new Random();
            var choice = random.Next(0, 2);
            return choice == 0 ? InOrderNode.Predecessor : InOrderNode.Successor;
        }

        private static NodeLinkToParentAs NodeLinkedToParentAs(BinaryTreeNode<TKey, TValue> node)
        {
            return node.Parent.Left == node ? NodeLinkToParentAs.Left : NodeLinkToParentAs.Right;
        }

        private NodeType GetNodeType(BinaryTreeNode<TKey, TValue> node)
        {
            if(node.Left == null && node.Right == null)
                return NodeType.LeafeNode;
            if(node.Left != null && node.Right != null)
                return NodeType.HasTwoChildren;

                return NodeType.HasOneChild;
        }

        public void Add(TKey key, TValue value)
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

        private static BinaryTreeNode<TKey, TValue> ReadLastRightNode(BinaryTreeNode<TKey, TValue> start)
        {
            var node = start;
            while (true)
            {
                if (node.Right != null)
                {
                    node = node.Right;
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