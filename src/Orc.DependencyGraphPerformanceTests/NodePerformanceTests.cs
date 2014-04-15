using System;
using System.Linq;
using Fasterflect;
using NUnitBenchmarker.Benchmark;

namespace Orc.DependencyGraphPerformanceTests
{
    using NUnit.Framework;
    using Orc.DependencyGraph;

    [TestFixture, RequiresSTA]
    public class NodePerformanceTest
    {
        [Test, TestCaseSource(typeof(TestFactory), "TestCases")]
        public void NodePerformanceTests(TestConfiguration conf)
        {
            var graph = CreateGraphInstance(conf.GraphType);

            GraphPerformanceTestHelper.FillGraph(graph, conf);

            // Get Neighbours
            var node = graph.GetNodes(conf.Levels / 2).First();
            var action = new Action(() => node.GetNeighbours(-1, 1).Enumerate());
            action.Benchmark(conf, "Node_GetNeighbours", conf.ToString());

            // Get Descendants
            node = graph.Find(0);
            action = new Action(() => node.Descendants.Enumerate());
            action.Benchmark(conf, "Node_Descendants", conf.ToString());

            // Get Precedents
            node = graph.Find(graph.CountNodes - 1);
            action = new Action(() => node.Precedents.Enumerate());
            action.Benchmark(conf, "Node_Precedents", conf.ToString());

            // Get Terminating Descendants
            node = graph.Find(1);
            action = new Action(() => node.TerminatingDescendants.Enumerate());
            action.Benchmark(conf, "Node_TerminatingDescendants", conf.ToString());

            // Get Terminating Precedents
            node = graph.Find(graph.CountNodes - 1);
            action = new Action(() => node.TerminatingPrecedents.Enumerate());
            action.Benchmark(conf, "Node_TerminatingPrecedents", conf.ToString());

            // Find middle
            action = new Action(() => graph.Find(graph.CountNodes/2));
            action.Benchmark(conf, "Node_FindMiddle", conf.ToString());

            // Find first
            action = new Action(() => graph.Find(0));
            action.Benchmark(conf, "Node_FindFirst", conf.ToString());

            // Find Last
            action = new Action(() => graph.Find(graph.CountNodes - 1));
            action.Benchmark(conf, "Node_FindLast", conf.ToString());
        }


        private static IGraph<int> CreateGraphInstance(Type type)
        {
            return type.MakeGenericType(typeof(int)).CreateInstance() as IGraph<int>;
        } 

    }
}
