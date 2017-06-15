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
        private int X;
        private int Y;

        private int Width;
        private int Height;

        public float[] raw { get; }

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
