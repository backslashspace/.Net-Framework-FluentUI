﻿using FluentUI;
using System;
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
            Theme.Changed += Theme_Changed;

        }

        private void Theme_Changed() => Theme.UpdateNonClientArea(this);

        // ############################################################################

        private void PrimaryButton_Click()
        {
            RareBar.IsIndeterminate = !RareBar.IsIndeterminate;
            TESTTSSS.IsEnabled = !TESTTSSS.IsEnabled;
        }

        private void SecondaryButton_Click()
        {
            if (Double.TryParse(YEEz.Text, out Double eee))
            {
                RareBar.Value = eee;
            }


            TESTTSSS.IsChecked = !TESTTSSS.IsChecked;
        }

        private void CheckBox_Checked(Object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("DDD");
        }

        private void ToggleButton_Checked(Object sender, RoutedEventArgs e)
        {
            SecondaryButton.IsEnabled = true;
            PrimaryButton.IsEnabled = true;
        }

        private void ToggleButton_UnChecked(Object sender, RoutedEventArgs e)
        {
            SecondaryButton.IsEnabled = false;
            PrimaryButton.IsEnabled = false;
        }

        private void TESTTSSS_Checked(Checkbox sender)
        {
            Debug.WriteLine("checked");
        }

        private void TESTTSSS_Unchecked(Checkbox sender)
        {
            Debug.WriteLine("unchecked");
        }
    }
}