using System.Drawing;

namespace ImageEditor.Interface.ViewModel.model
{
    public class CanvasSource
    {
        public float[] Raw { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public float Scale { get; private set; }

        public CanvasSource(float[] raw, int width, int height, float scale)
        {
            Raw = raw;
            Width = width;
            Height = height;
            Scale = scale;
        }

        public void ApplyScale(ImageScaler scaler, int maxWidth = 0, int maxHeight = 0)
        {
            int newWidth = (int) (Width * Scale);
            int newHeight = (int) (Height * Scale);
            Raw = scaler.Scale(Raw, Width, Height, newWidth, newHeight, xEnd: maxWidth, yEnd: maxHeight);
            Width = newWidth;
            Height = newHeight;
            Scale = 1;
        }

        public void ApplyBackground()
        {
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
                    Raw[index + 0] = Raw[index + 0] * aa + f;
                    Raw[index + 1] = Raw[index + 1] * aa + f;
                    Raw[index + 2] = Raw[index + 2] * aa + f;
                    Raw[index + 3] = Constants.Opaque;
                }
            }
        }
    }
}