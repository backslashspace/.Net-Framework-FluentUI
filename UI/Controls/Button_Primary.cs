using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#pragma warning disable IDE0079
#pragma warning disable CS8618

namespace FluentUI
{
    internal sealed class Button_Primary : Border
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

        private readonly Border _this;
        private readonly Border _innerBorder = new();
        private readonly TextBlock _textBlock = new();

        internal delegate void ClickHandler(Button_Primary sender);
        internal event ClickHandler Click;
        internal delegate void PreviewClickHandler();
        internal event PreviewClickHandler PreviewClick;

        public Button_Primary()
        {
            _this = this;
            UseLayoutRounding = true;
            CornerRadius = new(5);
            Focusable = true;
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
                _isEnabled = _this.IsEnabled; // xaml interface not using 'overridden' IsEnabled property
                ColorProviderChanged();
            };

            MouseEnter += MouseEnterHandler;
            PreviewMouseDown += PreviewMouseDownHandler;
            PreviewMouseUp += PreviewMouseUpHandler;
            MouseLeave += MouseLeaveHandler;

            UI.ColorProviderChanged += ColorProviderChanged;
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        #region BrushAnimations
        private static readonly Duration _shortDuration = new(new(0, 0, 0, 0, 24));
        private static readonly Duration _longDuration = new(new(0, 0, 0, 0, 48));

        private readonly SolidColorBrushAnimation _idle_Font_Animation = new() { Duration = _longDuration };
        private readonly SolidColorBrushAnimation _idle_Border_Animation = new() { Duration = _longDuration };
        private readonly SolidColorBrushAnimation _idle_Background_Animation = new() { Duration = _longDuration };

        private readonly SolidColorBrushAnimation _buttonOver_Border_Animation = new() { Duration = _longDuration };
        private readonly SolidColorBrushAnimation _buttonOver_Background_Animation = new() { Duration = _longDuration };

        private readonly SolidColorBrushAnimation _buttonDown_Font_Animation = new() { Duration = _shortDuration };
        private readonly SolidColorBrushAnimation _buttonDown_Border_Animation = new() { Duration = _shortDuration };
        private readonly SolidColorBrushAnimation _buttonDown_Background_Animation = new() { Duration = _shortDuration };

        private readonly SolidColorBrushAnimation _disable_Font_Animation = new() { Duration = _longDuration };
        private readonly SolidColorBrushAnimation _disable_Border_Animation = new() { Duration = _longDuration };
        private readonly SolidColorBrushAnimation _disable_Background_Animation = new() { Duration = _longDuration };

        private Boolean _buttonUpPending = false;

        private void BeginButtonDownAnimation()
        {
            _buttonDown_Border_Animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _buttonDown_Border_Animation);

            _buttonDown_Background_Animation.From = (SolidColorBrush)_innerBorder.Background;
            _innerBorder.BeginAnimation(BackgroundProperty, _buttonDown_Background_Animation);

            _buttonDown_Font_Animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _buttonDown_Font_Animation);

            _buttonUpPending = true;
        }

        private void BeginButtonUpAnimation()
        {
            _buttonOver_Border_Animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _buttonOver_Border_Animation);

            _buttonOver_Background_Animation.From = (SolidColorBrush)_innerBorder.Background;
            _innerBorder.BeginAnimation(BackgroundProperty, _buttonOver_Background_Animation);

            _idle_Font_Animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_Font_Animation);

            _buttonUpPending = false;
        }
        #endregion

        //

        #region KeyBoardHandler
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key != Key.Space) return;
            if (e.OriginalSource != this) return;

            BeginButtonDownAnimation();

            e.Handled = true;
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if (e.Key != Key.Space) return;

            PreviewClick?.Invoke();

            BeginButtonUpAnimation();

            e.Handled = true;
            Click?.Invoke(this);
        }
        #endregion

        #region MouseHandler
        private void MouseEnterHandler(Object sender, MouseEventArgs e)
        {
            _buttonOver_Border_Animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _buttonOver_Border_Animation);

            _buttonOver_Background_Animation.From = (SolidColorBrush)_innerBorder.Background;
            _innerBorder.BeginAnimation(BackgroundProperty, _buttonOver_Background_Animation);
        }

        private void PreviewMouseDownHandler(Object sender, MouseButtonEventArgs e)
        {
            BeginButtonDownAnimation();
        }

        private void PreviewMouseUpHandler(Object sender, MouseButtonEventArgs e)
        {
            PreviewClick?.Invoke();

            BeginButtonUpAnimation();

            Click?.Invoke(this);
        }

        private void MouseLeaveHandler(Object sender, MouseEventArgs e)
        {
            _idle_Border_Animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _idle_Border_Animation);

            _idle_Background_Animation.From = (SolidColorBrush)_innerBorder.Background;
            _innerBorder.BeginAnimation(BackgroundProperty, _idle_Background_Animation);

            if (_buttonUpPending) // when user presses mouse key down, then drags out of the object (skips mouse up event)
            {
                _idle_Font_Animation.From = (SolidColorBrush)_textBlock.Foreground;
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_Font_Animation);

                _buttonUpPending = false;
            }
        }
        #endregion

        #region En/Disable
        private void Disable()
        {
            MouseLeave -= MouseLeaveHandler;
            _this.IsEnabled = false;

            //

            _disable_Border_Animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _disable_Border_Animation);

            _disable_Background_Animation.From = (SolidColorBrush)_innerBorder.Background;
            _innerBorder.BeginAnimation(BackgroundProperty, _disable_Background_Animation);

            _disable_Font_Animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _disable_Font_Animation);
        }

        private void Enable()
        {
            _idle_Border_Animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _idle_Border_Animation);

            _idle_Background_Animation.From = (SolidColorBrush)_innerBorder.Background;
            _innerBorder.BeginAnimation(BackgroundProperty, _idle_Background_Animation);

            _idle_Font_Animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_Font_Animation);

            _this.IsEnabled = true;
            MouseLeave += MouseLeaveHandler;
        }
        #endregion

        //

        #region Colors
        private static readonly SolidColorBrush _darkMode_FontColor_MouseDown = new(Color.FromRgb(0x2d, 0x2d, 0x2d));
        private static readonly SolidColorBrush _darkMode_FontColor_Disabled = new(Color.FromRgb(0xa7, 0xa7, 0xa7));
        private static readonly SolidColorBrush _darkMode_BorderAndBackground_Disabled = new(Color.FromRgb(0x43, 0x43, 0x43));

        private static readonly SolidColorBrush _lightMode_FocusVisual = new(Color.FromRgb(0x1a, 0x1a, 0x1a));
        private static readonly SolidColorBrush _lightMode_FontColor_MouseDown = new(Color.FromRgb(0xeb, 0xeb, 0xeb));
        private static readonly SolidColorBrush _lightMode_BorderAndBackground_Disabled = new(Color.FromRgb(0xbf, 0xbf, 0xbf)); 
        #endregion

        private void ColorProviderChanged()
        {
            FrameworkElementFactory focusVisual_FrameworkElementFactory = new(typeof(Border));
            focusVisual_FrameworkElementFactory.SetValue(BorderThicknessProperty, new Thickness(2d));
            focusVisual_FrameworkElementFactory.SetValue(MarginProperty, new Thickness(-3d));
            focusVisual_FrameworkElementFactory.SetValue(CornerRadiusProperty, new CornerRadius(7.5d));

            if (Theme.IsDarkMode)
            {
                focusVisual_FrameworkElementFactory.SetValue(BorderBrushProperty, Brushes.White);

                Background = AccentColors.DarkMode.BorderColorAsBrush;
                _innerBorder.Background = AccentColors.DarkMode.PrimaryColorAsBrush;
                _textBlock.Foreground = Brushes.Black;

                //

                _idle_Font_Animation.To = Brushes.Black;
                _idle_Border_Animation.To = AccentColors.DarkMode.BorderColorAsBrush;
                _idle_Background_Animation.To = AccentColors.DarkMode.PrimaryColorAsBrush;

                _buttonOver_Border_Animation.To = AccentColors.DarkMode.BorderColorMouseOverAsBrush;
                _buttonOver_Background_Animation.To = AccentColors.DarkMode.MouseOverAsBrush;

                _buttonDown_Font_Animation.To = _darkMode_FontColor_MouseDown;
                _buttonDown_Border_Animation.To = AccentColors.DarkMode.MouseDownAsBrush;
                _buttonDown_Background_Animation.To = AccentColors.DarkMode.MouseDownAsBrush;

                _disable_Font_Animation.To = _darkMode_FontColor_Disabled;
                _disable_Border_Animation.To = _darkMode_BorderAndBackground_Disabled;
                _disable_Background_Animation.To = _darkMode_BorderAndBackground_Disabled;
            }
            else
            {
                focusVisual_FrameworkElementFactory.SetValue(BorderBrushProperty, _lightMode_FocusVisual);

                Background = AccentColors.LightMode.BorderColorAsBrush;
                _innerBorder.Background = AccentColors.LightMode.PrimaryColorAsBrush;
                _textBlock.Foreground = Brushes.White;

                //

                _idle_Font_Animation.To = Brushes.White;
                _idle_Border_Animation.To = AccentColors.LightMode.BorderColorAsBrush;
                _idle_Background_Animation.To = AccentColors.LightMode.PrimaryColorAsBrush;

                _buttonOver_Border_Animation.To = AccentColors.LightMode.BorderColorMouseOverAsBrush;
                _buttonOver_Background_Animation.To = AccentColors.LightMode.MouseOverAsBrush;

                _buttonDown_Font_Animation.To = _lightMode_FontColor_MouseDown;
                _buttonDown_Border_Animation.To = AccentColors.LightMode.MouseDownAsBrush;
                _buttonDown_Background_Animation.To = AccentColors.LightMode.MouseDownAsBrush;

                _disable_Font_Animation.To = Brushes.White;
                _disable_Border_Animation.To = _lightMode_BorderAndBackground_Disabled;
                _disable_Background_Animation.To = _lightMode_BorderAndBackground_Disabled;
            }

            ControlTemplate focusVisualControlTemplate = new(typeof(Control));
            focusVisualControlTemplate.VisualTree = focusVisual_FrameworkElementFactory;
            Style style = new();
            style.Setters.Add(new Setter(Control.TemplateProperty, focusVisualControlTemplate));
            FocusVisualStyle = style;

            if (_isEnabled)
            {
                _idle_Border_Animation.From = (SolidColorBrush)Background;
                BeginAnimation(BackgroundProperty, _idle_Border_Animation);

                _idle_Background_Animation.From = (SolidColorBrush)_innerBorder.Background;
                _innerBorder.BeginAnimation(BackgroundProperty, _idle_Background_Animation);

                _idle_Font_Animation.From = (SolidColorBrush)_textBlock.Foreground;
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_Font_Animation);
            }
            else
            {
                _disable_Border_Animation.From = (SolidColorBrush)Background;
                BeginAnimation(BackgroundProperty, _disable_Border_Animation);

                _disable_Background_Animation.From = (SolidColorBrush)_innerBorder.Background;
                _innerBorder.BeginAnimation(BackgroundProperty, _disable_Background_Animation);

                _disable_Font_Animation.From = (SolidColorBrush)_textBlock.Foreground;
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _disable_Font_Animation);
            }
        }
    }
}