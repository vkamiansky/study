using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ImageEditor.action
{
    public class MouseWheelAction : TriggerAction<FrameworkElement>
    {
        public int Delta
        {
            get => (int) GetValue(DeltaProperty);
            set => SetValue(DeltaProperty, value);
        }

        public Boolean IsAltPressed
        {
            get => (Boolean) GetValue(AltProperty);
            set => SetValue(AltProperty, value);
        }

        public static readonly DependencyProperty DeltaProperty
            = DependencyProperty.Register("Delta", typeof(int), typeof(MouseWheelAction),
                new PropertyMetadata(default(int)));

        public static readonly DependencyProperty AltProperty
            = DependencyProperty.Register("IsAltPressed", typeof(Boolean), typeof(MouseWheelAction),
                new PropertyMetadata(default(Boolean), AltPropertyChanged));

        private static void AltPropertyChanged(DependencyObject dependencyObject, 
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            MouseWheelAction mouseWheelAction = dependencyObject as MouseWheelAction;
            var newValue = dependencyPropertyChangedEventArgs.NewValue;
        }

        protected override void Invoke(object parameter)
        {
            if (!(parameter is MouseWheelEventArgs) /*|| !IsAltPressed*/) return;
            MouseWheelEventArgs args = (MouseWheelEventArgs) parameter;
            Delta = args.Delta;
        }
    }
}