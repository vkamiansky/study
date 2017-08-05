using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using ImageEditor.Interface.ViewModel;

namespace ImageEditor.action
{
    public class MouseDownAction : TriggerAction<FrameworkElement>
    {
/*        private readonly Stopwatch _stopwatch = new Stopwatch();

        private const long EmitThreshold = 0;*/

        public Tuple<double, double> Shift
        {
            get => (Tuple<double, double>) GetValue(ShiftProperty);
            set => SetValue(ShiftProperty, value);
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
            = DependencyProperty.Register("ToolMenu", typeof(ToolMenuItem), typeof(MouseDownAction),
                new PropertyMetadata(default(ToolMenuItem)));

        public static readonly DependencyProperty ShiftProperty
            = DependencyProperty.Register("Shift", typeof(Tuple<double, double>), typeof(MouseDownAction),
                new PropertyMetadata(default(Tuple<double, double>)));

        public static readonly DependencyProperty ImageProperty
            = DependencyProperty.Register("Image", typeof(Image), typeof(MouseDownAction),
                new PropertyMetadata(default(Image)));

        private double _x;
        private double _y;

        protected override void Invoke(object parameter)
        {
            if (!(parameter is MouseEventArgs)) return;

 /*           if (_stopwatch.IsRunning
                && _stopwatch.ElapsedMilliseconds < EmitThreshold)
            {
                return;
            }*/

            MouseEventArgs e = (MouseEventArgs) parameter;

            var point = e.GetPosition(Image);

            double x = point.X  / Image.ActualWidth;
            double y = point.Y / Image.ActualHeight;

            double dx = x - _x;
            double dy = y - _y;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (ToolMenu == ToolMenuItem.Move)
                {
                    if (dx != 0 || dy != 0)
                        Shift = new Tuple<double, double>(dx, dy);
                }
                else
                {
                    Shift = new Tuple<double, double>(x, y);
                }
            }

            _x = x;
            _y = y;

            //_stopwatch.Restart();
        }
    }
}