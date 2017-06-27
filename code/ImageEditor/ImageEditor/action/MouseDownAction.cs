using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using ImageEditor.Interface.ViewModel;

namespace ImageEditor.action
{
    public class MouseDownAction : TriggerAction<FrameworkElement>
    {
        private const double RoundCorrection = .5;

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

        public ToolMenuItem ToolMenu
        {
            get => (ToolMenuItem) GetValue(ToolMenuProperty);
            set => SetValue(ToolMenuProperty, value);
        }

        public static readonly DependencyProperty ToolMenuProperty
            = DependencyProperty.Register("ToolMenu", typeof(ToolMenuItem), typeof(MouseDownAction),
                new PropertyMetadata(default(ToolMenuItem)));

        public static readonly DependencyProperty ShiftProperty
            = DependencyProperty.Register("Shift", typeof(Tuple<int, int>), typeof(MouseDownAction),
                new PropertyMetadata(default(Tuple<int, int>)));

        public static readonly DependencyProperty GetPosElementProperty
            = DependencyProperty.Register("GetPosElement", typeof(IInputElement), typeof(MouseDownAction),
                new PropertyMetadata(default(IInputElement)));

        private int _x;
        private int _y;


        protected override void Invoke(object parameter)
        {
            if (!(parameter is MouseEventArgs)) return;
            MouseEventArgs e = (MouseEventArgs) parameter;

            var point = e.GetPosition(GetPosElement);

            int x = (int) (point.X + RoundCorrection);
            int y = (int) (point.Y + RoundCorrection);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (ToolMenu != ToolMenuItem.Move)
                {
                    Shift = new Tuple<int, int>(x, y);
                }
                else
                {
                    if (x != _x || y != _y)
                        Shift = new Tuple<int, int>(x - _x, y - _y);
                }
            }

            _x = x;
            _y = y;
        }
    }
}