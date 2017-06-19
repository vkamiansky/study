using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ImageEditor.ViewModel.model.Constants;

namespace ImageEditor.ViewModel.model
{
    public class Layer
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsSelected { get; set; }

        public int Width { get; }
        public int Height { get; }

        private float opacity;

        public float[] raw { get; set; }
        public float[] cachedRaw { get; set; }
        public int ScaledWidth { get; set; }
        public int ScaledHeight { get; set; }

        public Layer(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;

            raw = new float[width * height * ChannelsCount];
        }

        public Layer(int x, int y, int width, int height, float[] raw)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            this.raw = raw;
        }
    }
}