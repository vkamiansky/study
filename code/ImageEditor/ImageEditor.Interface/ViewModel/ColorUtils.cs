using System;

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

        public static void FixValueIfNeed(ref float v)
        {
            if (v < 0) v = 0f;
            if (v > 1) v = 1f;
        }

        public static void KernelProcess(float[] src, int w, int h, float[] kernel, float mult = 1f)
        {
            float[] dest = src.CloneArray();

            int side = (int) Math.Sqrt(kernel.Length);
            
            if (side % 2 == 0 || side < 3 || side > 100) throw new Exception("Illegal argument exc.");

            if (mult != 1f)
            {
                for (var i = 0; i < kernel.Length; i++)
                {
                    kernel[i] *= mult;
                }
            }

            int d = 1, ds = 3;

            while (ds != side)
            {
                d++;
                ds += 2;
            }

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    float r = 0f;
                    float g = 0f;
                    float b = 0f;
                    //float a = 0f;

                    int ii = 0;
                    for (float iy = i - d; iy <= i + d; iy++, ii++)
                    {
                        int id = 0;
                        for (float ix = j - d; ix <= j + d; ix++, id++)
                        {
                            float x = Min(w - 1, Max(0, ix));
                            float y = Min(h - 1, Max(0, iy));
                            int si = ((int) (y * w + x)) * 4;
                            float p = kernel[ii * side + id];
                            b += src[si] * p;
                            g += src[si + 1] * p;
                            r += src[si + 2] * p;
                            //a += src[si + 3] * p;
                        }
                    }
                    int di = (i * w + j) * 4;
                    
                    FixValueIfNeed(ref b);
                    FixValueIfNeed(ref g);
                    FixValueIfNeed(ref r);
                    //FixValueIfNeed(ref a);
                    
                    dest[di] = b;
                    dest[di + 1] = g;
                    dest[di + 2] = r;
                    //dest[di + 3] = a;
                    
                }
            }

            dest.CopyTo(src, 0);
        }
    }
}