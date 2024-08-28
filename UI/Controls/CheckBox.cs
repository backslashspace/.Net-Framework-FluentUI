using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
                    //Enable();
                }
                else
                {
                    //Disable();
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
        private readonly Path _optionMark = new();
        private readonly TextBlock _textBlock = new(false);
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

            //MouseEnter += MouseEnterHandler;
            //PreviewMouseDown += PreviewMouseDownHandler;
            //PreviewMouseUp += PreviewMouseUpHandler;
            //MouseLeave += MouseLeaveHandler;

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

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        #region BrushAnimations
        private readonly SolidColorBrushAnimation _idle_option_mark_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _idle_border_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _idle_background_animation = new() { Duration = UI.LongAnimationDuration };

        private readonly SolidColorBrushAnimation _mouse_over_border_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _mouse_over_background_animation = new() { Duration = UI.LongAnimationDuration };

        private readonly SolidColorBrushAnimation _mouse_down_option_mark_animation = new() { Duration = UI.ShortAnimationDuration };
        private readonly SolidColorBrushAnimation _mouse_down_border_animation = new() { Duration = UI.ShortAnimationDuration };
        private readonly SolidColorBrushAnimation _mouse_down_background_animation = new() { Duration = UI.ShortAnimationDuration };

        private readonly SolidColorBrushAnimation _disable_option_mark_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _disable_border_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _disable_background_animation = new() { Duration = UI.LongAnimationDuration };

        private Boolean _buttonUpPending = false;

        private void BeginButtonDownAnimation()
        {
            _mouse_down_border_animation.From = (SolidColorBrush)_checkboxBorder.Background;
            _checkboxBorder.BeginAnimation(Border.BackgroundProperty, _mouse_down_border_animation);

            _mouse_down_background_animation.From = (SolidColorBrush)_checkboxBackground.Background;
            _checkboxBackground.BeginAnimation(Border.BackgroundProperty, _mouse_down_background_animation);

            _mouse_down_option_mark_animation.From = (SolidColorBrush)_optionMark.Fill;
            _optionMark.BeginAnimation(Path.FillProperty, _mouse_down_option_mark_animation);

            _buttonUpPending = true;
        }

        private void BeginButtonUpAnimation()
        {
            _mouse_over_border_animation.From = (SolidColorBrush)_checkboxBorder.Background;
            _checkboxBorder.BeginAnimation(Border.BackgroundProperty, _mouse_over_border_animation);

            _mouse_over_background_animation.From = (SolidColorBrush)_checkboxBackground.Background;
            _checkboxBackground.BeginAnimation(Border.BackgroundProperty, _mouse_over_background_animation);

            _idle_option_mark_animation.From = (SolidColorBrush)_optionMark.Fill;
            _optionMark.BeginAnimation(Path.FillProperty, _idle_option_mark_animation);

            _buttonUpPending = false;
        }
        #endregion

        //

        //#region KeyBoardHandler
        //protected override void OnPreviewKeyDown(KeyEventArgs e)
        //{
        //    if (e.Key != Key.Space) return;
        //    if (e.OriginalSource != this) return;

        //    BeginButtonDownAnimation();

        //    e.Handled = true;
        //}

        //protected override void OnPreviewKeyUp(KeyEventArgs e)
        //{
        //    if (e.Key != Key.Space) return;

        //    PreviewClick?.Invoke();

        //    BeginButtonUpAnimation();

        //    e.Handled = true;
        //    Click?.Invoke(this);
        //}
        //#endregion

        //#region MouseHandler
        //private void MouseEnterHandler(Object sender, MouseEventArgs e)
        //{
        //    _mouse_over_border_animation.From = (SolidColorBrush)Background;
        //    BeginAnimation(Border.BackgroundProperty, _mouse_over_border_animation);

        //    _mouse_over_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
        //    _buttonBackground.BeginAnimation(Border.BackgroundProperty, _mouse_over_background_animation);
        //}

        //private void PreviewMouseDownHandler(Object sender, MouseButtonEventArgs e) => BeginButtonDownAnimation();

        //private void PreviewMouseUpHandler(Object sender, MouseButtonEventArgs e)
        //{
        //    PreviewClick?.Invoke();

        //    BeginButtonUpAnimation();

        //    Click?.Invoke(this);
        //}

        //private void MouseLeaveHandler(Object sender, MouseEventArgs e)
        //{
        //    _idle_border_animation.From = (SolidColorBrush)Background;
        //    BeginAnimation(Border.BackgroundProperty, _idle_border_animation);

        //    _idle_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
        //    _buttonBackground.BeginAnimation(Border.BackgroundProperty, _idle_background_animation);

        //    if (_buttonUpPending) // when user presses mouse key down, then drags out of the object (skips mouse up event)
        //    {
        //        _idle_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
        //        _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_font_animation);

        //        _buttonUpPending = false;
        //    }
        //}
        //#endregion

        //#region En/Disable
        //private void Disable()
        //{
        //    MouseLeave -= MouseLeaveHandler;
        //    _this.IsEnabled = false;

        //    //

        //    _disable_border_animation.From = (SolidColorBrush)Background;
        //    BeginAnimation(Border.BackgroundProperty, _disable_border_animation);

        //    _disable_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
        //    _buttonBackground.BeginAnimation(Border.BackgroundProperty, _disable_background_animation);

        //    _disable_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
        //    _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _disable_font_animation);
        //}

        //private void Enable()
        //{
        //    _idle_border_animation.From = (SolidColorBrush)Background;
        //    BeginAnimation(Border.BackgroundProperty, _idle_border_animation);

        //    _idle_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
        //    _buttonBackground.BeginAnimation(Border.BackgroundProperty, _idle_background_animation);

        //    _idle_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
        //    _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_font_animation);

        //    _this.IsEnabled = true;
        //    MouseLeave += MouseLeaveHandler;
        //}
        //#endregion

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

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