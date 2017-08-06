using System.Drawing;

namespace ImageEditor.Interface.ViewModel.model
{
    public class NewFileData
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Color Color { get; set; }

        public NewFileData(string name, int width, int height, Color color)
        {
            Name = name;
            Width = width;
            Height = height;
            Color = color;
        }
    }
}