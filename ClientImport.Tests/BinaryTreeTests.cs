using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClientImport.DTO;
using ClientImport.DataStructures;
using ClientImport.Extensions;
using ClientImport.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClientImport.Tests
{
    [TestClass]
    public class BinaryTreeTests
    {
        /// <summary>
        /// The UnbalancedBinaryTree acts very similar to the SortedDictionary in .NET, so we can compare how both sort.
        /// </summary>
        [TestMethod]
        public void BinaryTreeMustSortTheSameAsSortedDictionary()
        {
            // Arrange
            var asm = Assembly.GetExecutingAssembly();
            var importer = new Importer(asm.GetManifestResourceStream("ClientImport.Tests.records.txt"), ',');
            var dictionary = new SortedDictionary<string, IPerson>();
            var tree = new UnbalancedBinaryTree<string, IPerson>();

            //Act
            importer.Data
                           .ToList()
                           .ForEach(record =>
                           {
                               var person = new Person
                               {
                                   FirstName = record.FirstName,
                                   Surname = record.Surname,
                                   Age = Convert.ToInt16(record.Age)
                               };
                               var key = PersonRepository.BuildKey(person, SortKey.SurnameFirstNameAge);
                               if (tree.Find(key) == null)
                                   tree.Add(key, person);
                           }
                              );

            tree
                .ToList()
                .Shuffle() //Shuffle result from binary tree, to test sorting
                .ForEach(r => dictionary.Add(PersonRepository.BuildKey(r.KeyValue.Value, SortKey.SurnameFirstNameAge), r.KeyValue.Value));

            var expected = dictionary.Select(r => r.Value).ToList();
            var actual = tree.Select(n => n.KeyValue.Value).ToList();
            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CanDeleteLeafNode()
        {
            //Arrange
            var tree = new UnbalancedBinaryTree<int, int> { { 2, 2 }, { 1, 1 }, { 3, 3 }, { 4, 4 } };
            var expected = new[] { 1, 2, 3 }.ToList();
            //Act
            tree.Delete(4);
            var result = tree.Select(r => r.KeyValue.Value).ToList();
            //Assert
            CollectionAssert.AreEquivalent(expected, result);
        }

        [TestMethod]
        public void CanDeleteNodeWithOneRightChild()
        {
            //Arrange
            var tree = new UnbalancedBinaryTree<int, int> { { 9, 9 }, { 1, 1 }, { 3, 3 }, { 10, 10 }, { 4, 4 }, { 6, 6 } };
            var expected = new[] { 1, 4, 6, 9, 10 }.ToList();
            //Act
            tree.Delete(3);
            var result = tree.Select(r => r.KeyValue.Value).ToList();
            //Assert
            CollectionAssert.AreEquivalent(expected, result);
        }

        [TestMethod]
        public void CanDeleteNodeWithOneRightChildWithNestedBinaryTree()
        {
            //Arrange
            var tree = new UnbalancedBinaryTree<int, int> { { 9, 9 }, { 1, 1 }, { 3, 3 }, { 10, 10 }, { 4, 4 }, { 6, 6 }, { 5, 5 } };
            var expected = new[] { 1, 4, 5, 6, 9, 10 }.ToList();
            //Act
            tree.Delete(3);
            var result = tree.Select(r => r.KeyValue.Value).ToList();
            //Assert
            CollectionAssert.AreEquivalent(expected, result);
        }

        [TestMethod]
        public void CanDeleteNodeWithOneLeftChildNestedBinaryTree()
        {
            //Arrange
            var tree = new UnbalancedBinaryTree<int, int> { { 100, 100 }, { 50, 50 }, { 40, 40 }, { 25, 25 }, { 26, 26 }, { 60, 60 }, { 20, 20 } };
            var expected = new[] { 20, 25, 26, 50, 60, 100 }.ToList();
            //Act
            tree.Delete(40);
            var result = tree.Select(r => r.KeyValue.Value).ToList();
            //Assert
            CollectionAssert.AreEquivalent(expected, result);
        }

        [TestMethod]
        public void CanDeleteNodeWithTwoChildrenRandomChoice()
        {
            //Arrange
            var tree = new UnbalancedBinaryTree<int, int> { { 100, 100 }, { 50, 50 }, { 40, 40 }, { 25, 25 }, { 26, 26 }, { 60, 60 }, { 20, 20 } };
            var expected = new[] { 20, 26, 40, 50, 60, 100 }.ToList();
            //Act
            tree.Delete(25);
            var result = tree.Select(r => r.KeyValue.Value).ToList();
            //Assert
            CollectionAssert.AreEquivalent(expected, result);
        }

        [TestMethod]
        public void CanDeleteNodeWithTwoChildrenUseSuccessor()
        {
            //Arrange
            var tree = new UnbalancedBinaryTree<int, int> { { 100, 100 }, { 50, 50 }, { 40, 40 }, { 25, 25 }, { 26, 26 }, { 60, 60 }, { 20, 20 } };
            tree.ForceDeleteType = InOrderNode.Successor;
            var expected = new[] { 20, 26, 40, 50, 60, 100 }.ToList();
            //Act
            tree.Delete(25);
            var result = tree.Select(r => r.KeyValue.Value).ToList();
            //Assert
            CollectionAssert.AreEquivalent(expected, result);
        }

        [TestMethod]
        public void CanDeleteNodeWithTwoChildrenUsePredecessor()
        {
            //Arrange
            var tree = new UnbalancedBinaryTree<int, int> { { 100, 100 }, { 50, 50 }, { 40, 40 }, { 25, 25 }, { 26, 26 }, { 60, 60 }, { 20, 20 } };
            tree.ForceDeleteType = InOrderNode.Predecessor;
            var expected = new[] { 20, 26, 40, 50, 60, 100 }.ToList();
            //Act
            tree.Delete(25);
            var result = tree.Select(r => r.KeyValue.Value).ToList();
            //Assert
            CollectionAssert.AreEquivalent(expected, result);
        }

        [TestMethod]
        public void CanDeleteNodeWithTwoChildrenNestedBinaryTreeRandomChoice()
        {
            //Arrange
            var tree = new UnbalancedBinaryTree<int, int> { { 100, 100 }, { 50, 50 }, { 40, 40 }, { 25, 25 }, { 26, 26 }, { 60, 60 }, { 20, 20 } };
            var expected = new[] { 20, 25, 26, 40, 60, 100 }.ToList();
            //Act
            tree.Delete(50);
            var result = tree.Select(r => r.KeyValue.Value).ToList();
            //Assert
            CollectionAssert.AreEquivalent(expected, result);
        }

        [TestMethod]
        public void CanDeleteNodeWithTwoChildrenNestedBinaryTreeUseSuccessor()
        {
            //Arrange
            var tree = new UnbalancedBinaryTree<int, int> { { 100, 100 }, { 50, 50 }, { 40, 40 }, { 25, 25 }, { 26, 26 }, { 60, 60 }, { 20, 20 } };
            tree.ForceDeleteType = InOrderNode.Successor;
            var expected = new[] { 20, 25, 26, 40, 60, 100 }.ToList();
            //Act
            tree.Delete(50);
            var result = tree.Select(r => r.KeyValue.Value).ToList();
            //Assert
            CollectionAssert.AreEquivalent(expected, result);
        }

        [TestMethod]
        public void CanDeleteNodeWithTwoChildrenNestedBinaryTreeUsePredecessor()
        {
            //Arrange
            var tree = new UnbalancedBinaryTree<int, int> { { 100, 100 }, { 50, 50 }, { 40, 40 }, { 25, 25 }, { 26, 26 }, { 60, 60 }, { 20, 20 } };
            tree.ForceDeleteType = InOrderNode.Predecessor;
            var expected = new[] { 20, 25, 26, 40, 60, 100 }.ToList();
            //Act
            tree.Delete(50);
            var result = tree.Select(r => r.KeyValue.Value).ToList();
            //Assert
            CollectionAssert.AreEquivalent(expected, result);
        }


        [TestMethod]
        public void CanDeleteRootNodeWithOneChildrenNestedBinaryTree()
        {
            //Arrange
            var tree = new UnbalancedBinaryTree<int, int> { { 100, 100 }, { 50, 50 }, { 40, 40 }, { 25, 25 }, { 26, 26 }, { 60, 60 }, { 20, 20 } };
            var expected = new[] { 20, 25, 26, 40, 50, 60 }.ToList();
            //Act
            tree.Delete(100);
            var result = tree.Select(r => r.KeyValue.Value).ToList();
            //Assert
            CollectionAssert.AreEquivalent(expected, result);
        }

        [TestMethod]
        public void CanDeleteRootNodeWithTwoChildrenNestedBinaryTreeUsePredecessor()
        {
            //Arrange
            var tree = new UnbalancedBinaryTree<int, int> { { 100, 100 }, { 200, 200 }, { 50, 50 }, { 40, 40 }, { 25, 25 }, { 26, 26 }, { 60, 60 }, { 20, 20 } };
            tree.ForceDeleteType = InOrderNode.Predecessor;
            var expected = new[] { 20, 25, 26, 40, 50, 60, 200 }.ToList();
            //Act
            tree.Delete(100);
            var result = tree.Select(r => r.KeyValue.Value).ToList();
            //Assert
            CollectionAssert.AreEquivalent(expected, result);
        }

        [TestMethod]
        public void CanDeleteRootNodeWithTwoChildrenNestedBinaryTreeUseSuccessor()
        {
            //Arrange
            var tree = new UnbalancedBinaryTree<int, int> { { 100, 100 }, { 200, 200 }, { 50, 50 }, { 40, 40 }, { 25, 25 }, { 26, 26 }, { 60, 60 }, { 20, 20 } };
            tree.ForceDeleteType = InOrderNode.Successor;
            var expected = new[] { 20, 25, 26, 40, 50, 60, 200 }.ToList();
            //Act
            tree.Delete(100);
            var result = tree.Select(r => r.KeyValue.Value).ToList();
            //Assert
            CollectionAssert.AreEquivalent(expected, result);
        }

        [TestMethod]
        public void CanDeleteRootNodeWithTwoChildrenNestedBinaryTreeUseRandom()
        {
            //Arrange
            var tree = new UnbalancedBinaryTree<int, int> { { 100, 100 }, { 200, 200 }, { 50, 50 }, { 40, 40 }, { 25, 25 }, { 26, 26 }, { 60, 60 }, { 20, 20 } };
            var expected = new[] { 20, 25, 26, 40, 50, 60, 200 }.ToList();
            //Act
            tree.Delete(100);
            var result = tree.Select(r => r.KeyValue.Value).ToList();
            //Assert
            CollectionAssert.AreEquivalent(expected, result);
        }

        [TestMethod]
        public void CanDeleteNodeWithOneLeftChild()
        {
            //Arrange
            var tree = new UnbalancedBinaryTree<int, int> { { 9, 9 }, { 1, 1 }, { 3, 3 }, { 2, 2 }, { 10, 10 }, { 11, 11 } };
            var expected = new[] { 1, 2, 9, 10, 11 }.ToList();
            //Act
            tree.Delete(3);
            var result = tree.Select(r => r.KeyValue.Value).ToList();
            //Assert
            CollectionAssert.AreEquivalent(expected, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExpectExceptionForDuplicateSearchKey()
        {
            // Act
            var actual = new UnbalancedBinaryTree<string, IPerson> { { "key1", null }, { "key1", null } };
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
            var tree = new UnbalancedBinaryTree<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 }, { 4, 4 } };

            //Assert
            Assert.IsTrue(tree.Root.Parent == null);
        }

        [TestMethod]
        public void DefaultRootNodeHasLeftChildNode()
        {
            // Arrange
            var tree = new UnbalancedBinaryTree<int, int> { { 2, 1 }, { 1, 2 }, { 3, 3 }, { 4, 4 } };

            //Assert
            Assert.IsTrue(tree.Root.Left.KeyValue.Key == 1);
        }

        [TestMethod]
        public void DefaultRootNodeHasRightChildNode()
        {
            // Arrange
            var tree = new UnbalancedBinaryTree<int, int> { { 2, 1 }, { 1, 2 }, { 3, 3 }, { 4, 4 } };

            //Assert
            Assert.IsTrue(tree.Root.Right.KeyValue.Key == 3);
        }

        [TestMethod]
        public void UseIndexerToAccessKeyValue()
        {
            // Arrange
            var tree = new UnbalancedBinaryTree<int, int> { { 1, 777 }, { 2, 2 }, { 3, 333 }, { 4, 4 } };

            //Assert
            Assert.AreEqual(tree[1], 777);
            Assert.AreEqual(tree[3], 333);
        }

        [TestMethod]
        public void WhenUsingStringAsKeyOrdinalSortIsUsed()
        {
            var data = new UnbalancedBinaryTree<string, object>(StringComparison.Ordinal) { { '\u0069'.ToString(), null }, { '\u0131'.ToString(), null }, { '\u0049'.ToString(), null } };

            //Assert
            Assert.IsTrue(data.First().KeyValue.Key == '\u0049'.ToString());
            Assert.IsTrue(data.Last().KeyValue.Key == '\u0131'.ToString());
        }

        [TestMethod]
        public void WhenUsingStringAsKeyInVariantCultureSortIsDefault()
        {
            //Arrange Act
            var data = new UnbalancedBinaryTree<string, object> { { '\u0069'.ToString(), null }, { '\u0131'.ToString(), null }, { '\u0049'.ToString(), null } };

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
            CollectionAssert.AreEqual(expected, traversed.Select(r => r.KeyValue.Key).ToList());
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
            keys.Shuffle().ForEach(k => data.Add(k, k));

            //Act
            var traversed = data.ToList();

            //Assert
            traversed.ForEach(currentNode =>
                                  {
                                      if (currentNode.Parent == null) return;
                                      if (currentNode.Parent.Right == currentNode)
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
