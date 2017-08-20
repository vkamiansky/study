using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ColorBox;

namespace ImageEditor
{
    /// <summary>
    /// Логика взаимодействия для Editor.xaml
    /// </summary>
    public partial class Editor : UserControl
    {
        public Editor()
        {
            InitializeComponent();

            ScaleTextBox.PreviewKeyDown += ScaleTextBox_PreviewKeyDown;
            var actualWidth = GridOne.ColumnDefinitions[0].ActualWidth;
            var actualHeight = GridOne.RowDefinitions[2].ActualHeight;
            //GridOne.SizeChanged += OnSizeChanged;
            MainGrid.SizeChanged += OnSizeChanged;
            Window mainWindow = Application.Current.MainWindow;
            mainWindow.PreviewKeyUp += OnKeyUpHandler;
            mainWindow.PreviewKeyDown += OnKeyDownHandler;
        }

        public Boolean AltPressed { get; private set; }

        private void ScaleTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tBox = (TextBox) sender;
                DependencyProperty prop = TextBox.TextProperty;

                BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop);
                binding?.UpdateSource();
            }
        }

        private void OnSizeChanged(Object o, SizeChangedEventArgs args)
        {
            /*var size = GetElementPixelSize(MainGrid);
            Canvas.OnContainerSizeChanged((int) size.Width, (int) size.Height);*/
        }
        
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                AltPressed = true;
            }
        }
        
        private void OnKeyUpHandler(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                AltPressed = false;
            }
        }
        
    }
}