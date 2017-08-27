using System;
using System.Collections.Generic;
using System.Windows;
using ImageEditor.Interface.ViewModel;
using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor.filters
{
    /// <summary>
    /// Логика взаимодействия для BCWindow.xaml
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class BCWindow
    {
        private readonly List<ILayer> _selectedLayers;
        
        public BCWindow(List<ILayer> selectedLayers, Action onClose)
        {
            InitializeComponent();
            _selectedLayers = selectedLayers;
            _selectedLayers.ForEach(layer => layer.SaveToMemento());
            Closing += (sender, args) => onClose.Invoke();
        }

        private void ApplyFilter(float[] raw)
        {
            float brightness = (float) (Brightness.Value / 1000f);
            float contrast = (float) (Contrast.Value / 1000f);

            for (var i = 0; i < raw.Length; i += 4)
            {
                for (int j = i; j < i + 3; j++)
                {
                    raw[j] = contrast * (raw[j] - 0.5f) + 0.5f;
                    raw[j] += brightness;
                    ColorUtils.FixValueIfNeed(ref raw[j]);
                }
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