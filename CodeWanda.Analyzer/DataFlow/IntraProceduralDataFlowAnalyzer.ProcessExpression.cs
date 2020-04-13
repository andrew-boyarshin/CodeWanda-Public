using System;
using CodeWanda.Model.Graph;
using CodeWanda.Model.Graph.Nodes.Local;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Assignments;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;
using CodeWanda.Model.Semantic.Expressions.Operators.Other;
using CodeWanda.Model.Semantic.Expressions.Operators.Pure.Binary;
using CodeWanda.Model.Semantic.Expressions.Operators.Pure.Unary;
using JetBrains.Annotations;
using Serilog;

namespace CodeWanda.Analyzer.DataFlow
{
    public partial class IntraProceduralDataFlowAnalyzer
    {
        private void ProcessExpression([NotNull] GraphDataFlowExpressionNode node,
            [NotNull] GraphNode[] path)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (path == null) throw new ArgumentNullException(nameof(path));

            var implemented = false;
            switch (node.Expression)
            {
                case AssignmentBase assignmentBase:
                    ProcessAssignment(node, assignmentBase, path);
                    implemented = true;
                    break;
                case FunctionCallOperator functionCallOperator:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(node.Expression));
            }

            if (!implemented)
            {
                Log.Information("Expression: {Expression}",
                    node.Expression);
            }
        }

        private void ProcessAssignment(
            [NotNull] GraphDataFlowExpressionNode node,
            [NotNull] AssignmentBase assignmentBase,
            [NotNull] GraphNode[] path)
        {
            IValue left = assignmentBase.Left;
            var right = assignmentBase.Right;

            left = node.EnterAnalysisContext[left];

            if (left == null)
            {
                Log.Warning("Skipping {Assignment} since LHS expression resolved to null", assignmentBase);
                return;
            }

            var leftVariant = left is ValueVariantBase leftOldVariableVariant
                ? leftOldVariableVariant
                : throw new ArgumentOutOfRangeException(nameof(left));

            leftVariant = leftVariant.Clone(node.EnterAnalysisContext, path);

            var implemented = false;
            switch (assignmentBase)
            {
                case OperatorAssignment operatorAssignment:
                {
                    var @operator = operatorAssignment.Operator;
                    switch (@operator)
                    {
                        case BinaryPureBase binaryPureBase:
                        {
                            var interval = binaryPureBase.ApplyPossibleVariants(leftVariant, right);

                            if (interval != null)
                            {
                                leftVariant.Value = new DataValueInterval(interval, leftVariant.Value.Constraints);

                                implemented = true;
                            }

                            break;
                        }

                        case UnaryPureBase unaryPureBase:
                            goto default;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(@operator));
                    }

                    break;
                }

                case SimpleAssignment _:
                {
                    var interval = node.EnterAnalysisContext.ResolveToRange(right);

                    if (interval != null)
                    {
                        leftVariant.Value = new DataValueInterval(interval, Array.Empty<LogicExpressionBase>());

                        implemented = true;
                    }

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(assignmentBase));
            }

            if (!implemented)
            {
                Log.Information("Assignment: ({LeftKind}) {Operator} ({RightKind})",
                    left.GetType().Name,
                    assignmentBase,
                    right?.GetType().Name);
                return;
            }

            node.ExitAnalysisContext[leftVariant] = leftVariant;
        }
    }
}