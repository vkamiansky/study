using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using ImageEditor.Interface.ViewModel;

namespace ImageEditor.action
{
    public class MouseClickAction : TriggerAction<FrameworkElement>
    {
        private const double RoundCorrection = .5;

        public Tuple<int, int> Shift
        {
            get => (Tuple<int, int>)GetValue(ShiftProperty);
            set => SetValue(ShiftProperty, value);
        }

        public IInputElement GetPosElement
        {
            get => (IInputElement)GetValue(GetPosElementProperty);
            set => SetValue(GetPosElementProperty, value);
        }

        public ToolMenuItem ToolMenu
        {
            get => (ToolMenuItem)GetValue(ToolMenuProperty);
            set => SetValue(ToolMenuProperty, value);
        }

        public static readonly DependencyProperty ToolMenuProperty
            = DependencyProperty.Register("ToolMenu", typeof(ToolMenuItem), typeof(MouseClickAction),
                new PropertyMetadata(default(ToolMenuItem)));

        public static readonly DependencyProperty ShiftProperty
            = DependencyProperty.Register("Shift", typeof(Tuple<int, int>), typeof(MouseClickAction),
                new PropertyMetadata(default(Tuple<int, int>)));

        public static readonly DependencyProperty GetPosElementProperty
            = DependencyProperty.Register("GetPosElement", typeof(IInputElement), typeof(MouseClickAction),
                new PropertyMetadata(default(IInputElement)));


        protected override void Invoke(object parameter)
        {
            if (!(parameter is MouseEventArgs)) return;
            MouseEventArgs e = (MouseEventArgs)parameter;

            if (e.LeftButton == MouseButtonState.Released) return;
            if (ToolMenu == ToolMenuItem.Move) return;

            var point = e.GetPosition(GetPosElement);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                int x = (int)(point.X + RoundCorrection);
                int y = (int)(point.Y + RoundCorrection);

                Shift = new Tuple<int, int>(x, y);
            }
        }
    }
}
