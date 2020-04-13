using System;
using System.Collections.Generic;
using CodeWanda.Model.Graph.Nodes.Local;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Call
{
    public class CallGraphMethodNode : CallGraphNode
    {
        public CallGraphMethodNode([NotNull] GraphMethodRoot methodRoot)
        {
            MethodRoot = methodRoot ?? throw new ArgumentNullException(nameof(methodRoot));
        }

        [NotNull] public GraphMethodRoot MethodRoot { get; }

        [NotNull]
        [ItemNotNull]
        public List<CallGraphInvocationNode> OutgoingNodes { get; } = new List<CallGraphInvocationNode>();
    }
}