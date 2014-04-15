using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect;
using Orc.DependencyGraph;

namespace Orc.DependencyGraphPerformanceTests
{
    public class TestFactory
    { 
        private List<Type> Types { get; set; } 

        public TestFactory()
        {
            // FindImplementations classes that implement IGraph
            this.FindImplementations();
        }

        private void FindImplementations()
        {
            Types = Assembly.Load("Orc.DependencyGraph")
                .Types()
                .Where(x => x.IsClass && TypeExtensions.Implements(x, typeof(IGraph<>)))
                .Select(x => x).ToList();
        }

        public IEnumerable<TestConfiguration> TestCases()
        {
            for (var index = 0; index < Types.Count; index++)
            {
                var type = Types[index];
                var identifier = string.Format("{0} : {1}", type.Namespace, type.Name);

                yield return new TestConfiguration(identifier, type, 10, 1, 1);
                yield return new TestConfiguration(identifier, type, 100, 1, 1);
                yield return new TestConfiguration(identifier, type, 1000, 1, 1);
                yield return new TestConfiguration(identifier, type, 2000, 1, 1);
                yield return new TestConfiguration(identifier, type, 4000, 1, 1);
                yield return new TestConfiguration(identifier, type, 10, 4, 2);
                yield return new TestConfiguration(identifier, type, 10, 2, 4);
                yield return new TestConfiguration(identifier, type, "10000_10_10");
            }
        }
    }
}