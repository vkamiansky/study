using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor.filters
{
    /// <summary>
    /// Логика взаимодействия для TWindow.xaml
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class TWindow : Window
    {
        private readonly List<ILayer> _selectedLayers;

        public TWindow(List<ILayer> selectedLayers, Action onClose)
        {
            InitializeComponent();
            _selectedLayers = selectedLayers;
            _selectedLayers.ForEach(layer => layer.SaveToMemento());
            Closing += (sender, args) => onClose.Invoke();
            Update();
        }

        private void ApplyFilter(float[] raw)
        {
            float threshold = (float) (Threshold.Value / 255f);

            for (var i = 0; i < raw.Length; i += 4)
            {
                float a = (raw[i] + raw[i + 1] + raw[i + 2]) / 3f;
                raw[i] = raw[i + 1] = raw[i + 2] = a < threshold ? 0f : 1f;
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