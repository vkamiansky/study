using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor.action
{
    public class LayerTriggerAction : TriggerAction<FrameworkElement>
    {
        public List<ILayer> Layers
        {
            get => (List<ILayer>) GetValue(LayersProperty);
            set => SetValue(LayersProperty, value);
        }

        public static readonly DependencyProperty LayersProperty
            = DependencyProperty.Register("Layers", typeof(List<ILayer>), typeof(LayerTriggerAction),
                new PropertyMetadata(default(List<ILayer>)));

        public Tuple<LayerAction, ILayer> OutputTuple
        {
            get => (Tuple<LayerAction, ILayer>) GetValue(OutputTupleProperty);

            set => SetValue(OutputTupleProperty, value);
        }

        public static readonly DependencyProperty OutputTupleProperty
            = DependencyProperty.Register("OutputTuple", typeof(Tuple<LayerAction, ILayer>), typeof(LayerTriggerAction),
                new PropertyMetadata(default(Tuple<LayerAction, ILayer>)));

        public LayerAction ActionType
        {
            get => (LayerAction) GetValue(ActionTypeProperty);
            set => SetValue(ActionTypeProperty, value);
        }

        public static readonly DependencyProperty ActionTypeProperty
            = DependencyProperty.Register("ActionType", typeof(LayerAction), typeof(LayerTriggerAction),
                new PropertyMetadata(default(LayerAction)));

        protected override void Invoke(object parameter)
        {
            if (ActionType == null || Layers == null) return;

            var selectedLayers = new List<ILayer>();

            Layers.ForEach(layer =>
            {
                if (layer.IsSelected) selectedLayers.Add(layer);
            });

            if (selectedLayers.Count > 1 && ActionType != LayerAction.AddLayer) return;

            var selectedLayer = ActionType != LayerAction.AddLayer ? selectedLayers[0] : null;

            switch (ActionType)
            {
                case LayerAction.AddLayer:
                    OutputTuple = new Tuple<LayerAction, ILayer>(LayerAction.AddLayer, null);
                    break;
                case LayerAction.RemoveLayer:
                    OutputTuple = new Tuple<LayerAction, ILayer>(LayerAction.RemoveLayer, selectedLayer);
                    break;
                case LayerAction.DuplicateLayer:
                    OutputTuple = new Tuple<LayerAction, ILayer>(LayerAction.DuplicateLayer, selectedLayer);
                    break;
            }
        }
    }
}