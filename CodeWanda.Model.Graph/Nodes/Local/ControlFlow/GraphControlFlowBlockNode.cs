namespace CodeWanda.Model.Graph.Nodes.Local.ControlFlow
{
    public sealed class GraphControlFlowBlockNode : GraphBlockNode<GraphControlFlowExpressionNode>
    {
	    public GraphControlFlowBlockNode([System.Diagnostics.CodeAnalysis.NotNull] GraphMethodRoot methodRoot) : base(methodRoot)
        {
        }
    }
}