using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using ImageEditor.Interface.ViewModel;

namespace ImageEditor.action
{
    public class MouseClickAction : TriggerAction<FrameworkElement>
    {
        public Tuple<double, double> Shift
        {
            get => (Tuple<double, double>) GetValue(ShiftProperty);
            set => SetValue(ShiftProperty, value);
        }

        public FrameworkElement FrameworkElement
        {
            get => (FrameworkElement) GetValue(FrameworkElementProperty);
            set => SetValue(FrameworkElementProperty, value);
        }

        public Image Image
        {
            get => (Image) GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public ToolMenuItem ToolMenu
        {
            get => (ToolMenuItem) GetValue(ToolMenuProperty);
            set => SetValue(ToolMenuProperty, value);
        }

        public static readonly DependencyProperty ToolMenuProperty
            = DependencyProperty.Register("ToolMenu", typeof(ToolMenuItem), typeof(MouseClickAction),
                new PropertyMetadata(default(ToolMenuItem)));

        public static readonly DependencyProperty ShiftProperty
            = DependencyProperty.Register("Shift", typeof(Tuple<double, double>), typeof(MouseClickAction),
                new PropertyMetadata(default(Tuple<double, double>)));

        public static readonly DependencyProperty ImageProperty
            = DependencyProperty.Register("Image", typeof(Image), typeof(MouseClickAction),
                new PropertyMetadata(default(Image)));

        public static readonly DependencyProperty FrameworkElementProperty
            = DependencyProperty.Register("FrameworkElement", typeof(FrameworkElement), typeof(MouseClickAction),
                new PropertyMetadata(default(FrameworkElement)));

        protected override void Invoke(object parameter)
        {
            if (!(parameter is MouseEventArgs)) return;
            MouseEventArgs e = (MouseEventArgs) parameter;

            if (e.LeftButton == MouseButtonState.Released) return;
            if (ToolMenu == ToolMenuItem.Move) return;

            var point = e.GetPosition(FrameworkElement);

            Debug.WriteLine("Click: " + point.X + " " + point.Y);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Shift = new Tuple<double, double>(point.X / FrameworkElement.ActualWidth,
                    point.Y / FrameworkElement.ActualHeight);
            }
        }
    }
}