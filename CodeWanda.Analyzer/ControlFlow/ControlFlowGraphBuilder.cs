using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CodeWanda.Analyzer.DataFlow;
using CodeWanda.Model.Graph.Nodes.Local;
using CodeWanda.Model.Semantic;
using CodeWanda.Model.Semantic.Data;
using Serilog;

namespace CodeWanda.Analyzer.ControlFlow
{
    public partial class ControlFlowGraphBuilder
    {
        public static readonly ControlFlowGraphBuilder Instance = new ControlFlowGraphBuilder();

        private ControlFlowGraphBuilder()
        {
        }

        public void Build([DisallowNull] AnalysisSession session, [DisallowNull] ObservableCollection<GraphMethodRoot> methodRoots)
        {
	        if (session == null) throw new ArgumentNullException(nameof(session));
	        if (methodRoots == null) throw new ArgumentNullException(nameof(methodRoots));

	        foreach (var value in session.Files)
            {
                if (!value.LoadParsedSource())
                {
                    Log.Error("Failed to load & parse {File}", value.SourceFile);
                    continue;
                }

                foreach (var parsedClass in value.ParsedClasses)
                {
                    BuildGraphForClass(parsedClass, methodRoots);
                }
            }
        }

        private void BuildGraphForClass(ClassDefinition classDefinition, ICollection<GraphMethodRoot> methodRoots)
        {
            foreach (var classDefinitionMethod in classDefinition.Methods)
            {
                BuildGraphForMethod(classDefinitionMethod, methodRoots);
            }
        }

        private void BuildGraphForMethod(MethodDefinition methodDefinition, ICollection<GraphMethodRoot> methodRoots)
        {
            if (methodRoots.Any(x => x.Name == methodDefinition.Name))
            {
                Log.Warning("{MethodName} is already present in this analysis session", methodDefinition.Name);
                return;
            }

            var methodRoot = new GraphMethodRoot(methodDefinition.Name)
            {
	            Code = methodDefinition.Code
            };

            methodRoots.Add(methodRoot);

            methodRoot.Arguments.AddRange(methodDefinition.Arguments);
            methodRoot.ParameterVariables.AddRange(methodDefinition.Body.LocalVariables.OfType<ParameterVariable>());

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            BuildGraphForBlock(methodDefinition.Body, methodRoot);

            SplitBlocksAtMethodJumps(methodRoot);

            CalculateIncoming(methodRoot.Start);

            for (var i = 1; i <= 50; i++)
            {
                var res = SimplifyGraph(methodRoot);
                if (!res)
                    break;
            }

            CalculateIncoming(methodRoot.Start);

            var dataFlowGraphBuilder = new DataFlowGraphBuilder(methodRoot);
            dataFlowGraphBuilder.Convert();
            
            CallGraphBuilder.Instance.RegisterMethod(methodRoot);

            var analysis = new IntraProceduralDataFlowAnalyzer(methodRoot);
            analysis.Analyze();

            stopwatch.Stop();
            Log.Information("DFG: {Name}: processing: {Time}", methodRoot.Name, stopwatch.Elapsed);
        }
    }
}