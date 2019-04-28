using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphicEditor.Shapes
{
    [Serializable]
    public class Ellipse : Figure
    {
        protected int width, height;

        public Ellipse() { }

        public Ellipse(Point firstPoint, Point secondPoint, Brush color, double thickness)
            : base(firstPoint, secondPoint, color, thickness)
        {
            InitializeSize();
        }

        protected void InitializeSize()
        {
            width = (int) Math.Abs(secondPoint.X - firstPoint.X);
            height = (int) Math.Abs(secondPoint.Y - firstPoint.Y);
        }

        public override void Draw(Canvas canvas)
        {
            var ellipse = new System.Windows.Shapes.Ellipse()
            {
                Height = height,
                Width = width,
                Stroke = color,
                StrokeThickness = thickness
            };

            (double x, double y) = StartPoint();
            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);
            canvas.Children.Add(ellipse);
        }

        private (double, double) StartPoint()
        {
            if ((firstPoint.X < secondPoint.X) && (firstPoint.Y < secondPoint.Y))
                return (firstPoint.X, firstPoint.Y);

            if ((firstPoint.X < secondPoint.X) && (firstPoint.Y > secondPoint.Y))
                return (firstPoint.X, secondPoint.Y);

            if ((firstPoint.X > secondPoint.X) && (firstPoint.Y > secondPoint.Y))
                return (secondPoint.X, secondPoint.Y);

            return (secondPoint.X, firstPoint.Y);
        }
    }
}