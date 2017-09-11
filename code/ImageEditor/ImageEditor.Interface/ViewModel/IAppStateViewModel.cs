using Property;

namespace ImageEditor.Interface.ViewModel
{
    public interface IAppStateViewModel
    {
        IProperty<AppState> State { get; }
    }
}
