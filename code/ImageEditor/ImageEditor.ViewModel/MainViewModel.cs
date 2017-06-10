using System;
using System.Drawing;
using ImageEditor.Interface.ViewModel;
using System.Windows.Media.Imaging;
using System.Windows;
using Property;

namespace ImageEditor.ViewModel
{

  

    public class MainViewModel : IMainViewModel
    {
        public IAppStateViewModel AppState { get; set; }

        public Func<BitmapSource> GetImage { get; set; }
        public IProperty<BitmapSource> ImageSource { get; private set; }

        public MainViewModel()
        {
            ImageSource = Reloadable<BitmapSource>.On().First().Get(_ => Bitmap.FromFile("jake.jpg").BitmapToBitmapSource()).Create();
        }
    }
}
