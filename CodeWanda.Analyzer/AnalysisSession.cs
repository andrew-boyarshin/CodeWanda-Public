using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using CodeWanda.Analyzer.ControlFlow;
using CodeWanda.Model.Graph.Nodes.Local;

namespace CodeWanda.Analyzer
{
    public class AnalysisSession
    {
        private readonly ObservableCollection<GraphNode> _currentMethodRootsNodeCollection =
            new ObservableCollection<GraphNode>();

        private readonly object _currentMethodRootsLock = new object();

        public ObservableCollection<AnalysisSessionFile> Files { get; } =
            new ObservableCollection<AnalysisSessionFile>();

        public ObservableCollection<GraphMethodRoot> CurrentMethodRoots { get; } =
            new ObservableCollection<GraphMethodRoot>();

        public ReadOnlyObservableCollection<GraphNode> CurrentMethodRootsNodeView { get; }

        public AnalysisSession()
        {
            CurrentMethodRootsNodeView = new ReadOnlyObservableCollection<GraphNode>(_currentMethodRootsNodeCollection);
            CurrentMethodRoots.CollectionChanged += CurrentMethodRootsOnCollectionChanged;
        }

        public IEnumerable<(IEnumerable collection, object lockObject)> GetThreadSynchronizedCollections()
        {
            // ReSharper disable once InconsistentlySynchronizedField
            yield return (_currentMethodRootsNodeCollection, _currentMethodRootsLock);
            yield return (CurrentMethodRootsNodeView, _currentMethodRootsLock);
        }

        private void CurrentMethodRootsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            lock (_currentMethodRootsLock)
            {
                if (e.OldItems != null)
                {
                    foreach (GraphMethodRoot item in e.OldItems)
                    {
                        item.StartChanged -= GraphMethodRootOnStartChanged;
                        var start = item.Start;
                        if (start != null) _currentMethodRootsNodeCollection.Remove(start);
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (GraphMethodRoot item in e.NewItems)
                    {
                        item.StartChanged += GraphMethodRootOnStartChanged;
                        var start = item.Start;
                        if (start != null) _currentMethodRootsNodeCollection.Add(start);
                    }
                }

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        _currentMethodRootsNodeCollection.Clear();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(e));
                }
            }
        }

        private void GraphMethodRootOnStartChanged(GraphMethodRoot sender, GraphMethodRootStartChangedEventArgs e)
        {
            lock (_currentMethodRootsLock)
            {
                if (e.OldValue != null) _currentMethodRootsNodeCollection.Remove(e.OldValue);
                if (e.NewValue != null) _currentMethodRootsNodeCollection.Add(e.NewValue);
            }
        }

        public void Add(FileInfo sourceFile)
        {
            var analysisSessionFile = new AnalysisSessionFile(sourceFile);

            // todo: locks in threaded scenario
            if (!Files.Contains(analysisSessionFile))
            {
                Files.Add(analysisSessionFile);
            }
        }

        public void BuildGraph()
        {
            CurrentMethodRoots.Clear();

            ControlFlowGraphBuilder.Instance.Build(this, CurrentMethodRoots);
        }
    }
}