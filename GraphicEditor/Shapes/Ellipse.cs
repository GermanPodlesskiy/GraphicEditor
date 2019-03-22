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
        protected DrawingEllipse drEl;

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
            drEl = new DrawingEllipse
            {
                FirstPoint = firstPoint,
                SecondPoint = secondPoint,
                Color = color,
                Height = height,
                Width = width,
                Thickness = thickness,
            };
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
            var ellipse = new System.Windows.Shapes.Ellipse()
            {
                Height = Height,
                Width = Width,
                Stroke = Color,
                StrokeThickness = Thickness
            };

            (double item1, double item2) = StartPoint();
            Canvas.SetLeft(ellipse, item1);
            Canvas.SetTop(ellipse, item2);
            canvas.Children.Add(ellipse);
        }

        private (double, double) StartPoint()
        {
            if ((FirstPoint.X < SecondPoint.X) && (FirstPoint.Y < SecondPoint.Y))
                return (FirstPoint.X, SecondPoint.Y);

            if ((FirstPoint.X < SecondPoint.X) && (FirstPoint.Y > SecondPoint.Y))
                return (FirstPoint.X, SecondPoint.Y);

            if ((FirstPoint.X > SecondPoint.X) && (FirstPoint.Y > SecondPoint.Y))
                return (SecondPoint.X, SecondPoint.Y);

            return (SecondPoint.X, FirstPoint.Y);
        }
    }
}