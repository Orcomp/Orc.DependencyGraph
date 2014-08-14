using System.Collections.Generic;

namespace Orc.DependencyGraphPerformanceTests
{
    public static class PerformanceTestHelper
    {
        public static void Enumerate<T>(this IEnumerable<T> enumerable)
        {
            foreach (var item in enumerable) ;
        }
    }
}