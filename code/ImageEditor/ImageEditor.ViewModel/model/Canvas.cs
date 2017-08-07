using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using static ImageEditor.Interface.ViewModel.model.Constants;

namespace ImageEditor.ViewModel.model
{
    public class Canvas
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }

        public List<Layer> Layers { get; }

        public Canvas(int width, int height, float[] imgRaw, string name = "")
        {
            Height = height;
            Width = width;
            Length = Height * Width * ChannelsCount;

            Layers = new List<Layer>();
            var layer = new Layer(0, 0, width, height, imgRaw)
            {
                IsSelected = true,
                Name = name
            };
            Layers.Add(layer);
        }

        public float[] GetRaw()
        {
            var result = new float[Length];

            foreach (var layer in Layers)
            {
                if (!layer.IsSelected) continue;

                layer.Compose(result, Width, Height);
            }
            return result;
        }

        public void OnLayersChanged(Action action)
        {
            foreach (var layer in Layers)
            {
                layer.OnChanged = action;
            }
        }

        public void OnMoved(double dx, double dy)
        {
            var selectedLayer = GetSelectedLayer();
            if (selectedLayer == null) return;
            selectedLayer.X += dx;
            selectedLayer.Y += dy;

            Debug.WriteLine($"dx: {dx:F2} dy: {dy:F2}  X: {selectedLayer.X:F2} Y: {selectedLayer.Y:F3}");
        }

        private Layer GetSelectedLayer()
        {
            foreach (var layer in Layers)
            {
                if (layer.IsSelected)
                    return layer;
            }

            return null;
        }

        public void Draw(int x, int y, int size, float opacity, Color color)
        {
            var selectedLayer = GetSelectedLayer();
            if (selectedLayer == null) return;

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
            var selectedLayer = GetSelectedLayer();
            if (selectedLayer == null) return;

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
    }
}