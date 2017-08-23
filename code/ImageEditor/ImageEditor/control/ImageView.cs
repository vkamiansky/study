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
        
    public CanvasSource CleanCanvasSource
        {
            get => (CanvasSource) GetValue(CleanCanvasSourceProperty);

            set => SetValue(CleanCanvasSourceProperty, value);
        }

        public static readonly DependencyProperty CleanCanvasSourceProperty
            = DependencyProperty.Register("CleanCanvasSource", typeof(CanvasSource), typeof(ImageView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure
                                                    | FrameworkPropertyMetadataOptions.AffectsRender, OnCleanDataChanged,
                    null), null);


        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageView imageView = (ImageView) d;

            CanvasSource newCanvasSource = (CanvasSource) e.NewValue;

            imageView.Source = newCanvasSource
                .ApplyScale(ScalerChooser.Instance.ChooseScaler(newCanvasSource.Scale))
                .ApplyBackground()
                .ToBitmapSource();
        }
        
        private static void OnCleanDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageView imageView = (ImageView) d;

            CanvasSource newCanvasSource = (CanvasSource) e.NewValue;

            imageView.Source = newCanvasSource.ToBitmapSource();
        }
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