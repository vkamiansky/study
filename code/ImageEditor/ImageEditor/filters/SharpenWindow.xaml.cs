using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using ImageEditor.Interface.ViewModel;
using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor.filters
{
    /// <summary>
    /// Логика взаимодействия для SharpenWindow.xaml
    /// </summary>
    public partial class SharpenWindow
    {
        private readonly List<ILayer> _selectedLayers;
        private readonly DispatcherTimer _debounceTimer = new DispatcherTimer();
        private readonly float[] _sharpenKernel = {0f, -1f, 0f, -1f, 5f, -1f, 0f, -1f, 0f};

        public SharpenWindow(List<ILayer> selectedLayers, Action onClose)
        {
            InitializeComponent();
            _selectedLayers = selectedLayers;
            _selectedLayers.ForEach(layer => layer.SaveToMemento());
            Closing += (sender, args) => onClose.Invoke();
            _debounceTimer.Interval = TimeSpan.FromMilliseconds(250);
            _debounceTimer.Tick += (sender, args) => Update();
            Update();
        }

        private void Update()
        {
            _debounceTimer.Stop();
            if (_selectedLayers == null || _selectedLayers.Count == 0) return;
            foreach (var selectedLayer in _selectedLayers)
            {
                selectedLayer.RestoreFromMemento();
                float r = (float) Radius.Value;
                Sharpen(selectedLayer.Raw, selectedLayer.Width, selectedLayer.Height, r);
                selectedLayer.OnChanged?.Invoke();
            }
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

        private void Sharpen(float[] src, int w, int h, float rad)
        {
            var kernel = _sharpenKernel.CloneArray();
            var m = rad / 1000f;
            var f = m / 4f;
            if (m > 0)
            {
                kernel[4] += m;
            }
            else
            {
                kernel[1] += f;
                kernel[3] += f;
                kernel[5] += f;
                kernel[7] += f;
            }
            ColorUtils.KernelProcess(src, w, h, kernel);
        }

        private void Radius_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _debounceTimer.Stop();
            RadLabel.Content = $"Radius: {Radius.Value:0}";
            _debounceTimer.Start();
        }
    }
}