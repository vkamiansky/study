using ImageEditor.Interface.ViewModel;
using System;
using Property;

namespace ImageEditor.ViewModel
{
    public class AppStateViewModel : IAppStateViewModel
    {
        private readonly IInputProperty<AppState> _state;
        public IProperty<AppState> State => _state;

        public void DoAction(AppStateAction action)
        {
            throw new NotImplementedException();
        }

        public AppStateViewModel()
        {
            _state = Reloadable<AppState>.On().Each().Input().Create(AppState.EditorView);
        }
    }
}
