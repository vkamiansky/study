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

        public IProperty<CanvasSource> ImageSource => _imageSource;
        public IProperty<List<ILayer>> Layers => _layers;
        public IInputProperty<string> ImagePath { get; }
        public IInputProperty<int> MouseWheelDelta { get; }
        public IInputProperty<Tuple<int, int>> Shift { get; }
        public IInputProperty<string> ImageScale { get; }
        public IInputProperty<ToolMenuItem> ToolMenu { get; }
        public IInputProperty<int> ToolSize { get; }
        public IInputProperty<float> ToolOpacity { get; }
        public IInputProperty<SolidColorBrush> ToolBrush { get; }

        private readonly ICallProperty<CanvasSource> _imageSource;
        private readonly ICallProperty<List<ILayer>> _layers;

        private Canvas _canvas;

        private bool calledFromUI = true;
        private float _scale = 1f;

        private float Scale
        {
            get => _scale;
            set
            {
                if (value < 0.0002) value = 0.0002f;
                if (value > 32) value = 32;
                if (value == _scale) return;

                _scale = value;
                _imageSource.Go();

                calledFromUI = false;
                ImageScale.Input = _scale * 100 + "%";
                calledFromUI = true;
            }
        }

        public EditorViewModel()
        {
            ImageScale = Reloadable<string>.On().Each().Input().Create();

            ImageScale.Input = "100%";

            ImageScale.OnChanged(() =>
            {
                if (!calledFromUI) return;
                string s = ImageScale.Value;

                // ReSharper disable once RedundantAssignment
                float sc = -1f;
                float.TryParse(s.Remove(s.Length - 1).Replace(",", "."), out sc);
                sc /= 100f;
                if (Scale == sc) return;
                Scale = sc;
            });

            _imageSource = Reloadable<CanvasSource>.On().Each()
                .Call(_ => _canvas.ToCanvasSource(_scale)).Create();

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
                var value = MouseWheelDelta.Value;
                if (value > 0)
                {
                    Scale *= value * Constants.ScaleRatio;
                }
                if (value < 0)
                {
                    value = -value;
                    Scale /= value * Constants.ScaleRatio;
                }
            });

            ImagePath = Reloadable<string>.On().Each().Input().Create();
            ImagePath.OnChanged(() =>
            {
                _canvas = new Bitmap(ImagePath.Value).ToCanvas();
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