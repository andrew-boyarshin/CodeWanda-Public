using System;
using CodeWanda.Model.Graph.Nodes.Local;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Call
{
    public class CallGraphInvocationNode : CallGraphNode
    {
        public CallGraphInvocationNode([NotNull] GraphExpressionNodeBase controlFlowGraphNode)
        {
            ControlFlowGraphNode =
                controlFlowGraphNode ?? throw new ArgumentNullException(nameof(controlFlowGraphNode));
        }

        [NotNull] public GraphExpressionNodeBase ControlFlowGraphNode { get; }
        public CallGraphMethodNode Target { get; set; }
    }
}