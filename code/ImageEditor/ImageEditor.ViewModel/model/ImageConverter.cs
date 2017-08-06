using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ImageEditor.Interface.ViewModel.model;
using static ImageEditor.Interface.ViewModel.model.Constants;

namespace ImageEditor.ViewModel.model
{
    public static class ImageConverter
    {
        public static Canvas ToCanvas(this Bitmap bitmap, string fileName)
        {
            return new Canvas(bitmap.Width, bitmap.Height, bitmap.ToRaw(), fileName);
        }

        public static CanvasSource ToCanvasSource(this Canvas canvas, float scale)
        {
            return new CanvasSource(canvas.GetRaw(), canvas.Width, canvas.Height, scale);
        }

        public static Canvas ToCanvas(this NewFileData fileData)
        {
            var width = fileData.Width;
            var height = fileData.Height;
            Bitmap bitmap = new Bitmap(width, height);
            var ghx = Graphics.FromImage(bitmap);
            ghx.FillRectangle(new SolidBrush(fileData.Color), new Rectangle(0, 0, width, height));
            ghx.Dispose();
            return new Canvas(width, height, bitmap.ToRaw(), fileData.Name);
        }

        public static float[] ToRaw(this Bitmap bitmap)
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
            return raw;
        }
    }
}