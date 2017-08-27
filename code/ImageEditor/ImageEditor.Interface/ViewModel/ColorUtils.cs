namespace ImageEditor.Interface.ViewModel
{
    public class ColorUtils
    {
        // Given a rgb values in range of 0-1
        // return h,s,l in range of 0-1
        public static void RgbToHsl(float r, float g, float b, out float h, out float s, out float l)
        {
            h = 0;
            s = 0;

            var v = Max(r, g, b);
            var m = Min(r, g, b);

            l = (m + v) / 2.0f;

            if (l <= 0.0f) return;

            var vm = v - m;
            s = vm;

            if (s <= 0) return;

            s /= (l <= 0.5f) ? (v + m) : (2.0f - v - m);

            var r2 = (v - r) / vm;
            var g2 = (v - g) / vm;
            var b2 = (v - b) / vm;

            if (r == v)
            {
                h = (g == m ? 5.0f + b2 : 1.0f - g2);
            }
            else if (g == v)
            {
                h = (b == m ? 1.0f + r2 : 3.0f - b2);
            }
            else
            {
                h = (r == m ? 3.0f + g2 : 5.0f - r2);
            }
            h /= 6.0f;
        }

        // Given h,s,l in range of 0-1
        public static void Hsl2Rgb(float h, float s, float l, out float r, out float g, out float b)
        {
            // default to gray
            r = l;
            g = l;
            b = l;
            
            float v = (l <= 0.5f) ? (l * (1.0f + s)) : (l + s - l * s);
            if (v > 0)
            {
                float m = l + l - v;
                var sv = (v - m) / v;
                h *= 6.0f;
                var sextant = (int) h;
                float fract = h - sextant;
                var vsf = v * sv * fract;
                float mid1 = m + vsf;
                var mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }
        }

        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }

        public static float Min(float a, float b, float c)
        {
            return Min(Min(a, b), c);
        }

        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        public static float Max(float a, float b, float c)
        {
            return Max(Max(a, b), c);
        }
    }
}