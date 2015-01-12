namespace Orc.DependencyGraphTests
{
    using System;
    using System.Linq;

    using NUnit.Framework;

    using Orc.DependencyGraph;

    [TestFixture]
    public class GraphGeneratorTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateGraphThrowsArgumentExceptionOnNonMultipleParameters()
        {
            GraphGenerator.CalculateNodeCount(1, 3, 2);
        }

        [TestCase( 3, 2, 2, 1)]
        [TestCase( 7, 3, 2, 1)]
        [TestCase(15, 4, 2, 1)]
        [TestCase(13, 3, 3, 1)]
        [TestCase(40, 4, 3, 1)]
        [TestCase(14, 3, 4, 2)]
        [TestCase(14, 3, 2, 4)]
        public void CalculateNodeCount(int nodes, int levels, int descendants, int precedents)
        {
            Assert.AreEqual(nodes, GraphGenerator.CalculateNodeCount(levels, descendants, precedents));
        }

        [TestCase( 8, 0, 3, 2, 4)]
        [TestCase( 4, 1, 3, 2, 4)]
        [TestCase( 2, 2, 3, 2, 4)]
        public void NodesOnLevel(int nodes, int level, int levels, int descendants, int precedents)
        {
            Assert.AreEqual(nodes, GraphGenerator.NodesOnLevel(level, levels, descendants, precedents));
        }

        [TestCase(10,10, 1, 1)]
        [TestCase(13, 3, 3, 1)]
        [TestCase(14, 3, 2, 4)]
        [TestCase(13, 3, 1, 3)]
        [TestCase(14, 3, 4, 2)]
        [TestCase(40, 10, 4, 4)]
        public void GenerateGraphGeneratesCorrectNumberOfNodesAndTheirDescendantsAndPrecedents(
            int expectedNodeCount, int levels, int descendants, int precedents)
        {
            var target = GraphGenerator.GenerateGraph(levels, descendants, precedents);
            Assert.AreEqual(expectedNodeCount, target.CountNodes,
                "Graph expected to have {0} nodes but has {1}", expectedNodeCount, target.CountNodes);
            this.AssertEveryNodeHasNDescendants(target, descendants);
            this.AssertEveryNodeHasNPrecedents(target, precedents);
        }

        [Test]
        public void GenerateGraphGeneratesVerticalGraphStructure()
        {
            var target = GraphGenerator.GenerateGraph(10, 1, 1);

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
        }

        [Test]
        public void GenerateGraphGeneratesSquareGraphStructure()
        {
            var target = GraphGenerator.GenerateGraph(3, 2, 2);

            GraphTestHelper.AssertConsistsOfSequences(target, new[]
            {
                new[] {0,2},
                new[] {0,3},
                new[] {1,2},
                new[] {1,3},
                new[] {2,4},
                new[] {2,5},
                new[] {3,4},
                new[] {3,5},
            });
        }

        [Test]
        public void GenerateGraphGeneratesTriangleGraphStructure1()
        {
            var target = GraphGenerator.GenerateGraph(3, 3, 1);

            GraphTestHelper.AssertConsistsOfSequences(target, new[]
            {
                new[] {0,1},
                new[] {0,2},
                new[] {0,3},
                new[] {1,4},
                new[] {1,5},
                new[] {1,6},
                new[] {2,7},
                new[] {2,8},
                new[] {2,9},
                new[] {3,10},
                new[] {3,11},
                new[] {3,12},
            });
        }

        [Test]
        public void GenerateGraphGeneratesTriangleGraphStructure2()
        {
            var target = GraphGenerator.GenerateGraph(3, 2, 4);

            GraphTestHelper.AssertConsistsOfSequences(target, new[]
            {
                new[] { 0, 8},
                new[] { 1, 8},
                new[] { 2, 8},
                new[] { 3, 8},
                new[] { 0, 9},
                new[] { 1, 9},
                new[] { 2, 9},
                new[] { 3, 9},
                new[] { 4,10},
                new[] { 5,10},
                new[] { 6,10},
                new[] { 7,10},
                new[] { 4,11},
                new[] { 5,11},
                new[] { 6,11},
                new[] { 7,11},
                new[] { 8,12},
                new[] { 9,12},
                new[] {10,12},
                new[] {11,12},
                new[] { 8,13},
                new[] { 9,13},
                new[] {10,13},
                new[] {11,13},
            });
        }
        [Test]
        public void GenerateGraphGeneratesTriangleGraphStructure1_BackSequencesTest()
        {
            var target = GraphGenerator.GenerateGraph(3, 2, 4);

            GraphTestHelper.AssertConsistsOfBackSequences(target, new[]
            {
                new[] { 8, 0},
                new[] { 8, 1},
                new[] { 8, 2},
                new[] { 8, 3},
                new[] { 9, 0},
                new[] { 9, 1},
                new[] { 9, 2},
                new[] { 9, 3},
                new[] {10, 4},
                new[] {10, 5},
                new[] {10, 6},
                new[] {10, 7},
                new[] {11, 4},
                new[] {11, 5},
                new[] {11, 6},
                new[] {11, 7},
                new[] {12, 8},
                new[] {12, 9},
                new[] {12,10},
                new[] {12,11},
                new[] {13, 8},
                new[] {13, 9},
                new[] {13,10},
                new[] {13,11},
            });
        }

        [Test]
        public void GenerateGraphGeneratesBackTriangleGraphStructure1()
        {
            var target = GraphGenerator.GenerateGraph(3, 1, 3);

            GraphTestHelper.AssertConsistsOfSequences(target, new[]
            {
                new[] { 0, 9},
                new[] { 1, 9},
                new[] { 2, 9},
                new[] { 3,10},
                new[] { 4,10},
                new[] { 5,10},
                new[] { 6,11},
                new[] { 7,11},
                new[] { 8,11},
                new[] { 9,12},
                new[] {10,12},
                new[] {11,12},
            });
        }

        [Test]
        public void GenerateGraphGeneratesBackTriangleGraphStructure2()
        {
            var target = GraphGenerator.GenerateGraph(3, 4, 2);

            GraphTestHelper.AssertConsistsOfSequences(target, new[]
            {
                new[] { 0, 2},
                new[] { 0, 3},
                new[] { 0, 4},
                new[] { 0, 5},
                new[] { 1, 2},
                new[] { 1, 3},
                new[] { 1, 4},
                new[] { 1, 5},
                new[] { 2, 6},
                new[] { 2, 7},
                new[] { 2, 8},
                new[] { 2, 9},
                new[] { 3, 6},
                new[] { 3, 7},
                new[] { 3, 8},
                new[] { 3, 9},
                new[] { 4,10},
                new[] { 4,11},
                new[] { 4,12},
                new[] { 4,13},
                new[] { 5,10},
                new[] { 5,11},
                new[] { 5,12},
                new[] { 5,13},
            });
        }

        private void AssertEveryNodeHasNDescendants(IGraph<int> graph, int expectedDescendantCount)
        {
            foreach (var node in graph.Nodes)
            {
                var descendentCount = node.ImmediateDescendants.Count();
                if (descendentCount == 0) continue; // skip leafs
                Assert.AreEqual(expectedDescendantCount, descendentCount,
                    "Number of descendants in the node expected to be {0} but is {1}", expectedDescendantCount, descendentCount);
            }
        }
        private void AssertEveryNodeHasNPrecedents(IGraph<int> graph, int expectedPrecedentsCount)
        {
            foreach (var node in graph.Nodes)
            {
                var precedentsCount = node.ImmediatePrecedents.Count();
                if (precedentsCount == 0) continue; // skip roots
                Assert.AreEqual(expectedPrecedentsCount, precedentsCount,
                    "Number of precedents in the node expected to be {0} but is {1}", expectedPrecedentsCount, precedentsCount);
            }
        }
    }
}