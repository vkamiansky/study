using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor.scalers
{
    public class StubScaler : ImageScaler
    {
        public float[] Scale(float[] source, int w1, int h1, int w2, int h2, int xStart = 0, int yStart = 0)
        {
            return source;
        }
    }
}