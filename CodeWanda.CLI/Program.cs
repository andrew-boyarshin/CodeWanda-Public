using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CodeWanda.Analyzer;
using CodeWanda.Model.Graph;
using CodeWanda.Model.Graph.Nodes.Local;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context;
using CodeWanda.Model.Semantic.Data;
using Serilog;

namespace CodeWanda.CLI
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            var analysisSession = new AnalysisSession();
            foreach (var s in args)
            {
                var file = new FileInfo(s);
                if (!file.Exists)
                    continue;
                if (file.Extension != ".cs")
                    continue;

                analysisSession.Add(file);
            }

            if (analysisSession.Files.Count == 0)
            {
                return;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            analysisSession.BuildGraph();

            stopwatch.Stop();

            Log.Information("DFG: {Time}", stopwatch.Elapsed);

            foreach (var methodRoot in analysisSession.CurrentMethodRoots)
            {
                Print(methodRoot);
            }
        }

        private static void Print(GraphMethodRoot methodRoot)
        {
            var terminal = methodRoot.Start
                .IterateGraphNodesRecursive(true)
                .Where(x => !x.AllOutgoing.Any())
                .ToArray();

            foreach (var graphNode in terminal)
            {
                switch (graphNode)
                {
                    case GraphDataFlowReturnNode graphReturnNode:
                        Print(graphReturnNode.EnterAnalysisContext);
                        break;
                    case GraphDataFlowBlockNode graphDataFlowBlockNode:
                        var expressionNode = graphDataFlowBlockNode.Body.Last();
                        Print(expressionNode.ExitAnalysisContext);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(graphNode));
                }
            }
        }

        private static void Print(AnalysisContext analysisContext)
        {
            var variables = new HashSet<Variable>();
            var arrays = new HashSet<Variable>();
            foreach (var parent in analysisContext.IterateParents())
            {
                foreach (var variable in parent.AssignedVariables)
                {
                    variables.Add(variable);
                }

                foreach (var (arrayVariant, _) in parent.AssignedArrayItems)
                {
                    arrays.Add(arrayVariant.StorageVariable);
                }
            }

            foreach (var variable in variables)
            {
                var variant = analysisContext[variable];
                if (variant == null)
                {
                    Log.Warning("No {Index} array found", variable);
                    continue;
                }

                Log.Information("{Array} = {Value}",
                    variable, variant);
            }

            foreach (var variable in arrays)
            {
                var arrayVariant = analysisContext[variable] as ArrayVariant;
                if (arrayVariant == null)
                {
                    Log.Warning("No {Index} array found", variable);
                    continue;
                }

                foreach (var itemVariant in arrayVariant[analysisContext, (ValueInterval) null])
                {
                    Log.Information("{Array}[{Index}] = {Value}",
                        variable, itemVariant.Index, itemVariant);
                }
            }
        }
    }
}