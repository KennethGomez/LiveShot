using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LiveShot.API.Drawing.Tools
{
    public class ArrowTool : DrawingTool
    {
        private readonly ILiveShotService _liveShotService;

        private Path? _arrow;

        public ArrowTool(ILiveShotService liveShotService)
        {
            _liveShotService = liveShotService;
        }

        public override CanvasTool Tool => CanvasTool.Arrow;

        public override UIElement? OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.ButtonState != MouseButtonState.Pressed || _liveShotService.DrawCanvas is null) return null;

            LastPoint = e.GetPosition(_liveShotService.DrawCanvas);

            return null;
        }

        public override UIElement? OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            LastPoint = null;

            var tmp = _arrow;

            _arrow = null;

            return tmp;
        }

        public override UIElement? OnMouseMove(MouseEventArgs e)
        {
            if (LastPoint is not { } lastPoint || _liveShotService.DrawCanvas is null) return null;

            var point = e.GetPosition(_liveShotService.DrawCanvas);

            var arrow = GetArrow(lastPoint, point);

            if (_arrow is null)
            {
                _arrow = arrow;

                _liveShotService.DrawCanvas.Children.Add(_arrow);
            }
            else
            {
                _arrow.Data = arrow.Data;
            }

            return null;
        }

        private Path GetArrow(Point p1, Point p2)
        {
            GeometryGroup lineGroup = new();

            double theta = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X) * 180 / Math.PI;

            PathGeometry pathGeometry = new();
            PathFigure pathFigure = new();

            var p = new Point(p1.X + (p2.X - p1.X), p1.Y + (p2.Y - p1.Y));

            pathFigure.StartPoint = p;

            const double xDisplacement = 8;
            const double yDisplacement = 30;

            var leftPoint = new Point(p.X + xDisplacement, p.Y + yDisplacement);
            var rightPoint = new Point(p.X - xDisplacement, p.Y + yDisplacement);

            pathFigure.Segments.Add(new LineSegment
            {
                Point = leftPoint
            });

            pathFigure.Segments.Add(new LineSegment
            {
                Point = rightPoint
            });

            pathFigure.Segments.Add(new LineSegment
            {
                Point = p
            });

            pathGeometry.Figures.Add(pathFigure);

            RotateTransform transform = new()
            {
                Angle = theta + 90,
                CenterX = p.X,
                CenterY = p.Y
            };

            pathGeometry.Transform = transform;

            lineGroup.Children.Add(pathGeometry);

            LineGeometry connectorGeometry = new()
            {
                StartPoint = p1,
                EndPoint = p2
            };
            lineGroup.Children.Add(connectorGeometry);

            double strokeThickness = _liveShotService.DrawCanvas?.DrawingStrokeThickness ?? 1;
            var drawingColor = _liveShotService.DrawCanvas?.DrawingColor ?? Brushes.Black;

            Path path = new()
            {
                Data = lineGroup,
                StrokeThickness = GetStrokeThickness(strokeThickness),
                Stroke = drawingColor,
                Fill = drawingColor,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeDashCap = PenLineCap.Round
            };

            return path;
        }

        public override void UpdateThickness(double thickness)
        {
            if (_arrow is not null && _liveShotService.DrawCanvas is not null)
                _arrow.StrokeThickness = GetStrokeThickness(_liveShotService.DrawCanvas.DrawingStrokeThickness);
        }

        private static double GetStrokeThickness(double thickness) => thickness + 3;
    }
}