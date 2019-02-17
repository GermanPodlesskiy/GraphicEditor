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
        protected DrawingEllipse drEl = new DrawingEllipse();

        public Ellipse() { }
        public Ellipse(Point firstPoint, Point secondPoint, Brush color, double thickness)
            : base(firstPoint,secondPoint, color, thickness) { }

        protected void InitializeSize()
        {
            width = (int)Math.Abs(secondPoint.X - firstPoint.X);
            height = (int)Math.Abs(secondPoint.Y - firstPoint.Y);
        }

        public override void Draw(Canvas canvas)
        {
            InitializeSize();
            drEl.FirstPoint = firstPoint;
            drEl.SecondPoint = secondPoint;
            drEl.Color = color;
            drEl.Height = height;
            drEl.Width = width;
            drEl.Thickness = thickness;
            drEl.Draw(canvas);
        }
    }

    public class DrawingEllipse
    {
        public Point FirstPoint { get; set; }
        public Point SecondPoint { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Brush Color { get; set; }
        public double Thickness { get; set; }

        public void Draw(Canvas canvas)
        {
            System.Windows.Shapes.Ellipse ellipse = new System.Windows.Shapes.Ellipse()
            {
                Height = Height,
                Width = Width,
                Stroke = Color,
                StrokeThickness = Thickness
            };

            var point = StartPoint();
            Canvas.SetLeft(ellipse, point.Item1);
            Canvas.SetTop(ellipse, point.Item2);
            canvas.Children.Add(ellipse);
        }

        private (double, double) StartPoint()
        {
            if ((FirstPoint.X < SecondPoint.X) && (FirstPoint.Y < SecondPoint.Y))
            {
                return (FirstPoint.X, SecondPoint.Y);
            }
            else if ((FirstPoint.X < SecondPoint.X) && (FirstPoint.Y > SecondPoint.Y))
            {
                return (FirstPoint.X, SecondPoint.Y);
            }
            else if ((FirstPoint.X > SecondPoint.X) && (FirstPoint.Y > SecondPoint.Y))
            {
                return (SecondPoint.X, SecondPoint.Y);
            }
            else
                return (SecondPoint.X, FirstPoint.Y);
        }
    }
}
