using System.Linq;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Assignments;
using CodeWanda.Model.Semantic.Expressions.Literals;
using CodeWanda.Model.Semantic.Statements;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Serilog;

namespace CodeWanda.Parser.CSharp
{
    public static partial class CSharpParser
    {
        private static void ProcessRoslynStatementIntoWandaBlock(StatementSyntax roslynStatement,
            SimpleCompoundStatement wandaBlock)
        {
            switch (roslynStatement)
            {
                case LocalDeclarationStatementSyntax declarationStatement:
                {
                    ProcessVariableDeclarationSyntax(declarationStatement.Declaration, wandaBlock);

                    break;
                }

                case ExpressionStatementSyntax expressionStatementSyntax:
                {
                    foreach (var expressionBase in ConvertRoslynExpressionToWandaExpression(wandaBlock,
                        expressionStatementSyntax.Expression))
                    {
                        ProcessWandaExpressionIntoWandaBlock(wandaBlock, expressionBase);
                    }

                    break;
                }

                case ForStatementSyntax forStatementSyntax:
                {
                    if (forStatementSyntax.Declaration != null)
                    {
                        ProcessVariableDeclarationSyntax(forStatementSyntax.Declaration, wandaBlock);
                    }

                    var forIterationStatement = new ForIterationStatement(wandaBlock);

                    foreach (var expressionSyntax in forStatementSyntax.Initializers)
                    {
                        foreach (var expressionBase in ConvertRoslynExpressionToWandaExpression(wandaBlock,
                            expressionSyntax))
                        {
                            ProcessWandaExpressionIntoWandaBlock(wandaBlock, expressionBase);
                        }
                    }

                    forIterationStatement.Condition.AddRange(
                        ConvertRoslynExpressionToWandaExpression(wandaBlock, forStatementSyntax.Condition));
                    forIterationStatement.IterationExpression.AddRange(forStatementSyntax.Incrementors
                        .SelectMany(x =>
                            ConvertRoslynExpressionToWandaExpression(wandaBlock, x)));

                    ProcessRoslynSelectionStatementIntoWandaSelectionStatementBlock(forStatementSyntax.Statement,
                        forIterationStatement.Block);

                    wandaBlock.Body.Add(forIterationStatement);

                    break;
                }

                case WhileStatementSyntax whileStatementSyntax:
                {
                    var whileIterationStatement = new WhileIterationStatement(wandaBlock);

                    whileIterationStatement.Condition.AddRange(
                        ConvertRoslynExpressionToWandaExpression(wandaBlock, whileStatementSyntax.Condition));

                    ProcessRoslynSelectionStatementIntoWandaSelectionStatementBlock(whileStatementSyntax.Statement,
                        whileIterationStatement.Block);

                    wandaBlock.Body.Add(whileIterationStatement);

                    break;
                }

                case IfStatementSyntax ifStatementSyntax:
                {
                    var ifStatement = new IfStatement(wandaBlock);

                    ifStatement.Condition.AddRange(
                        ConvertRoslynExpressionToWandaExpression(wandaBlock, ifStatementSyntax.Condition));

                    ProcessRoslynSelectionStatementIntoWandaSelectionStatementBlock(ifStatementSyntax.Statement,
                        ifStatement.Block);

                    if (ifStatementSyntax.Else != null)
                    {
                        ifStatement.ElseBlock = new SimpleCompoundStatement
                        {
                            ParentBlock = wandaBlock, ParentMethod = wandaBlock.ParentMethod
                        };
                        ProcessRoslynSelectionStatementIntoWandaSelectionStatementBlock(
                            ifStatementSyntax.Else.Statement,
                            ifStatement.ElseBlock);
                    }

                    wandaBlock.Body.Add(ifStatement);

                    break;
                }

                case ReturnStatementSyntax returnStatementSyntax:
                {
                    var returnStatement = new ReturnStatement();

                    if (returnStatementSyntax.Expression != null)
                    {
                        returnStatement.What = ResolveValueToBlock(wandaBlock, returnStatementSyntax.Expression, true);
                    }

                    wandaBlock.Body.Add(returnStatement);

                    break;
                }

                case ContinueStatementSyntax _:
                {
                    var continueStatement = new ContinueStatement();

                    wandaBlock.Body.Add(continueStatement);

                    break;
                }

                default:
                    Log.Debug("Roslyn → Wanda Statement: {Statement} ({Kind})", roslynStatement,
                        roslynStatement.Kind());
                    break;
            }
        }

        private static void ProcessVariableDeclarationSyntax(VariableDeclarationSyntax variableDeclarationSyntax,
            SimpleCompoundStatement wandaBlock)
        {
            foreach (var variableDeclaratorSyntax in variableDeclarationSyntax.Variables)
            {
                var variable = new Variable
                {
                    TypeRef = ResolveType(variableDeclarationSyntax.Type),
                    Name = variableDeclaratorSyntax.Identifier.Text
                };
                wandaBlock.LocalVariables.Add(variable);
                var declaration = new SimpleDeclarationStatement
                {
                    Variable = variable,
                    ParentBlock = wandaBlock,
                    ParentMethod = wandaBlock.ParentMethod,
                };
                wandaBlock.Body.Add(declaration);
                if (variableDeclaratorSyntax.Initializer == null)
                    continue;

                var initializerValue = variableDeclaratorSyntax.Initializer.Value;

                var withWhat = ResolveValueToBlock(wandaBlock, initializerValue,
                    replaceCreatedDummyVariableWith: variable);

                if (withWhat == variable)
                    continue;

                if (withWhat == null)
                {
                    Log.Warning("Unable to parse assigned value expression {Expression} for {Variable}, using null", initializerValue, variable.Name);
	                withWhat = NullLiteral.Instance;
                }

                var assignment = new SimpleExpressionStatement
                {
                    Expression = new SimpleAssignment(variable, withWhat),
                    ParentBlock = wandaBlock,
                    ParentMethod = wandaBlock.ParentMethod,
                    // todo: location
                };

                if (withWhat is ILValue value)
                    variable.TypeRef = value.TypeRef;

                wandaBlock.Body.Add(assignment);
            }
        }
    }
}