namespace Orc.DependencyGraphPerformanceTests
{
	#region using...

	using System;
	using System.Linq;
	using DependencyGraph;
	using Fasterflect;
	using NUnit.Framework;
	using NUnitBenchmarker;

	#endregion

	[TestFixture, RequiresSTA]
	public class NodePerformanceTest
	{
		[TestFixtureSetUp]
		public void TestFixture()
		{
			Benchmarker.Init();
		}


		private static IGraph<int> CreateGraphInstance(Type type)
		{
			return type.MakeGenericType(typeof (int)).CreateInstance() as IGraph<int>;
		}

		[Test, TestCaseSource(typeof (TestFactory), "TestCases")]
		public void NodePerformanceTests(TestConfiguration conf)
		{
			var graph = CreateGraphInstance(conf.GraphType);
				GraphPerformanceTestHelper.FillGraph(graph, conf);

			// Original 'Get Neighbours' was:
			////// Get Neighbours
			////var node = graph.GetNodes(conf.Levels / 2).First();
			////var action = new Action(() => node.GetNeighbours(-1, 1).Enumerate());
			////action.Benchmark(conf, "Node_GetNeighbours", conf.ToString());


			// The full alternative for simplified Benchmark would be the following sample
			// As all these Benchmarks (the Run action) uses the preparation in _read_ only mode
			// the new API conf.Prepare action is not mandatory, see simplified version

			// Get Neighbours
			////conf.Prepare = (c => ((TestConfiguration)c).Node = graph.GetNodes(conf.Levels / 2).First());
			////conf.Run = (c => ((TestConfiguration)c).Node.GetNeighbours(-1, 1).Enumerate());
			////conf.IsReusable = true;
			////conf.Benchmark("Node_GetNeighbours", conf.ToString(), 5);


			// Simplified version: conf.Prepare remains null and using local variables for 
			// the communication with the Run action (the 'node' variable)
			// Get Neighbours
			var node = graph.GetNodes(conf.Levels/2).First();
			conf.Run = (c => node.GetNeighbours(-1, 1).Enumerate());
			conf.Benchmark("Node_GetNeighbours", conf.ToString(), 5);

			// Get Descendants
			node = graph.Find(0);
			conf.Run = (c => node.Descendants.Enumerate());
			conf.Benchmark("Node_Descendants", conf.ToString(), 5);

			// Get Precedents
			node = graph.Find(graph.CountNodes - 1);
			conf.Run = (c => node.Precedents.Enumerate());
			conf.Benchmark("Node_Precedents", conf.ToString(), 5);

			// Get Terminating Descendants
			node = graph.Find(1);
			conf.Run = (c => node.TerminatingDescendants.Enumerate());
			conf.Benchmark("Node_TerminatingDescendants", conf.ToString(), 5);

			// Get Terminating Precedents
			node = graph.Find(graph.CountNodes - 1);
			conf.Run = (c => node.TerminatingPrecedents.Enumerate());
			conf.Benchmark("Node_TerminatingPrecedents", conf.ToString(), 5);

			// Find middle
			conf.Run = (c => graph.Find(graph.CountNodes/2));
			conf.Benchmark("Node_FindMiddle", conf.ToString(), 5);

			// Find first
			conf.Run = (c => graph.Find(0));
			conf.Benchmark("Node_FindFirst", conf.ToString(), 5);

			// Find Last
			conf.Run = (c => graph.Find(graph.CountNodes - 1));
			conf.Benchmark("Node_FindLast", conf.ToString(), 5);
		}
	}
}