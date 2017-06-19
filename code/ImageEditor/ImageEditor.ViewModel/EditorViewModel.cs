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
using ImageEditor.ViewModel.model;
using ImageConverter = ImageEditor.ViewModel.model.ImageConverter;
using Size = System.Windows.Size;

namespace ImageEditor.ViewModel
{
    public class EditorViewModel : IEditorViewModel
    {
        public IProperty<ImageSource> ImageSource => _imageSource;
        public IInputProperty<string> ImagePath { get; }
        public IInputProperty<int> MouseWheelDelta { get; }
        public IInputProperty<Tuple<int, int>> Shift { get; }

        private readonly ICallProperty<ImageSource> _imageSource;
        private Canvas _canvas;
        private readonly ImageConverter _converter = new ImageConverter();

        public EditorViewModel()
        {
            _imageSource = Reloadable<ImageSource>.On().Each()
                .Call(_ => _converter.ConvertToBitmapSource(_canvas)).Create();

            Shift = Reloadable<Tuple<int, int>>.On().Each().Input().Create();

            Shift.OnChanged(() =>
            {
                _canvas.OnMoved(Shift.Value.Item1, Shift.Value.Item2);
                _imageSource.Go();
            });

            MouseWheelDelta = Reloadable<int>.On().Each().Input().Create();

            MouseWheelDelta.OnChanged(() =>
            {
                _canvas.OnSizeChanged(MouseWheelDelta.Value);
                _imageSource.Go();
            });

            ImagePath = Reloadable<string>.On().Each().Input().Create();
            ImagePath.OnChanged(() =>
            {
                _canvas = _converter.ConvertToCanvas(new Bitmap(ImagePath.Value));
                _imageSource.Go();
            });
        }
    }
}