using System;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context;
using CodeWanda.Model.Semantic.Expressions;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow
{
    public sealed class GraphDataFlowExpressionNode : GraphDataFlowExpressionNodeBase
    {
        [NotNull] public GraphDataFlowBlockNode BlockNode { get; }
        [NotNull] public AnalysisContext ExitAnalysisContext { get; }

        public GraphDataFlowExpressionNode(
            GraphMethodRoot methodRoot,
            [CanBeNull] ExpressionBase expression,
            [NotNull] GraphDataFlowBlockNode blockNode
        ) : base(methodRoot, expression)
        {
            BlockNode = blockNode;
            ExitAnalysisContext = new AnalysisContext(EnterAnalysisContext);
            
            SubscribeToAnalysisContextChanges(ExitAnalysisContext);
            
            RefreshText();
        }

        public override void ReplaceOutgoingNode(GraphNode oldNode, GraphNode newNode) =>
            throw new InvalidOperationException();
    }
}