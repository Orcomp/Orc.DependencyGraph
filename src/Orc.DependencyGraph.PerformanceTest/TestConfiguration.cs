using System;


namespace Orc.DependencyGraphPerformanceTests
{
	using DependencyGraph;
	using NUnitBenchmarker.Configuration;

	public class TestConfiguration : PerformanceTestCaseConfigurationBase
    {
        public string TestFile { get; set; }
        public int Levels { get; set; }
        public int Descendants { get; set; }
        public int Precedents { get; set; }
        public Type GraphType { get; set; }
		public int[][] SequencesToAdd { get; set; }
		public IGraph<int> Graph { get; set; }

		public TestConfiguration(string identifier, Type graphType, int levels, int descendants, int precedents, bool last = false)
        {
			Identifier = identifier;
            GraphType = graphType;
            Levels = levels;
            Descendants = descendants;
            Precedents = precedents;
        }
        public TestConfiguration(string identifier, Type graphType, string testFile, bool last = false)
        {
            Identifier = identifier;
            GraphType = graphType;
            TestFile = testFile;

            var values = testFile.Split('_');
            Levels = int.Parse(values[0]);
            Descendants = int.Parse(values[1]);
            Precedents = int.Parse(values[2]);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(TestFile))
                return string.Format("{0}_{1}_{2}", Levels, Descendants, Precedents);
            return TestFile;
        }
    }
}