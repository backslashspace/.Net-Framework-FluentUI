using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FluentUI
{
    internal class ToggleButton : Border
    {
        private readonly Border _background = new();
        private readonly Canvas _canvas = new();
        private readonly Ellipse _indicator = new();

        private Point _offset;
        private Ellipse _dragObject = null;

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

        private void IndicatorMouseDown(Object sender, MouseButtonEventArgs e)
        {
            _dragObject = sender as Ellipse;

            _offset = e.GetPosition(_canvas);
            _offset.Y -= Canvas.GetTop(_dragObject);
            _offset.X -= Canvas.GetLeft(_dragObject);

            _canvas.CaptureMouse();
        }

        private void CanvasMouseMove(Object sender, MouseEventArgs e)
        {
            if (_dragObject == null) return;

            Point position = e.GetPosition(sender as IInputElement);

            Canvas.SetTop(_dragObject, position.Y - _offset.Y);
            Canvas.SetLeft(_dragObject, position.X - _offset.X);
        }

        private void CanvasMouseUp(Object sender, MouseButtonEventArgs e)
        {
            _dragObject = null;
            _canvas.ReleaseMouseCapture();
        }
    }
}