using System;
using System.Linq;
using System.Text;
using NUnitBenchmarker.Benchmark;
using Orc.DependencyGraph;

namespace Orc.DependencyGraphPerformanceTests
{
    using NUnit.Framework;

    using Orc.DependencyGraph.GraphD;

    [TestFixture, RequiresSTA]
    public class GraphPerformanceTest
    {
        [Test, TestCaseSource(typeof(TestFactory), "TestCases")]
        public void AddTest(TestConfiguration conf)
        {
            var sequencesToAdd = GenerateSequencesToAdd(conf);
            var graph = GraphPerformanceTestHelper.CreateGraphInstance(conf.GraphType);

            Assert.IsTrue(graph.CountNodes == 0);

            var action = new Action(() =>
            {
                foreach (var sequence in sequencesToAdd)
                {
                    graph.AddSequence(sequence);
                }
            });

            action.Benchmark(conf, "AddSequence", conf.ToString());
        }

        [Test, TestCaseSource(typeof(TestFactory), "TestCases")]
        [MaxTime(10000)]
        public void AddRangeTest(TestConfiguration conf)
        {
            var sequencesToAdd = GenerateSequencesToAdd(conf);
            var graph = GraphPerformanceTestHelper.CreateGraphInstance(conf.GraphType);

            Assert.IsTrue(graph.CountNodes == 0);

            var action = new Action(() => graph.AddSequences(sequencesToAdd));

            action.Benchmark(conf, "AddSequences", conf.ToString());
        }

        [Test, TestCaseSource(typeof(TestFactory), "TestCases")]
        [MaxTime(10000)]
        public void GetNodesLevel0Test(TestConfiguration conf)
        {
            var graph = GraphPerformanceTestHelper.CreateGraphInstance(conf.GraphType);

            Assert.IsTrue(graph.CountNodes == 0);

            GraphPerformanceTestHelper.FillGraph(graph, conf);
            for (var i = 0; i < 10; i++)
            {
                var result = graph.GetNodes(i).ToArray();
            }
            var sb = new StringBuilder();
            foreach (var node in graph.Sort().OrderBy(x=>x.Level))
            {
                sb.AppendFormat("{0}: level {1} \n", node.Value, node.Level);
            }



        }

        [Test, TestCaseSource(typeof(TestFactory), "TestCases")]
        [MaxTime(10000)]
        public void GetNodesMiddleTest(TestConfiguration conf)
        {
            var graph = GraphPerformanceTestHelper.CreateGraphInstance(conf.GraphType);

            Assert.IsTrue(graph.CountNodes == 0);

            GraphPerformanceTestHelper.FillGraph(graph, conf);

            var action = new Action(() => graph.GetNodes(conf.Levels / 2).Enumerate());

            action.Benchmark(conf, "GetNodesMiddle", conf.ToString());
        }

        [Test, TestCaseSource(typeof(TestFactory), "TestCases")]
        [MaxTime(10000)]
        public void GetNodesLevelMaxTest(TestConfiguration conf)
        {
            var graph = GraphPerformanceTestHelper.CreateGraphInstance(conf.GraphType);

            Assert.IsTrue(graph.CountNodes == 0);

            GraphPerformanceTestHelper.FillGraph(graph, conf);

            var action = new Action(() => graph.GetNodes(conf.Levels-1).Enumerate());

            action.Benchmark(conf, "GetNodesLevelMax", conf.ToString());
        }

        [Test, TestCaseSource(typeof(TestFactory), "TestCases")]
        [MaxTime(10000)]
        public void GetNodesBetweenTest(TestConfiguration conf)
        {
            var graph = GraphPerformanceTestHelper.CreateGraphInstance(conf.GraphType);

            Assert.IsTrue(graph.CountNodes == 0);

            GraphPerformanceTestHelper.FillGraph(graph, conf);

            var middleLevel = conf.Levels/2;
            var action = new Action(() => graph.GetNodesBetween(middleLevel - 1, middleLevel + 1).Enumerate());
            action.Benchmark(conf, "GetNodesBetween", conf.ToString());
        }

        [Test, TestCaseSource(typeof(TestFactory), "TestCases")]
        [MaxTime(10000)]
        public void GetNodesBetween0AndMax(TestConfiguration conf)
        {
            var graph = GraphPerformanceTestHelper.CreateGraphInstance(conf.GraphType);

            Assert.IsTrue(graph.CountNodes == 0);

            GraphPerformanceTestHelper.FillGraph(graph, conf);

            var action = new Action(() => graph.GetNodesBetween(0, conf.Levels - 1).Enumerate());
            action.Benchmark(conf, "GetNodesBetween0AndMax", conf.ToString());

            var count = graph.GetNodesBetween(0, conf.Levels - 1).Count();
            Assert.IsTrue(count == graph.CountNodes);
        }

        [Test, TestCaseSource(typeof(TestFactory), "TestCases")]
        [MaxTime(10000)]
        public void SortTest(TestConfiguration conf)
        {
            var graph = GraphPerformanceTestHelper.CreateGraphInstance(conf.GraphType);

            Assert.IsTrue(graph.CountNodes == 0);

            GraphPerformanceTestHelper.FillGraph(graph, conf);

            var action = new Action(() => graph.Sort().Enumerate());

            action.Benchmark(conf, "Sort", conf.ToString());
        }

        [Test, TestCaseSource(typeof(TestFactory), "TestCases")]
        [MaxTime(10000)]
        public void CanSortTest(TestConfiguration conf)
        {
            var graph = GraphPerformanceTestHelper.CreateGraphInstance(conf.GraphType);

            Assert.IsTrue(graph.CountNodes == 0);

            GraphPerformanceTestHelper.FillGraph(graph, conf);

            var action = new Action(() => graph.CanSort(new[] { 1, 2 }));

            action.Benchmark(conf, "CanSort", conf.ToString());
        }

        private static int[][] GenerateSequencesToAdd(TestConfiguration conf)
        {
            // Using Orc.DependencyGraph.Graph implementation to generate sequences to add
            var graph = new Graph<int>() as IInternalGraph<int>;
            GraphPerformanceTestHelper.FillGraph(graph, conf);

            var sequencesToAdd = graph.Edges
                .Select(edge => new[]
                {
                    edge[0].Value,
                    edge[1].Value
                })
                .ToArray();

            return sequencesToAdd;
        }
    }
}
