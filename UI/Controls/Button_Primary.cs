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

        new internal Boolean IsInitialized { get; private set; } = false;

        #region Definitions
        private readonly Border _this;
        private readonly Border _buttonBackground = new();
        private readonly TextBlock _textBlock = new(true);

        internal delegate void ClickHandler(Button_Primary sender);
        internal event ClickHandler Click;
        internal delegate void PreviewClickHandler();
        internal event PreviewClickHandler PreviewClick;
        #endregion

        public Button_Primary()
        {
            _this = this;
            UseLayoutRounding = true;
            CornerRadius = new(5d);
            Focusable = true;
            Height = 32d;
            Width = 152d;
            Child = _buttonBackground;

            _buttonBackground.Margin = new(1d);
            _buttonBackground.CornerRadius = new(4d);
            _buttonBackground.Child = _textBlock;

            _textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            _textBlock.VerticalAlignment = VerticalAlignment.Center;

            if (Theme.IsDarkMode)
            {
                Background = AccentColors.DarkMode.BorderColorAsBrush;
                _buttonBackground.Background = AccentColors.DarkMode.PrimaryColorAsBrush;
                _textBlock.Foreground = Brushes.Black;
            }
            else
            {
                Background = AccentColors.LightMode.BorderColorAsBrush;
                _buttonBackground.Background = AccentColors.LightMode.PrimaryColorAsBrush;
                _textBlock.Foreground = Brushes.White;
            }

            MouseEnter += MouseEnterHandler;
            PreviewMouseDown += PreviewMouseDownHandler;
            PreviewMouseUp += PreviewMouseUpHandler;
            MouseLeave += MouseLeaveHandler;

            UI.ColorProviderChanged += ColorProviderChanged;

            Loaded += (s, e) =>
            {
                _isEnabled = _this.IsEnabled; // xaml interface not using 'overridden' IsEnabled property
                ColorProviderChanged();

                IsInitialized = true;
            };
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        #region BrushAnimations
        private static readonly Duration _shortDuration = new(new(0, 0, 0, 0, 24));
        private static readonly Duration _longDuration = new(new(0, 0, 0, 0, 48));

        private readonly SolidColorBrushAnimation _idle_font_animation = new() { Duration = _longDuration };
        private readonly SolidColorBrushAnimation _idle_border_animation = new() { Duration = _longDuration };
        private readonly SolidColorBrushAnimation _idle_background_animation = new() { Duration = _longDuration };

        private readonly SolidColorBrushAnimation _button_over_border_animation = new() { Duration = _longDuration };
        private readonly SolidColorBrushAnimation _button_over_background_animation = new() { Duration = _longDuration };

        private readonly SolidColorBrushAnimation _button_down_font_animation = new() { Duration = _shortDuration };
        private readonly SolidColorBrushAnimation _button_down_border_animation = new() { Duration = _shortDuration };
        private readonly SolidColorBrushAnimation _button_down_background_animation = new() { Duration = _shortDuration };

        private readonly SolidColorBrushAnimation _disable_font_animation = new() { Duration = _longDuration };
        private readonly SolidColorBrushAnimation _disable_border_animation = new() { Duration = _longDuration };
        private readonly SolidColorBrushAnimation _disable_background_animation = new() { Duration = _longDuration };

        private Boolean _buttonUpPending = false;

        private void BeginButtonDownAnimation()
        {
            _button_down_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _button_down_border_animation);

            _button_down_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(BackgroundProperty, _button_down_background_animation);

            _button_down_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _button_down_font_animation);

            _buttonUpPending = true;
        }

        private void BeginButtonUpAnimation()
        {
            _button_over_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _button_over_border_animation);

            _button_over_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(BackgroundProperty, _button_over_background_animation);

            _idle_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_font_animation);

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
            _button_over_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _button_over_border_animation);

            _button_over_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(BackgroundProperty, _button_over_background_animation);
        }

        private void PreviewMouseDownHandler(Object sender, MouseButtonEventArgs e) => BeginButtonDownAnimation();

        private void PreviewMouseUpHandler(Object sender, MouseButtonEventArgs e)
        {
            PreviewClick?.Invoke();

            BeginButtonUpAnimation();

            Click?.Invoke(this);
        }

        private void MouseLeaveHandler(Object sender, MouseEventArgs e)
        {
            _idle_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _idle_border_animation);

            _idle_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(BackgroundProperty, _idle_background_animation);

            if (_buttonUpPending) // when user presses mouse key down, then drags out of the object (skips mouse up event)
            {
                _idle_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_font_animation);

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

            _disable_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _disable_border_animation);

            _disable_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(BackgroundProperty, _disable_background_animation);

            _disable_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _disable_font_animation);
        }

        private void Enable()
        {
            _idle_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _idle_border_animation);

            _idle_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(BackgroundProperty, _idle_background_animation);

            _idle_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_font_animation);

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

                _idle_font_animation.To = Brushes.Black;
                _idle_border_animation.To = AccentColors.DarkMode.BorderColorAsBrush;
                _idle_background_animation.To = AccentColors.DarkMode.PrimaryColorAsBrush;

                _button_over_border_animation.To = AccentColors.DarkMode.BorderColorMouseOverAsBrush;
                _button_over_background_animation.To = AccentColors.DarkMode.MouseOverAsBrush;

                _button_down_font_animation.To = _darkMode_FontColor_MouseDown;
                _button_down_border_animation.To = AccentColors.DarkMode.MouseDownAsBrush;
                _button_down_background_animation.To = AccentColors.DarkMode.MouseDownAsBrush;

                _disable_font_animation.To = _darkMode_FontColor_Disabled;
                _disable_border_animation.To = _darkMode_BorderAndBackground_Disabled;
                _disable_background_animation.To = _darkMode_BorderAndBackground_Disabled;
            }
            else
            {
                focusVisual_FrameworkElementFactory.SetValue(BorderBrushProperty, _lightMode_FocusVisual);

                _idle_font_animation.To = Brushes.White;
                _idle_border_animation.To = AccentColors.LightMode.BorderColorAsBrush;
                _idle_background_animation.To = AccentColors.LightMode.PrimaryColorAsBrush;

                _button_over_border_animation.To = AccentColors.LightMode.BorderColorMouseOverAsBrush;
                _button_over_background_animation.To = AccentColors.LightMode.MouseOverAsBrush;

                _button_down_font_animation.To = _lightMode_FontColor_MouseDown;
                _button_down_border_animation.To = AccentColors.LightMode.MouseDownAsBrush;
                _button_down_background_animation.To = AccentColors.LightMode.MouseDownAsBrush;

                _disable_font_animation.To = Brushes.White;
                _disable_border_animation.To = _lightMode_BorderAndBackground_Disabled;
                _disable_background_animation.To = _lightMode_BorderAndBackground_Disabled;
            }

            ControlTemplate focusVisualControlTemplate = new(typeof(Control));
            focusVisualControlTemplate.VisualTree = focusVisual_FrameworkElementFactory;
            Style style = new();
            style.Setters.Add(new Setter(Control.TemplateProperty, focusVisualControlTemplate));
            FocusVisualStyle = style;

            if (IsInitialized)
            {
                AnimateToNewState();
            }
        }
    
        private void AnimateToNewState()
        {
            if (_isEnabled)
            {
                _idle_border_animation.From = (SolidColorBrush)Background;
                BeginAnimation(BackgroundProperty, _idle_border_animation);

                _idle_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
                _buttonBackground.BeginAnimation(BackgroundProperty, _idle_background_animation);

                _idle_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_font_animation);
            }
            else
            {
                _disable_border_animation.From = (SolidColorBrush)Background;
                BeginAnimation(BackgroundProperty, _disable_border_animation);

                _disable_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
                _buttonBackground.BeginAnimation(BackgroundProperty, _disable_background_animation);

                _disable_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _disable_font_animation);
            }
        }
    }
}