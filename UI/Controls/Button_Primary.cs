using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FluentUI
{
    internal sealed class Button_Primary : Border
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
        internal Boolean IsEnabled
        {
            get => _IsEnabled;
            set
            {
                _IsEnabled = value;

                if (IsEnabled)
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

        internal delegate void ClickHandler(Button_Primary sender);
        internal delegate void PreviewClickHandler();
        internal event ClickHandler Click;
        internal event PreviewClickHandler PreviewClick;

        public Button_Primary()
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
            _textBlock.Foreground = Brushes.Black;
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
        private readonly SolidColorBrushAnimation MouseEnter_Border_Animation = new() { Duration = longDuration };
        private readonly SolidColorBrushAnimation MouseEnter_Background_Animation = new() { Duration = longDuration };

        private void Button_MouseEnter(Object sender, System.Windows.Input.MouseEventArgs e)
        {
            BeginAnimation(BackgroundProperty, MouseEnter_Border_Animation);
            _innerBorder.BeginAnimation(BackgroundProperty, MouseEnter_Background_Animation);
        }
        #endregion

        #region MouseDown
        private readonly SolidColorBrushAnimation MouseDown_Border_Animation = new() { Duration = shortDuration };
        private readonly SolidColorBrushAnimation MouseDown_Background_Animation = new() { Duration = shortDuration };
        private readonly SolidColorBrushAnimation MouseDown_Font_Animation = new() { Duration = shortDuration };

        private void Button_Primary_PreviewMouseDown(Object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BeginAnimation(BackgroundProperty, MouseDown_Border_Animation);
            _innerBorder.BeginAnimation(BackgroundProperty, MouseDown_Background_Animation);
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, MouseDown_Font_Animation);

            MouseUpPending = true;
        }
        #endregion

        #region MouseUp
        private readonly SolidColorBrushAnimation MouseUp_Border_Animation = new() { Duration = longDuration };
        private readonly SolidColorBrushAnimation MouseUp_Background_Animation = new() { Duration = longDuration };
        private readonly SolidColorBrushAnimation MouseUp_Font_Animation = new() { Duration = longDuration };

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
        private readonly SolidColorBrushAnimation MouseLeave_Border_Animation = new() { Duration = longDuration };
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
                BeginAnimation(BackgroundProperty, MouseLeave_Border_Animation);
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

        private static readonly SolidColorBrush DarkMode_FontColor_MouseDown = new(Color.FromRgb(0x2d, 0x2d, 0x2d));

        private static readonly SolidColorBrush DarkMode_FontColor_Disabled = new(Color.FromRgb(0xa7, 0xa7, 0xa7));
        private static readonly SolidColorBrush DarkMode_BorderAndBackground_Disabled = new(Color.FromRgb(0x43, 0x43, 0x43));

        private static readonly SolidColorBrush LightMode_FontColor_MouseDown = new(Color.FromRgb(0xeb, 0xeb, 0xeb));

        private static readonly SolidColorBrush LightMode_BorderAndBackground_Disabled = new(Color.FromRgb(0xbf, 0xbf, 0xbf));


        private void ColorProviderChanged()
        {
            if (Theme.IsDarkMode)
            {
                #region MouseEnter
                MouseEnter_Border_Animation.From = AccentColors.DarkMode.BorderColorAsBrush;
                MouseEnter_Border_Animation.To = AccentColors.DarkMode.BorderColorMouseOverAsBrush;

                MouseEnter_Background_Animation.From = AccentColors.DarkMode.PrimaryColorAsBrush;
                MouseEnter_Background_Animation.To = AccentColors.DarkMode.MouseOverAsBrush;
                #endregion

                #region MouseDown
                MouseDown_Font_Animation.From = Brushes.Black;
                MouseDown_Font_Animation.To = DarkMode_FontColor_MouseDown;

                MouseDown_Border_Animation.From = AccentColors.DarkMode.BorderColorMouseOverAsBrush;
                MouseDown_Border_Animation.To = AccentColors.DarkMode.MouseDownAsBrush;

                MouseDown_Background_Animation.From = AccentColors.DarkMode.MouseOverAsBrush;
                MouseDown_Background_Animation.To = AccentColors.DarkMode.MouseDownAsBrush;
                #endregion

                #region MouseUp
                MouseUp_Font_Animation.From = DarkMode_FontColor_MouseDown;
                MouseUp_Font_Animation.To = Brushes.Black;

                MouseUp_Border_Animation.From = AccentColors.DarkMode.MouseDownAsBrush;
                MouseUp_Border_Animation.To = AccentColors.DarkMode.BorderColorMouseOverAsBrush;

                MouseUp_Background_Animation.From = AccentColors.DarkMode.MouseDownAsBrush;
                MouseUp_Background_Animation.To = AccentColors.DarkMode.MouseOverAsBrush;
                #endregion

                #region MouseLeave
                MouseLeave_Border_Animation.From = AccentColors.DarkMode.BorderColorMouseOverAsBrush;
                MouseLeave_Border_Animation.To = AccentColors.DarkMode.BorderColorAsBrush;

                MouseLeave_Background_Animation.From = AccentColors.DarkMode.MouseOverAsBrush;
                MouseLeave_Background_Animation.To = AccentColors.DarkMode.PrimaryColorAsBrush;

                MouseLeave_FromDown_Border_Animation.From = AccentColors.DarkMode.MouseDownAsBrush;
                MouseLeave_FromDown_Border_Animation.To = AccentColors.DarkMode.BorderColorAsBrush;

                MouseLeave_FromDown_Background_Animation.From = AccentColors.DarkMode.MouseDownAsBrush;
                MouseLeave_FromDown_Background_Animation.To = AccentColors.DarkMode.PrimaryColorAsBrush;
                #endregion

                //

                #region Disable
                Disable_Font_Animation.From = Brushes.White;
                Disable_Font_Animation.To = DarkMode_FontColor_Disabled;

                Disable_Border_Animation.From = AccentColors.DarkMode.BorderColorAsBrush;
                Disable_Border_Animation.To = DarkMode_BorderAndBackground_Disabled;

                Disable_Background_Animation.From = AccentColors.DarkMode.PrimaryColorAsBrush;
                Disable_Background_Animation.To = DarkMode_BorderAndBackground_Disabled;
                #endregion

                #region Enable
                Enable_Font_Animation.From = DarkMode_FontColor_Disabled;
                Enable_Font_Animation.To = Brushes.Black;

                Enable_Border_Animation.From = DarkMode_BorderAndBackground_Disabled;
                Enable_Border_Animation.To = AccentColors.DarkMode.BorderColorAsBrush;

                Enable_Background_Animation.From = DarkMode_BorderAndBackground_Disabled;
                Enable_Background_Animation.To = AccentColors.DarkMode.PrimaryColorAsBrush;
                #endregion
            }
            else
            {
                #region MouseEnter
                MouseEnter_Border_Animation.From = AccentColors.LightMode.BorderColorAsBrush;
                MouseEnter_Border_Animation.To = AccentColors.LightMode.BorderColorMouseOverAsBrush;

                MouseEnter_Background_Animation.From = AccentColors.LightMode.PrimaryColorAsBrush;
                MouseEnter_Background_Animation.To = AccentColors.LightMode.MouseOverAsBrush;
                #endregion

                #region MouseDown
                MouseDown_Font_Animation.From = Brushes.White;
                MouseDown_Font_Animation.To = LightMode_FontColor_MouseDown;

                MouseDown_Border_Animation.From = AccentColors.LightMode.BorderColorMouseOverAsBrush;
                MouseDown_Border_Animation.To = AccentColors.LightMode.MouseDownAsBrush;

                MouseDown_Background_Animation.From = AccentColors.LightMode.MouseOverAsBrush;
                MouseDown_Background_Animation.To = AccentColors.LightMode.MouseDownAsBrush;
                #endregion

                #region MouseUp
                MouseUp_Font_Animation.From = LightMode_FontColor_MouseDown;
                MouseUp_Font_Animation.To = Brushes.White;

                MouseUp_Border_Animation.From = AccentColors.LightMode.MouseDownAsBrush;
                MouseUp_Border_Animation.To = AccentColors.LightMode.BorderColorMouseOverAsBrush;

                MouseUp_Background_Animation.From = AccentColors.LightMode.MouseDownAsBrush;
                MouseUp_Background_Animation.To = AccentColors.LightMode.MouseOverAsBrush;
                #endregion

                #region MouseLeave
                MouseLeave_Border_Animation.From = AccentColors.LightMode.BorderColorMouseOverAsBrush;
                MouseLeave_Border_Animation.To = AccentColors.LightMode.BorderColorAsBrush;

                MouseLeave_Background_Animation.From = AccentColors.LightMode.MouseOverAsBrush;
                MouseLeave_Background_Animation.To = AccentColors.LightMode.PrimaryColorAsBrush;

                MouseLeave_FromDown_Border_Animation.From = AccentColors.LightMode.MouseDownAsBrush;
                MouseLeave_FromDown_Border_Animation.To = AccentColors.LightMode.BorderColorAsBrush;

                MouseLeave_FromDown_Background_Animation.From = AccentColors.LightMode.MouseDownAsBrush;
                MouseLeave_FromDown_Background_Animation.To = AccentColors.LightMode.PrimaryColorAsBrush;
                #endregion

                #region Disable
                Disable_Border_Animation.From = AccentColors.LightMode.BorderColorAsBrush;
                Disable_Border_Animation.To = LightMode_BorderAndBackground_Disabled;

                Disable_Background_Animation.From = AccentColors.LightMode.PrimaryColorAsBrush;
                Disable_Background_Animation.To = LightMode_BorderAndBackground_Disabled;
                #endregion

                #region Enable
                Enable_Border_Animation.From = LightMode_BorderAndBackground_Disabled;
                Enable_Border_Animation.To = AccentColors.LightMode.BorderColorAsBrush;

                Enable_Background_Animation.From = LightMode_BorderAndBackground_Disabled;
                Enable_Background_Animation.To = AccentColors.LightMode.PrimaryColorAsBrush;
                #endregion
            }

            if (_IsEnabled)
            {
                BeginAnimation(BackgroundProperty, MouseLeave_Border_Animation);
                _innerBorder.BeginAnimation(BackgroundProperty, MouseLeave_Background_Animation);
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, MouseUp_Font_Animation);
            }
            else
            {
                BeginAnimation(BackgroundProperty, Disable_Border_Animation);
                _innerBorder.BeginAnimation(BackgroundProperty, Disable_Background_Animation);

                _textBlock.Foreground = Theme.IsDarkMode ? DarkMode_FontColor_Disabled : Brushes.White;

                //_textBlock.BeginAnimation(TextBlock.ForegroundProperty, Disable_Font_Animation);
            }
        }
    }
}