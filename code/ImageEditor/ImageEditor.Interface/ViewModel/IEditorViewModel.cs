using System;
using System.Collections.Generic;
using ImageEditor.Interface.ViewModel.model;
using Property;
using Color = System.Windows.Media.Color;

namespace ImageEditor.Interface.ViewModel
{
    public interface IEditorViewModel
    {
        IProperty<CanvasSource> ImageSource { get; }
        
        IProperty<CanvasSource> DelayedImageSource { get; }

        IProperty<List<ILayer>> Layers { get; }

        IInputProperty<ToolMenuItem> ToolMenu { get; }

        IInputProperty<string> ImagePath { get; }

        IInputProperty<string> ImageScale { get; }
        
        IInputProperty<NewFileData> NewFile { get; }

        IInputProperty<int> MouseWheelDelta { get; }

        IInputProperty<Tuple<double, double>> Shift { get; }

        IInputProperty<int> ToolSize { get; }

        IInputProperty<float> ToolOpacity { get; }

        IInputProperty<Color> ToolColor { get; }

        IInputProperty<Tuple<LayerAction, ILayer>> LayerActionProperty { get; }
    }
}