using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ImageEditor.Interface.ViewModel.model;
using Property;

namespace ImageEditor
{
    /// <summary>
    /// Логика взаимодействия для HistogramWindow.xaml
    /// </summary>
    public partial class HistogramWindow
    {
        private readonly IProperty<CanvasSource> _imageSource;

        public HistogramWindow(IProperty<CanvasSource> imageSource)
        {
            _imageSource = imageSource;
            InitializeComponent();
            if (imageSource == null)
            {
                Close();
                return;
            }
            var disposable = imageSource.OnChanged(() => { OnNewCanvasSource(imageSource.Value); });
            Closing += (sender, args) => disposable.Dispose();
            SizeChanged += (sender, args) => OnNewCanvasSource(imageSource.Value);
        }

        public void OnNewCanvasSource(CanvasSource canvasSource)
        {
            int width = (int) (Container.ActualWidth * 1.25d);
            int height = (int) ((Container.ActualHeight - StackPanel.ActualHeight) * 1.25d);

            if (width == 0 || height == 0 || canvasSource == null)
            {
                return;
            }
            
            Debugger.Log(1, "info", "width " + width + " height " + height + "\n");

            float[] dest = new float[width * height * Constants.ChannelsCount];

            if (All.IsChecked == true)
            {
                RGB(canvasSource.Raw, dest, width, height, true, true, true);
            }
            else if (Red.IsChecked == true)
            {
                RGB(canvasSource.Raw, dest, width, height, true, false, false);
            }
            else if (Green.IsChecked == true)
            {
                RGB(canvasSource.Raw, dest, width, height, false, true, false);
            }
            else if (Blue.IsChecked == true)
            {
                RGB(canvasSource.Raw, dest, width, height, false, false, true);
            }
            else
            {
                GrayScale(canvasSource.Raw, dest, width, height);
            }

            Canvas.CleanCanvasSource = new CanvasSource(dest, width, height);
        }

        public void GrayScale(float[] src, float[] dest, int w2, int h2)
        {
            var map = new Dictionary<int, int>();

            for (int i = 0; i <= 255; i++)
            {
                map[i] = 0;
            }

            for (var i = 0; i < src.Length; i += 4)
            {
                var a = src[i + 3];
                var r = src[i] * a * 255f;
                var g = src[i + 1] * a * 255f;
                var b = src[i + 2] * a * 255f;
                int c = (int) ((r + g + b) / 3);
                map[c]++;
            }

            Normalize(map);

            var max = 0;

            for (var i = 0; i < map.Count; i++)
            {
                if (map[i] > max) max = map[i];
            }
            max /= 3;
            Debugger.Log(1, "info", "max " + max + "\n");
            float dw = w2 / 256f;
            float dh = h2 * 1f / max;

            for (int i = 0; i < h2; i++)
            {
                for (int j = 0; j < w2; j++)
                {
                    int srcIndex = (int) (j / dw);
                    int srcIndex2 = srcIndex < 255 ? srcIndex + 1 : 255;
                    float t = j / dw - srcIndex;
                    float l = (1 - t) * map[srcIndex] + t * map[srcIndex2];
                    if ((h2 - i) / dh > l) continue;
                    int destIndex = (i * w2 + j) * Constants.ChannelsCount;
                    dest[destIndex] = 0.8f;
                    dest[destIndex + 1] = 0.8f;
                    dest[destIndex + 2] = 0.8f;
                    dest[destIndex + 3] = 1f;
                }
            }
        }

        public void RGB(float[] src, float[] dest, int w2, int h2, bool R, bool G, bool B)
        {
            var mapR = new Dictionary<int, int>();
            var mapG = new Dictionary<int, int>();
            var mapB = new Dictionary<int, int>();

            for (int i = 0; i <= 255; i++)
            {
                mapR[i] = 0;
                mapG[i] = 0;
                mapB[i] = 0;
            }

            for (var i = 0; i < src.Length; i += 4)
            {
                var a = src[i + 3];
                if (R) mapR[(int) (src[i] * a * 255f)]++;
                if (B) mapB[(int) (src[i + 1] * a * 255f)]++;
                if (G) mapG[(int) (src[i + 2] * a * 255f)]++;
            }

            if (R) Normalize(mapR);
            if (G) Normalize(mapG);
            if (B) Normalize(mapB);

            var maxR = 0;
            var maxG = 0;
            var maxB = 0;

            if (R)
            {
                for (var i = 0; i < mapR.Count; i++)
                {
                    if (mapR[i] > maxR) maxR = mapR[i];
                }
            }
            if (G)
            {
                for (var i = 0; i < mapG.Count; i++)
                {
                    if (mapG[i] > maxG) maxG = mapG[i];
                }
            }
            if (B)
            {
                for (var i = 0; i < mapB.Count; i++)
                {
                    if (mapB[i] > maxB) maxB = mapB[i];
                }
            }

            float dw = w2 / 256f;
            float dhR = h2 * 1f / maxR;
            float dhG = h2 * 1f / maxG;
            float dhB = h2 * 1f / maxB;

            for (int i = 0; i < h2; i++)
            {
                for (int j = 0; j < w2; j++)
                {
                    int destIndex = (i * w2 + j) * Constants.ChannelsCount;

                    int srcIndex = (int) (j / dw);
                    int srcIndex2 = srcIndex < 255 ? srcIndex + 1 : 255;
                    float t = j / dw - srcIndex;

                    if (R && (h2 - i) / dhR <= (1 - t) * mapR[srcIndex] + t * mapR[srcIndex2])
                    {
                        dest[destIndex + 2] = 1f;
                        dest[destIndex + 3] = 1f;
                    }
                    if (G && (h2 - i) / dhG <= (1 - t) * mapG[srcIndex] + t * mapG[srcIndex2])
                    {
                        dest[destIndex + 1] = 1f;
                        dest[destIndex + 3] = 1f;
                    }
                    if (B && (h2 - i) / dhB <= (1 - t) * mapB[srcIndex] + t * mapB[srcIndex2])
                    {
                        dest[destIndex] = 1f;
                        dest[destIndex + 3] = 1f;
                    }
                }
            }
        }

        public void Normalize(Dictionary<int, int> dic)
        {
            float[] n = {0.25f, 0.5f, 0.25f};

            Dictionary<int, int> dic2 = new Dictionary<int, int>(dic.Count);

            for (var i = 0; i < dic2.Count; i++)
            {
                dic2[i] = 0;
            }

            for (var i = 1; i < dic.Count - 1; i++)
            {
                dic2[i] = (int) (n[0] * dic[i - 1] + n[1] * dic[i] + n[2] * dic[i]);
            }
            dic2[0] = (int) (dic[0] * (n[0] + n[1]) + dic[1] * n[2]);
            dic2[dic.Count - 1] = (int) (dic[dic.Count - 2] * n[0] + dic[dic.Count - 2] * (n[1] + n[2]));

            for (var i = 0; i < dic2.Count; i++)
            {
                dic[i] = dic2[i];
            }
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            if (_imageSource.Value == null) return;
            OnNewCanvasSource(_imageSource.Value);
        }
    }
}