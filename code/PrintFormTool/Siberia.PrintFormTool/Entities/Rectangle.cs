namespace Siberia.PrintFormTool.Entities
{
    public struct Rectangle
    {
        public int Height, Width, Top, Left;

        public Rectangle(int height, int width, int top, int left)
        {
            Height = height;
            Width = width;
            Top = top;
            Left = left;
        }
    }
}
