using System.Windows;
using System.Windows.Interactivity;
using ImageEditor.Interface.ViewModel.model;
using ImageEditor;

namespace ImageEditor.action
{
    public class NewFileDialogAction : TriggerAction<FrameworkElement>
    {
        public NewFileData FileData
        {
            get => (NewFileData) GetValue(FileDataProperty);
            set => SetValue(FileDataProperty, value);
        }

        public static readonly DependencyProperty FileDataProperty
            = DependencyProperty.Register("FileData", typeof(NewFileData), typeof(NewFileDialogAction),
                new PropertyMetadata(default(NewFileData)));


        protected override void Invoke(object parameter)
        {
            NewFileDialog dialog = new NewFileDialog(data => FileData = data);
            dialog.Show();
        }
    }
}