using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;
using ImageEditor.Interface.ViewModel.model;
using Property;

namespace ImageEditor.Interface.ViewModel
{
    public interface IEditorViewModel
    {
        IProperty<Bitmap> ImageSource { get; }

        IProperty<List<ILayer>> Layers { get; }

        IInputProperty<ToolMenuItem> ToolMenu { get; }

        IInputProperty<string> ImagePath { get; }

        IInputProperty<string> ImageScale { get; }

        IInputProperty<int> MouseWheelDelta { get; }

        IInputProperty<Tuple<int, int>> Shift { get; }

        IInputProperty<int> ToolSize { get; }

        IInputProperty<float> ToolOpacity { get; }

        IInputProperty<SolidColorBrush> ToolBrush { get; }
    }
}