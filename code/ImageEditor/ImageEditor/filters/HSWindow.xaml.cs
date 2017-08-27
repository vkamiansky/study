﻿using System;
using System.Collections.Generic;
using System.Windows;
using ImageEditor.Interface.ViewModel;
using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor.filters
{
    /// <summary>
    /// Логика взаимодействия для HSWindow.xaml
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class HSWindow
    {
        private readonly List<ILayer> _selectedLayers;

        public HSWindow(List<ILayer> selectedLayers, Action onClose)
        {
            InitializeComponent();
            _selectedLayers = selectedLayers;
            _selectedLayers.ForEach(layer => layer.SaveToMemento());
            Closing += (sender, args) => onClose.Invoke();
        }

        private void ApplyFilter(float[] raw)
        {
            float hue = (float) (Hue.Value / 1000f);
            float saturation = (float) (Saturation.Value / 1000f);
            float lightness = (float) (Lightness.Value / 1000f);

            for (var i = 0; i < raw.Length; i += 4)
            {
                float h, s, l;
                ColorUtils.RgbToHsl(raw[i], raw[i + 1], raw[i + 2], out h, out s, out l);

                h += hue;
                s += saturation;
                l += lightness;

                ColorUtils.FixValueIfNeed(ref h);
                ColorUtils.FixValueIfNeed(ref s);
                ColorUtils.FixValueIfNeed(ref l);

                ColorUtils.Hsl2Rgb(h, s, l, out raw[i + 2], out raw[i + 1], out raw[i]);
            }
        }

        private void Update()
        {
            if (_selectedLayers == null || _selectedLayers.Count == 0) return;
            foreach (var selectedLayer in _selectedLayers)
            {
                selectedLayer.RestoreFromMemento();
                ApplyFilter(selectedLayer.Raw);
                selectedLayer.OnChanged?.Invoke();
            }
        }


        private void Brightness_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Update();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            _selectedLayers.ForEach(layer => layer.SaveToMemento());
            Close();
        }
    }
}