using Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ImageEditor.Interface
{
    public interface IEditorViewModel
    {
        IProperty<ImageSource> ImageSource { get; }
    }
}
