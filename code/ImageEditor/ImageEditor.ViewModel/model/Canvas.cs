using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ImageEditor.ViewModel.model.Constants;

namespace ImageEditor.ViewModel.model
{
    public class Canvas
    {
        public int Width { get; }
        public int Height { get; }
        public int Length { get; }

        private float[] raw;
        private List<Layer> layers;
        private float _scale = 1f;

        public Canvas(int width, int height, float[] imgRaw)
        {
            Width = width;
            Height = height;
            Length = width * height * ChannelsCount;

            raw = new float[Length];
            layers = new List<Layer>();
            layers.Add(new Layer(0, 0, width, height, imgRaw));
        }

        public float[] GetRaw()
        {
            return layers[0].raw;
        }
    }
}