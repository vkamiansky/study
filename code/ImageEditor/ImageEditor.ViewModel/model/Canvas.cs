using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media;
using ImageEditor.Interface.ViewModel.model;
using static ImageEditor.Interface.ViewModel.model.Constants;

namespace ImageEditor.ViewModel.model
{
    public class Canvas
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
        public string FilePath { get; set; }
        public List<Layer> Layers { get; }

        private float[] _lastRaw;

        public Canvas(int width, int height, float[] imgRaw, string path = "")
        {
            Height = height;
            Width = width;
            Length = Height * Width * ChannelsCount;
            Layers = new List<Layer>();

            //FilePath = path;
            
            var layer = new Layer(0, 0, width, height, imgRaw)
            {
                Name = path.Length == 0 ? "New layer" : Path.GetFileName(path)?.Split('.')[0]
            };
            Layers.Add(layer);
        }
        
        public Canvas(int width, int height)
        {
            Height = height;
            Width = width;
            Length = Height * Width * ChannelsCount;
            Layers = new List<Layer>();
        }

        public float[] GetRaw()
        {
            var last = Layers.Last();
            if (last.IsVisible && last.AfterDraw)
            {
                last.ComposeAftedDraw(_lastRaw, Width, Height);
            }
            else
            {
                var result = new float[Length];
                foreach (var layer in Layers)
                {
                    if (!layer.IsVisible) continue;

                    layer.Compose(result, Width, Height);
                }
                _lastRaw = result;
            }

            return _lastRaw;
        }

        private Action _changeTrigger;

        public void OnLayersChanged(Action action)
        {
            _changeTrigger = action;
            foreach (var layer in Layers)
            {
                layer.OnChanged = action;
            }
        }

        public void OnMoved(double dx, double dy)
        {
            var selectedLayers = GetSelectedLayers();
            selectedLayers?.ForEach(layer =>
            {
                layer.X += dx;
                layer.Y += dy;
            });
        }

        private List<Layer> GetSelectedLayers()
        {
            List<Layer> selectedLayers = new List<Layer>();
            Layers.ForEach(layer =>
            {
                if (layer.IsSelected) selectedLayers.Add(layer);
            });
            return selectedLayers;
        }

        public void Draw(int x, int y, int size, float opacity, Color color)
        {
            var selectedLayers = GetSelectedLayers();
            if (selectedLayers == null || selectedLayers.Count > 1) return;

            var selectedLayer = selectedLayers[0];

            float[] brush = CreateBrush(size, opacity, color);

            if (selectedLayer.NeedExpandForDraw(x, y, size, Width, Height))
            {
                selectedLayer.Expand(x, y, size, Width, Height);
            }

            selectedLayer.Draw(x, y, brush, size);
        }

        float[] CreateBrush(int size, float opacity, Color color)
        {
            float[] col = NormalizeColor(color);

            var h = size * 2;
            var w = size * 2;

            float[] brush = new float[h * w * ChannelsCount];

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    var d2 = dist2(j, i, size, size);
                    if (d2 >= size * size) continue;

                    var d = (float) Math.Sqrt(d2);
                    int index = (i * w + j) * ChannelsCount;
                    brush[index + 0] = col[0];
                    brush[index + 1] = col[1];
                    brush[index + 2] = col[2];
                    brush[index + 3] = col[3] * (1 - d / size) * opacity;
                }
            }

            return brush;
        }

        float[] NormalizeColor(Color color)
        {
            float[] colors = new float[4];
            colors[0] = color.B * ColorNormalizeRatio;
            colors[1] = color.G * ColorNormalizeRatio;
            colors[2] = color.R * ColorNormalizeRatio;
            colors[3] = color.A * ColorNormalizeRatio;
            return colors;
        }

        float dist2(float x1, float y1, float x2, float y2)
        {
            return (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
        }

        public void Erase(int x, int y, int size, float opacity)
        {
            var selectedLayers = GetSelectedLayers();
            if (selectedLayers == null || selectedLayers.Count > 1) return;

            var selectedLayer = selectedLayers[0];

            //x = (int) (x / Scale + 0.5);
            //y = (int) (y / Scale + 0.5);

            int x1 = x - size;
            int y1 = y - size;

            var h = size * 2;
            var w = size * 2;

            float d, d2;

            int x2 = y1 + h;
            int y2 = x1 + w;

            if (y1 < 0) y1 = 0;
            if (x1 < 0) x1 = 0;

            if (x2 > selectedLayer.Width) x2 = selectedLayer.Width;
            if (y2 > selectedLayer.Height) y2 = selectedLayer.Height;

            for (int j = y1; j < x2; j++)
            {
                for (int i = x1; i < y2; i++)
                {
                    d2 = dist2(i, j, x, y);
                    if (d2 < size * size)
                    {
                        d = (float) Math.Sqrt(d2);
                        int index = (j * selectedLayer.Width + i) * ChannelsCount;
                        selectedLayer.Raw[index + 3] *= d / size * opacity + 0.001f;
                    }
                }
            }
        }

        public void AddLayer()
        {
            Layers.Add(new Layer(0, 0, Width, Height) {Name = "New_layer", Opacity = 1f, IsSelected = false, OnChanged = _changeTrigger});
        }

        public void AddLayer(Layer layer)
        {
            Layers.Add(layer);
        }

        public void RemoveLayer(ILayer iLayer)
        {
            var layer1 = iLayer as Layer;
            if (layer1 != null)
            {
                Layer layer = layer1;
                Layers.Remove(layer);
            }
        }

        public void DuplicateLayer(ILayer iLayer)
        {
            var layer1 = iLayer as Layer;
            if (layer1 != null)
            {
                Layers.Add(layer1.Clone());
            }
        }
    }
}