using System;

namespace ImageEditor.Interface.ViewModel.model
{
    public interface ILayer
    {
        string Name { get; set; }
        bool IsSelected { get; set; }
        bool IsVisible { get; set; }
        float Opacity { get; set; }
        Action OnChanged { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        float[] Raw { get; set; }

        void SaveToMemento();
        void RestoreFromMemento();
    }  
}