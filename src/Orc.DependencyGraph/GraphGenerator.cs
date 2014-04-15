using System;
using System.Collections.Generic;
using System.Linq;

namespace Orc.DependencyGraph
{
    using Orc.DependencyGraph.GraphD;

    using Node = Orc.DependencyGraph.GraphD.IInternalNode<int>;

    internal static class GraphGenerator
    {
        public static IGraph<int> GenerateGraph(int levels, int descendants, int precedents)
        {
            var graph = new Graph<int>();
            FillGraph(graph, levels, descendants, precedents);
            return graph;
        }

        public static void FillGraph(IGraph<int> graph, int levels, int descendants, int precedents)
        {
            var nodes = new Dictionary<int, QueueElement>();
            ValidateParameters(descendants, precedents);

            var parentQueue = new Queue<QueueElement>();
            var childrenQueue = new LinkedList<QueueElement>();
            var rootNodeCount = NodesOnLevel(0, levels, descendants, precedents);
            for (var i = 0; i < rootNodeCount; i++) // add root's
                EnqueueParent(parentQueue, GetOrCreateNode(nodes, nodes.Count));

            var nodeCount = CalculateNodeCount(levels, descendants, precedents);
            while (graph.CountNodes < nodeCount || childrenQueue.Count != 0)
            {
                var parent = DequeueFreeParent(parentQueue, descendants);
                var nextChildNode = DequeueFreeChild(childrenQueue, parent, precedents);
                if (nextChildNode == null)
                {
                    nextChildNode = GetOrCreateNode(nodes, nodes.Count);
                    EnqueueParent(parentQueue, nextChildNode);
                    EnqueueChild(childrenQueue, nextChildNode, precedents);
                }
                parent.Edges.Add(nextChildNode.Node);

                graph.AddSequence(new[] { parent.Node, nextChildNode.Node });
            }
        }

        private static void EnqueueParent(Queue<QueueElement> parentQueue, QueueElement nextChildNode)
        {
            parentQueue.Enqueue(nextChildNode);
        }

        private static QueueElement DequeueFreeParent(Queue<QueueElement> parentQueue, int descendants)
        {
            var freeParent = parentQueue.Peek();
            if (++freeParent.DescendantCount == descendants)
                parentQueue.Dequeue();
            return freeParent;
        }

        private static void EnqueueChild(LinkedList<QueueElement> childrenQueue, QueueElement nextChildNode, int precedents)
        {
            if (precedents > 1)
            {
                nextChildNode.PrecedentCount = 1;
                childrenQueue.AddLast(nextChildNode);
            }
        }

        private static QueueElement DequeueFreeChild(LinkedList<QueueElement> childrenQueue, QueueElement parentNode, int precedents)
        {
            QueueElement freeChild = childrenQueue
                .FirstOrDefault(childNode =>
                    parentNode.Edges.Count(_ => _ == childNode.Node) <= 0);
            if (freeChild == null) return null;
            if (++freeChild.PrecedentCount == precedents)
                childrenQueue.Remove(freeChild);
            return freeChild;
        }

        internal static int CalculateNodeCount(int levels, int descendants, int precendents)
        {
            var min = Math.Min(descendants, precendents);
            var max = Math.Max(descendants, precendents);
            if (max%min != 0) throw new ArgumentException();

            var growth = max / min;
            var result = 0;
            var nodesOnLevel = new Func<int, int>(level =>
            {
                return (int)(min * Math.Pow(growth, levels - level - 1));
            });
            for (var i = 0; i < levels; i++)
                result += nodesOnLevel(i);
            
            return result;
        }

        internal static int NodesOnLevel(int level, int levels, int descendants, int precendents)
        {
            var min = Math.Min(descendants, precendents);
            var max = Math.Max(descendants, precendents);
            var growth = max / min;
            if (descendants > precendents)
                return (int)(min * Math.Pow(growth, level));
            else
                return (int)(min * Math.Pow(growth, levels - level - 1));
        }

        private static void ValidateParameters(int descendantCount, int precedentCount)
        {
            var min = Math.Min(descendantCount, precedentCount);
            var max = Math.Max(descendantCount, precedentCount);
            if (max%min != 0)
                throw new ArgumentException("One of the parameters have to be multiple of another one.");
        }
        private static QueueElement GetOrCreateNode(Dictionary<int, QueueElement> dictionary, int key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            var node = new QueueElement(key, 0, 0);
            dictionary.Add(key, node);
            return node;
        }
        private class QueueElement
        {
            public int Node { get; set; }
            public int DescendantCount { get; set; }
            public int PrecedentCount { get; set; }
            public IList<int> Edges { get; private set; }

            public QueueElement(int node, int descendantCount, int precedentCount)
            {
                Edges = new List<int>();
                Node = node;
                DescendantCount = descendantCount;
                PrecedentCount = precedentCount;
            }
        }

    }
}