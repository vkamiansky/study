using System;
using ImageEditor.Interface.ViewModel;
using Property;
using ImageEditor.Interface;

namespace ImageEditor.ViewModel
{
    public class MainViewModel : IMainViewModel
    {
        public IAppStateViewModel AppState { get; set; }

        public Func<IEditorViewModel> GetEditor { get; set; }
        public IProperty<IEditorViewModel> Editor { get; private set; }

        public MainViewModel()
        {
            Editor = Reloadable<IEditorViewModel>.On().First().Get(_ => GetEditor()).Create();
        }
    }
}
