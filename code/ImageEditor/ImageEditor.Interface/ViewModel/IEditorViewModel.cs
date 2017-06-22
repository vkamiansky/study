using Property;
using System;
using System.Collections.Generic;

using System.Windows.Media;
using ImageEditor.Interface.ViewModel;
using ImageEditor.ViewModel.model;

namespace ImageEditor.Interface
{
    public interface IEditorViewModel
    {
        IProperty<ImageSource> ImageSource { get; }

        IProperty<List<Layer>> Layers { get; }

        IInputProperty<ToolMenuItem> ToolMenu { get; }

        IInputProperty<string> ImagePath { get; }

        IInputProperty<string> ImageScale { get; }

        IInputProperty<int> MouseWheelDelta { get; }

        IInputProperty<Tuple<int, int>> Shift { get; }

        IInputProperty<int> ToolSize { get; }

        IInputProperty<float> ToolOpacity { get; }

        IInputProperty<System.Drawing.Color> ToolColor { get; }
    }
}