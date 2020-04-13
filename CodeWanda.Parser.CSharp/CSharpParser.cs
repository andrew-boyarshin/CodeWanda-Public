using System.Collections.Generic;
using System.Linq;
using CodeWanda.Model.Semantic;
using CodeWanda.Model.Semantic.Expressions;
using CodeWanda.Model.Semantic.Statements;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeWanda.Parser.CSharp
{
    public static partial class CSharpParser
    {
        private static AdhocWorkspace _workspace = new AdhocWorkspace();

        public static List<ClassDefinition> ProcessCSharpSyntaxTree(CompilationUnitSyntax root)
        {
            var classes = new List<ClassDefinition>();

            foreach (var classDeclarationSyntax in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                var classDef = new ClassDefinition
                {
                    Name = classDeclarationSyntax.Identifier.Text
                };

                classes.Add(classDef);

                foreach (var methodDeclaration in classDeclarationSyntax.DescendantNodes()
                    .OfType<MethodDeclarationSyntax>())
                {
                    var normalizeWhitespace =
                        methodDeclaration.Body.Statements.Select(x => x.NormalizeWhitespace("\t", true).ToString());
                    var methodDef = new MethodDefinition
                    {
                        Name = methodDeclaration.Identifier.Text,
                        Code = string.Join('\n', normalizeWhitespace)
                        // normalizeWhitespace.ToString()
                        //Formatter.Format(normalizeWhitespace, _workspace).ToString().Split('\n')
                    };

                    methodDef.Arguments.AddRange(
                        methodDeclaration.ParameterList.Parameters
                            .Select(x =>
                            {
                                var parameterDefinition =
                                    new MethodDefinition.MethodParameterDefinition(x.Identifier.Text);
                                parameterDefinition.Variable.TypeRef = ResolveType(x.Type);
                                return parameterDefinition;
                            }));

                    methodDef.Body.ParentMethod = methodDef;

                    foreach (var methodParameterDefinition in methodDef.Arguments)
                    {
                        methodDef.Body.LocalVariables.Add(methodParameterDefinition.Variable);
                    }

                    ProcessRoslynBlockIntoWandaBlock(methodDeclaration.Body, methodDef.Body);

                    classDef.Methods.Add(methodDef);
                }
            }

            return classes;
        }

        private static void ProcessRoslynBlockIntoWandaBlock(BlockSyntax roslynBody,
            SimpleCompoundStatement wandaBlock)
        {
            foreach (var statement in roslynBody.Statements)
            {
                ProcessRoslynStatementIntoWandaBlock(statement, wandaBlock);
            }
        }

        private static void ProcessRoslynSelectionStatementIntoWandaSelectionStatementBlock(
            StatementSyntax innerStatement,
            SimpleCompoundStatement block)
        {
            if (innerStatement is BlockSyntax blockSyntax)
            {
                ProcessRoslynBlockIntoWandaBlock(blockSyntax, block);
            }
            else
            {
                ProcessRoslynStatementIntoWandaBlock(innerStatement, block);
            }
        }

        private static void ProcessWandaExpressionIntoWandaBlock(SimpleCompoundStatement wandaBlock,
            ExpressionBase wandaExpression)
        {
            var simpleExpressionStatement = new SimpleExpressionStatement
            {
                Expression = wandaExpression,
                ParentBlock = wandaBlock,
                ParentMethod = wandaBlock.ParentMethod
                //Location = invocationExpressionSyntax.GetLocation()
            };
            wandaBlock.Body.Add(simpleExpressionStatement);
        }
    }
}