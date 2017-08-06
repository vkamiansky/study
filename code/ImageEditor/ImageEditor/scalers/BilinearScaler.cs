using System.Diagnostics.CodeAnalysis;
using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor.scalers
{
    public class BilinearScaler : ImageScaler
    {
        [SuppressMessage("ReSharper", "TooWideLocalVariableScope")]
        public float[] Scale(float[] source, int w1, int h1, int w2, int h2,
            int xStart = 0, int yStart = 0, int xEnd = 0, int yEnd = 0)
        {
            xEnd = xEnd == 0 ? w2 : xEnd;
            yEnd = yEnd == 0 ? h2 : yEnd;
            int srcLength = source.Length;
            int destLength = h2 * w2 * Constants.ChannelsCount;

            float[] dest = new float[destLength];

            int dx = 0, dy = 0;

            int x1, y1, x2, y2, i0, i1, i2, i3, i4;
            float a, b, c, d, w, h, xf, yf, nxDiff, nyDiff, m1, m2, m3, m4;

            // don't optimize it, left it for flexibility of code
            float xRatio = w1 * 1f / w2;
            float yRatio = h1 * 1f / h2;


            for (int y = yStart; y < yEnd; y++)
            {
                for (int x = xStart; x < yEnd; x++)
                {
                    xf = (xRatio * (x + dx));
                    yf = (yRatio * (y + dy));

                    x1 = (int) (xf);
                    y1 = (int) (yf);

                    if (x1 < 0 || x1 >= w1 - 1 || y1 < 0 || y1 >= h1 - 1)
                        continue;

                    w = xf - x1;
                    h = yf - y1;


                    i1 = (y1 * w1 + x1) * 4;
                    i2 = (y1 * w1 + x1 + 1) * 4;
                    i3 = ((y1 + 1) * w1 + x1) * 4;
                    i4 = ((y1 + 1) * w1 + x1 + 1) * 4;

                    x2 = x;
                    y2 = y;

                    if (x2 < 0 || x2 >= w2 - 1 || y2 < 0 || y2 >= h2 - 1)
                        continue;

                    i0 = ((y2 * w2 + x2) * 4);

                    nxDiff = 1 - w;
                    nyDiff = 1 - h;

                    m1 = nxDiff * nyDiff;
                    m2 = w * nyDiff;
                    m3 = h * nxDiff;
                    m4 = w * h;

                    for (int i = 0; i < 4; i++)
                    {
                        if (i4 + i >= srcLength || i1 + i < 0
                            || i0 + i < 0 || i0 + i >= destLength) break;
                        a = source[i1 + i];
                        b = source[i2 + i];
                        c = source[i3 + i];
                        d = source[i4 + i];

                        dest[i0 + i] =
                            a * m1
                            + b * m2
                            + c * m3
                            + d * m4;
                    }
                }
            }
            return dest;
        }
    }
}