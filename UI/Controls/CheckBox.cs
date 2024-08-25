using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace FluentUI
{
    internal sealed class CheckBox : Grid
    {
        private Boolean _isEnabled = true;
        new internal Boolean IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;

                //if (IsEnabled)
                //{
                //    Enable();
                //}
                //else
                //{
                //    Disable();
                //}
            }
        }





        private readonly Grid _this;
        private readonly Border _background = new();

        public CheckBox()
        {











            _this = this;
            Width = 20d;
            Height = 20d;
            Background = Brushes.Yellow;
            //Child = _background;

            _background.Margin = new(1d);
            _background.Background = Brushes.Green;

            Loaded += (s, e) =>
            {
                _isEnabled = _this.IsEnabled; // xaml interface not using 'overridden' IsEnabled property
                ColorProviderChanged();
            };
        }

        private void ColorProviderChanged()
        {
        }
    }
}