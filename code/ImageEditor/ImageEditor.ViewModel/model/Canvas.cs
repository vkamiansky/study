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

        private float[] _raw;
        public List<Layer> Layers { get; }

        public Canvas(int width, int height, float[] imgRaw, string name = "")
        {
            Height = height;
            Width = width;
            Length = Height * Width * ChannelsCount;

            _raw = GenerateBackground();
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
            var result = GenerateBackground();

            foreach (var layer in Layers)
            {
                if (!layer.IsSelected) continue;

                compose(layer.Raw, layer.Width, layer.Height,
                    result, Width, Height, (int) (layer.X), (int) (layer.Y), 0, 0, layer.Opacity);
            }
            return result;
        }

        private float[] _cachedBackgroung;

        private float[] GenerateBackground()
        {
            float[] rawArr;
            if (_cachedBackgroung != null && _cachedBackgroung.Length == Length)
            {
                rawArr = _cachedBackgroung;
            }
            else
            {
                rawArr = new float[Length];
                _cachedBackgroung = rawArr;
            }
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int index = (y * Width + x) * ChannelsCount;
                    var d = y / BgTileSide % 2;
                    var f = x / BgTileSide % 2;
                    if (d == 0 && f == 0 || d != 0 && f != 0)
                    {
                        rawArr[index + 0] = BgWhite;
                        rawArr[index + 1] = BgWhite;
                        rawArr[index + 2] = BgWhite;
                        rawArr[index + 3] = Opaque;
                    }
                    else
                    {
                        rawArr[index + 0] = BgGrey;
                        rawArr[index + 1] = BgGrey;
                        rawArr[index + 2] = BgGrey;
                        rawArr[index + 3] = Opaque;
                    }
                }
            }
            return Clone(rawArr);
        }

        public void OnLayersChanged(Action action)
        {
            foreach (var layer in Layers)
            {
                layer.OnChanged = action;
            }
        }

        private float[] Clone(float[] arr)
        {
            float[] clone = new float[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                clone[i] = arr[i];
            }
            return clone;
        }
        
        private void compose(float[] a, int widthA, int heightA, float[] b, int widthB, int heightB,
            int bX = 0, int bY = 0, int aX = 0, int aY = 0, float opacity = 1f)
        {
            if (bX < 0)
            {
                aX += -bX;
                bX = 0;
            }

            if (bY < 0)
            {
                aY += -bY;
                bY = 0;
            }

            for (int y1 = aY, y2 = bY; y1 < heightA && y2 < heightB; y1++, y2++)
            {
                for (int x1 = aX, x2 = bX; x1 < widthA && x2 < widthB; x1++, x2++)
                {
                    int srcIndex = (y1 * widthA + x1) * ChannelsCount;
                    int destIndex = (y2 * widthB + x2) * ChannelsCount;

                    float aa = a[srcIndex + 3] * opacity;
                    float ab = b[destIndex + 3];
                    float naa = 1 - aa;
                    float div = aa + ab * naa;

                    b[destIndex + 0] =
                        (a[srcIndex + 0] * aa + b[destIndex + 0] * ab * naa) / div;

                    b[destIndex + 1] =
                        (a[srcIndex + 1] * aa + b[destIndex + 1] * ab * naa) / div;

                    b[destIndex + 2] =
                        (a[srcIndex + 2] * aa + b[destIndex + 2] * ab * naa) / div;

                    b[destIndex + 3] = div;
                }
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

            //x = (int) (x / Scale + 0.5);
            //y = (int) (y / Scale + 0.5);

            float[] col = NormalizeColor(color);

            int x1 = x - size;
            int y1 = y - size;

            float d, d2;

            var h = size * 2;
            var w = size * 2;

            float[] brush = new float[h * w * ChannelsCount];

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    d2 = dist2(j, i, size, size);
                    if (d2 < size * size)
                    {
                        d = (float) Math.Sqrt(d2);
                        int index = (i * w + j) * ChannelsCount;
                        brush[index + 0] = col[0];
                        brush[index + 1] = col[1];
                        brush[index + 2] = col[2];
                        brush[index + 3] = col[3] * (1 - d / size);
                    }
                }
            }

            compose(brush, w, h, selectedLayer.Raw,
                selectedLayer.Width, selectedLayer.Height, x1, y1, 0, 0, opacity);
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
                        selectedLayer.Raw[index + 3] *=  d / size * opacity + 0.001f;
                    }
                }
            }
        }
    }
}