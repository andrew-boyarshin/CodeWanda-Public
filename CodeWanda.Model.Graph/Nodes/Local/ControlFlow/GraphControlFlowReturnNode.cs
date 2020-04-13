using System;
using CodeWanda.Model.Semantic.Expressions;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.ControlFlow
{
    public sealed class GraphControlFlowReturnNode : GraphExpressionNodeBase
    {
        public bool Synthetic { get; }
        
        public GraphControlFlowReturnNode(GraphMethodRoot methodRoot, [CanBeNull] ExpressionBase expression, bool synthetic)
            : base(methodRoot, expression)
        {
            Synthetic = synthetic;
        }

        public override void ReplaceOutgoingNode(GraphNode oldNode, GraphNode newNode) =>
            throw new InvalidOperationException();
    }
}