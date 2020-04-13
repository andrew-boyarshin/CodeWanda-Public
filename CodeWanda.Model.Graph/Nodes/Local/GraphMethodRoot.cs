using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CodeWanda.Model.Graph.Nodes.Call;
using CodeWanda.Model.Graph.StrongConnectivity;
using CodeWanda.Model.Semantic;
using CodeWanda.Model.Semantic.Data;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local
{
    public sealed class GraphMethodRoot : INotifyPropertyChanged
    {
        private Guid _file;
        private GraphNode _start;
        
        [NotNull] public string Name { get; }

        public GraphNode Start
        {
            get => _start;
            set
            {
                var oldValue = _start;
                _start = value;
                OnStartChanged(oldValue, _start);
            }
        }

        public Guid File
        {
            get => _file;
            set
            {
                if (value.Equals(_file)) return;
                
                _file = value;
                OnPropertyChanged();
            }
        }

        [NotNull] public CallGraphMethodNode CallGraphMethodNode { get; }

        [NotNull]
        [ItemNotNull]
        public List<ParameterVariable> ParameterVariables { get; } = new List<ParameterVariable>();

        [NotNull]
        [ItemNotNull]
        public List<ComponentNodeGroup> AllGroups { get; } = new List<ComponentNodeGroup>();

        public string Code { get; set; }

        public GraphMethodRoot([NotNull] string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            CallGraphMethodNode = new CallGraphMethodNode(this);
        }

        [NotNull]
        [ItemNotNull]
        public List<MethodDefinition.MethodParameterDefinition> Arguments { get; } =
            new List<MethodDefinition.MethodParameterDefinition>();

        public event GraphMethodRootStartChangedEventHandler StartChanged;

        private void OnStartChanged(GraphNode oldValue, GraphNode newValue)
        {
            StartChanged?.Invoke(this, new GraphMethodRootStartChangedEventArgs(oldValue, newValue));
            OnPropertyChanged(nameof(Start));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}