using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using ImageEditor.Interface.ViewModel;

namespace ImageEditor.action
{
    public class ToolMenuSetupAction : TriggerAction<FrameworkElement>
    {
        public Grid ToolMenu
        {
            get => (Grid)GetValue(GridProperty);
            set => SetValue(GridProperty, value);
        }

        public static readonly DependencyProperty GridProperty
            = DependencyProperty.Register("ToolMenu", typeof(Grid), typeof(ToolMenuSetupAction),
                new PropertyMetadata(default(Grid)));



        public ToolMenuItem ToolMenuItem
        {
            get => (ToolMenuItem)GetValue(ToolMenuItemProperty);
            set => SetValue(ToolMenuItemProperty, value);
        }

        public static readonly DependencyProperty ToolMenuItemProperty
            = DependencyProperty.Register("ToolMenuItem", typeof(ToolMenuItem), typeof(ToolMenuSetupAction),
                new PropertyMetadata(default(ToolMenuItem)));


        public SolidColorBrush MenuPressed
        {
            get => (SolidColorBrush)GetValue(ItemPressedBrushProperty);
            set => SetValue(ItemPressedBrushProperty, value);
        }

        public static readonly DependencyProperty ItemPressedBrushProperty
            = DependencyProperty.Register("MenuPressed", typeof(SolidColorBrush), typeof(ToolMenuSetupAction),
                new PropertyMetadata(default(SolidColorBrush)));



        public SolidColorBrush MenuNotPressed
        {
            get => (SolidColorBrush)GetValue(ItemNotPressedBrushProperty);
            set => SetValue(ItemNotPressedBrushProperty, value);
        }

        public static readonly DependencyProperty ItemNotPressedBrushProperty
            = DependencyProperty.Register("MenuNotPressed", typeof(SolidColorBrush), typeof(ToolMenuSetupAction),
                new PropertyMetadata(default(SolidColorBrush)));



        public SolidColorBrush MenuHover
        {
            get => (SolidColorBrush)GetValue(ItemHoverBrushProperty);
            set => SetValue(ItemHoverBrushProperty, value);
        }

        public static readonly DependencyProperty ItemHoverBrushProperty
            = DependencyProperty.Register("MenuHover", typeof(SolidColorBrush), typeof(ToolMenuSetupAction),
                new PropertyMetadata(default(SolidColorBrush)));



        private List<MenuItem> _items;

        protected override void Invoke(object parameter)
        {
            _items = new List<MenuItem>(ToolMenu.Children.Count);

            foreach (var toolMenuChild in ToolMenu.Children)
            {
                if (!(toolMenuChild is MenuItem)) continue;

                var menuItem = toolMenuChild as MenuItem;
                menuItem.Click += MenuItem_Click;
                menuItem.MouseEnter += Item_MouseEnter;
                menuItem.MouseLeave += MenuItem_MouseLeave;
                _items.Add(menuItem);
            }
            MenuItem_Click(_items[0], null);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            switch (_items.IndexOf(item))
            {
                case 0:
                    ToolMenuItem = ToolMenuItem.Move;
                    break;
                case 1:
                    ToolMenuItem = ToolMenuItem.Select;
                    break;
                case 2:
                    ToolMenuItem = ToolMenuItem.Brush;
                    break;
                case 3:
                    ToolMenuItem = ToolMenuItem.Erase;
                    break;
            }
            foreach (var menuItem in _items)
            {
                menuItem.Background = item.Equals(menuItem) ? MenuPressed : MenuNotPressed;
            }
        }

        private void Item_MouseEnter(object sender, MouseEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            if (Equals(item.Background, MenuPressed)) return;
            item.Background = MenuHover;
        }

        private void MenuItem_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            if (Equals(item.Background, MenuPressed)) return;
            item.Background = MenuNotPressed;
        }
    }
}
