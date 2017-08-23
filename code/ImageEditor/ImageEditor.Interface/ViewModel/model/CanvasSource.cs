using System;
using System.Drawing;

namespace ImageEditor.Interface.ViewModel.model
{
    public class CanvasSource
    {
        public readonly float[] Raw;
        public readonly int Width;
        public readonly int Height;
        public float Scale { get; private set; }

        public CanvasSource(float[] raw, int width, int height, float scale = 1f)
        {
            Raw = raw;
            Width = width;
            Height = height;
            Scale = scale;
        }

        public CanvasSource ApplyScale(ImageScaler scaler, int maxWidth = 0, int maxHeight = 0)
        {
            int newWidth = (int) (Width * Scale);
            int newHeight = (int) (Height * Scale);
            float[] raw = scaler.Scale(Raw, Width, Height, newWidth, newHeight, xEnd: maxWidth, yEnd: maxHeight);
            return new CanvasSource(raw, newWidth, newHeight);
        }

        public CanvasSource ApplyBackground()
        {
            float[] raw = new float[Raw.Length];

            Array.Copy(Raw, raw, Raw.Length);
            
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int index = (y * Width + x) * Constants.ChannelsCount;
                    if (Raw[index + 3] > 0.99) continue;
                    var r = (y / Constants.BgTileSide % 2) ^ (x / Constants.BgTileSide % 2);
                    float c = (1 - r) * Constants.White + r * Constants.Grey;

                    float aa = Raw[index + 3];
                    var f = c * (1 - aa);
                    raw[index + 0] = Raw[index + 0] * aa + f;
                    raw[index + 1] = Raw[index + 1] * aa + f;
                    raw[index + 2] = Raw[index + 2] * aa + f;
                    raw[index + 3] = Constants.Opaque;
                }
            }
            
            return new CanvasSource(raw, Width, Height, Scale);
        }
    }
}