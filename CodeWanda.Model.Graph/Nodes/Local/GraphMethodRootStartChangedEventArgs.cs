using System;

namespace CodeWanda.Model.Graph.Nodes.Local
{
    public sealed class GraphMethodRootStartChangedEventArgs : EventArgs
    {
        public GraphMethodRootStartChangedEventArgs(GraphNode oldValue, GraphNode newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public GraphNode OldValue { get; }
        public GraphNode NewValue { get; }
    }
}