using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static ImageEditor.ViewModel.model.Constants;

namespace ImageEditor.ViewModel.model
{
    public class ImageConverter
    {
        public Canvas ConvertToCanvas(Bitmap bitmap)
        {

            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                Bitmap temp = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);
                using (var gr = Graphics.FromImage(temp))
                    gr.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                bitmap = temp;
            }

            int height = bitmap.Height;
            int width = bitmap.Width;
            int length = width * height * 4;

            float[] raw = new float[length];
            byte[] byteRaw = new byte[length];

            Rectangle rect = new Rectangle(0, 0, width, height);

            BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadOnly,
                bitmap.PixelFormat);
            Marshal.Copy(data.Scan0, byteRaw, 0, length);
            bitmap.UnlockBits(data);

            //normalize colors
            for (int i = 0; i < length; i++)
            {
                raw[i] = byteRaw[i] * ColorNormalizeRatio;
            }

            return new Canvas(width, height, raw);
        }

        public BitmapSource ConvertToBitmapSource(Canvas canvas)
        {

            float[] raw = canvas.GetRaw();
            int resultLength = raw.Length;
            byte[] byteRaw = new byte[resultLength];

            for (int i = 0; i < resultLength; i++)
            {
                byteRaw[i] = (byte) (raw[i] * ColorDenormalizeRatio);
            }

            Bitmap bmp = new Bitmap(canvas.Width, canvas.Height, PixelFormat.Format32bppArgb);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, canvas.Width, canvas.Height),
                ImageLockMode.WriteOnly, bmp.PixelFormat);

            Marshal.Copy(byteRaw, 0, data.Scan0, resultLength);
            bmp.UnlockBits(data);

            BitmapSource source = BitmapToBitmapSource(bmp);

            bmp.Dispose();

            return source;
        }

        public BitmapSource BitmapToBitmapSource(Image source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                ((Bitmap)source).GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
    }
}