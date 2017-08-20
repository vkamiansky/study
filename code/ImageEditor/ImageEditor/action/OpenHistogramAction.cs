using System.Diagnostics;
using System.Windows;
using System.Windows.Interactivity;
using ImageEditor.Interface.ViewModel.model;
using Property;

namespace ImageEditor.action
{
    public class OpenHistogramAction : TriggerAction<FrameworkElement>
    {
        public IProperty<CanvasSource> HistogramImageSource
        {
            get => (IProperty<CanvasSource>) GetValue(HistogramImageSourceProperty);
            set => SetValue(HistogramImageSourceProperty, value);
        }

        public static readonly DependencyProperty HistogramImageSourceProperty
            = DependencyProperty.Register("HistogramImageSource", typeof(IProperty<CanvasSource>), typeof(OpenHistogramAction),
                new PropertyMetadata(default(IProperty<CanvasSource>)));

        protected override void Invoke(object parameter)
        {
            if (HistogramImageSource == null) return;
            new HistogramWindow(HistogramImageSource).Show();
        }
    }
}