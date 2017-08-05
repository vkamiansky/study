namespace ImageEditor.Interface.ViewModel.model
{
    // ReSharper disable once InconsistentNaming
    public interface ImageScaler
    {
        
        
        float[] Scale(float[] source, int w1, int h1, int w2, int h2, int xStart = 0, int yStart = 0);
    }
}