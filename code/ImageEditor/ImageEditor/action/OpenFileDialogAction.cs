﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using Microsoft.Win32;

namespace ImageEditor.action
{
    public class OpenFileDialogAction : TriggerAction<FrameworkElement>
    {
        public string FilePath
        {
            get => (string)GetValue(FilePathProperty);
            set => SetValue(FilePathProperty, value);
        }

        public static readonly DependencyProperty FilePathProperty
            = DependencyProperty.Register("FilePath", typeof(string), typeof(OpenFileDialogAction),
                new PropertyMetadata(""));

        protected override void Invoke(object parameter)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dialog.ShowDialog() == true)
            {
                FilePath = dialog.FileName;
            }
        }
    }
}