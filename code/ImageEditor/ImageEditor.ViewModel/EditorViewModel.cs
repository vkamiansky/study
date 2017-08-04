using System;
using System.Collections.Generic;
using Property;
using System.Windows.Media;
using System.Drawing;
using ImageEditor.Interface.ViewModel;
using ImageEditor.Interface.ViewModel.model;
using ImageEditor.ViewModel.model;
using ImageConverter = ImageEditor.ViewModel.model.ImageConverter;

namespace ImageEditor.ViewModel
{
    public class EditorViewModel : IEditorViewModel
    {
        private const float DefaultOpacity = 1f;
        private const int DefaultSize = 6;

        public IProperty<ImageSource> ImageSource => _imageSource;
        public IProperty<List<ILayer>> Layers => _layers;
        public IInputProperty<string> ImagePath { get; }
        public IInputProperty<int> MouseWheelDelta { get; }
        public IInputProperty<Tuple<int, int>> Shift { get; }
        public IInputProperty<string> ImageScale { get; }
        public IInputProperty<ToolMenuItem> ToolMenu { get; }
        public IInputProperty<int> ToolSize { get; }
        public IInputProperty<float> ToolOpacity { get; }
        public IInputProperty<SolidColorBrush> ToolBrush { get; }

        private readonly ICallProperty<ImageSource> _imageSource;
        private readonly ICallProperty<List<ILayer>> _layers;

        private Canvas _canvas;
        private readonly ImageConverter _converter = new ImageConverter();

        public EditorViewModel()
        {
            ImageScale = Reloadable<string>.On().Each().Input().Create();

            ImageScale.Input = "100%";

            ImageScale.OnChanged(() =>
            {
                string s = ImageScale.Value;
                // ReSharper disable once RedundantAssignment
                float scale = -1f;
                float.TryParse(s.Remove(s.Length - 1).Replace(",", "."), out scale);
                if (scale <= 1 || scale >= 3200) return;

                scale /= 100;

                _canvas.Scale = scale;

                _imageSource.Go();
            });

            _imageSource = Reloadable<ImageSource>.On().Each()
                .Call(_ => _converter.ConvertToBitmapSource(_canvas)).Create();

            Shift = Reloadable<Tuple<int, int>>.On().Each().Input().Create();

            ToolBrush = Reloadable<SolidColorBrush>.On().Each().Input().Create();

            ToolBrush.Input = new SolidColorBrush(Colors.Crimson);

            Shift.OnChanged(() =>
            {
                switch (ToolMenu.Value)
                {
                    case ToolMenuItem.Move:
                        _canvas.OnMoved(Shift.Value.Item1, Shift.Value.Item2);
                        break;

                    case ToolMenuItem.Brush:
                        _canvas.Draw(Shift.Value.Item1, Shift.Value.Item2, ToolSize.Value, ToolOpacity.Value,
                            ToolBrush.Value.Color);
                        break;

                    case ToolMenuItem.Erase:
                        _canvas.Erase(Shift.Value.Item1, Shift.Value.Item2, ToolSize.Value, ToolOpacity.Value);
                        break;
                }

                _imageSource.Go();
            });

            MouseWheelDelta = Reloadable<int>.On().Each().Input().Create();

            MouseWheelDelta.OnChanged(() =>
            {
                _canvas.OnSizeChanged(MouseWheelDelta.Value);
                ImageScale.Input = _canvas.Scale * 100 + "%";
                _imageSource.Go();
            });

            ImagePath = Reloadable<string>.On().Each().Input().Create();
            ImagePath.OnChanged(() =>
            {
                _canvas = _converter.ConvertToCanvas(new Bitmap(ImagePath.Value));
                _canvas.OnLayersChanged(() => _imageSource.Go());
                _layers.Go();
                _imageSource.Go();
            });

            _layers = Reloadable<List<ILayer>>.On().Each()
                .Call(_ => _canvas.Layers.ConvertAll(x => x as ILayer)).Create();

            ToolMenu = Reloadable<ToolMenuItem>.On().Each().Input().Create();

            ToolSize = Reloadable<int>.On().Each().Input().Create();
            ToolOpacity = Reloadable<float>.On().Each().Input().Create();

            ToolSize.Input = DefaultSize;
            ToolOpacity.Input = DefaultOpacity;
        }
    }
}