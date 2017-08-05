using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEditor.control
{
    public class ImageView : System.Windows.Controls.Image
    {
        public Bitmap Bitmap
        {
            get => (Bitmap) GetValue(BitmapProperty);

            set => SetValue(BitmapProperty, value);
        }

        public static readonly DependencyProperty BitmapProperty
            = DependencyProperty.Register("Bitmap", typeof(Bitmap), typeof(ImageView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure
                                                    | FrameworkPropertyMetadataOptions.AffectsRender, OnBitmapChanged,
                    null), null);


        private static void OnBitmapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageView imageView = (ImageView) d;

            Bitmap oldBitmap = (Bitmap) e.OldValue;
            Bitmap newBitmap = (Bitmap) e.NewValue;

            imageView.Source = newBitmap.ToBitmapSource();
            oldBitmap?.Dispose();
        }
    }

    static class Converter
    {
        public static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height, 96, 96, PixelFormats.Bgra32, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }
    }
}