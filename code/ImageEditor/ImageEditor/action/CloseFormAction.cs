using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace ImageEditor.action
{
    public class CloseFormAction : TriggerAction<FrameworkElement>
    {
        public Window Window
        {
            get => (Window)GetValue(WindowProperty);
            set => SetValue(WindowProperty, value);
        }

        public static readonly DependencyProperty WindowProperty
            = DependencyProperty.Register("Window", typeof(Window), typeof(CloseFormAction),
                new PropertyMetadata(default(Window)));

        protected override void Invoke(object parameter)
        {
           Window.Close();
        }
    }
}
