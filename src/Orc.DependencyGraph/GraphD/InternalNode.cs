namespace Orc.DependencyGraph.GraphD
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    [DebuggerDisplay("{Value.ToString()}: [{(this as IInternalNode<T>).PrintImmediatePrecedents}]")]
    internal class InternalNode<T>
        : IInternalNode<T> where T : IEquatable<T>
    {
        internal InternalNode(T publicNode, Graph<T> graph)
        {
            this.Edges = new List<InternalNode<T>>();
            this.Parents = new List<InternalNode<T>>(); 

            this.Value = publicNode;
            this._graph = graph;
        }

        public List<InternalNode<T>> Edges { get; private set; }
        public List<InternalNode<T>> Parents { get; private set; }
        IEnumerable<IInternalNode<T>> IInternalNode<T>.Edges { get { return this.Edges; } }
        IEnumerable<IInternalNode<T>> IInternalNode<T>.Parents { get { return this.Parents; } } 

        public T Value { get; internal set; }

        public IGraph<T> Graph
        {
            get { return this._graph; }
        }

        public int Level
        {
            get { return this._graph.ReferencePoint + this.ReferenceRelativeLevel; }
        }

        private readonly Graph<T> _graph;

        public int ReferenceRelativeLevel { get; set; }

        public DependencyGraph.IOrderedEnumerable<INode<T>> GetNeighbours(int relativeLevelFrom, int relativeLevelTo)
        {
            return new OrderedEnumerable<INode<T>>(() => this.GetNeighboursInternal(relativeLevelFrom, relativeLevelTo));
        }
        private IEnumerable<INode<T>> GetNeighboursInternal(int relativeLevelFrom, int relativeLevelTo)
        {
            if (relativeLevelFrom < 0 && relativeLevelTo < 0)
                return this._graph.GetPrecedents(this, (_ => _ != this && _.Level >= this.Level + relativeLevelFrom && _.Level <= this.Level + relativeLevelTo)).OrderBy(_ => _.Level);
            else if (relativeLevelFrom > 0 && relativeLevelTo > 0)
            {
                return this._graph.GetDescendants(this, (_ => _ != this && _.Level >= this.Level + relativeLevelFrom && _.Level <= this.Level + relativeLevelTo)).OrderBy(_ => _.Level);
            }
            else
            {
                var precedents = this._graph.GetPrecedents(this, (_ => _ != this && _.Level >= this.Level + relativeLevelFrom && _.Level <= this.Level + relativeLevelTo));
                var descendants = this._graph.GetDescendants(this, (_ => _ != this && _.Level >= this.Level + relativeLevelFrom && _.Level <= this.Level + relativeLevelTo));

                return precedents.Union(descendants).OrderBy(_ => _.Level);
            }
        }

        public DependencyGraph.IOrderedEnumerable<INode<T>> Precedents
        {
            get
            {
                return new OrderedEnumerable<INode<T>>(
                    () => this._graph.GetPrecedents(this, (_ => _.Level < this.Level)).OrderBy(_ => _.Level));
            }
        }

        public DependencyGraph.IOrderedEnumerable<INode<T>> Descendants
        {
            get
            {
                return new OrderedEnumerable<INode<T>>(() => this._graph.GetDescendants(this, (_ => _.Level > this.Level)).OrderBy(_ => _.Level));
            }
        }

        public DependencyGraph.IOrderedEnumerable<INode<T>> ImmediatePrecedents
        {
            get
            {
                return new OrderedEnumerable<INode<T>>(() => this.Parents.OrderBy(x => x.Level));
            }
        }

        public DependencyGraph.IOrderedEnumerable<INode<T>> ImmediateDescendants
        {
            get
            {
                return new OrderedEnumerable<INode<T>>(() => this.Edges.OrderBy(x => x.Level));
            }
        }

        public DependencyGraph.IOrderedEnumerable<INode<T>> TerminatingPrecedents
        {
            get
            {
                return new OrderedEnumerable<INode<T>>(() => this._graph.GetPrecedents(this, (_ => _.Level < this.Level && !_.Parents.Any())).OrderBy(_ => _.Level));
            }
        }

        public DependencyGraph.IOrderedEnumerable<INode<T>> TerminatingDescendants
        {
            get
            {
                return new OrderedEnumerable<INode<T>>(() => this._graph.GetDescendants(this, (_ => _.Level > this.Level && !_.Edges.Any())).OrderBy(_ => _.Level));
            }
        }

        string IInternalNode<T>.PrintImmediatePrecedents
        {
            get
            {
                if (!this.Edges.Any()) return string.Empty;
                var sb = new StringBuilder();
                foreach (var immediatePrecedents in this.Edges)
                {
                    sb.Append(immediatePrecedents.Value);
                    sb.Append(", ");
                }
                return sb.Remove(sb.Length-2,2).ToString();
            }
        }
    }
}