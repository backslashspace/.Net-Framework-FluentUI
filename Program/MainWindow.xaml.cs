﻿using FluentUI;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FluentUI_Framework
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            UI.MainWindow = this;

            InitializeComponent();

            Loaded += (s, e) => Theme_Changed();
            Theme.Changed += Theme_Changed; ;


            Test();
        }

        private async Task Test()
        {
            while (true)
            {
                await Task.Delay(2000).ConfigureAwait(true);

                PrimaryButton.IsEnabled = false;

                SecondaryButton.IsEnabled = false;

                await Task.Delay(2000).ConfigureAwait(true);

                PrimaryButton.IsEnabled = true;

                SecondaryButton.IsEnabled = true;
            }
        }

        private void Theme_Changed()
        {
            Theme.UpdateNonClientArea(this);

            Background = Theme.IsDarkMode ? new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x20)) : new SolidColorBrush(Color.FromRgb(0xf3, 0xf3, 0xf3));
        }
    }
}