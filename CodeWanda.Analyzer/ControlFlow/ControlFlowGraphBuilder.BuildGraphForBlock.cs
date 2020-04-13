using System;
using System.Collections.Generic;
using System.Linq;
using CodeWanda.Model.Graph.Nodes.Local;
using CodeWanda.Model.Graph.Nodes.Local.ControlFlow;
using CodeWanda.Model.Semantic.Expressions;
using CodeWanda.Model.Semantic.Statements;
using JetBrains.Annotations;
using Serilog;

namespace CodeWanda.Analyzer.ControlFlow
{
    public partial class ControlFlowGraphBuilder
    {
        private void BuildGraphForBlock([NotNull] SimpleCompoundStatement compoundStatement,
            [NotNull] GraphMethodRoot methodRoot)
        {
            var graphNode = new GraphControlFlowBlockNode(methodRoot);
            methodRoot.Start = graphNode;
            var (preemptiveExits, next) = AddStatementToGraphNode(compoundStatement,
                methodRoot,
                graphNode,
                null,
                null);

            if (next != null)
            {
                next.Outgoing = new GraphControlFlowReturnNode(methodRoot, null, true);
            }
        }

        private (GraphNode[] preemptiveExits, GraphControlFlowBlockNode next)
            AddStatementToGraphNode(StatementBase statement,
                GraphMethodRoot methodRoot,
                GraphControlFlowBlockNode target,
                GraphNode continueTarget,
                GraphNode breakTarget)
        {
            switch (statement)
            {
                case ContinueStatement _:
                    target.Outgoing = continueTarget;
                    return (Array.Empty<GraphNode>(), null);

                case ForIterationStatement forStatement:
                {
                    var processConditional = ProcessConditional(forStatement);
                    if (!processConditional.HasValue)
                        return (Array.Empty<GraphNode>(), target);

                    var (conditionalStartNode, conditionalNode) = processConditional.Value;

                    var preemptiveExits = new List<GraphNode>();

                    // block inside FOR that contains incrementors and stuff
                    var iterationExpressionBlock = new GraphControlFlowBlockNode(methodRoot);

                    // block after FOR
                    var nextBlock = new GraphControlFlowBlockNode(methodRoot);

                    {
                        foreach (var expression in forStatement.IterationExpression)
                            iterationExpressionBlock.Body.Add(
                                new GraphControlFlowExpressionNode(methodRoot, expression)
                            );

                        iterationExpressionBlock.Outgoing = conditionalStartNode;
                    }

                    {
                        var trueBlock = new GraphControlFlowBlockNode(methodRoot);
                        conditionalNode.TrueOutgoing = trueBlock;

                        var (truePreemptiveExits, trueNext) = AddStatementToGraphNode(forStatement.Block,
                            methodRoot,
                            trueBlock,
                            iterationExpressionBlock,
                            nextBlock);

                        preemptiveExits.AddRange(truePreemptiveExits);

                        if (trueNext != null)
                        {
                            trueNext.Outgoing = iterationExpressionBlock;
                        }
                    }

                    conditionalNode.FalseOutgoing = nextBlock;

                    return (preemptiveExits.ToArray(), nextBlock);
                }

                case IfStatement ifStatement:
                {
                    var processConditional = ProcessConditional(ifStatement);
                    if (!processConditional.HasValue)
                        return (Array.Empty<GraphNode>(), target);

                    var (_, conditionalNode) = processConditional.Value;

                    var preemptiveExits = new List<GraphNode>();

                    // block after IF
                    var nextBlock = new GraphControlFlowBlockNode(methodRoot);

                    {
                        var trueBlock = new GraphControlFlowBlockNode(methodRoot);
                        conditionalNode.TrueOutgoing = trueBlock;

                        var (truePreemptiveExits, trueNext) = AddStatementToGraphNode(ifStatement.Block,
                            methodRoot,
                            trueBlock,
                            continueTarget,
                            breakTarget);

                        preemptiveExits.AddRange(truePreemptiveExits);

                        if (trueNext != null)
                        {
                            trueNext.Outgoing = nextBlock;
                        }
                    }

                    if (ifStatement.ElseBlock != null)
                    {
                        var falseBlock = new GraphControlFlowBlockNode(methodRoot);
                        conditionalNode.FalseOutgoing = falseBlock;

                        var (falsePreemptiveExits, falseNext) = AddStatementToGraphNode(ifStatement.ElseBlock,
                            methodRoot,
                            falseBlock,
                            continueTarget,
                            breakTarget);

                        preemptiveExits.AddRange(falsePreemptiveExits);

                        if (falseNext != null)
                        {
                            falseNext.Outgoing = nextBlock;
                        }
                    }
                    else
                    {
                        conditionalNode.FalseOutgoing = nextBlock;
                    }

                    return (preemptiveExits.ToArray(), nextBlock);
                }

                case ReturnStatement returnStatement:
                {
                    var graphReturnNode = new GraphControlFlowReturnNode(
                        methodRoot,
                        (ExpressionBase) returnStatement.What,
                        false
                    );
                    target.Outgoing = graphReturnNode;
                    return (new GraphNode[] {graphReturnNode}, null);
                }

                case SimpleCompoundStatement simpleCompoundStatement:
                {
                    var preemptiveExits = new List<GraphNode>();
                    var currentTarget = target;

                    foreach (var statementBase in simpleCompoundStatement.Body)
                    {
                        var (newPreemptiveExits, newNext) = AddStatementToGraphNode(statementBase,
                            methodRoot,
                            currentTarget,
                            continueTarget,
                            breakTarget);

                        preemptiveExits.AddRange(newPreemptiveExits);

                        currentTarget = newNext;
                    }

                    if (currentTarget != null)
                    {
                        var newBlockNode = new GraphControlFlowBlockNode(methodRoot);
                        currentTarget.Outgoing = newBlockNode;
                        currentTarget = newBlockNode;
                    }

                    return (preemptiveExits.ToArray(), currentTarget);
                }

                case SimpleDeclarationStatement _:
                    return (Array.Empty<GraphNode>(), target);

                case SimpleExpressionStatement simpleExpressionStatement:
                {
                    var node = new GraphControlFlowExpressionNode(methodRoot, simpleExpressionStatement.Expression);
                    target.Body.Add(node);
                    return (Array.Empty<GraphNode>(), target);
                }

                case WhileIterationStatement whileStatement:
                {
                    var processConditional = ProcessConditional(whileStatement);
                    if (!processConditional.HasValue)
                        return (Array.Empty<GraphNode>(), target);

                    var (conditionalStartNode, conditionalNode) = processConditional.Value;

                    var preemptiveExits = new List<GraphNode>();

                    // block after WHILE
                    var nextBlock = new GraphControlFlowBlockNode(methodRoot);

                    {
                        var trueBlock = new GraphControlFlowBlockNode(methodRoot);
                        conditionalNode.TrueOutgoing = trueBlock;

                        var (truePreemptiveExits, trueNext) = AddStatementToGraphNode(whileStatement.Block,
                            methodRoot,
                            trueBlock,
                            conditionalStartNode,
                            nextBlock);

                        preemptiveExits.AddRange(truePreemptiveExits);

                        if (trueNext != null)
                        {
                            trueNext.Outgoing = conditionalStartNode;
                        }
                    }

                    conditionalNode.FalseOutgoing = nextBlock;

                    return (preemptiveExits.ToArray(), nextBlock);
                }

                case SimpleSelectionStatement _:
                    goto default;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statement));
            }

            (GraphNode conditionalStartNode, GraphControlFlowConditionalNode conditionalNode)?
                ProcessConditional(SimpleSelectionStatement statement)
            {
                GraphControlFlowConditionalNode conditionalNode;

                if (statement.Condition.Count >= 1)
                {
                    conditionalNode = new GraphControlFlowConditionalNode(methodRoot, statement.Condition[^1]);
                }
                else
                {
                    Log.Error("Statement {Statement} has no conditions!", statement);
                    return null;
                }

                GraphControlFlowBlockNode conditionalParent;

                if (statement.Condition.Count == 1)
                {
                    conditionalParent = target;
                }
                else
                {
                    var block = new GraphControlFlowBlockNode(methodRoot);

                    foreach (var expression in statement.Condition.SkipLast(1))
                        block.Body.Add(new GraphControlFlowExpressionNode(methodRoot, expression));

                    target.Outgoing = block;
                    conditionalParent = block;
                }

                conditionalParent.Outgoing = conditionalNode;

                return (target.Outgoing, conditionalNode);
            }
        }
    }
}