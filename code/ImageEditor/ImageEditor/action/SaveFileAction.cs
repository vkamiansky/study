using System.Windows;
using System.Windows.Interactivity;
using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor.action
{
    public class SaveFileAction : TriggerAction<FrameworkElement>
    {
        
        public SaveAction SaveActionOutput
        {
            get => (SaveAction) GetValue(SaveActionOutputProperty);
            set => SetValue(SaveActionOutputProperty, value);
        }

        public static readonly DependencyProperty SaveActionOutputProperty
            = DependencyProperty.Register("SaveActionOutput", typeof(SaveAction), typeof(SaveFileAction),
                new PropertyMetadata(default(SaveAction)));
        
          public SaveAction SaveAction
        {
            get => (SaveAction) GetValue(SaveActionProperty);
            set => SetValue(SaveActionProperty, value);
        }

        public static readonly DependencyProperty SaveActionProperty
            = DependencyProperty.Register("SaveAction", typeof(SaveAction), typeof(SaveFileAction),
                new PropertyMetadata(default(SaveAction)));
        
        
        protected override void Invoke(object parameter)
        {
            SaveActionOutput = SaveAction;
        }
    }
}