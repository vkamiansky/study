using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor.scalers
{
    public class NearestNeighbourScaler : ImageScaler
    {
        public float[] Scale(float[] source, int w1, int h1, int w2, int h2,
            int xStart = 0, int yStart = 0, int xEnd = 0, int yEnd = 0)
        {
            xEnd = xEnd == 0 ? w2 : xEnd;
            yEnd = yEnd == 0 ? h2 : yEnd;

            int srcLength = source.Length;
            int destLength = h2 * w2 * Constants.ChannelsCount;
            float[] dest = new float[destLength];
            float xRatio = w2 * 1f / w1;
            float yRatio = h2 * 1f / h1;
            for (int y = 0; y < yEnd; y++)
            {
                for (int x = 0; x < xEnd; x++)
                {
                    int x1 = (int) (x / xRatio);
                    int y1 = (int) (y / yRatio);

                    if (x1 < 0 || x1 >= w1 || y1 < 0 || y1 >= h1)
                        continue;

                    if (x < 0 || x >= w2 - 1 || y < 0 || y >= h2 - 1)
                        continue;


                    int destIndex = (y * w2 + x) * Constants.ChannelsCount;
                    int sourceIndex = (y1 * w1 + x1) * Constants.ChannelsCount;

                    if (destIndex + 3 >= destLength || destIndex < 0
                        || sourceIndex + 3 >= srcLength || sourceIndex < 0) continue;

                    for (int i = 0; i < 4; i++)
                    {
                        dest[destIndex + i] = source[sourceIndex + i];
                    }
                }
            }
            
            // TODO: scale edges

            return dest;
        }
    }
}