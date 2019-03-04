using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace GraphicEditor.Shapes
{
    [Serializable]
    public class Rectangle : Figure
    {
        protected int width, height;

        public Rectangle() { }

        public Rectangle(Point firstPoint, Point secondPoint, Brush color, double thickness)
            : base(firstPoint, secondPoint, color, thickness) { InitializeSize(); }

        protected void InitializeSize()
        {
            width = (int)Math.Abs(secondPoint.X - firstPoint.X);
            height = (int)Math.Abs(secondPoint.Y - firstPoint.Y);
        }

        public override void Draw(Canvas canvas)
        {
            System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle()
            {
                Height = height,
                Width = width,
                Stroke = color,
                StrokeThickness = thickness
            };

            (double, double) point = StartPoint();
            Canvas.SetLeft(rectangle, point.Item1);
            Canvas.SetTop(rectangle, point.Item2);
            canvas.Children.Add(rectangle);
        }

        protected (double, double) StartPoint()
        {
            if ((firstPoint.X < secondPoint.X) && (firstPoint.Y <= secondPoint.Y))
            {
                return (firstPoint.X, firstPoint.Y);
            }
            else if ((firstPoint.X <= secondPoint.X) && (firstPoint.Y >= secondPoint.Y))
            {
                return (firstPoint.X, secondPoint.Y);
            }
            else if((firstPoint.X >= secondPoint.X) && (firstPoint.Y > secondPoint.Y))
            {
                return (secondPoint.X, secondPoint.Y);
            }
            else
                return (secondPoint.X, firstPoint.Y);
        }
    }
}
