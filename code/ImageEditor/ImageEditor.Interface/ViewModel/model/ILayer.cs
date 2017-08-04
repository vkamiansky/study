namespace ImageEditor.Interface.ViewModel.model
{
    public interface ILayer
    {
        string Name { get; set; }
        bool IsSelected { get; set; }
        float Opacity { get; set; }
    }
}