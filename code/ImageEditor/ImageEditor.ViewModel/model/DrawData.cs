namespace ImageEditor.ViewModel.model
{
    public class DrawData
    {
        private readonly float[] _brush;
        private readonly int _size;
        private readonly int _x;
        private readonly int _y;

        public DrawData(float[] brush, int size, int x, int y)
        {
           _brush = brush;
           _size = size;
           _x = x;
           _y = y;
        }

        public void Draw(Layer layer, float[] raw, int width, int height)
        {
            layer.Compose(_brush, _size * 2, _size * 2, raw, width, height, bX: _x, bY: _y);
        }
    }
}