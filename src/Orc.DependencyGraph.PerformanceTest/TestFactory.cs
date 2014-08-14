using System;
using System.Collections.Generic;
using System.Linq;
using Orc.DependencyGraph;

namespace Orc.DependencyGraphPerformanceTests
{
	using NUnitBenchmarker;

	public class TestFactory
    { 
        private List<Type> Types { get; set; } 

        public TestFactory()
        {
			// See app.config:
			Types = Benchmarker.GetImplementations(typeof(IGraph<>), true).ToList();
			
			//// FindImplementations classes that implement IGraph
			//FindImplementations();
        }

		//private void FindImplementations()
		//{
		//	Types = Assembly.Load("Orc.DependencyGraph")
		//		.Types()
		//		.Where(x => x.IsClass && x.Implements(typeof(IGraph<>)))
		//		.Select(x => x).ToList();
		//}

        public IEnumerable<TestConfiguration> TestCases()
        {
            for (var index = 0; index < Types.Count; index++)
            {
                var type = Types[index];
                var identifier = string.Format("{0} : {1}", type.Namespace, type.Name);

                // TargetImplementationType property must have value:
				yield return new TestConfiguration(identifier, type, 10, 1, 1) {TargetImplementationType = type};
				yield return new TestConfiguration(identifier, type, 100, 1, 1) { TargetImplementationType = type };
				yield return new TestConfiguration(identifier, type, 1000, 1, 1) { TargetImplementationType = type };
				yield return new TestConfiguration(identifier, type, 2000, 1, 1) { TargetImplementationType = type };
				yield return new TestConfiguration(identifier, type, 4000, 1, 1) { TargetImplementationType = type };
				yield return new TestConfiguration(identifier, type, 10, 4, 2) { TargetImplementationType = type };
				yield return new TestConfiguration(identifier, type, 10, 2, 4) { TargetImplementationType = type };
				// TODO: Comment back in case you would like to see results for 10000
				//yield return new TestConfiguration(identifier, type, "10000_10_10") { TargetImplementationType = type };
            }
        }
    }
}