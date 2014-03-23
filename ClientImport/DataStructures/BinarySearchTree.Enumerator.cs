using System;
using System.Collections;
using System.Collections.Generic;

namespace ClientImport.DataStructures
{
    public partial class BinarySearchTree<TKey, TValue>
    {
        class Enumerator : IEnumerator<BinaryTreeNode<TKey, TValue>>
        {
            BinaryTreeNode<TKey, TValue> current;
            readonly BinarySearchTree<TKey, TValue> _theSearchTree;

            public Enumerator(BinarySearchTree<TKey, TValue> searchTree)
            {
                _theSearchTree = searchTree;
                current = null;
            }

            public bool MoveNext()
            {
                return _theSearchTree.Root != null && InOrderTraversal();
            }

            private bool InOrderTraversal()
            {
                if (current == null)
                    current = ReadLastLeftNode(_theSearchTree.Root);
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
                                var compare = _theSearchTree.Comparer.Compare(current.KeyValue.Key, currentKey);
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
