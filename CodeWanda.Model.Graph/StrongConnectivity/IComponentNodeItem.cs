using System.ComponentModel;
using CodeWanda.Model.Graph.Nodes.Local;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.StrongConnectivity
{
    public interface IComponentNodeItem<T> where T : GraphNode, IComponentNodeItem<T>
    {
        [NotNull] public ComponentNode<T> StrongConnectivityNode { get; }
        
        public void StrongConnectivityNodeOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ((GraphNode) this).RefreshText();
        }
        
        public void SubscribeToStrongConnectivityNodePropertyChanged()
        {
            StrongConnectivityNode.PropertyChanged += StrongConnectivityNodeOnPropertyChanged;
        }
    }
}