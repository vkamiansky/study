using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using ImageEditor.Interface.ViewModel;
using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor.filters
{
    /// <summary>
    /// Логика взаимодействия для GBWindow.xaml
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class GBWindow
    {
        private readonly List<ILayer> _selectedLayers;
        private readonly DispatcherTimer _debounceTimer = new DispatcherTimer();

        public GBWindow(List<ILayer> selectedLayers, Action onClose)
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
                float r = (float) (Radius.Value / 100f);
                GaussianBlur(selectedLayer.Raw, selectedLayer.Width, selectedLayer.Height, r);
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

        private void GaussianBlur(float[] src, int w, int h, float rad)
        {
            float[] dest = src.CloneArray();

            float rs = (float) Math.Ceiling(rad * 2.57); // significant radius
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    float r = 0f;
                    float g = 0f;
                    float b = 0f;
                    float a = 0f;
                    float wsum = 0;
                    for (float iy = i - rs; iy < i + rs + 1; iy++)
                    {
                        for (float ix = j - rs; ix < j + rs + 1; ix++)
                        {
                            float x = ColorUtils.Min(w - 1, ColorUtils.Max(0, ix));
                            float y = ColorUtils.Min(h - 1, ColorUtils.Max(0, iy));
                            float dsq = (ix - j) * (ix - j) + (iy - i) * (iy - i);
                            float wght = (float) (Math.Exp(-dsq / (2 * rad * rad)) / (Math.PI * 2 * rad * rad));
                            int si = ((int) (y * w + x)) * 4;
                            b += src[si] * wght;
                            g += src[si + 1] * wght;
                            r += src[si + 2] * wght;
                            a += src[si + 3] * wght;
                            wsum += wght;
                        }
                    }
                    int di = (i * w + j) * 4;
                    dest[di] = b / wsum;
                    dest[di + 1] = g / wsum;
                    dest[di + 2] = r / wsum;
                    dest[di + 3] = a / wsum;
                }
            }

            dest.CopyTo(src, 0);
        }

        private void Radius_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _debounceTimer.Stop();
            float r = (float) (Radius.Value / 100f);
            RadLabel.Content = $"Radius: {r:0.00}";
            _debounceTimer.Start();
        }
    }
}