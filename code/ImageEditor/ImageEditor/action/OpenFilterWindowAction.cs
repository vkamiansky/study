using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using ImageEditor.filters;
using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor.action
{
    public class OpenFilterWindowAction : TriggerAction<FrameworkElement>
    {
        public Filter Filter
        {
            get => (Filter) GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        public static readonly DependencyProperty FilterProperty
            = DependencyProperty.Register("Filter", typeof(Filter), typeof(OpenFilterWindowAction),
                new PropertyMetadata(default(Filter)));

        public List<ILayer> Layers
        {
            get => (List<ILayer>) GetValue(LayersProperty);
            set => SetValue(LayersProperty, value);
        }

        public static readonly DependencyProperty LayersProperty
            = DependencyProperty.Register("Layers", typeof(List<ILayer>), typeof(OpenFilterWindowAction),
                new PropertyMetadata(default(List<ILayer>)));


        protected override void Invoke(object parameter)
        {
            if (Layers == null) return;
            List<ILayer> selectedLayers = new List<ILayer>();
            foreach (var layer in Layers)
            {
                if (layer.IsSelected)
                    selectedLayers.Add(layer);
            }

            selectedLayers.ForEach(layer => layer.SaveToMemento());

            void OnCloseAction()
            {
                selectedLayers.ForEach(layer => layer.RestoreFromMemento());
            }

            switch (Filter)
            {
                case Filter.BrightnessContrast:
                    new BCWindow(selectedLayers, OnCloseAction).Show();
                    break;
                case Filter.HueSaturation:
                    new HSWindow(selectedLayers, OnCloseAction).Show();
                    break;
                case Filter.BlackWhite:
                    new BWWindow(selectedLayers, OnCloseAction).Show();
                    break;
                case Filter.Threshold:
                    new TWindow(selectedLayers, OnCloseAction).Show();
                    break;
                case Filter.GaussianBlur:
                    new GBWindow(selectedLayers, OnCloseAction).Show();
                    break;
                case Filter.BoxBlur:
                    new BBWindow(selectedLayers, OnCloseAction).Show();
                    break;
                case Filter.Sharpen:
                    new SharpenWindow(selectedLayers, OnCloseAction).Show();
                    break;
                case Filter.EdgeDetection:
                    new EDWindow(selectedLayers, OnCloseAction).Show();
                    break;
            }
        }
    }
}