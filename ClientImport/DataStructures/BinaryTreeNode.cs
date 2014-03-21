using System.Collections.Generic;

namespace ClientImport.DataStructures
{
    public class BinaryTreeNode<TKey, TValue>
    {
        public BinaryTreeNode<TKey, TValue> Parent;
        public BinaryTreeNode<TKey, TValue> Left;
        public BinaryTreeNode<TKey, TValue> Right;
        public KeyValuePair<TKey, TValue> KeyValue;
    }
}