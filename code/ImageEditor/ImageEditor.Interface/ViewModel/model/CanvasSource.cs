using System.Drawing;

namespace ImageEditor.Interface.ViewModel.model
{
    public class CanvasSource
    {
        public float[] Raw { get; private set; }
        public int Width { get; private set;}
        public int Height { get; private set;}
        public float Scale { get; private set; }

        public CanvasSource(float[] raw, int width, int height, float scale)
        {
            Raw = raw;
            Width = width;
            Height = height;
            Scale = scale;
        }

        public void ApplyScale(ImageScaler scaler)
        {
            int newWidth = (int) (Width * Scale);
            int newHeight = (int) (Height * Scale);
            Raw = scaler.Scale(Raw, Width, Height, newWidth, newHeight);
            Width = newWidth;
            Height = newHeight;
            Scale = 1;
        }
    }
}