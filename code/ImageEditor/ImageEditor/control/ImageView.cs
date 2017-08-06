using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageEditor.Interface.ViewModel.model;
using ImageEditor.scalers;

namespace ImageEditor.control
{
    public class ImageView : System.Windows.Controls.Image
    {
        public static int MaxWidth;
        public static int MaxHeight;

        public CanvasSource CanvasSource
        {
            get => (CanvasSource) GetValue(CanvasSourceProperty);

            set => SetValue(CanvasSourceProperty, value);
        }

        public static readonly DependencyProperty CanvasSourceProperty
            = DependencyProperty.Register("CanvasSource", typeof(CanvasSource), typeof(ImageView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure
                                                    | FrameworkPropertyMetadataOptions.AffectsRender, OnDataChanged,
                    null), null);


        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageView imageView = (ImageView) d;

            CanvasSource newCanvasSource = (CanvasSource) e.NewValue;

            newCanvasSource.ApplyScale(ScalerChooser.Instance.ChooseScaler(newCanvasSource.Scale));
            newCanvasSource.ApplyBackground();

            imageView.Source = newCanvasSource.ToBitmapSource();
        }


        public void OnContainerSizeChanged(int newWidth, int newHeight)
        {
            MaxWidth = newWidth;
            MaxHeight = newHeight;
        }

        /* private float[] _cachedBackgroung;
 
         private float[] GenerateBackground()
         {
             float[] rawArr;
             if (_cachedBackgroung != null && _cachedBackgroung.Length == Length)
             {
                 rawArr = _cachedBackgroung;
             }
             else
             {
                 rawArr = new float[Length];
                 _cachedBackgroung = rawArr;
             }
             for (int y = 0; y < Height; y++)
             {
                 for (int x = 0; x < Width; x++)
                 {
                     int index = (y * Width + x) * ChannelsCount;
                     var d = y / BgTileSide % 2;
                     var f = x / BgTileSide % 2;
                     if (d == 0 && f == 0 || d != 0 && f != 0)
                     {
                         rawArr[index + 0] = BgWhite;
                         rawArr[index + 1] = BgWhite;
                         rawArr[index + 2] = BgWhite;
                         rawArr[index + 3] = Opaque;
                     }
                     else
                     {
                         rawArr[index + 0] = BgGrey;
                         rawArr[index + 1] = BgGrey;
                         rawArr[index + 2] = BgGrey;
                         rawArr[index + 3] = Opaque;
                     }
                 }
             }
             return Clone(rawArr);
         }*/
    }

    static class Converter
    {
        public static BitmapSource ToBitmapSource(this CanvasSource canvasSource)
        {
            Bitmap bitmap = new Bitmap(canvasSource.Width, canvasSource.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, canvasSource.Width, canvasSource.Height),
                ImageLockMode.ReadWrite, bitmap.PixelFormat);

            Marshal.Copy(canvasSource.Raw.ToByteArray(), 0, bitmapData.Scan0, canvasSource.Raw.Length);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height, bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgra32, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            bitmap.Dispose();
            return bitmapSource;
        }

        public static byte[] ToByteArray(this float[] raw)
        {
            int resultLength = raw.Length;
            byte[] byteRaw = new byte[resultLength];

            for (int i = 0; i < resultLength; i++)
            {
                byteRaw[i] = (byte) (raw[i] * Constants.ColorDenormalizeRatio);
            }

            return byteRaw;
        }
    }
}