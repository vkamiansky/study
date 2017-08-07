using System;
using System.Diagnostics;
using System.Windows.Documents;
using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor.ViewModel.model
{
    public class Layer : ILayer
    {
        private const float DivideByZeroFix = 1f / 99999999;

        public double X { get; set; }
        public double Y { get; set; }
        private bool _isSelected;
        private int _width;
        private int _height;
        private float _opacity = 1f;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnChanged?.Invoke();
            }
        }

        public string Name { get; set; }

        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                OnChanged?.Invoke();
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                OnChanged?.Invoke();
            }
        }

        public float Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                OnChanged?.Invoke();
            }
        }

        public float[] Raw { get; set; }

        public Action OnChanged { get; set; }

        public Layer(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            _width = width;
            Height = height;

            Raw = new float[width * height * Constants.ChannelsCount];
        }

        public Layer(int x, int y, int width, int height, float[] raw)
        {
            X = x;
            Y = y;
            _width = width;
            Height = height;
            Raw = raw;
        }

        public void Compose(float[] b, int widthB, int heightB,
            int bX = 0, int bY = 0, int aX = 0, int aY = 0)
        {
            Compose(Raw, Width, Height, b, widthB, heightB, (int) X, (int) Y, aX, aY, Opacity);
        }


        //ax, ay - start get data from "a" array from these indexes  
        //bx, by - start put data to "b" array from these indexes  
        private void Compose(float[] a, int widthA, int heightA, float[] b, int widthB, int heightB,
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
                    int srcIndex = (y1 * widthA + x1) * Constants.ChannelsCount;
                    int destIndex = (y2 * widthB + x2) * Constants.ChannelsCount;

                    float aa = a[srcIndex + 3] * opacity;
                    float ab = b[destIndex + 3];
                    float naa = 1 - aa;
                    float div = aa + ab * naa + DivideByZeroFix;

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

        public void Draw(int x, int y, float[] brush, int size)
        {
            MapCoord(ref x, ref y);
            x -= size;
            y -= size;

            Compose(brush, size * 2, size * 2, Raw, Width, Height, bX: x, bY: y);
        }

        public void Expand(int x, int y, int size, int maxWidth, int maxHeight)
        {
            MapCoord(ref x, ref y);
            MapCoord(ref maxWidth, ref maxHeight);
            int left = 0, right = 0, top = 0, bottom = 0;
            int t = x - size;
            if (t < 0)
            {
                left = -t;
                if (left > X) left = (int) X;
            }
            t = x + size;
            if (t > _width)
            {
                if (t > maxWidth ) t = maxWidth;
                right = t - _width;
            }
            t = y - size;
            if (t < 0)
            {
                top = -t;
                if (top > Y) top = (int) Y;
            }
            t = y + size;
            if (t > _height)
            {
                if (t > maxHeight) t = maxHeight;
                bottom = t;
            }
            //left = left < 0 ? 0 : left;
            //top = top < 0 ? 0 : top;
            //right = right < 0 ? 0 : right;
            //bottom = bottom < 0 ? 0 : bottom;
            Resize(left, top, right, bottom);
        }

        // left +20 add 20 to start of layer
        // left -20 remove 20 from start
        public void Resize(int left = 0, int top = 0, int right = 0, int bottom = 0)
        {
            if (left == 0 && top == 0 && right == 0 && bottom == 0) return;

            int width = _width + left + right;
            int height = _height + top + bottom;

            float[] raw = new float[width * height * Constants.ChannelsCount];

            int x = left < 0 ? 0 : left;
            int y = top < 0 ? 0 : top;

            for (int y1 = 0, y2 = y; y1 < Height && y2 < height; y1++, y2++)
            {
                for (int x1 = 0, x2 = x; x1 < Width && x2 < width; x1++, x2++)
                {
                    int sourceIndex = (y1 * Width + x1) * Constants.ChannelsCount;
                    int destIndex = (y2 * width + x2) * Constants.ChannelsCount;

                    for (int i = 0; i < Constants.ChannelsCount; i++)
                        raw[destIndex + i] = Raw[sourceIndex + i];
                }
            }

            X -= left;
            Y -= top;
            _width = width;
            _height = height;
            Raw = raw;
        }

        private void MapCoord(ref int x, ref int y)
        {
            x -= (int) (X + .5f);
            y -= (int) (Y + .5f);
        }

        public bool NeedExpandForDraw(int x, int y, int size, int maxWidth, int maxHeight)
        {
            MapCoord(ref x, ref y);
            var rightEdge = X + Width;
            var bottomEdge = Y + Height;
            return x - size < 0 || y - size < 0
                   || Width < x + size && rightEdge < maxWidth
                   || Height < y + size && bottomEdge < maxHeight;
        }
    }
}