using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FluentUI
{
    internal sealed class Checkbox : Grid
    {
        private String _content;
        internal String Content
        {
            get => _content;
            set
            {
                _content = value;
                _textBlock.Text = value;
            }
        }

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

        private Boolean _isChecked = false;
        internal Boolean IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;

                /*
                 * 
                 * 
                 * 
                 */
            }
        }

        new internal Boolean IsInitialized { get; private set; } = false;

        #region Definitions
        private readonly Grid _this;
        private readonly Border _checkboxBorder = new();
        private readonly Border _checkboxBackground = new();
        private readonly System.Windows.Shapes.Path _optionMark = new();
        private readonly TextBlock _textBlock = new(false);
        #endregion

        public Checkbox()
        {
            _this = this;

            _checkboxBorder.Height = 20d;
            _checkboxBorder.Width = 20d;
            _checkboxBorder.CornerRadius = new(4.5);
            _checkboxBorder.HorizontalAlignment = HorizontalAlignment.Left;
            Children.Add(_checkboxBorder);

            _checkboxBackground.Margin = new(1);
            _checkboxBackground.CornerRadius = new(3.5);
            _checkboxBorder.Child = _checkboxBackground;

            _optionMark.HorizontalAlignment = HorizontalAlignment.Center;
            _optionMark.VerticalAlignment = VerticalAlignment.Center;
            _optionMark.Height = 12d;
            _optionMark.Width = 12d;
            _optionMark.Stretch = Stretch.Uniform;
            _optionMark.Data = Geometry.Parse("M 12 -4 L 4 4 L 0 0 L 1 -1 L 4 2 L 11 -5 L 12 -4");
            _checkboxBackground.Child = _optionMark;

            _textBlock.Margin = new(28, 1, 0, 0);
            Children.Add(_textBlock);

            UI.ColorProviderChanged += ColorProviderChanged;

            Loaded += (s, e) =>
            {
                _isEnabled = _this.IsEnabled; // xaml interface not using 'overridden' IsEnabled property

                if (_isEnabled)
                {
                    if (_isChecked)
                    {
                        _checkboxBorder.Background = Theme.IsDarkMode ? AccentColors.DarkMode.PrimaryColorAsBrush : AccentColors.LightMode.PrimaryColorAsBrush;
                        _checkboxBackground.Background = Theme.IsDarkMode ? AccentColors.DarkMode.PrimaryColorAsBrush : AccentColors.LightMode.PrimaryColorAsBrush;
                    }
                    else
                    {
                        _checkboxBorder.Background = Theme.IsDarkMode ? new SolidColorBrush(Color.FromRgb(0x9e, 0x9e, 0x9e)) : new SolidColorBrush(Color.FromRgb(0x89, 0x89, 0x89));
                        _checkboxBackground.Background = Theme.IsDarkMode ? new SolidColorBrush(Color.FromRgb(0x27, 0x27, 0x27)) : new SolidColorBrush(Color.FromRgb(0xf5, 0xf5, 0xf5));
                        _optionMark.Visibility = Visibility.Collapsed;
                    }

                    _optionMark.Fill = Theme.IsDarkMode ? Brushes.Black : Brushes.White;
                }
                else
                {
                    if (_isChecked)
                    {
                        _checkboxBorder.Background = Theme.IsDarkMode ? new SolidColorBrush(Color.FromRgb(0x5b, 0x5b, 0x5b)) : new SolidColorBrush(Color.FromRgb(0xb0, 0xb0, 0xb0));
                        _checkboxBackground.Background = Theme.IsDarkMode ? new SolidColorBrush(Color.FromRgb(0x4c, 0x4c, 0x4c)) : new SolidColorBrush(Color.FromRgb(0xc5, 0xc5, 0xc5));
                    }
                    else
                    {
                        _checkboxBorder.Background = Theme.IsDarkMode ? new SolidColorBrush(Color.FromRgb(0x4c, 0x4c, 0x4c)) : new SolidColorBrush(Color.FromRgb(0xc5, 0xc5, 0xc5));
                        _checkboxBackground.Background = Theme.IsDarkMode ? new SolidColorBrush(Color.FromRgb(0x2b, 0x2b, 0x2b)) : new SolidColorBrush(Color.FromRgb(0xfb, 0xfb, 0xfb));
                        _optionMark.Visibility = Visibility.Collapsed;
                    }

                    _optionMark.Fill = Theme.IsDarkMode ? new SolidColorBrush(Color.FromRgb(0xa7, 0xa7, 0xa7)) : Brushes.White;
                }
            };
        }

        private void ColorProviderChanged()
        {
            if (_isEnabled)
            {
                if (_isChecked)
                {
                    _checkboxBorder.Background = Theme.IsDarkMode ? AccentColors.DarkMode.PrimaryColorAsBrush : AccentColors.LightMode.PrimaryColorAsBrush;
                    _checkboxBackground.Background = Theme.IsDarkMode ? AccentColors.DarkMode.PrimaryColorAsBrush : AccentColors.LightMode.PrimaryColorAsBrush;
                }
                else
                {
                    _checkboxBorder.Background = Theme.IsDarkMode ? new SolidColorBrush(Color.FromRgb(0x9e, 0x9e, 0x9e)) : new SolidColorBrush(Color.FromRgb(0x89, 0x89, 0x89));
                    _checkboxBackground.Background = Theme.IsDarkMode ? new SolidColorBrush(Color.FromRgb(0x27, 0x27, 0x27)) : new SolidColorBrush(Color.FromRgb(0xf5, 0xf5, 0xf5));
                    _optionMark.Visibility = Visibility.Collapsed;
                }

                _optionMark.Fill = Theme.IsDarkMode ? Brushes.Black : Brushes.White;
            }
            else
            {
                if (_isChecked)
                {
                    _checkboxBorder.Background = Theme.IsDarkMode ? new SolidColorBrush(Color.FromRgb(0x5b, 0x5b, 0x5b)) : new SolidColorBrush(Color.FromRgb(0xb0, 0xb0, 0xb0));
                    _checkboxBackground.Background = Theme.IsDarkMode ? new SolidColorBrush(Color.FromRgb(0x4c, 0x4c, 0x4c)) : new SolidColorBrush(Color.FromRgb(0xc5, 0xc5, 0xc5));
                }
                else
                {
                    _checkboxBorder.Background = Theme.IsDarkMode ? new SolidColorBrush(Color.FromRgb(0x4c, 0x4c, 0x4c)) : new SolidColorBrush(Color.FromRgb(0xc5, 0xc5, 0xc5));
                    _checkboxBackground.Background = Theme.IsDarkMode ? new SolidColorBrush(Color.FromRgb(0x2b, 0x2b, 0x2b)) : new SolidColorBrush(Color.FromRgb(0xfb, 0xfb, 0xfb));
                    _optionMark.Visibility = Visibility.Collapsed;
                }

                _optionMark.Fill = Theme.IsDarkMode ? new SolidColorBrush(Color.FromRgb(0xa7, 0xa7, 0xa7)) : Brushes.White;
            }










            if (IsInitialized)
            {
                AnimateToNewState();
            }
        }

        private void AnimateToNewState()
        {

        }
    }
}