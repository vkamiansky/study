using ImageEditor.Interface.ViewModel;
using System;
using Property;

namespace ImageEditor.ViewModel
{
    public class AppStateViewModel : IAppStateViewModel
    {
        private readonly IInputProperty<AppState> _state;
        public IProperty<AppState> State => _state;

        public AppStateViewModel()
        {
            _state = Reloadable<AppState>.On().Each().Input().Create(AppState.EditorView);
        }
    }
}
