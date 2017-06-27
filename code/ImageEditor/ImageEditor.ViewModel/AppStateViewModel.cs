using ImageEditor.Interface.ViewModel;
using System;
using System.Collections.Generic;
using Property;

namespace ImageEditor.ViewModel
{
    public class AppStateViewModel : IAppStateViewModel
    {
        private readonly IInputProperty<AppState> _State;
        public IProperty<AppState> State => _State;

        private readonly Dictionary<AppStateDataKey, object> _Data;

        public void DoAction(AppStateAction action)
        {
            throw new NotImplementedException();
        }

        public AppStateViewModel()
        {
            _State = Reloadable<AppState>.On().Each().Input().Create(AppState.EditorView);
        }
    }
}
