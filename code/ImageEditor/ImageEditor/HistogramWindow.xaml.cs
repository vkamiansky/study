using System;
using System.ComponentModel;
using System.Windows.Controls;
using ImageEditor.Interface.ViewModel.model;
using Property;

namespace ImageEditor
{
    /// <summary>
    /// Логика взаимодействия для HistogramWindow.xaml
    /// </summary>
    public partial class HistogramWindow
    {
        public HistogramWindow(IProperty<CanvasSource> imageSource)
        {
            InitializeComponent();
            if (imageSource == null) Close();
            imageSource.OnChanged(() => { OnNewCanvasSource(imageSource.Value); });
        }

        public void OnNewCanvasSource(CanvasSource canvasSource)
        {
            //float[] histogram = new float[256];
            
        }
    }
}