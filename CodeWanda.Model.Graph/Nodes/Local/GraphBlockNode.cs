using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local
{
    public class GraphBlockNode<T> : GraphNode where T : GraphExpressionNodeBase
    {
        private GraphNode _outgoing;
        private readonly StringBuilder _stringBuilder = new StringBuilder(30);

        [NotNull] [ItemNotNull] public ObservableCollection<T> Body { get; } = new ObservableCollection<T>();

        public GraphNode Outgoing
        {
            get => GetOutgoingNode(_outgoing);
            set => UpdateOutgoingNode(ref _outgoing, value);
        }

        protected GraphBlockNode([NotNull] GraphMethodRoot methodRoot) : base(methodRoot)
        {
            Body.CollectionChanged += BodyOnCollectionChanged;
        }

        private void BodyOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var node in e.NewItems.OfType<T>())
                {
                    node.PropertyChanged += NodeOnPropertyChanged;
                }

            RefreshText();
        }

        private void NodeOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshText();
        }

        public override void ReplaceOutgoingNode(GraphNode oldNode, GraphNode newNode)
        {
            Debug.Assert(Outgoing == oldNode);
            Outgoing = newNode;
        }

        public override string ToString()
        {
            _stringBuilder.AppendJoin('\n', Body);
            var value = _stringBuilder.ToString();
            _stringBuilder.Clear();
            return value;
        }
    }
}