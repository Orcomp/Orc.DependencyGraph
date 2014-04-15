using System;
using Orc.DependencyGraph.GraphB;
using Orc.DependencyGraph.GraphD;

namespace Orc.DependencyGraphTests
{
    using System.Linq;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Orc.DependencyGraph;

    [TestFixture]
    public class GraphTest
    {
        private Type TargetGraph
        {
            get
            {
                return typeof(Graph<int>);
//                return typeof(GraphFast<int>);
//                return typeof(GraphB<int>);
            }
        }
            
        [Test]
        public void ConstructorWithInitialSequenceArgumentInitializesGraph()
        {
            var target = GraphTestHelper.CreateEmptyGraph(TargetGraph);
            target.AddSequence(new[] { 1, 2, 3, 4, 5 });
            Assert.AreEqual(5, target.CountNodes);
            GraphTestHelper.AssertConsistsOfSequences(target, new[]
            {
                new[] {1,2},
                new[] {2,3},
                new[] {3,4},
                new[] {4,5},
            });
        }

        [Test]
        public void ConstructorWithInitialSequencesArgumentInitializesGraph()
        {
            var target = GraphTestHelper.CreateEmptyGraph(TargetGraph);
            target.AddSequences(new[]
            {
                new [] { 1, 2, 3, 4, 5},
                new [] {11,12,13,14, 5},
            });
            Assert.AreEqual(9, target.CountNodes);
            GraphTestHelper.AssertConsistsOfSequences(target, new[]
            {
                new[] { 1, 2},
                new[] { 2, 3},
                new[] { 3, 4},
                new[] { 4, 5},
                new[] {11,12},
                new[] {12,13},
                new[] {13,14},
                new[] {14, 5},
            });
        }

        [TestCase(31)]
        [TestCase(45)]
        [TestCase(11)]
        public void FindReturnsCorrectValue(int node)
        {
            var graph = GraphTestHelper.CreateExampleGraph(TargetGraph);
            Assert.AreEqual(node, graph.Find(node).Value);
        }

        [Test]
        public void AddAddsSequence()
        {
            var target = GraphTestHelper.CreateEmptyGraph(TargetGraph);
            target.AddSequence(new [] {0,1});
            target.AddSequence(new [] {1,2});
            target.AddSequence(new [] {2,3});
            target.AddSequence(new [] {3,4});
            target.AddSequence(new [] {4,5});
            target.AddSequence(new [] {5,6});
            target.AddSequence(new [] {6,7});
            target.AddSequence(new [] {7,8});
            target.AddSequence(new [] {8,9});
            Assert.AreEqual(10, target.CountNodes);
            GraphTestHelper.AssertConsistsOfSequences(target, new[]
            {
                new[] {0,1},
                new[] {1,2},
                new[] {2,3},
                new[] {3,4},
                new[] {4,5},
                new[] {5,6},
                new[] {6,7},
                new[] {7,8},
                new[] {8,9},
            });
            GraphTestHelper.AssertConsistsOfBackSequences(target, new[]
            {
                new[] {1,0},
                new[] {2,1},
                new[] {3,2},
                new[] {4,3},
                new[] {5,4},
                new[] {6,5},
                new[] {7,6},
                new[] {8,7},
                new[] {9,8},
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Adding failed because sequence cannot contain a single node.")]
        public void SingleSequenceCannotBeAdded()
        {
            var graph = GraphTestHelper.CreateEmptyGraph(TargetGraph);
            graph.AddSequence(new[]{1});
        }

        [Test]
        public void AddAddsExistingSequence()
        {
            var graph = GraphTestHelper.CreateSimpleGraph(TargetGraph);
            graph.AddSequence(new[] {51, 61});
            graph.AddSequence(new[] {42, 52});
            Assert.AreEqual(7, graph.CountNodes);

            GraphTestHelper.AssertConsistsOfSequences(graph, new[]
            {
                new[] {41, 51},
                new[] {51, 61},
                new[] {61,100},
                new[] {42, 52},
                new[] {52, 62},
                new[] {62,100},
            });

        }

        [Test]
        public void AddRangeAddsSequences()
        {
            var graph = GraphTestHelper.CreateEmptyGraph(TargetGraph);
            graph.AddSequences(new []
            {
                new[] {41, 51, 61, 100},
                new[] {42, 52, 62, 100},
            });
            Assert.AreEqual(7, graph.CountNodes);
            GraphTestHelper.AssertConsistsOfSequences(graph, new[]
            {
                new[] {41, 51},
                new[] {51, 61},
                new[] {61,100},
                new[] {42, 52},
                new[] {52, 62},
                new[] {62,100},
            });
        }

        [Test]
        public void CanSortReturnsWhetherGraphCanBeSorted1()
        {
            var graph = GraphTestHelper.CreateSimpleGraph(TargetGraph);
            Assert.True(graph.CanSort());
        }
        [Test]
        public void CanSortReturnsWhetherGraphCanBeSorted2()
        {
            var graph = GraphTestHelper.CreateSimpleGraph(TargetGraph);
            graph.AddSequences(new[]
            {
                new[] {100, 41},
            });
            Assert.False(graph.CanSort());
        }

        [Test]
        public void CanSortReturnsWhetherGraphWithTheSequenceCanBeSorted1()
        {
            var graph = GraphTestHelper.CreateSimpleGraph(TargetGraph);
            Assert.True(graph.CanSort(new[] { 43, 52, 63, 100 }));
        }
        [Test]
        public void CanSortReturnsWhetherGraphWithTheSequenceCanBeSorted2()
        {
            var graph = GraphTestHelper.CreateSimpleGraph(TargetGraph);
            Assert.False(graph.CanSort(new[] { 100, 41 }));
        }

        [Test]
        public void CountReturnsCountOfGraph()
        {
            var graph = GraphTestHelper.CreateExampleGraph(TargetGraph);
            Assert.AreEqual(20, graph.CountNodes);
        }

        [Test]
        public void CountLevelsReturnsNumberOfLevelsInGraph1()
        {
            var graph = GraphTestHelper.CreateExampleGraph(TargetGraph);
            Assert.AreEqual(6, graph.CountLevels);
        }

        [Test]
        public void CountLevelsReturnsNumberOfLevelsInGraph2()
        {
            var graph = GraphTestHelper.CreateEmptyGraph(TargetGraph);
            graph.AddSequences(new[]
            {
                new[]{1,2,6},
                new[]{2,3,4,5},
                new[]{9,8,7,6},
            });
            Assert.AreEqual(6, graph.CountLevels);
        }

        [TestCase(0, new[] { 11, 12 })]
        [TestCase(1, new[] { 21, 22, 23, 24, 25, 26, 27 })]
        [TestCase(2, new[] { 31, 32 })]
        [TestCase(3, new[] { 41, 42, 43, 44, 45, 46 })]
        [TestCase(4, new[] { 51 })]
        [TestCase(5, new[] { 61, 62 })]
        public void GetNodesReturnsNodesOnTheLevel(int level, int[] expectedNodes)
        {
            var graph = GraphTestHelper.CreateExampleGraph(TargetGraph);
            GraphTestHelper.AssertCollectionsConsistsOfNodes(expectedNodes,
                graph.GetNodes(level));
            GraphTestHelper.AssertNodesAreOrderedByLevel(graph.GetNodes(level));
        }

        [TestCase(0, 1, new[] { 11, 12, 21, 22, 23, 24, 25, 26, 27})]
        [TestCase(1, 2, new[] { 21, 22, 23, 24, 25, 26, 27, 31, 32 })]
        [TestCase(0, 2, new[] { 11, 12, 21, 22, 23, 24, 25, 26, 27, 31, 32 })]
        [TestCase(4, 5, new[] { 51, 61, 62 })]
        public void GetNodesWithBetweenReturnsNodesOnTheLevelsInTheRange(int levelFrom, int levelTo, int[] expectedNodes)
        {
            var graph = GraphTestHelper.CreateExampleGraph(TargetGraph);
            GraphTestHelper.AssertCollectionsConsistsOfNodes(expectedNodes,
                graph.GetNodesBetween(levelFrom, levelTo));
            GraphTestHelper.AssertNodesAreOrderedByLevel(graph.GetNodesBetween(levelFrom, levelTo));
        }
        [Test]
        public void SortReturnsNodesInTopologicalOrder()
        {
            var graph = GraphTestHelper.CreateExampleGraph(TargetGraph);
            var nodes = graph.Sort();

            Assert.AreEqual(20, nodes.Count());
            AssertNodesAreInTopologicalOrder(nodes);
        }
        [Test]
        [ExpectedException(typeof(TopologicalSortException), ExpectedMessage = "Topological sort failed due to loops in the graph")]
        public void SortThrowsTopologicalSortExceptionExceptionWhenGraphContainsLoops()
        {
            var graph = GraphTestHelper.CreateExampleGraph(TargetGraph);
            graph.AddSequence(new[]{ 61, 11});
            graph.Sort().ToArray();
        }

        private static void AssertNodesAreInTopologicalOrder(IEnumerable<INode<int>> nodes)
        {
            var visitedNodes = new List<int>();
            foreach (var node in nodes)
            {
                if (node.ImmediatePrecedents.Count() != 0)
                {
                    foreach (var parent in node.ImmediatePrecedents)
                    {
                        if (!visitedNodes.Contains(parent.Value))
                            Assert.Fail("Topological sort is not valid. {0} is before {1}", node.Value, parent.Value);
                    }
                }
                visitedNodes.Add(node.Value);
            }
        }
    }
}
