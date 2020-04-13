using System;
using System.Collections.Generic;
using CodeWanda.Model.Graph.Nodes.Local;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.StrongConnectivity
{
    public class ComponentNodeGroup
    {
        [NotNull] [ItemNotNull] private readonly List<GraphNode> _nodes = new List<GraphNode>();

        [NotNull] [ItemNotNull] public IReadOnlyList<GraphNode> Nodes => _nodes;

        [CanBeNull] public GraphNode[] RootIncomingPath { get; set; }

        public ComponentNodeGroup([NotNull] GraphNode root)
        {
            Root = root ?? throw new ArgumentNullException(nameof(root));
            Root.MethodRoot.AllGroups.Add(this);
        }

        public void Add(GraphNode node)
        {
            _nodes.Add(node);
        }

        [NotNull] public GraphNode Root { get; }
    }
}