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
using ImageEditor.Interface.ViewModel;
using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor.filters
{
    /// <summary>
    /// Логика взаимодействия для HSWindow.xaml
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class HSWindow
    {
        private readonly List<ILayer> _selectedLayers;

        public HSWindow(List<ILayer> selectedLayers, Action onClose)
        {
            InitializeComponent();
            _selectedLayers = selectedLayers;
            _selectedLayers.ForEach(layer => layer.SaveToMemento());
            Closing += (sender, args) => onClose.Invoke();
        }

        private void ApplyFilter(float[] raw)
        {
            float hue = (float) (Hue.Value / 1000f);
            float saturation = (float) (Saturation.Value / 1000f);
            float lightness = (float) (Lightness.Value / 1000f);

            for (var i = 0; i < raw.Length; i += 4)
            {
                float h, s, l;
                ColorUtils.RgbToHsl(raw[i], raw[i + 1], raw[i + 2], out h, out s, out l);

                h += hue;
                s += saturation;
                l += lightness;

                FixValueIfNeed(ref h);
                FixValueIfNeed(ref s);
                FixValueIfNeed(ref l);

                ColorUtils.Hsl2Rgb(h, s, l, out raw[i + 2], out raw[i + 1], out raw[i]);
            }
        }

        private void FixValueIfNeed(ref float v)
        {
            if (v < 0) v = 0f;
            if (v > 1) v = 1f;
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