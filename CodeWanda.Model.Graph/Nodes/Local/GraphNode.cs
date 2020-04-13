using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Serilog;

namespace CodeWanda.Model.Graph.Nodes.Local
{
    public abstract class GraphNode : INotifyPropertyChanged
    {
        private readonly ObservableCollection<GraphNode> _allOutgoingInternal = new ObservableCollection<GraphNode>();
        private string _text;

        [NotNull] public ObservableCollection<GraphNode> Incoming { get; } = new ObservableCollection<GraphNode>();
        [NotNull] public ReadOnlyObservableCollection<GraphNode> AllOutgoing { get; }

        [NotNull] public GraphMethodRoot MethodRoot { get; }

        public GraphNode([NotNull] GraphMethodRoot methodRoot)
        {
            MethodRoot = methodRoot ?? throw new ArgumentNullException(nameof(methodRoot));
            AllOutgoing = new ReadOnlyObservableCollection<GraphNode>(_allOutgoingInternal);
            PropertyChanged += TextManagerPropertyChanged;
        }

        private void TextManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Text))
                return;

            RefreshText();
        }

        public void RefreshText()
        {
            Text = ToString();
        }

        public string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;

                _text = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [JetBrains.Annotations.NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Log.Verbose(
                "GraphNode[{Id}]: {PropertyName} changed",
                RuntimeHelpers.GetHashCode(this),
                propertyName
            );

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void UpdateOutgoingNode(ref GraphNode oldNode, GraphNode newNode,
            [CallerMemberName] string propertyName = null)
        {
            VerifyOldOutgoingNode(oldNode);

            if (ReferenceEquals(oldNode, newNode)) return;

            if (oldNode != null)
            {
                _allOutgoingInternal.Remove(oldNode);
            }

            oldNode = newNode;

            if (newNode != null)
            {
                _allOutgoingInternal.Add(newNode);
            }

            OnPropertyChanged(propertyName);
        }

        protected GraphNode GetOutgoingNode(GraphNode storage)
        {
            VerifyOldOutgoingNode(storage);

            return storage;
        }

        private void VerifyOldOutgoingNode(GraphNode storage)
        {
            Debug.Assert(storage == null && !AllOutgoing.Contains(null) ||
                         storage != null && AllOutgoing.Contains(storage));
        }

        public abstract void ReplaceOutgoingNode(GraphNode oldNode, GraphNode newNode);

        public void ReplaceIncomingNode([DisallowNull] GraphNode oldParentNode, IReadOnlyCollection<GraphNode> newParentNodes,
            bool allowRemoval)
        {
            if (oldParentNode == null) throw new ArgumentNullException(nameof(oldParentNode));

            Debug.Assert(Incoming.Contains(oldParentNode));
            oldParentNode.VerifyOldOutgoingNode(this);

            Incoming.Remove(oldParentNode);

            var notARemoval = newParentNodes != null && newParentNodes.Count != 0;
            Debug.Assert(allowRemoval || notARemoval);
            
            if (notARemoval)
            {
                foreach (var newParentNode in newParentNodes) Incoming.Add(newParentNode);
            }
        }
    }
}