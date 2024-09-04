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
            _canvas.PreviewMouseUp += CanvasMouseUp;
            _canvas.Children.Add(_indicator);
            Canvas.SetTop(_indicator, 3);
            Canvas.SetLeft(_indicator, 3);

            _indicator.Fill = Brushes.LightGray;
            _indicator.Height = 12;
            _indicator.Width = 12;
            _indicator.PreviewMouseDown += IndicatorMouseDown;



        }

        private Boolean _isInDragMode = false;
        private Boolean _mouseMoved = false;


        #region ManualMouseMove
        private Point _offset;

        private void IndicatorMouseDown(Object sender, MouseButtonEventArgs e)
        {
            _isInDragMode = true;

            _offset = e.GetPosition(_canvas);
            _offset.Y -= Canvas.GetTop(_indicator);
            _offset.X -= Canvas.GetLeft(_indicator);

            _canvas.CaptureMouse();
        }

        private void CanvasMouseMove(Object sender, MouseEventArgs e)
        {
            if (!_isInDragMode) return;

            _mouseMoved = true;

            _indicator.BeginAnimation(Canvas.LeftProperty, null);

            Double positionOnX = e.GetPosition(sender as IInputElement).X - _offset.X;

            if (positionOnX < 3) Canvas.SetLeft(_indicator, 3);
            else if (positionOnX > 23) Canvas.SetLeft(_indicator, 23);
            else Canvas.SetLeft(_indicator, positionOnX);
        }

        private readonly DoubleAnimation manualIndicatorAnimation = new() { Duration = UI.LongAnimationDuration, DecelerationRatio = 1, FillBehavior = FillBehavior.HoldEnd };

        private void CanvasMouseUp(Object sender, MouseButtonEventArgs e)
        {
            Double positionOnX = Canvas.GetLeft(_indicator);
            if (!_mouseMoved)
            {


                if (positionOnX < 13) Canvas.SetLeft(_indicator, 23);
                else Canvas.SetLeft(_indicator, 3);
                _mouseMoved = false;
                _isInDragMode = false;
                return;
            }


            _isInDragMode = false;
            _canvas.ReleaseMouseCapture();
            _mouseMoved = false;
            // todo: smooth clip to state




            manualIndicatorAnimation.From = positionOnX;

            if (positionOnX < 13) manualIndicatorAnimation.To = 3;
            else manualIndicatorAnimation.To = 23;

            //if (positionOnX < 13) Canvas.SetLeft(_indicator, 3);
            //else Canvas.SetLeft(_indicator, 23);


            _indicator.BeginAnimation(Canvas.LeftProperty, manualIndicatorAnimation);



        } 
        #endregion
    }
}