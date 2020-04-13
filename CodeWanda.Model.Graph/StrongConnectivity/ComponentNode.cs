using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CodeWanda.Model.Graph.Nodes.Local;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.StrongConnectivity
{
    public class ComponentNode<T> : INotifyPropertyChanged where T : GraphNode, IComponentNodeItem<T>
    {
        [CanBeNull] private ComponentNodeGroup _strongConnectivityComponent;
        private bool _visited;

        public ComponentNode([NotNull] T graphNode)
        {
            GraphNode = graphNode ?? throw new ArgumentNullException(nameof(graphNode));
        }

        public T GraphNode { get; }

        public bool Visited
        {
            get => _visited;
            set
            {
                if (value == _visited) return;
                
                _visited = value;
                OnPropertyChanged();
            }
        }

        [CanBeNull]
        public ComponentNodeGroup StrongConnectivityComponent
        {
            get => _strongConnectivityComponent;
            set
            {
                if (Equals(value, _strongConnectivityComponent)) return;
                
                _strongConnectivityComponent = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}