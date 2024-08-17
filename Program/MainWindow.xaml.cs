using FluentUI;
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

        private void Theme_Changed()
        {
            Theme.UpdateNonClientArea(this);
        }

        // ############################################################################

        private void PrimaryButton_Click()
        {
            RareBar.IsIndeterminate = RareBar.IsIndeterminate ? false : true;
        }

        private void SecondaryButton_Click()
        {
            if (Double.TryParse(YEEz.Text, out Double eee))
            {
                RareBar.Value = eee;
            }
            
        }
    }
}