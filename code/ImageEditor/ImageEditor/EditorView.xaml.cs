using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor
{
    /// <summary>
    /// Логика взаимодействия для Editor.xaml
    /// </summary>
    public partial class Editor
    {
        public Color Color
        {
            get => (Color) GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly DependencyProperty ColorProperty
            = DependencyProperty.Register("Color", typeof(Color), typeof(Editor),
                new PropertyMetadata(Colors.Red));


        public Editor()
        {
            InitializeComponent();

            ScaleTextBox.PreviewKeyDown += ScaleTextBox_PreviewKeyDown;
            var actualWidth = GridOne.ColumnDefinitions[0].ActualWidth;
            var actualHeight = GridOne.RowDefinitions[2].ActualHeight;
            ColorBox.ColorChanged += (sender, args) =>
            {
                Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                    new Action(() => { Color = ((SolidColorBrush) ((ColorBox.ColorBox) sender).Brush).Color; })
                );
            };

            MainGrid.SizeChanged += OnSizeChanged;
            Window mainWindow = Application.Current.MainWindow;
            mainWindow.PreviewKeyUp += OnKeyUpHandler;
            mainWindow.PreviewKeyDown += OnKeyDownHandler;
            ListBox.SelectionChanged += (sender, args) =>
            {
                var allItems = ListBox.ItemsSource;
                var selectedItems = ListBox.SelectedItems;
                foreach (var item in allItems)
                {
                    var layer = item as ILayer;
                    if (layer == null) continue;
                    layer.IsSelected = selectedItems.Contains(item);
                }
            };
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