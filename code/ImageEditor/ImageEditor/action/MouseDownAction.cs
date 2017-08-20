using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using ImageEditor.Interface.ViewModel;

namespace ImageEditor.action
{
    public class MouseDownAction : TriggerAction<FrameworkElement>
    {
        public const double MoveTolerance = 0.001d;

        public FrameworkElement ContainerElement
        {
            get => (FrameworkElement) GetValue(ContainerElementProperty);
            set => SetValue(ContainerElementProperty, value);
        }

        public FrameworkElement TranslateElement
        {
            get => (FrameworkElement) GetValue(TranslateElementProperty);
            set => SetValue(TranslateElementProperty, value);
        }

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

        public static readonly DependencyProperty ContainerElementProperty
            = DependencyProperty.Register("ContainerElement", typeof(FrameworkElement), typeof(MouseDownAction),
                new PropertyMetadata(default(FrameworkElement)));

        public static readonly DependencyProperty TranslateElementProperty
            = DependencyProperty.Register("TranslateElement", typeof(FrameworkElement), typeof(MouseDownAction),
                new PropertyMetadata(default(FrameworkElement)));

        private double _x;
        private double _y;

        private double _downX;
        private double _downY;
        private bool _isDownAssigned;


        protected override void Invoke(object parameter)
        {
            if (!(parameter is MouseEventArgs)) return;

            MouseEventArgs e = (MouseEventArgs) parameter;

            var point = e.GetPosition(ContainerElement);
            point = ContainerElement.TranslatePoint(point, TranslateElement);

            double x = point.X / TranslateElement.ActualWidth;
            double y = point.Y / TranslateElement.ActualHeight;

            double dx = x - _x;
            double dy = y - _y;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // ReSharper disable CompareOfFloatsByEqualityOperator
                if (!_isDownAssigned)
                {
                    _isDownAssigned = true;
                    _downX = x;
                    _downY = y;
                }
                if (CheckCoord(_downX, _downY))
                {
                    if (ToolMenu == ToolMenuItem.Move)
                    {
                        if (Math.Abs(dx) > MoveTolerance
                            || Math.Abs(dy) > MoveTolerance)
                            Shift = new Tuple<double, double>(dx, dy);
                    }
                    else
                    {
                        Shift = new Tuple<double, double>(x, y);
                    }
                }
            }
            else if (e.LeftButton == MouseButtonState.Released)
            {
                _isDownAssigned = false;
            }

            _x = x;
            _y = y;
        }

        private bool CheckCoord(double x, double y)
        {
            return x >= 0 && y >= 0
                   && x < 1d
                   && y < 1d;
        }
    }
}