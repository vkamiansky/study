using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ImageEditor.ViewModel.model.Constants;

namespace ImageEditor.ViewModel.model
{
    public class Canvas
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }

        private readonly int _width;
        private readonly int _height;
        private readonly int _length;
        private float[] raw;
        private List<Layer> layers;
        private float _scale = 1f;
        private bool _isDirty = true;

        public Canvas(int width, int height, float[] imgRaw)
        {
            Height = height;
            Width = width;
            Length = Height * Width * ChannelsCount;
            _width = width;
            _height = height;
            _length = width * height * ChannelsCount;

            raw = GenerateBackground();
            layers = new List<Layer>();
            var layer = new Layer(0, 0, width, height, imgRaw);
            layer.IsSelected = true;
            layers.Add(layer);
        }


        public float[] GetRaw()
        {
            Stopwatch sw = new Stopwatch();
            
            Console.WriteLine("Start raw");
            sw.Start();

            Height = (int) (_height * _scale);
            Width = (int) (_width * _scale);
            Length = Height * Width * ChannelsCount;

            var result = GenerateBackground();

            foreach (var layer in layers)
            {
                if (_isDirty)
                {
                    layer.ScaledWidth = (int) (layer.Width * _scale);
                    layer.ScaledHeight = (int) (layer.Height * _scale);
                    layer.cachedRaw = ApplyScale(layer.raw, layer.Width, layer.Height, layer.ScaledWidth,
                        layer.ScaledHeight);
                }

                compose(layer.cachedRaw, layer.ScaledWidth, layer.ScaledHeight,
                    result, Width, Height, layer.X, layer.Y);
            }

            _isDirty = false;

     
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
            return rawArr;
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
            float x_ratio = 1f / _scale;
            float y_ratio = 1f / _scale;


            for (int y = 0; y < h2; y++)
            {
                for (int x = 0; x < w2; x++)
                {
                    xf = (x_ratio * (x + dx));
                    yf = (y_ratio * (y + dy));

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

                    x2 = (int) (x);
                    y2 = (int) (y);

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
            selectedLayer.X += dx;
            selectedLayer.Y += dy;

            Console.WriteLine("dx: " + dx + " dy: " + dy + " X: " + selectedLayer.X + " Y: " + selectedLayer.Y);
        }

        private Layer GetSelectedLayer()
        {
            foreach (var layer in layers)
            {
                if (layer.IsSelected)
                    return layer;
            }

            return layers[0];
        }
    }
}