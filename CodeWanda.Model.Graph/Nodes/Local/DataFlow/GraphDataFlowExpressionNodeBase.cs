using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CodeWanda.Model.Graph.Nodes.Call;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context;
using CodeWanda.Model.Semantic.Expressions;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow
{
    public abstract class GraphDataFlowExpressionNodeBase : GraphExpressionNodeBase
    {
        [JetBrains.Annotations.NotNull] public AnalysisContext EnterAnalysisContext { get; } = new AnalysisContext();

        [CanBeNull] public CallGraphInvocationNode CallGraphInvocationNode { get; set; }

        protected GraphDataFlowExpressionNodeBase(GraphMethodRoot methodRoot, [CanBeNull] ExpressionBase expression) :
            base(methodRoot, expression)
        {
            SubscribeToAnalysisContextChanges(EnterAnalysisContext);
        }

        protected void SubscribeToAnalysisContextChanges([DisallowNull] AnalysisContext analysisContext)
        {
            if (analysisContext == null) throw new ArgumentNullException(nameof(analysisContext));

            analysisContext.PropertyChanged += AnalysisContextOnPropertyChanged;
            analysisContext.CollectionChanged += AnalysisContextOnCollectionChanged;
        }

        private void AnalysisContextOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshText();
        }

        private void AnalysisContextOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshText();
        }
    }
}