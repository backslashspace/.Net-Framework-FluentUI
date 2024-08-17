using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FluentUI
{
    internal sealed class ProgressBar : Border
    {
        private Double _progress = 0;
        internal Double Value
        {
            get => _progress;
            set
            {
                _progress = value;

                if (!_isIndeterminate) AnimateToNewState();
            }
        }

        private Double _maximum = 100;
        internal Double Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;

                if (!_isIndeterminate) AnimateToNewState();
            }
        }

        private Boolean _isIndeterminate = false;
        internal Boolean IsIndeterminate
        {
            get => _isIndeterminate;
            set
            {
                if (value)
                {
                    if (_isIndeterminate) return;

                    StartIndeterminateAnimation();
                }
                else
                {
                    _indicator.BeginAnimation(MarginProperty, null);

                    _indicator.HorizontalAlignment = HorizontalAlignment.Left;
                    _indicator.Width = _progress / _maximum * (Width + 2);
                }

                _isIndeterminate = value;
            }
        }

        //

        private readonly Border _indicator = new();

        public ProgressBar()
        {
            Height = 1d;
            CornerRadius = new(0.5d);
            UseLayoutRounding = true;
            Child = _indicator;

            _indicator.Margin = new(-1d);
            _indicator.CornerRadius = new(1.5d);
            _indicator.HorizontalAlignment = HorizontalAlignment.Left;

            if (Theme.IsDarkMode)
            {
                _indicator.Background = AccentColors.DarkMode.PrimaryColorAsBrush;
                Background = _darkMode_Background;
            }
            else
            {
                _indicator.Background = AccentColors.LightMode.PrimaryColorAsBrush;
                Background = _lightMode_Background;
            }

            UI.ColorProviderChanged += UI_ColorProviderChanged;

            Loaded += (s, e) =>
            {
                if (_isIndeterminate)
                {
                    StartIndeterminateAnimation();
                }
                else
                {
                    _indicator.Width = _progress / _maximum * (Width + 2);
                }
            };
        }

        #region Animations
        private static DoubleAnimation _indicatorAnimation = new()
        {
            Duration = new(new(0, 0, 0, 0, 320)),
            DecelerationRatio = 1,
            AccelerationRatio = 0
        };

        private void AnimateToNewState()
        {
            if (Double.IsNaN(Width)) return;

            _indicatorAnimation.To = _progress / _maximum * (Width + 2);
            _indicator.BeginAnimation(WidthProperty, _indicatorAnimation);
        }

        private void StartIndeterminateAnimation()
        {
            if (Double.IsNaN(Width)) return;

            _indicator.BeginAnimation(WidthProperty, null);

            _indicator.HorizontalAlignment = HorizontalAlignment.Stretch;
            _indicator.Width = Double.NaN;

            Thickness startPosition = new(-1d, -1d, Width - _indicator.Margin.Top, -1d);
            Thickness midLeftPosition = new(-1d, -1d, Width - _indicator.Margin.Top - ((Width + 2) * 0.35), -1d);
            Thickness midRightPosition = new(Width - _indicator.Margin.Top - ((Width + 2) * 0.35), -1d, -1d, -1d);
            Thickness endPosition = new(Width - _indicator.Margin.Top, -1d, -1d, -1d);

            ThicknessAnimationUsingKeyFrames animation = new();
            animation.RepeatBehavior = RepeatBehavior.Forever;
            animation.KeyFrames.Add(new LinearThicknessKeyFrame(startPosition, TimeSpan.FromSeconds(0d)));
            animation.KeyFrames.Add(new LinearThicknessKeyFrame(midLeftPosition, TimeSpan.FromSeconds(0.25d)));
            animation.KeyFrames.Add(new LinearThicknessKeyFrame(midRightPosition, TimeSpan.FromSeconds(0.75d)));
            animation.KeyFrames.Add(new LinearThicknessKeyFrame(endPosition, TimeSpan.FromSeconds(1d)));

            _indicator.BeginAnimation(MarginProperty, animation);
        } 
        #endregion

        //

        private static readonly SolidColorBrush _darkMode_Background = new(Color.FromRgb(159, 159, 159));
        private static readonly SolidColorBrush _lightMode_Background = new(Color.FromRgb(139, 140, 137));

        private readonly SolidColorBrushAnimation _foregroundAnimation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _backgroundAnimation = new() { Duration = UI.LongAnimationDuration };

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        private void UI_ColorProviderChanged()
        {
            _foregroundAnimation.From = (SolidColorBrush)_indicator.Background;
            _backgroundAnimation.From = (SolidColorBrush)Background;

            if (Theme.IsDarkMode)
            {
                _foregroundAnimation.To = AccentColors.DarkMode.PrimaryColorAsBrush;
                _backgroundAnimation.To = _darkMode_Background;
            }
            else
            {
                _foregroundAnimation.To = AccentColors.LightMode.PrimaryColorAsBrush;
                _backgroundAnimation.To = _lightMode_Background;
            }

            BeginAnimation(BackgroundProperty, _backgroundAnimation);
            _indicator.BeginAnimation(BackgroundProperty, _foregroundAnimation);
        }
    }
}