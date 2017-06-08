namespace ImageUsageSample.Entities
{
    public struct SelectionRectangle
    {
        public int Height, Width, Top, Left;

        public SelectionRectangle(int height, int width, int top, int left)
        {
            Height = height;
            Width = width;
            Top = top;
            Left = left;
        }
    }
}
