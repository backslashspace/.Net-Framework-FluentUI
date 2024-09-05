using System;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace FluentUI
{
    internal class ToggleButton : Border
    {
        private readonly Border _background = new();
        private readonly Canvas _canvas = new();
        private readonly Ellipse _indicator = new();

        

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
            }
        }

        /// <summary>Whether changes to <seealso cref="IsChecked"/> made by the backend should raise <seealso cref="Checked"/> or <seealso cref="Unchecked"/> events.</summary>
        /// <remarks>Default = false</remarks>
        internal Boolean DecoupledEvents { get; set; } = false;

        new internal Boolean IsInitialized { get; private set; } = false;

        #region Definitions
        internal delegate void CheckedHandler(Checkbox sender);
        internal event CheckedHandler Checked;
        internal delegate void UncheckedHandler(Checkbox sender);
        internal event UncheckedHandler Unchecked;
        #endregion

        public ToggleButton()
        {
            Height = 20d;
            Width = 40d;
            CornerRadius = new(10d);
            Background = Brushes.DimGray;
            Child = _background;

            _background.Background = Brushes.DarkBlue;
            _background.CornerRadius = new(9d);
            _background.Margin = new(1d);
            _background.Child = _canvas;

            _canvas.Height = 18d;
            _canvas.Width = 38d;
            _canvas.PreviewMouseMove += CanvasMouseMove;
            _canvas.PreviewMouseUp += (s, e) => { CanvasMouseUp(); e.Handled = true; };
            _canvas.Children.Add(_indicator);
            Canvas.SetTop(_indicator, 3);
            Canvas.SetLeft(_indicator, 3);

            _indicator.Fill = Brushes.LightGray;
            _indicator.Height = 12;
            _indicator.Width = 12;
            _indicator.PreviewMouseDown += IndicatorMouseDown;

            PreviewMouseUp += (s, e) => { CanvasMouseUp(); e.Handled = true; };

            Loaded += OnLoaded;
        }

        private void OnLoaded(Object sender, RoutedEventArgs e)
        {
            _isEnabled = base.IsEnabled; // xaml interface not using 'overridden' IsEnabled property

            /*
             * 
             * 
             * 
             * 
             */

            ColorProviderChanged();

            IsInitialized = true;
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #




        #region ManualMouseMove
        private Boolean _dragMode = false;
        private Point _offset;
        private Double _initialIndicatorPositionX;

        private void IndicatorMouseDown(Object sender, MouseButtonEventArgs e)
        {
            _dragMode = true;

            _initialIndicatorPositionX = Canvas.GetLeft(_indicator);

            _offset = e.GetPosition(_canvas);
            _offset.X -= _initialIndicatorPositionX;
            _offset.Y -= Canvas.GetTop(_indicator);

            _canvas.CaptureMouse();
        }

        private void CanvasMouseMove(Object sender, MouseEventArgs e)
        {
            if (!_dragMode) return;

            if (_indicator.HasAnimatedProperties) _indicator.BeginAnimation(Canvas.LeftProperty, null);

            Double positionOnX = e.GetPosition(sender as IInputElement).X - _offset.X;

            Canvas.SetLeft(_indicator, positionOnX switch
            {
                < 3 => 3,
                > 23 => 23,
                _ => positionOnX
            });
        }

        private readonly DoubleAnimation manualIndicatorAnimation = new() { Duration = UI.LongAnimationDuration, DecelerationRatio = 1, FillBehavior = FillBehavior.HoldEnd };

        private void CanvasMouseUp()
        {
            Double positionOnX = Canvas.GetLeft(_indicator);

            if (!_dragMode || _initialIndicatorPositionX == positionOnX)
            {
                manualIndicatorAnimation.To = _isChecked ? 3 : 23;
                _isChecked = !_isChecked;
            }
            else
            {
                manualIndicatorAnimation.To = positionOnX < 13 ? 3 : 23;
                _isChecked = !_isChecked;
            }

            _dragMode = false;
            _canvas.ReleaseMouseCapture();

            _indicator.BeginAnimation(Canvas.LeftProperty, manualIndicatorAnimation);
        }
        #endregion










        private void ColorProviderChanged()
        {

        }

        private void AnimateToNewState()
        {

        }
    }
}