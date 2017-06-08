using System;

using ImageEditor.Interface.ViewModel;

namespace ImageEditor.ViewModel
{
    public class MainViewModel : IMainViewModel
    {
        public IAppStateViewModel AppState { get; set; }

        public MainViewModel()
        {

        }
    }
}
