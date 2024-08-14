using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FluentUI
{
    internal sealed class Button_Secondary : Border
    {
        private String _Content;
        internal String Content
        {
            get => _Content;
            set
            {
                _Content = value;
                _textBlock.Text = value;
            }
        }

        private Boolean _IsEnabled = true;
        internal new Boolean IsEnabled
        {
            get => _IsEnabled;
            set
            {
                _IsEnabled = value;

                if (value)
                {
                    Enable();
                }
                else
                {
                    Disable();
                }
            }
        }

        private readonly Border _This;
        private readonly Border _innerBorder = new();
        private readonly TextBlock _textBlock = new();

        internal delegate void ClickHandler(Button_Secondary sender);
        internal delegate void PreviewClickHandler();
        internal event ClickHandler Click;
        internal event PreviewClickHandler PreviewClick;

        public Button_Secondary()
        {
            _This = this;
            UseLayoutRounding = true;
            CornerRadius = new(5);
            Child = _innerBorder;

            _innerBorder.Margin = new(1);
            _innerBorder.CornerRadius = new(4);
            _innerBorder.Child = _textBlock;

            _textBlock.FontSize = 14;
            _textBlock.FontFamily = Fonts.Inter;
            _textBlock.FontWeight = FontWeights.Medium;
            _textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            _textBlock.VerticalAlignment = VerticalAlignment.Center;
            _textBlock.SetValue(TextOptions.TextRenderingModeProperty, TextRenderingMode.Grayscale);
            _textBlock.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
            _textBlock.SetValue(RenderOptions.ClearTypeHintProperty, ClearTypeHint.Enabled);
            _textBlock.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);

            Loaded += (s, e) =>
            {
                _IsEnabled = _This.IsEnabled; // xaml interface not using 'overridden' IsEnabled property
                ColorProviderChanged();
            };

            MouseEnter += Button_MouseEnter;
            MouseLeave += Button_MouseLeave;
            PreviewMouseDown += Button_Primary_PreviewMouseDown;
            PreviewMouseUp += Button_PreviewMouseUp;

            UI.ColorProviderChanged += ColorProviderChanged;
        }

        private static readonly Duration shortDuration = new(new(0, 0, 0, 0, 24));
        private static readonly Duration longDuration = new(new(0, 0, 0, 0, 48));

        private Boolean MouseUpPending = false;

        #region MouseEnter
        private readonly SolidColorBrushAnimation MouseEnter_Background_Animation = new() { Duration = longDuration };

        private void Button_MouseEnter(Object sender, System.Windows.Input.MouseEventArgs e)
        {
            _innerBorder.BeginAnimation(BackgroundProperty, MouseEnter_Background_Animation);
        }
        #endregion

        #region MouseDown
        private readonly SolidColorBrushAnimation MouseDown_Font_Animation = new() { Duration = shortDuration };
        private readonly SolidColorBrushAnimation MouseDown_Border_Animation = new() { Duration = shortDuration };
        private readonly SolidColorBrushAnimation MouseDown_Background_Animation = new() { Duration = shortDuration };
        private void Button_Primary_PreviewMouseDown(Object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BeginAnimation(BackgroundProperty, MouseDown_Border_Animation);
            _innerBorder.BeginAnimation(BackgroundProperty, MouseDown_Background_Animation);
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, MouseDown_Font_Animation);

            MouseUpPending = true;
        }
        #endregion

        #region MouseUp
        private readonly SolidColorBrushAnimation MouseUp_Font_Animation = new() { Duration = longDuration };
        private readonly SolidColorBrushAnimation MouseUp_Border_Animation = new() { Duration = longDuration };
        private readonly SolidColorBrushAnimation MouseUp_Background_Animation = new() { Duration = longDuration };

        private void Button_PreviewMouseUp(Object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BeginAnimation(BackgroundProperty, MouseUp_Border_Animation);
            _innerBorder.BeginAnimation(BackgroundProperty, MouseUp_Background_Animation);
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, MouseUp_Font_Animation);

            MouseUpPending = false;

            PreviewClick?.Invoke();
            Click?.Invoke(this);
        }
        #endregion

        #region MouseLeave
        private readonly SolidColorBrushAnimation MouseLeave_Background_Animation = new() { Duration = longDuration };

        private readonly SolidColorBrushAnimation MouseLeave_FromDown_Border_Animation = new() { Duration = longDuration };
        private readonly SolidColorBrushAnimation MouseLeave_FromDown_Background_Animation = new() { Duration = longDuration };

        private void Button_MouseLeave(Object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (MouseUpPending) // when user presses mouse key down, then drags out of the object (skips mouse up event)
            {
                BeginAnimation(BackgroundProperty, MouseLeave_FromDown_Border_Animation);
                _innerBorder.BeginAnimation(BackgroundProperty, MouseLeave_FromDown_Background_Animation);
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, MouseUp_Font_Animation);

                MouseUpPending = false;
            }
            else
            {
                _innerBorder.BeginAnimation(BackgroundProperty, MouseLeave_Background_Animation);
            }
        }
        #endregion

        //

        #region Disable
        private readonly SolidColorBrushAnimation Disable_Border_Animation = new() { Duration = longDuration };
        private readonly SolidColorBrushAnimation Disable_Background_Animation = new() { Duration = longDuration };
        private readonly SolidColorBrushAnimation Disable_Font_Animation = new() { Duration = longDuration };

        private void Disable()
        {
            MouseLeave -= Button_MouseLeave;
            _This.IsEnabled = false;

            BeginAnimation(BackgroundProperty, Disable_Border_Animation);
            _innerBorder.BeginAnimation(BackgroundProperty, Disable_Background_Animation);
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, Disable_Font_Animation);
        }
        #endregion

        #region Enable
        private readonly SolidColorBrushAnimation Enable_Border_Animation = new() { Duration = longDuration };
        private readonly SolidColorBrushAnimation Enable_Background_Animation = new() { Duration = longDuration };
        private readonly SolidColorBrushAnimation Enable_Font_Animation = new() { Duration = longDuration };

        private void Enable()
        {
            BeginAnimation(BackgroundProperty, Enable_Border_Animation);
            _innerBorder.BeginAnimation(BackgroundProperty, Enable_Background_Animation);
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, Enable_Font_Animation);

            _This.IsEnabled = true;
            MouseLeave += Button_MouseLeave;
        }
        #endregion

        private static class Colors
        {
            internal static class LightMode
            {
                internal static readonly SolidColorBrush Border = new(Color.FromRgb(0xd0, 0xd0, 0xd0));
                internal static readonly SolidColorBrush BorderMouseDown = new(Color.FromRgb(0xe0, 0xe0, 0xe0));
                internal static readonly SolidColorBrush BorderDisabled = new(Color.FromRgb(0xec, 0xec, 0xec));

                internal static readonly SolidColorBrush Font = new(Color.FromRgb(0x1b, 0x1b, 0x1b));
                internal static readonly SolidColorBrush FontMouseDown = new(Color.FromRgb(0x5d, 0x5d, 0x5d));
                internal static readonly SolidColorBrush FontDisabled = new(Color.FromRgb(0xa0, 0xa0, 0xa0));

                internal static readonly SolidColorBrush Background = new(Color.FromRgb(0xfb, 0xfb, 0xfb));
                internal static readonly SolidColorBrush BackgroundMouseOver = new(Color.FromRgb(0xf6, 0xf6, 0xf6));
                internal static readonly SolidColorBrush BackgroundMouseDown = new(Color.FromRgb(0xf5, 0xf5, 0xf5));
                internal static readonly SolidColorBrush BackgroundDisabled = new(Color.FromRgb(0xfa, 0xfa, 0xfa));
            }

            internal static class DarkMode
            {
                internal static readonly SolidColorBrush Border = new(Color.FromRgb(0x35, 0x35, 0x35));
                internal static readonly SolidColorBrush BorderMouseDown = new(Color.FromRgb(0x30, 0x30, 0x30));
                internal static readonly SolidColorBrush BorderDisabled = new(Color.FromRgb(0x3a, 0x3a, 0x3a));

                internal static readonly SolidColorBrush FontMouseDown = new(Color.FromRgb(0xce, 0xce, 0xce));
                internal static readonly SolidColorBrush FontDisabled = new(Color.FromRgb(0x7e, 0x7e, 0x7e));

                internal static readonly SolidColorBrush Background = new(Color.FromRgb(0x2d, 0x2d, 0x2d));
                internal static readonly SolidColorBrush BackgroundMouseOver = new(Color.FromRgb(0x32, 0x32, 0x32));
                internal static readonly SolidColorBrush BackgroundMouseDown = new(Color.FromRgb(0x27, 0x27, 0x27));
                internal static readonly SolidColorBrush BackgroundDisabled = new(Color.FromRgb(0x34, 0x34, 0x34));
            }
        }

        private void ColorProviderChanged()
        {
            if (Theme.IsDarkMode)
            {
                #region MouseEnter
                MouseEnter_Background_Animation.From = Colors.DarkMode.Background;
                MouseEnter_Background_Animation.To = Colors.DarkMode.BackgroundMouseOver;
                #endregion

                #region MouseDown
                MouseDown_Font_Animation.From = Brushes.White;
                MouseDown_Font_Animation.To = Colors.DarkMode.FontMouseDown;

                MouseDown_Border_Animation.From = Colors.DarkMode.Border;
                MouseDown_Border_Animation.To = Colors.DarkMode.BorderMouseDown;

                MouseDown_Background_Animation.From = Colors.DarkMode.BackgroundMouseOver;
                MouseDown_Background_Animation.To = Colors.DarkMode.BackgroundMouseDown;
                #endregion

                #region MouseUp
                MouseUp_Font_Animation.From = Colors.DarkMode.FontMouseDown;
                MouseUp_Font_Animation.To = Brushes.White;

                MouseUp_Border_Animation.From = Colors.DarkMode.BorderMouseDown;
                MouseUp_Border_Animation.To = Colors.DarkMode.Border;

                MouseUp_Background_Animation.From = Colors.DarkMode.BackgroundMouseDown;
                MouseUp_Background_Animation.To = Colors.DarkMode.BackgroundMouseOver;
                #endregion

                #region MouseLeave
                MouseLeave_FromDown_Border_Animation.From = Colors.DarkMode.BorderMouseDown;
                MouseLeave_FromDown_Border_Animation.To = Colors.DarkMode.Border;

                MouseLeave_FromDown_Background_Animation.From = Colors.DarkMode.BackgroundMouseDown;
                MouseLeave_FromDown_Background_Animation.To = Colors.DarkMode.Background;

                MouseLeave_Background_Animation.From = Colors.DarkMode.BackgroundMouseOver;
                MouseLeave_Background_Animation.To = Colors.DarkMode.Background;
                #endregion

                //

                #region Disable
                Disable_Font_Animation.From = Brushes.White;
                Disable_Font_Animation.To = Colors.DarkMode.FontDisabled;

                Disable_Border_Animation.From = Colors.DarkMode.Border;
                Disable_Border_Animation.To = Colors.DarkMode.BorderDisabled;

                Disable_Background_Animation.From = Colors.DarkMode.Background;
                Disable_Background_Animation.To = Colors.DarkMode.BackgroundDisabled;
                #endregion

                #region Enable
                Enable_Font_Animation.From = Colors.DarkMode.FontDisabled;
                Enable_Font_Animation.To = Brushes.White;

                Enable_Border_Animation.From = Colors.DarkMode.BorderDisabled;
                Enable_Border_Animation.To = Colors.DarkMode.Border;

                Enable_Background_Animation.From = Colors.DarkMode.BackgroundDisabled;
                Enable_Background_Animation.To = Colors.DarkMode.Background;
                #endregion
            }
            else
            {
                #region MouseEnter
                MouseEnter_Background_Animation.From = Colors.LightMode.Background;
                MouseEnter_Background_Animation.To = Colors.LightMode.BackgroundMouseOver;
                #endregion

                #region MouseDown
                MouseDown_Font_Animation.From = Colors.LightMode.Font;
                MouseDown_Font_Animation.To = Colors.LightMode.FontMouseDown;

                MouseDown_Border_Animation.From = Colors.LightMode.Border;
                MouseDown_Border_Animation.To = Colors.LightMode.BorderMouseDown;

                MouseDown_Background_Animation.From = Colors.LightMode.BackgroundMouseOver;
                MouseDown_Background_Animation.To = Colors.LightMode.BackgroundMouseDown;
                #endregion

                #region MouseUp
                MouseUp_Font_Animation.From = Colors.LightMode.FontMouseDown;
                MouseUp_Font_Animation.To = Colors.LightMode.Font;

                MouseUp_Border_Animation.From = Colors.LightMode.BorderMouseDown;
                MouseUp_Border_Animation.To = Colors.LightMode.Border;

                MouseUp_Background_Animation.From = Colors.LightMode.BackgroundMouseDown;
                MouseUp_Background_Animation.To = Colors.LightMode.BackgroundMouseOver;
                #endregion

                #region MouseLeave
                MouseLeave_FromDown_Border_Animation.From = Colors.LightMode.BorderMouseDown;
                MouseLeave_FromDown_Border_Animation.To = Colors.LightMode.Border;

                MouseLeave_FromDown_Background_Animation.From = Colors.LightMode.BackgroundMouseDown;
                MouseLeave_FromDown_Background_Animation.To = Colors.LightMode.Background;

                MouseLeave_Background_Animation.From = Colors.LightMode.BackgroundMouseOver;
                MouseLeave_Background_Animation.To = Colors.LightMode.Background;
                #endregion

                //

                #region Disable
                Disable_Font_Animation.From = Colors.LightMode.Font;
                Disable_Font_Animation.To = Colors.LightMode.FontDisabled;

                Disable_Border_Animation.From = Colors.LightMode.Border;
                Disable_Border_Animation.To = Colors.LightMode.BorderDisabled;

                Disable_Background_Animation.From = Colors.LightMode.Background;
                Disable_Background_Animation.To = Colors.LightMode.BackgroundDisabled;
                #endregion

                #region Enable
                Enable_Font_Animation.From = Colors.LightMode.FontDisabled;
                Enable_Font_Animation.To = Colors.LightMode.Font;

                Enable_Border_Animation.From = Colors.LightMode.BorderDisabled;
                Enable_Border_Animation.To = Colors.LightMode.Border;

                Enable_Background_Animation.From = Colors.LightMode.BackgroundDisabled;
                Enable_Background_Animation.To = Colors.LightMode.Background;
                #endregion
            }

            if (_IsEnabled)
            {
                BeginAnimation(BackgroundProperty, MouseUp_Border_Animation);
                _innerBorder.BeginAnimation(BackgroundProperty, MouseLeave_Background_Animation);
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, MouseUp_Font_Animation);
            }
            else
            {
                BeginAnimation(BackgroundProperty, Disable_Border_Animation);
                _innerBorder.BeginAnimation(BackgroundProperty, Disable_Background_Animation);
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, Disable_Font_Animation);
            }
        }
    }
}