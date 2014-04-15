using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fasterflect;
using Orc.DependencyGraph;

namespace Orc.DependencyGraphPerformanceTests
{
    static internal class GraphPerformanceTestHelper
    {
        public static IGraph<int> CreateGraphInstance(Type type)
        {
            return type.MakeGenericType(typeof(int)).CreateInstance() as IGraph<int>;
        }

        public static void FillGraph(IGraph<int> graph, TestConfiguration conf)
        {
            if (String.IsNullOrEmpty(conf.TestFile))
            {
                GraphGenerator.FillGraph(graph, conf.Levels, conf.Descendants, conf.Precedents);
                return;
            }
            var sequencesToAdd = LoadSequencesFromFile(conf.TestFile);
            graph.AddSequences(sequencesToAdd);
        }

        private static int[][] LoadSequencesFromFile(string testFile)
        {
            var reader = new StreamReader(File.OpenRead(@"PredefinedSequences\" + testFile));
            var result = new List<int[]>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',').ToArray();
                result.Add(new[]
                {
                    Convert.ToInt32(values[0]),
                    Convert.ToInt32(values[1])
                });
            }
            return result.ToArray();
        }
    }
}