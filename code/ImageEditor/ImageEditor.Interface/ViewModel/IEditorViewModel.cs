using Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ImageEditor.Interface.ViewModel;

namespace ImageEditor.Interface
{
    public interface IEditorViewModel
    {
        IProperty<ImageSource> ImageSource { get; }

        IProperty<ToolMenuItem> ToolMenu { get; }

        IInputProperty<string> ImagePath { get; }

        IInputProperty<string> ImageScale { get; }

        IInputProperty<int> MouseWheelDelta { get; }

        IInputProperty<Tuple<int, int>> Shift { get; }
    }
}