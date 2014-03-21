using System;
using System.Linq;
using ClientImport.DTO;
using ClientImport.DataStructures;
using ClientImport.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClientImport.Tests
{
    [TestClass]
    public class BinaryTreeTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExpectExceptionForDuplicateSearchKey()
        {
            // Act
            new UnbalancedBinaryTree<string, IPerson> {{"key1", null}, {"key1", null}};
        }

        [TestMethod]
        public void DefaultRootNodeIsInitializedEvenWhenEmpty()
        {
            // Arrange
            var data = new UnbalancedBinaryTree<string, IPerson>();

            //Act
            var result = data.ToList();

            //Assert
            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod]
        public void DefaultRootNodeHasNoParent()
        {
            // Arrange
            var tree = new UnbalancedBinaryTree<int, int> {{1, 1}, {2, 2}, {3, 3}, {4, 4}};

            //Assert
            Assert.IsTrue(tree.Root.Parent == null);
        }

        [TestMethod]
        public void DefaultRootNodeHasLeftChildNode()
        {
            // Arrange
            var tree = new UnbalancedBinaryTree<int, int> {{2, 1}, {1, 2}, {3, 3}, {4, 4}};

            //Assert
            Assert.IsTrue(tree.Root.Left.KeyValue.Key == 1);
        }

        [TestMethod]
        public void DefaultRootNodeHasRightChildNode()
        {
            // Arrange
            var tree = new UnbalancedBinaryTree<int, int> {{2, 1}, {1, 2}, {3, 3}, {4, 4}};

            //Assert
            Assert.IsTrue(tree.Root.Right.KeyValue.Key == 3);
        }

        [TestMethod]
        public void WhenUsingStringAsKeyOrdinalSortIsUsed()
        {
            var data = new UnbalancedBinaryTree<string, object>(StringComparison.Ordinal) 
            {{'\u0069'.ToString(), null}, {'\u0131'.ToString(), null}, {'\u0049'.ToString(), null}};

            //Assert
            Assert.IsTrue(data.First().KeyValue.Key == '\u0049'.ToString());
            Assert.IsTrue(data.Last().KeyValue.Key == '\u0131'.ToString());
        }

        [TestMethod]
        public void WhenUsingStringAsKeyInVariantCultureSortIsDefault()
        {
            //Arrange Act
            var data = new UnbalancedBinaryTree<string, object>
            {{'\u0069'.ToString(), null}, {'\u0131'.ToString(), null}, {'\u0049'.ToString(), null}};

            //Assert
            Assert.IsTrue(data.First().KeyValue.Key == '\u0069'.ToString());
            Assert.IsTrue(data.Last().KeyValue.Key == '\u0131'.ToString());
        }

        [TestMethod]
        public void TreeTraversalIsSortedInAscendingOrder()
        {
            // Arrange
            var data = new UnbalancedBinaryTree<int, int>();

            var keys = Enumerable.Range(1, 50).ToList();
            keys.Shuffle().ForEach(k => data.Add(k, k));

            var expected = keys.OrderBy(k => k).ToList();

            //Act
            var traversed = data.ToList();

            //Assert
            CollectionAssert.AreEqual(expected, traversed.Select(r => r.KeyValue.Key).ToList() );
        }

        /// <summary>
        /// A degenerate tree is a tree where for each parent node, there is only one associated child node. 
        /// What this means is that in a performance measurement, the tree will essentially behave like a linked list data structure.
        /// In this case, it will have no left child nodes, so it is a linked list structure.
        /// </summary>
        [TestMethod]
        public void UnbalancedTreeIsDegenerateWhenGivenSortedData()
        {
            // Arrange
            var data = new UnbalancedBinaryTree<int, int>();

            var keys = Enumerable.Range(1, 50).ToList();
            keys.ForEach(k => data.Add(k, k));

            //Act
            var traversed = data.ToList();

            //Assert
            traversed.ForEach(currentNode =>
            {
                if (currentNode.Parent == null) return;
                    Assert.IsTrue(currentNode.Left == null);
            });
        }

        [TestMethod]
        public void UnbalancedTreeIsNotDegenerateWhenGivenUnSortedData()
        {
            // Arrange
            var data = new UnbalancedBinaryTree<int, int>();
            var count = 0;

            var keys = Enumerable.Range(1, 50).ToList();
            keys.Shuffle().ForEach(k => data.Add(k, k));

            //Act
            var traversed = data.ToList();

            //Assert
            traversed.ForEach(currentNode =>
            {
                if (currentNode.Parent == null) return;
                if (currentNode.Left != null)
                    count++;
            });

            Assert.IsTrue(count > 0);
        }

        [TestMethod]
        public void RighttKeyIsAlwaysGreaterThanParentKey()
        {
            // Arrange
            var data = new UnbalancedBinaryTree<int, int>();

            var keys = Enumerable.Range(1, 50).ToList();
            keys.Shuffle().ForEach(k => data.Add(k,k));

            //Act
            var traversed = data.ToList();

            //Assert
            traversed.ForEach(currentNode =>
                                  {
                                      if (currentNode.Parent == null) return;
                                      if(currentNode.Parent.Right == currentNode)
                                          Assert.IsTrue(currentNode.KeyValue.Key > currentNode.Parent.KeyValue.Key);
                                  });
        }

        [TestMethod]
        public void LefttKeyIsAlwaysSmallerThanParentKey()
        {
            // Arrange
            var data = new UnbalancedBinaryTree<int, int>();

            var keys = Enumerable.Range(1, 50).ToList();
            keys.Shuffle().ForEach(k => data.Add(k, k));

            //Act
            var traversed = data.ToList();

            //Assert
            traversed.ForEach(currentNode =>
                                  {
                                      if (currentNode.Parent == null) return;
                                      if (currentNode.Parent.Left == currentNode)
                                          Assert.IsTrue(currentNode.KeyValue.Key < currentNode.Parent.KeyValue.Key);
                                  });
        }
    }
}
