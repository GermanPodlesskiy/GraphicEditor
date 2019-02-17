using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphicEditor.Shapes
{
    class Ellipse : Figure
    {
        protected int width, height;

        public Ellipse(Point firstPoint, Point secondPoint, Brush color, double thickness)
            : base(firstPoint,secondPoint, color, thickness)
        {
            width = (int)Math.Abs(secondPoint.X - firstPoint.X);
            height = (int)Math.Abs(secondPoint.Y - firstPoint.Y);
        }

        public override void Draw(Canvas canvas)
        {
            System.Windows.Shapes.Ellipse ellipse = new System.Windows.Shapes.Ellipse()
            {
                Height = height,
                Width = width,
                Stroke = color,
                StrokeThickness = thickness
            };

            var point = StartPoint();
            Canvas.SetLeft(ellipse, point.Item1);
            Canvas.SetTop(ellipse, point.Item2);
            canvas.Children.Add(ellipse);
        }
        protected (double, double) StartPoint()
        {
            if ((firstPoint.X < secondPoint.X) && (firstPoint.Y < secondPoint.Y))
            {
                return (firstPoint.X, firstPoint.Y);
            }
            else if ((firstPoint.X < secondPoint.X) && (firstPoint.Y > secondPoint.Y))
            {
                return (firstPoint.X, secondPoint.Y);
            }
            else if ((firstPoint.X > secondPoint.X) && (firstPoint.Y > secondPoint.Y))
            {
                return (secondPoint.X, secondPoint.Y);
            }
            else
                return (secondPoint.X, firstPoint.Y);
        }
    }
}
