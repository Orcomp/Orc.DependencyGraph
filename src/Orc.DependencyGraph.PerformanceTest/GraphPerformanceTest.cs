using System.Linq;

namespace Orc.DependencyGraphPerformanceTests
{
    using NUnit.Framework;
    using NUnitBenchmarker;
    using DependencyGraph.GraphD;

    [TestFixture, RequiresSTA]
    public class GraphPerformanceTest
    {
        [Test, TestCaseSource(typeof(TestFactory), "TestCases")]
        public void AddTest(TestConfiguration conf)
        {
	        conf.Prepare = (i =>
	        {
		        var c = ((TestConfiguration) i);
		        c.SequencesToAdd = GenerateSequencesToAdd(conf);
		        c.Graph = GraphPerformanceTestHelper.CreateGraphInstance(conf.GraphType);
		        Assert.IsTrue(c.Graph.CountNodes == 0);
	        });

			conf.Run = (i =>
			{
				var c = ((TestConfiguration)i);
                foreach (var sequence in c.SequencesToAdd)
                {
                    c.Graph.AddSequence(sequence);
                }
            });
            
			conf.Benchmark("AddSequence", conf.ToString(), 5);
        }

        [Test, TestCaseSource(typeof(TestFactory), "TestCases")]
        [MaxTime(10000)]
        public void AddRangeTest(TestConfiguration conf)
        {
			conf.Prepare = (i =>
			{
				var c = ((TestConfiguration)i);
				c.SequencesToAdd = GenerateSequencesToAdd(conf);
				c.Graph = GraphPerformanceTestHelper.CreateGraphInstance(conf.GraphType);
				Assert.IsTrue(c.Graph.CountNodes == 0);
			});

			conf.Run = (i =>
			{
				var c = ((TestConfiguration)i);
				c.Graph.AddSequences(c.SequencesToAdd);
			});

			conf.Benchmark("AddSequences", conf.ToString(), 5);
        }

		
		// TODO: Benchmark Run action was not defined. Not migrated in lack of action definition
		
		//[Test, TestCaseSource(typeof(TestFactory), "TestCases")]
		//[MaxTime(10000)]
		//public void GetNodesLevel0Test(TestConfiguration conf)
		//{
		//	var graph = GraphPerformanceTestHelper.CreateGraphInstance(conf.GraphType);

		//	Assert.IsTrue(graph.CountNodes == 0);

		//	GraphPerformanceTestHelper.FillGraph(graph, conf);
		//	for (var i = 0; i < 10; i++)
		//	{
		//		var result = graph.GetNodes(i).ToArray();
		//	}
		//	var sb = new StringBuilder();
		//	foreach (var node in graph.Sort().OrderBy(x=>x.Level))
		//	{
		//		sb.AppendFormat("{0}: level {1} \n", node.Value, node.Level);
		//	}
		//}

        [Test, TestCaseSource(typeof(TestFactory), "TestCases")]
        [MaxTime(10000)]
        public void GetNodesMiddleTest(TestConfiguration conf)
        {
			// Simplified Prepare/Run version, no prepare because the Benchmark preparation data is read only:
            var graph = GraphPerformanceTestHelper.CreateGraphInstance(conf.GraphType);
            Assert.IsTrue(graph.CountNodes == 0);
            GraphPerformanceTestHelper.FillGraph(graph, conf);

            conf.Run = (c => graph.GetNodes(conf.Levels / 2).Enumerate());
			conf.Benchmark("GetNodesMiddle", conf.ToString(), 5);
        }

        [Test, TestCaseSource(typeof(TestFactory), "TestCases")]
        [MaxTime(10000)]
        public void GetNodesLevelMaxTest(TestConfiguration conf)
        {
			// Simplified Prepare/Run version, no prepare because the Benchmark preparation data is read only:
            var graph = GraphPerformanceTestHelper.CreateGraphInstance(conf.GraphType);
            Assert.IsTrue(graph.CountNodes == 0);
            GraphPerformanceTestHelper.FillGraph(graph, conf);

			conf.Run = (c => graph.GetNodes(conf.Levels - 1).Enumerate());
			conf.Benchmark("GetNodesLevelMax", conf.ToString(), 5);
        }

        [Test, TestCaseSource(typeof(TestFactory), "TestCases")]
        [MaxTime(10000)]
        public void GetNodesBetweenTest(TestConfiguration conf)
        {
			// Simplified Prepare/Run version, no prepare because the Benchmark preparation data is read only:
            var graph = GraphPerformanceTestHelper.CreateGraphInstance(conf.GraphType);
            Assert.IsTrue(graph.CountNodes == 0);
            GraphPerformanceTestHelper.FillGraph(graph, conf);
            var middleLevel = conf.Levels/2;

			conf.Run = (c => graph.GetNodesBetween(middleLevel - 1, middleLevel + 1).Enumerate());
			conf.Benchmark("GetNodesBetween", conf.ToString(), 5);
        }

        [Test, TestCaseSource(typeof(TestFactory), "TestCases")]
        [MaxTime(10000)]
        public void GetNodesBetween0AndMax(TestConfiguration conf)
        {
			// Simplified Prepare/Run version, no prepare because the Benchmark preparation data is read only:
            var graph = GraphPerformanceTestHelper.CreateGraphInstance(conf.GraphType);
            Assert.IsTrue(graph.CountNodes == 0);
            GraphPerformanceTestHelper.FillGraph(graph, conf);

			conf.Run = (c => graph.GetNodesBetween(0, conf.Levels - 1).Enumerate());
			conf.Benchmark("GetNodesBetween0AndMax", conf.ToString(), 5);
            
            var count = graph.GetNodesBetween(0, conf.Levels - 1).Count();
            Assert.IsTrue(count == graph.CountNodes);
        }

        [Test, TestCaseSource(typeof(TestFactory), "TestCases")]
        [MaxTime(10000)]
        public void SortTest(TestConfiguration conf)
        {
			// Simplified Prepare/Run version, no prepare because the Benchmark preparation data is read only:
            var graph = GraphPerformanceTestHelper.CreateGraphInstance(conf.GraphType);
            Assert.IsTrue(graph.CountNodes == 0);
            GraphPerformanceTestHelper.FillGraph(graph, conf);

			conf.Run = (c => graph.Sort().Enumerate());
			conf.Benchmark("Sort", conf.ToString(), 5);
        }

        [Test, TestCaseSource(typeof(TestFactory), "TestCases")]
        [MaxTime(10000)]
        public void CanSortTest(TestConfiguration conf)
        {
			// Simplified Prepare/Run version, no prepare because the Benchmark preparation data is read only:
            var graph = GraphPerformanceTestHelper.CreateGraphInstance(conf.GraphType);
            Assert.IsTrue(graph.CountNodes == 0);
            GraphPerformanceTestHelper.FillGraph(graph, conf);

			conf.Run = (c => graph.CanSort(new[] { 1, 2 }));
			conf.Benchmark("CanSort", conf.ToString(), 5);
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
