using System;
using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor.ViewModel.model
{
    public class Layer : ILayer
    {
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

        public string Name { get; set; } = "Layer 1";

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
        public float[] CachedRaw { get; set; }
        public int ScaledWidth { get; set; }
        public int ScaledHeight { get; set; }

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
    }
}