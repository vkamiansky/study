using ImageEditor.Interface.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Property;

namespace ImageEditor.ViewModel
{
    public class AppStateViewModel : IAppStateViewModel
    {
        public IProperty<AppState> State => throw new NotImplementedException();

        private readonly Dictionary<AppStateDataKey, object> _Data;

        public void DoAction(AppStateAction action)
        {
            throw new NotImplementedException();
        }
    }
}
