using System;

namespace Orc.DependencyGraph
{
    public class TopologicalSortException : Exception
    {
        public TopologicalSortException(string exceptionMessage)
            :base(exceptionMessage)
        {
        }
    }
}