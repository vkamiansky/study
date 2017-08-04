using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using static ImageEditor.ViewModel.model.Constants;

namespace ImageEditor.ViewModel.model
{
    public class Canvas
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }

        public float Scale
        {
            get => _scale;
            set
            {
                _isDirty = true;
                _scale = value;
            }
        }

        private readonly int _width;
        private readonly int _height;
        private readonly int _length;
        private float[] _raw;
        public List<Layer> Layers { get; }
        private float _scale = 1f;
        #pragma warning disable 414
        private bool _isDirty = true;
        #pragma warning restore 414

        public Canvas(int width, int height, float[] imgRaw)
        {
            Height = height;
            Width = width;
            Length = Height * Width * ChannelsCount;
            _width = width;
            _height = height;
            _length = width * height * ChannelsCount;

            _raw = GenerateBackground();
            Layers = new List<Layer>();
            var layer = new Layer(0, 0, width, height, imgRaw);
            layer.IsSelected = true;
            Layers.Add(layer);
        }

        public float[] GetRaw()
        {
            Stopwatch sw = new Stopwatch();

            Console.WriteLine("Start raw");
            sw.Start();

            Height = (int) ((_height - 1) * _scale);
            Width = (int) ((_width - 1) * _scale);
            Length = Height * Width * ChannelsCount;

            var result = GenerateBackground();

            foreach (var layer in Layers)
            {
                if (!layer.IsSelected) continue;

                layer.ScaledWidth = (int) (layer.Width * _scale);
                layer.ScaledHeight = (int) (layer.Height * _scale);
                layer.CachedRaw = ApplyScale(layer.Raw, layer.Width, layer.Height, layer.ScaledWidth,
                    layer.ScaledHeight);


                compose(layer.CachedRaw, layer.ScaledWidth, layer.ScaledHeight,
                    result, Width, Height, (int) (layer.X), (int) (layer.Y), 0, 0, layer.Opacity);
            }


            sw.Stop();
            Console.WriteLine("stop:" + sw.ElapsedMilliseconds);

            return result;
        }

        public void OnSizeChanged(int delta)
        {
            _scale += delta * ScaleRatio;
            _isDirty = true;
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

        private float[] ApplyScale(float[] source, int w1, int h1, int w2, int h2)
        {
            int srcLength = _length;
            int destLength = h2 * w2 * ChannelsCount;

            float[] dest = new float[destLength];

            int dx = 0, dy = 0;

            int x1, y1, x2, y2, i0, i1, i2, i3, i4;
            float a, b, c, d, w, h, xf, yf, nxDiff, nyDiff, m1, m2, m3, m4;

            // don't optimize it, left it for flexibility of code
            float xRatio = 1f / _scale;
            float yRatio = 1f / _scale;


            for (int y = 0; y < h2; y++)
            {
                for (int x = 0; x < w2; x++)
                {
                    xf = (xRatio * (x + dx));
                    yf = (yRatio * (y + dy));

                    x1 = (int) (xf);
                    y1 = (int) (yf);

                    if (x1 < 0 || x1 >= w1 - 1 || y1 < 0 || y1 >= h1 - 1)
                        continue;

                    w = xf - x1;
                    h = yf - y1;


                    i1 = (y1 * w1 + x1) * 4;
                    i2 = (y1 * w1 + x1 + 1) * 4;
                    i3 = ((y1 + 1) * w1 + x1) * 4;
                    i4 = ((y1 + 1) * w1 + x1 + 1) * 4;

                    x2 = x;
                    y2 = y;

                    if (x2 < 0 || x2 >= w2 - 1 || y2 < 0 || y2 >= h2 - 1)
                        continue;

                    i0 = ((y2 * w2 + x2) * 4);

                    nxDiff = 1 - w;
                    nyDiff = 1 - h;

                    m1 = nxDiff * nyDiff;
                    m2 = w * nyDiff;
                    m3 = h * nxDiff;
                    m4 = w * h;

                    for (int i = 0; i < 4; i++)
                    {
                        if (i4 + i >= srcLength || i1 + i < 0
                            || i0 + i < 0 || i0 + i >= destLength) break;
                        a = source[i1 + i];
                        b = source[i2 + i];
                        c = source[i3 + i];
                        d = source[i4 + i];

                        dest[i0 + i] =
                            a * m1
                            + b * m2
                            + c * m3
                            + d * m4;
                    }
                }
            }
            return dest;
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

        public void OnMoved(int dx, int dy)
        {
            var selectedLayer = GetSelectedLayer();
            if (selectedLayer == null) return;
            selectedLayer.X += dx;
            selectedLayer.Y += dy;

            Console.WriteLine("dx: " + dx + " dy: " + dy + " X: " + selectedLayer.X + " Y: " + selectedLayer.Y);
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

            x = (int) (x / Scale + 0.5);
            y = (int) (y / Scale + 0.5);

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

            x = (int) (x / Scale + 0.5);
            y = (int) (y / Scale + 0.5);

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