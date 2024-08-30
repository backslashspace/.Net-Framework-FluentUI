using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

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

        private Boolean _isChecked = false;
        internal Boolean IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;

                //start animation to new state

                if (!EventDecoupling)
                {
                    if (_isChecked) Checked?.Invoke(this);
                    else Unchecked?.Invoke(this);
                }
            }
        }

        /// <summary>Whether changes to <seealso cref="IsChecked"/> made by the backend should raise <seealso cref="Checked"/> or <seealso cref="Unchecked"/> events.</summary>
        /// <remarks>Default = false</remarks>
        internal Boolean EventDecoupling = false;

        new internal Boolean IsInitialized { get; private set; } = false;

        #region Definitions
        private readonly Grid _this;
        private readonly Border _checkboxBorder = new();
        private readonly Border _checkboxBackground = new();
        private readonly Path _optionMark = new();
        private readonly TextBlock _textBlock = new(false);

        internal delegate void CheckedHandler(Checkbox sender);
        internal event CheckedHandler Checked;
        internal delegate void UncheckedHandler(Checkbox sender);
        internal event UncheckedHandler Unchecked;
        #endregion

        public Checkbox()
        {
            _this = this;
            Height = 20d;
            Width = 128d;
            UseLayoutRounding = true;

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
            _textBlock.HorizontalAlignment = HorizontalAlignment.Left;
            _textBlock.VerticalAlignment = VerticalAlignment.Center;
            Children.Add(_textBlock);

            PreviewMouseDown += PreviewMouseDownHandler;
            PreviewMouseUp += PreviewMouseUpHandler;
            MouseLeave += MouseLeaveHandler;

            UI.ColorProviderChanged += ColorProviderChanged;

            Loaded += OnLoaded;
        }

        private void OnLoaded(Object sender, RoutedEventArgs e)
        {
            _isEnabled = _this.IsEnabled; // xaml interface not using 'overridden' IsEnabled property

            if (_isEnabled)
            {
                MouseEnter += MouseEnterHandler;

                if (_isChecked)
                {
                    _checkboxBorder.Background = Theme.IsDarkMode ? AccentColors.DarkMode.IdleAsBrush : AccentColors.LightMode.IdleAsBrush;
                    _checkboxBackground.Background = Theme.IsDarkMode ? AccentColors.DarkMode.IdleAsBrush : AccentColors.LightMode.IdleAsBrush;
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

            ColorProviderChanged();

            IsInitialized = true;
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        #region Animation Definitions
        private readonly SolidColorBrushAnimation _idle_font_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _idle_option_mark_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly DoubleAnimation          _idle_option_mark_opacity_animation = new() { Duration = UI.LongAnimationDuration, To = 1d };
        private readonly SolidColorBrushAnimation _idle_border_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _idle_background_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _idle_unchecked_border_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _idle_unchecked_background_animation = new() { Duration = UI.LongAnimationDuration };

        private readonly SolidColorBrushAnimation _mouse_over_border_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _mouse_over_background_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _mouse_over_unchecked_border_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _mouse_over_unchecked_background_animation = new() { Duration = UI.LongAnimationDuration };

        private readonly DoubleAnimation          _mouse_down_option_mark_animation = new() { Duration = UI.ShortAnimationDuration, To = 0.5d };
        private readonly SolidColorBrushAnimation _mouse_down_border_animation = new() { Duration = UI.ShortAnimationDuration };
        private readonly SolidColorBrushAnimation _mouse_down_background_animation = new() { Duration = UI.ShortAnimationDuration };
        private readonly SolidColorBrushAnimation _mouse_down_unchecked_border_animation = new() { Duration = UI.ShortAnimationDuration };
        private readonly SolidColorBrushAnimation _mouse_down_unchecked_background_animation = new() { Duration = UI.ShortAnimationDuration };

        private readonly SolidColorBrushAnimation _disable_font_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _disable_option_mark_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _disable_border_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _disable_background_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _disable_unchecked_border_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _disable_unchecked_background_animation = new() { Duration = UI.LongAnimationDuration };

        private Boolean _buttonUpPending = false;

        private void BeginButtonDownAnimation()
        {
            _mouse_down_border_animation.From = (SolidColorBrush)_checkboxBorder.Background;
            _checkboxBorder.BeginAnimation(Border.BackgroundProperty, _mouse_down_border_animation);

            _mouse_down_background_animation.From = (SolidColorBrush)_checkboxBackground.Background;
            _checkboxBackground.BeginAnimation(Border.BackgroundProperty, _mouse_down_background_animation);

            _mouse_down_option_mark_animation.From = _optionMark.Opacity;
            _optionMark.BeginAnimation(Path.OpacityProperty, _mouse_down_option_mark_animation);

            _buttonUpPending = true;
        }

        private void BeginButtonUpAnimation()
        {
            _mouse_over_border_animation.From = (SolidColorBrush)_checkboxBorder.Background;
            _checkboxBorder.BeginAnimation(Border.BackgroundProperty, _mouse_over_border_animation);

            _mouse_over_background_animation.From = (SolidColorBrush)_checkboxBackground.Background;
            _checkboxBackground.BeginAnimation(Border.BackgroundProperty, _mouse_over_background_animation);

            _idle_option_mark_opacity_animation.From = _optionMark.Opacity;
            _optionMark.BeginAnimation(Path.OpacityProperty, _idle_option_mark_opacity_animation);

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

            BeginButtonUpAnimation();

            _isChecked = !_isChecked;

            e.Handled = true;

            if (_isChecked) Checked?.Invoke(this);
            else Unchecked?.Invoke(this);
        }
        #endregion

        #region MouseHandler
        private void MouseEnterHandler(Object sender, MouseEventArgs e)
        {
            _mouse_over_border_animation.From = (SolidColorBrush)_checkboxBorder.Background;
            _checkboxBorder.BeginAnimation(Border.BackgroundProperty, _mouse_over_border_animation);

            _mouse_over_background_animation.From = (SolidColorBrush)_checkboxBackground.Background;
            _checkboxBackground.BeginAnimation(Border.BackgroundProperty, _mouse_over_background_animation);
        }

        private void PreviewMouseDownHandler(Object sender, MouseButtonEventArgs e) => BeginButtonDownAnimation();

        private void PreviewMouseUpHandler(Object sender, MouseButtonEventArgs e)
        {
            BeginButtonUpAnimation();

            _isChecked = !_isChecked;

            if (_isChecked) Checked?.Invoke(this);
            else Unchecked?.Invoke(this);
        }

        private void MouseLeaveHandler(Object sender, MouseEventArgs e)
        {
            _idle_border_animation.From = (SolidColorBrush)_checkboxBorder.Background;
            _checkboxBorder.BeginAnimation(Border.BackgroundProperty, _idle_border_animation);

            _idle_background_animation.From = (SolidColorBrush)_checkboxBackground.Background;
            _checkboxBackground.BeginAnimation(Border.BackgroundProperty, _idle_background_animation);

            if (_buttonUpPending) // when user presses mouse key down, then drags out of the object (skips mouse up event)
            {
                _idle_option_mark_animation.From = (SolidColorBrush)_optionMark.Fill;
                _optionMark.BeginAnimation(Path.FillProperty, _idle_option_mark_animation);

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

            _disable_border_animation.From = (SolidColorBrush)_checkboxBorder.Background;
            _checkboxBorder.BeginAnimation(Border.BackgroundProperty, _disable_border_animation);

            _disable_background_animation.From = (SolidColorBrush)_checkboxBackground.Background;
            _checkboxBackground.BeginAnimation(Border.BackgroundProperty, _disable_background_animation);

            _disable_option_mark_animation.From = (SolidColorBrush)_optionMark.Fill;
            _optionMark.BeginAnimation(Path.FillProperty, _disable_option_mark_animation);

            _disable_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _disable_option_mark_animation);
        }

        private void Enable()
        {
            _idle_border_animation.From = (SolidColorBrush)_checkboxBorder.Background;
            _checkboxBorder.BeginAnimation(Border.BackgroundProperty, _idle_border_animation);

            _idle_background_animation.From = (SolidColorBrush)_checkboxBackground.Background;
            _checkboxBackground.BeginAnimation(Border.BackgroundProperty, _idle_background_animation);

            _disable_option_mark_animation.From = (SolidColorBrush)_optionMark.Fill;
            _optionMark.BeginAnimation(Path.FillProperty, _disable_option_mark_animation);

            _idle_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_font_animation);

            _this.IsEnabled = true;
            MouseLeave += MouseLeaveHandler;
        }
        #endregion

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        private static class Colors
        {
            internal static class DarkMode
            {
                internal static readonly SolidColorBrush IdleFont = Brushes.White;
                internal static readonly SolidColorBrush IdleOptionMark = Brushes.Black;
                internal static readonly SolidColorBrush IdleUncheckedBorder = new(Color.FromRgb(0x9e, 0x9e, 0x9e));
                internal static readonly SolidColorBrush IdleUncheckedBackground = new(Color.FromRgb(0x27, 0x27, 0x27));

                internal static readonly SolidColorBrush MouseOverUncheckedBorder = new(Color.FromRgb(0xa0, 0xa0, 0xa0));
                internal static readonly SolidColorBrush MouseOverUncheckedBackground = new(Color.FromRgb(0x34, 0x34, 0x34));

                internal static readonly SolidColorBrush MouseDownUncheckedBorder = new(Color.FromRgb(0x52, 0x52, 0x52));
                internal static readonly SolidColorBrush MouseDownUncheckedBackground = new(Color.FromRgb(0x3a, 0x3a, 0x3a));

                internal static readonly SolidColorBrush DisabledFont = new(Color.FromRgb(0x78, 0x78, 0x78));
                internal static readonly SolidColorBrush DisabledOptionMark = new(Color.FromRgb(0xa5, 0xa5, 0xa5));
                internal static readonly SolidColorBrush DisabledBorder = new(Color.FromRgb(0x5b, 0x5b, 0x5b));
                internal static readonly SolidColorBrush DisabledBackground = new(Color.FromRgb(0x4c, 0x4c, 0x4c));
                internal static readonly SolidColorBrush DisabledUncheckedBorder = new(Color.FromRgb(0x4c, 0x4c, 0x4c));
                internal static readonly SolidColorBrush DisabledUncheckedBackground = new(Color.FromRgb(0x2b, 0x2b, 0x2b));
            }

            internal static class LightMode
            {
                internal static readonly SolidColorBrush IdleFont = new(Color.FromRgb(0x1b, 0x1b, 0x1b));
                internal static readonly SolidColorBrush IdleOptionMark = Brushes.White;
                internal static readonly SolidColorBrush IdleUncheckedBorder = new(Color.FromRgb(0x89, 0x89, 0x89));
                internal static readonly SolidColorBrush IdleUncheckedBackground = new(Color.FromRgb(0xf5, 0xf5, 0xf5));

                internal static readonly SolidColorBrush MouseOverUncheckedBorder = new(Color.FromRgb(0x89, 0x89, 0x89));
                internal static readonly SolidColorBrush MouseOverUncheckedBackground = new(Color.FromRgb(0xec, 0xec, 0xec));

                internal static readonly SolidColorBrush MouseDownUncheckedBorder = new(Color.FromRgb(0xbb, 0xbb, 0xbb));
                internal static readonly SolidColorBrush MouseDownUncheckedBackground = new(Color.FromRgb(0xe3, 0xe3, 0xe3));

                internal static readonly SolidColorBrush DisabledFont = new(Color.FromRgb(0xa0, 0xa0, 0xa0));
                internal static readonly SolidColorBrush DisabledOptionMark = Brushes.White;
                internal static readonly SolidColorBrush DisabledBorder = new(Color.FromRgb(0xb0, 0xb0, 0xb0));
                internal static readonly SolidColorBrush DisabledBackground = new(Color.FromRgb(0xc5, 0xc5, 0xc5));
                internal static readonly SolidColorBrush DisabledUncheckedBorder = new(Color.FromRgb(0xc5, 0xc5, 0xc5));
                internal static readonly SolidColorBrush DisabledUncheckedBackground = new(Color.FromRgb(0xfb, 0xfb, 0xfb));
            }
        }

        private void ColorProviderChanged()
        {
            if (Theme.IsDarkMode)
            {
                _idle_font_animation.To = Colors.DarkMode.IdleFont;
                _idle_option_mark_animation.To = Colors.DarkMode.IdleOptionMark;
                _idle_border_animation.To = AccentColors.DarkMode.IdleBorderAsBrush;
                _idle_background_animation.To = AccentColors.DarkMode.IdleAsBrush;
                _idle_unchecked_border_animation.To = Colors.DarkMode.IdleUncheckedBorder;
                _idle_unchecked_background_animation.To = Colors.DarkMode.IdleUncheckedBackground;

                _mouse_over_border_animation.To = AccentColors.DarkMode.MouseOverBorderAsBrush;
                _mouse_over_background_animation.To = AccentColors.DarkMode.MouseOverBackgroundAsBrush;
                _mouse_over_unchecked_border_animation.To = Colors.DarkMode.MouseOverUncheckedBorder;
                _mouse_over_unchecked_background_animation.To = Colors.DarkMode.MouseOverUncheckedBackground;

                _mouse_down_border_animation.To = AccentColors.DarkMode.MouseDownAsBrush;
                _mouse_down_background_animation.To = AccentColors.DarkMode.MouseDownAsBrush;
                _mouse_down_unchecked_border_animation.To = Colors.DarkMode.MouseDownUncheckedBorder;
                _mouse_down_unchecked_background_animation.To = Colors.DarkMode.MouseDownUncheckedBackground;

                _disable_font_animation.To = Colors.DarkMode.DisabledFont;
                _disable_option_mark_animation.To = Colors.DarkMode.DisabledOptionMark;
                _disable_border_animation.To = Colors.DarkMode.DisabledBorder;
                _disable_background_animation.To = Colors.DarkMode.DisabledBackground;
                _disable_unchecked_border_animation.To = Colors.DarkMode.DisabledUncheckedBorder;
                _disable_unchecked_background_animation.To = Colors.DarkMode.DisabledUncheckedBackground;
            }
            else
            {
                _idle_font_animation.To = Colors.LightMode.IdleFont;
                _idle_option_mark_animation.To = Colors.LightMode.IdleOptionMark;
                _idle_border_animation.To = AccentColors.LightMode.IdleAsBrush;
                _idle_background_animation.To = AccentColors.LightMode.IdleAsBrush;
                _idle_unchecked_border_animation.To = Colors.LightMode.IdleUncheckedBorder;
                _idle_unchecked_background_animation.To = Colors.LightMode.IdleUncheckedBackground;

                _mouse_over_border_animation.To = AccentColors.LightMode.MouseOverBorderAsBrush;
                _mouse_over_background_animation.To = AccentColors.LightMode.MouseOverBackgroundAsBrush;
                _mouse_over_unchecked_border_animation.To = Colors.LightMode.MouseOverUncheckedBorder;
                _mouse_over_unchecked_background_animation.To = Colors.LightMode.MouseOverUncheckedBackground;

                _mouse_down_border_animation.To = AccentColors.LightMode.MouseDownAsBrush;
                _mouse_down_background_animation.To = AccentColors.LightMode.MouseDownAsBrush;
                _mouse_down_unchecked_border_animation.To = Colors.LightMode.MouseDownUncheckedBorder;
                _mouse_down_unchecked_background_animation.To = Colors.LightMode.MouseDownUncheckedBackground;

                _disable_font_animation.To = Colors.LightMode.DisabledFont;
                _disable_option_mark_animation.To = Colors.LightMode.DisabledOptionMark;
                _disable_border_animation.To = Colors.LightMode.DisabledBorder;
                _disable_background_animation.To = Colors.LightMode.DisabledBackground;
                _disable_unchecked_border_animation.To = Colors.LightMode.DisabledUncheckedBorder;
                _disable_unchecked_background_animation.To = Colors.LightMode.DisabledUncheckedBackground;
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