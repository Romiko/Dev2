using System;
using System.Collections;
using System.Collections.Generic;

namespace ClientImport.DataStructures
{
    public partial class UnbalancedBinaryTree<TKey, TValue>
    {
        class Enumerator : IEnumerator<BinaryTreeNode<TKey, TValue>>
        {
            BinaryTreeNode<TKey, TValue> current;
            readonly UnbalancedBinaryTree<TKey, TValue> theTree;

            public Enumerator(UnbalancedBinaryTree<TKey, TValue> tree)
            {
                theTree = tree;
                current = null;
            }

            public bool MoveNext()
            {
                return theTree.Root != null && InOrderTraversal();
            }

            private bool InOrderTraversal()
            {
                if (current == null)
                    current = ReadLastLeftNode(theTree.Root);
                else
                {
                    if (current.Right != null)
                        current = ReadLastLeftNode(current.Right);
                    else
                    {
                        var currentKey = current.KeyValue.Key;

                        while (current != null)
                        {
                            current = current.Parent;
                            if (current != null)
                            {
                                var compare = theTree.comparer.Compare(current.KeyValue.Key, currentKey);
                                if (compare < 0) continue;
                            }
                            break;
                        }
                    }
                }
                return (current != null);
            }

            public BinaryTreeNode<TKey, TValue> Current
            {
                get
                {
                    if (current == null)
                        throw new InvalidOperationException();
                    return current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (current == null)
                        throw new InvalidOperationException();
                    return current.KeyValue.Value;
                }
            }

            public void Dispose() { }
            public void Reset() { current = null; }
        }
    }
}
