using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ColorBox;
using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor
{
    /// <summary>
    /// Логика взаимодействия для NewFileDialog.xaml
    /// </summary>
    public partial class NewFileDialog : Window
    {
        private readonly Action<NewFileData> _action;

        public NewFileData FileData
        {
            get => (NewFileData) GetValue(FileDataProperty);
            set
            {
                var isValid = ValidateNewFileData(value);
                if (OkButton != null) OkButton.IsEnabled = isValid;
                if (isValid) SetValue(FileDataProperty, value);
            }
        }

        public static readonly DependencyProperty FileDataProperty
            = DependencyProperty.Register("FileData", typeof(NewFileData), typeof(NewFileDialog),
                new PropertyMetadata(default(NewFileData)));

        public NewFileDialog(Action<NewFileData> action)
        {
            _action = action;
            FileData = new NewFileData("Untitled", 100, 100, System.Drawing.Color.White);
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            _action.Invoke(FileData);
            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnColorChanged(object sender, ColorChangedEventArgs e)
        {
            var c = e.Color;
            FileData.Color = System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        private bool ValidateNewFileData(NewFileData data)
        {
            return data.Name.Length != 0 && data.Width > 0 && data.Height > 0;
        }
    }
}