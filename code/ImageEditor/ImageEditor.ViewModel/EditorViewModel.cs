using ImageEditor.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Property;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;

namespace ImageEditor.ViewModel
{
    public static class BitmapConversion
    {
        public static BitmapSource BitmapToBitmapSource(this Image source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            (source as Bitmap).GetHbitmap(),
            IntPtr.Zero,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
        }
    }

    public class EditorViewModel : IEditorViewModel
    {
        public IProperty<ImageSource> ImageSource { get; private set; }

        public EditorViewModel()
        {
            ImageSource = Reloadable<ImageSource>.On().First().Get(_ => Bitmap.FromFile("jake.jpg").BitmapToBitmapSource()).Create();
        }
    }
}
