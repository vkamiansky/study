using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor.Interface.ViewModel
{
    public interface IMainViewModel
    {
        IAppStateViewModel AppState { get; }
    }
}
