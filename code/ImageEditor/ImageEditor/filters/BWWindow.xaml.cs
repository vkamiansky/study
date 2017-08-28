using System;
using System.Collections.Generic;
using System.Windows;
using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor.filters
{
    /// <summary>
    /// Логика взаимодействия для BWWindow.xaml
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class BWWindow
    {
        private readonly List<ILayer> _selectedLayers;

        public BWWindow(List<ILayer> selectedLayers, Action onClose)
        {
            InitializeComponent();
            _selectedLayers = selectedLayers;
            _selectedLayers.ForEach(layer => layer.SaveToMemento());
            Closing += (sender, args) => onClose.Invoke();
            Update();
        }

        private void ApplyFilter(float[] raw)
        {
            float reds = (float) (Reds.Value / 1000f);
            float greens = (float) (Greens.Value / 1000f);
            float blues = (float) (Blues.Value / 1000f);

            for (var i = 0; i < raw.Length; i += 4)
            {
                float red = raw[i + 2];
                float green = raw[i + 1];
                float blue = raw[i + 0];

                bool c = red > reds || green > greens || blue > blues;

                raw[i + 0] = raw[i + 1] = raw[i + 2] = c ? 0f : 1f;
            }
        }


        private void Update()
        {
            if (_selectedLayers == null || _selectedLayers.Count == 0) return;
            foreach (var selectedLayer in _selectedLayers)
            {
                selectedLayer.RestoreFromMemento();
                ApplyFilter(selectedLayer.Raw);
                selectedLayer.OnChanged?.Invoke();
            }
        }

        private void Brightness_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Update();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            _selectedLayers.ForEach(layer => layer.SaveToMemento());
            Close();
        }
    }
}