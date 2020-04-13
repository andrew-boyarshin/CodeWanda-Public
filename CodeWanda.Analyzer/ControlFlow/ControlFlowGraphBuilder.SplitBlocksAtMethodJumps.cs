using System;
using System.Linq;
using CodeWanda.Model.Graph;
using CodeWanda.Model.Graph.Nodes.Local;
using CodeWanda.Model.Graph.Nodes.Local.ControlFlow;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Assignments;
using CodeWanda.Model.Semantic.Expressions.Operators.Other;

namespace CodeWanda.Analyzer.ControlFlow
{
    public partial class ControlFlowGraphBuilder
    {
        private void SplitBlocksAtMethodJumps(GraphMethodRoot methodRoot)
        {
            var graphNodes = methodRoot.Start.IterateGraphNodesRecursive(true).ToArray();
            foreach (var graphNode in graphNodes)
            {
                switch (graphNode)
                {
                    case null:
                        throw new ArgumentNullException(nameof(graphNode));

                    case GraphControlFlowBlockNode graphBlockNode:
                    {
                        var currentBlockNode = graphBlockNode;
                        while (currentBlockNode != null)
                        {
                            var index = 0;
                            for (; index < currentBlockNode.Body.Count; index++)
                            {
                                var expressionBase = currentBlockNode.Body[index];
                                var functionCall = ScanExpressionForFunctionCalls(expressionBase.Expression);
                                if (functionCall == null)
                                    continue;

                                if (index == currentBlockNode.Body.Count - 1)
                                    continue;

                                // split
                                var node = new GraphControlFlowBlockNode(methodRoot)
                                {
                                    Outgoing = currentBlockNode.Outgoing
                                };

                                var moveStart = index + 1;
                                var moveCount = currentBlockNode.Body.Count - moveStart;
                                
                                foreach (var expressionNode in currentBlockNode.Body.ToArray()[moveStart..(moveStart+moveCount)])
                                {
                                    node.Body.Add(expressionNode);
                                }

                                for (var i = 0; i < moveCount; i++)
                                {
                                    currentBlockNode.Body.RemoveAt(moveStart);
                                }
                                
                                currentBlockNode.Outgoing = node;

                                currentBlockNode = node;
                                break;
                            }

                            if (index == currentBlockNode.Body.Count)
                                break;
                        }

                        break;
                    }

                    case GraphControlFlowConditionalNode _:
                    case GraphControlFlowReturnNode _:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(graphNode));
                }
            }

            static FunctionCallOperator ScanExpressionForFunctionCalls(IValue expressionBase) =>
                expressionBase switch
                {
                    AssignmentBase assignmentBase => ScanExpressionForFunctionCalls(assignmentBase.Right),
                    FunctionCallOperator functionCallOperator => functionCallOperator,
                    _ => null
                };
        }
    }
}