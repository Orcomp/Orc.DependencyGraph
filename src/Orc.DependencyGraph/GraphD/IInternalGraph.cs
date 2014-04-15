namespace Orc.DependencyGraph.GraphD
{
    using System;
    using System.Collections.Generic;

    public interface IInternalGraph<T> : IGraph<T> where T : IEquatable<T>
    {
        IEnumerable<IInternalNode<T>[]> Edges { get; }
        IEnumerable<IInternalNode<T>[]> BackEdges { get; }
        IInternalNode<T> GetOrCreateNode(T publicNode);
        void ToFile(string filePath);
    }
}