using System;

namespace Orc.DependencyGraph
{
    /*
     * Precedents   - parents
     * Descendants  - children
     */
    public interface INode<T>
        where T: IEquatable<T>
    {
        /// <summary>
        /// The value associated with the node
        /// </summary>
        T Value { get; }

        /// <summary>
        /// A topological level of the node.
        /// I.e. level 1 consists of nodes whose Precedents are of Level 0.
        /// In general level is the longest path from the node to the root of the graph.
        /// </summary>
        int Level { get; }

        //  relativeLevel >= relativeLevelFrom && relativeLevel <= relativeLevelTo       
        /// <summary>
        /// Find neighbors related to the node.
        /// </summary>
        /// <param name="relativeLevelFrom">A value that defines a start of relative level range of nodes to search for</param>
        /// <param name="relativeLevelTo">A value that defines an end of relative level range of nodes to search for</param>
        /// <returns>An IOrderedEnumerable whose elements are sorted in topological order</returns>
        IOrderedEnumerable<INode<T>> GetNeighbours(int relativeLevelFrom, int relativeLevelTo);

        //  relativeLevel < 0
        /// <summary>
        /// Finds precedents of the node.
        /// </summary>
        IOrderedEnumerable<INode<T>> Precedents { get; }

        //  relativeLevel > 0
        /// <summary>
        /// Finds Descendants of the node.
        /// </summary>
        IOrderedEnumerable<INode<T>> Descendants { get; }

        /// <summary>
        /// Finds immediate precedents of the node (i.e. parents)
        /// </summary>
        IOrderedEnumerable<INode<T>> ImmediatePrecedents { get; }

        /// <summary>
        /// Finds immediate descendants (i.e. children)
        /// </summary>
        IOrderedEnumerable<INode<T>> ImmediateDescendants { get; }

        //  Precedents of the node which don't have precedents (i.e. roots)
        /// <summary>
        /// Finds precedents on the first level of the graph.
        /// </summary>
        IOrderedEnumerable<INode<T>> TerminatingPrecedents { get; }

        //  Descendants of the node which don't have descendants (i.e. leafs)
        /// <summary>
        /// Finds descendants on the last level of the graph
        /// </summary>
        IOrderedEnumerable<INode<T>> TerminatingDescendants { get; }
    }
}