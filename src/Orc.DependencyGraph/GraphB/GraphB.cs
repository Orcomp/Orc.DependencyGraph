using System;
using System.Collections.Generic;
using System.Linq;

namespace Orc.DependencyGraph.GraphB
{
    public class GraphB<T> : Orc.Sort.TopologicalSort.TopologicalSort<T>, IGraph<T>
        where T : IEquatable<T>
    {
        protected IList<INode<T>> graphList;
        protected IList<INode<T>> graphSort;

        protected List<int> levelList;

        public GraphB()
            :this(false, false)
        {
        }

        public GraphB(bool usesPriority, bool usesTracking)
            : base(usesPriority, usesTracking)
        {
            this.graphList = new List<INode<T>>();
            this.graphSort = new List<INode<T>>();
            this.levelList = new List<int>();
        }

        public bool CanSort()
        {
            return this.Sort() != null;
        }

        public int CountNodes { get { return this.graphList.Count; } }

        public int CountLevels { get { return this.levelList.Max() + 1; } }

        public void AddSequence(IEnumerable<T> sequence)
        {
            base.Add(sequence);

            int node_level = 0;

            foreach (var node in sequence)
            {
                int key = NodeKey(node);

                if (this.levelList[key] == -1)
                {
                    this.levelList[key] = node_level;
                }
                else if (this.levelList[key] < node_level)
                {
                    int level_delta = node_level - this.levelList[key];
                    this.levelList[key] = node_level;

                    foreach (int dep in GetDependents(key, false, false))
                    {
                        this.levelList[dep] += level_delta;
                    }
                }

                node_level = this.levelList[key] + 1;
            }
        }

        public void AddSequences(IEnumerable<IEnumerable<T>> sequences)
        {
            foreach (var sequence in sequences)
            {
                this.AddSequence(sequence);
            }
        }

        public IEnumerable<INode<T>> Nodes { get; private set; }

        public INode<T> Find(T node)
        {
            int key;

            if (!nodesDict.TryGetValue(node, out key))
                return null;
            else
                return this.graphList[key];
        }

        public IOrderedEnumerable<INode<T>> GetNodes(int level)
        {
            return new OrderedEnumerable<INode<T>>(() => this.graphList.Where(n => n.Level == level));
        }

        public IOrderedEnumerable<INode<T>> GetNodesBetween(int levelFrom, int levelTo)
        {
            return new OrderedEnumerable<INode<T>>(() => this.graphList.Where(n => levelFrom <= n.Level && n.Level <= levelTo));
        }

        public IEnumerable<INode<T>> GetNodesRelatedTo(T node)
        {
            int key;
            if (!nodesDict.TryGetValue(node, out key))
                throw new ArgumentException("node note present in graph");

            var set = new SortedSet<int>();
            set.UnionWith(GetPrecedents(key, false, false));
            set.UnionWith(GetDependents(key, false, false));

            return set.Select(k => this.graphList[k]);
        }

        public IEnumerable<INode<T>> GetNodesRelatedTo(T node, int levelFrom, int levelTo)
        {
            return this.GetNodesRelatedTo(node).Where(n => levelFrom <= n.Level && n.Level <= levelTo);
        }

        public IOrderedEnumerable<INode<T>> Sort()
        {
            base.Sort();

            if (nodesSort == null)
            {
                return null;
            }
            else if (nodesSort.Count != this.graphSort.Count)
            {
                this.graphSort = nodesSort.Select(this.Find).ToList();
            }

            return new OrderedEnumerable<INode<T>>(() => this.graphSort);
        }

        protected int NodeKey(T node)
        {
            int key = base.NodeKey(node);

            if (this.graphList.Count <= key)
            {
                this.graphList.Add(new Node<T>(this, key));
                this.levelList.Add(-1);
            }

            return key;
        }

        public class Node<N> : INode<N>
            where N : IEquatable<N>
        {
            public Node(GraphB<N> graph, int index)
            {
                this.GraphB = graph;
                this.typedGraphList = graph.graphList;
                this.key = index;
            }

            private int key;
            private IList<INode<N>> typedGraphList;

            public N Value { get { return this.GraphB.nodesList[this.key]; } }

            public GraphB<N> GraphB { get; private set; }

            public int Level { get { return this.GraphB.levelList[this.key]; } }

            public IOrderedEnumerable<INode<N>> GetNeighbours(int relativeLevelFrom, int relativeLevelTo)
            {
                int levelFrom = this.Level + relativeLevelFrom;
                int levelTo = this.Level + relativeLevelTo;

                return this.GraphB.GetNodesBetween(levelFrom, levelTo);
            }

            // relativeLevel < 0
            public IOrderedEnumerable<INode<N>> Precedents
            {
                get
                {
                    return new OrderedEnumerable<INode<N>>(() => this.GraphB.GetPrecedents(this.key, false, false).Select(i => this.typedGraphList[i]));
                }
            }

            // relativeLevel > 0
            public IOrderedEnumerable<INode<N>> Descendants
            {
                get
                {
                    return new OrderedEnumerable<INode<N>>(() => this.GraphB.GetDependents(this.key, false, false).Select(i => this.typedGraphList[i]));
                }
            }

            // relativeLevel == relativeLevel - 1
            public IOrderedEnumerable<INode<N>> ImmediatePrecedents
            {
                get
                {
                    return new OrderedEnumerable<INode<N>>(() => this.GraphB.GetPrecedents(this.key, true, false).Select(i => this.typedGraphList[i]));
                }
            }

            // relativeLevel == relativeLevel + 1
            public IOrderedEnumerable<INode<N>> ImmediateDescendants
            {
                get
                {
                    return new OrderedEnumerable<INode<N>>(() => this.GraphB.GetDependents(this.key, true, false).Select(i => this.typedGraphList[i]));
                }
            }

            // relativeLevel == 0
            public IOrderedEnumerable<INode<N>> TerminatingPrecedents
            {
                get
                {
                    return new OrderedEnumerable<INode<N>>(() => this.GraphB.GetPrecedents(this.key, false, true).Select(i => this.typedGraphList[i]));
                }
            }

            // relativeLevel == this.Graph.CountLevel-1
            public IOrderedEnumerable<INode<N>> TerminatingDescendants
            {
                get
                {
                    return new OrderedEnumerable<INode<N>>(() => this.GraphB.GetDependents(this.key, false, true).Select(i => this.typedGraphList[i]));
                }
            }

            public INode<N> Next
            {
                get
                {
                    if (this.key + 1 >= this.typedGraphList.Count)
                        return null;
                    else
                        return this.typedGraphList[this.key + 1];
                }
            }

            public INode<N> Previous
            {
                get
                {
                    if (this.key - 1 < 0)
                        return null;
                    else
                        return this.typedGraphList[this.key - 1];
                }
            }

            public override string ToString()
            {
                return String.Format("Node({0},{1},{2},{3})", this.Value, this.Level, this.ImmediatePrecedents.Count(), this.ImmediateDescendants.Count());
            }
        }
    }
}