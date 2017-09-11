using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageEditor.Interface.ViewModel;
using ImageEditor.Interface.ViewModel.model;
using static ImageEditor.Interface.ViewModel.model.Constants;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ImageEditor.ViewModel.model
{
    public static class ImageConverter
    {
        public static Canvas ToCanvas(this Bitmap bitmap, string filePath)
        {
            return new Canvas(bitmap.Width, bitmap.Height, bitmap.ToRaw(), filePath);
        }

        public static CanvasSource ToCanvasSource(this Canvas canvas, float scale = 1f)
        {
            return new CanvasSource(canvas.GetRaw().CloneArray(), canvas.Width, canvas.Height, scale);
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
        
        public static Bitmap ToBitmap(this Canvas canvas)
        {
            var canvasSource = canvas.ToCanvasSource();
            
            Bitmap bitmap = new Bitmap(canvasSource.Width, canvasSource.Height, PixelFormat.Format32bppArgb);

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, canvasSource.Width, canvasSource.Height),
                ImageLockMode.ReadWrite, bitmap.PixelFormat);

            Marshal.Copy(canvasSource.Raw.ToByteArray(), 0, bitmapData.Scan0, canvasSource.Raw.Length);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height, bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgra32, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        public static byte[] ToByteArray(this float[] raw)
        {
            int resultLength = raw.Length;
            byte[] byteRaw = new byte[resultLength];

            for (int i = 0; i < resultLength; i++)
            {
                byteRaw[i] = (byte) (raw[i] * ColorDenormalizeRatio);
            }

            return byteRaw;
        }
        
        public static float[] ToFloatArray(this byte[] raw)
        {
            int resultLength = raw.Length;
            float[] floatRaw = new float[resultLength];

            for (int i = 0; i < resultLength; i++)
            {
                floatRaw[i] = raw[i] * ColorNormalizeRatio;
            }

            return floatRaw;
        }
    }
}