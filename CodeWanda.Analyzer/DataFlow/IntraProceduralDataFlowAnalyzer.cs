using System;
using System.Collections.Generic;
using CodeWanda.Model.Graph.Nodes.Local;
using CodeWanda.Model.Graph.Nodes.Local.ControlFlow;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow;

namespace CodeWanda.Analyzer.DataFlow
{
    public partial class IntraProceduralDataFlowAnalyzer
    {
        private GraphMethodRoot MethodRoot { get; }
        private readonly HashSet<GraphNode> _nodes = new HashSet<GraphNode>();

        public IntraProceduralDataFlowAnalyzer(GraphMethodRoot methodRoot)
        {
            MethodRoot = methodRoot;
        }

        public void Analyze()
        {
            _nodes.Clear();
            AssignParentsToContexts(MethodRoot);
            switch (MethodRoot.Start)
            {
                case null:
                    throw new ArgumentNullException(nameof(MethodRoot.Start));
                case GraphControlFlowBlockNode _:
                    goto default;
                case GraphDataFlowConditionalNode graphConditionalNode:
                    CreateMethodParameterVariables(graphConditionalNode.EnterAnalysisContext);
                    break;
                case GraphDataFlowReturnNode graphReturnNode:
                    CreateMethodParameterVariables(graphReturnNode.EnterAnalysisContext);
                    break;
                case GraphDataFlowExpressionNode _:
                    goto default;
                case GraphExpressionNodeBase _:
                    goto default;
                case GraphDataFlowBlockNode graphDataFlowBlockNode:
                    CreateMethodParameterVariables(graphDataFlowBlockNode.Body[0].EnterAnalysisContext);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ResolveStrongConnectivity();

            ProcessNode(MethodRoot.Start, new[] {MethodRoot.Start});

            foreach (var componentNodeGroup in MethodRoot.AllGroups)
            {
                for (var i = 0; i < 20; i++)
                {
                    _nodes.ExceptWith(componentNodeGroup.Nodes);
                    var incomingPath = componentNodeGroup.RootIncomingPath ?? Array.Empty<GraphNode>();
                    ProcessNode(componentNodeGroup.Root, incomingPath, componentNodeGroup);
                }
            }
        }
    }
}