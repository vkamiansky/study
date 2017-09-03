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
using System.Windows.Threading;
using ImageEditor.Interface.ViewModel;
using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor.filters
{
    /// <summary>
    /// Логика взаимодействия для BBWindow.xaml
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class BBWindow
    {
        private readonly List<ILayer> _selectedLayers;
        private readonly DispatcherTimer _debounceTimer = new DispatcherTimer();

        public BBWindow(List<ILayer> selectedLayers, Action onClose)
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

            var bxs = BoxesForGauss(rad, 3);
            float rs = (bxs[0] - 1) / 2f;
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    float r = 0f;
                    float g = 0f;
                    float b = 0f;
                    float a = 0f;

                    for (float iy = i - rs; iy < i + rs + 1; iy++)
                    {
                        for (float ix = j - rs; ix < j + rs + 1; ix++)
                        {
                            float x = ColorUtils.Min(w - 1, ColorUtils.Max(0, ix));
                            float y = ColorUtils.Min(h - 1, ColorUtils.Max(0, iy));
                            int si = ((int) (y * w + x)) * 4;
                            b += src[si];
                            g += src[si + 1];
                            r += src[si + 2];
                            a += src[si + 3];
                        }
                    }
                    int di = (i * w + j) * 4;

                    var den = 1f / ((rs + rs + 1) * (rs + rs + 1));
                    dest[di] = b * den;
                    dest[di + 1] = g * den;
                    dest[di + 2] = r * den;
                    dest[di + 3] = a * den;
                }
            }

            dest.CopyTo(src, 0);
        }

        private float[] BoxesForGauss(float sigma, int n) // standard deviation, number of boxes
        {
            var wIdeal = Math.Sqrt((12 * sigma * sigma / n) + 1); // Ideal averaging filter width 
            var wl = Math.Floor(wIdeal);
            if (wl % 2 == 0) wl--;
            var wu = wl + 2;

            var mIdeal = (12 * sigma * sigma - n * wl * wl - 4 * n * wl - 3 * n) / (-4 * wl - 4);
            var m = Math.Round(mIdeal);
            // var sigmaActual = Math.sqrt( (m*wl*wl + (n-m)*wu*wu - n)/12 );

            float[] sizes = new float[n];
            for (var i = 0; i < n; i++) sizes[i] = (float) (i < m ? wl : wu);
            return sizes;
        }

        private void Radius_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _debounceTimer.Stop();
            RadLabel.Content = $"Radius: {Radius.Value:0}";
            _debounceTimer.Start();
        }
    }
}