using System;
using System.Collections.Generic;
using System.Linq;
using CodeWanda.Model.Graph;
using CodeWanda.Model.Graph.Nodes.Call;
using CodeWanda.Model.Graph.Nodes.Local;
using CodeWanda.Model.Graph.Nodes.Local.ControlFlow;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow;
using CodeWanda.Model.Semantic.Expressions.Operators.Other;
using CodeWanda.Model.Semantic.Utilities;
using JetBrains.Annotations;
using Microsoft.Collections.Extensions;
using Serilog;

namespace CodeWanda.Analyzer.ControlFlow
{
    public class CallGraphBuilder
    {
        public static readonly CallGraphBuilder Instance = new CallGraphBuilder();

        [NotNull] private readonly IDictionary<string, CallGraphMethodNode> _methodNodes =
            new Dictionary<string, CallGraphMethodNode>();

        private readonly MultiValueDictionary<string, CallGraphInvocationNode> _callRegistrationQueue
            = new MultiValueDictionary<string, CallGraphInvocationNode>();

        private CallGraphBuilder()
        {
        }

        public bool HasPendingInvocationNodesInQueue => _callRegistrationQueue.Count > 0;

        public void RegisterMethod(GraphMethodRoot methodRoot)
        {
            var node = methodRoot.CallGraphMethodNode;
            if (!_methodNodes.TryAdd(methodRoot.Name, node))
            {
                Log.Warning("Method {Name} is already registered in CallGraphBuilder",
                    methodRoot.Name);
                return;
            }

            if (_callRegistrationQueue.TryGetValue(methodRoot.Name, out var pendingCalls))
            {
                foreach (var callGraphInvocationNode in pendingCalls)
                {
                    callGraphInvocationNode.Target = node;
                }

                _callRegistrationQueue.Remove(methodRoot.Name);
            }

            foreach (var graphNode in methodRoot.Start.IterateGraphNodesRecursive(true))
            {
                switch (graphNode)
                {
                    case null:
                        throw new ArgumentNullException(nameof(graphNode));
                    case GraphDataFlowBlockNode graphDataFlowBlockNode:
                        foreach (var graphDataFlowExpressionNode in graphDataFlowBlockNode.Body)
                        {
                            RegisterExpression(graphDataFlowExpressionNode);
                        }

                        break;
                    case GraphControlFlowBlockNode _:
                        goto default;
                    case GraphDataFlowExpressionNodeBase dataFlowExpressionNode:
                        RegisterExpression(dataFlowExpressionNode);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(graphNode));
                }
            }

            void RegisterExpression(GraphDataFlowExpressionNodeBase graphExpressionNodeBase)
            {
                var count = 0;
                foreach (var functionCallOperator in graphExpressionNodeBase.Expression
                    .DescendantValues()
                    .OfType<FunctionCallOperator>())
                {
                    var invocationNode = new CallGraphInvocationNode(graphExpressionNodeBase);

                    count++;
                    if (count > 1)
                    {
                        Log.Warning("Multiple FunctionCallOperator in {Expression}", graphExpressionNodeBase);
                    }
                    else
                    {
                        graphExpressionNodeBase.CallGraphInvocationNode = invocationNode;
                    }

                    node.OutgoingNodes.Add(invocationNode);
                    var targetName = functionCallOperator.Name;
                    if (_methodNodes.TryGetValue(targetName, out var targetNode))
                    {
                        invocationNode.Target = targetNode;
                    }
                    else
                    {
                        _callRegistrationQueue.Add(targetName, invocationNode);
                    }
                }
            }
        }
    }
}