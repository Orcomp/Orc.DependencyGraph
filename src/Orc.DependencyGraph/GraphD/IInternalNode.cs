namespace Orc.DependencyGraph.GraphD
{
    using System;
    using System.Collections.Generic;

    public interface IInternalNode<T>
        : INode<T> where T : IEquatable<T>
    {
        IEnumerable<IInternalNode<T>> Parents { get; }
        IEnumerable<IInternalNode<T>> Edges { get; }
        string PrintImmediatePrecedents { get; }
    }
}