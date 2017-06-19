using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ImageEditor.action
{
    public class MouseDownAction : TriggerAction<FrameworkElement>
    {
        public Tuple<int, int> Shift
        {
            get => (Tuple<int, int>) GetValue(ShiftProperty);
            set => SetValue(ShiftProperty, value);
        }

        public IInputElement GetPosElement
        {
            get => (IInputElement) GetValue(GetPosElementProperty);
            set => SetValue(GetPosElementProperty, value);
        }

        public static readonly DependencyProperty ShiftProperty
            = DependencyProperty.Register("Shift", typeof(Tuple<int, int>), typeof(MouseDownAction),
                new PropertyMetadata(default(Tuple<int, int>)));

        public static readonly DependencyProperty GetPosElementProperty
            = DependencyProperty.Register("GetPosElement", typeof(IInputElement), typeof(MouseDownAction),
                new PropertyMetadata(default(IInputElement)));

        private double _mouseX;
        private double _mouseY;

        private double _dx;
        private double _dy;

        protected override void Invoke(object parameter)
        {
            if (!(parameter is MouseEventArgs)) return;
            MouseEventArgs e = (MouseEventArgs) parameter;

            var point = e.GetPosition(GetPosElement);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _dx += point.X - _mouseX;
                _dy += point.Y - _mouseY;

                if (_dx >= 1 || _dy >= 1 || _dx <= -1 || _dy <= -1)
                {
                    Shift = new Tuple<int, int>((int) _dx, (int) _dy);
                    _dx = 0f;
                    _dy = 0f;
                }
            }
            _mouseX = point.X;
            _mouseY = point.Y;
        }
    }
}