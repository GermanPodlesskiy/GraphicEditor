using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphicEditor.Shapes
{
    [Serializable]
    public class Ellipse : Figure
    {
        protected int Width, Height;

        public Ellipse()
        {
        }

        public Ellipse(Point firstPoint, Point secondPoint, Brush color, double thickness)
            : base(firstPoint, secondPoint, color, thickness)
        {
            InitializeSize();
        }

        protected void InitializeSize()
        {
            Width = (int) Math.Abs(SecondPoint.X - FirstPoint.X);
            Height = (int) Math.Abs(SecondPoint.Y - FirstPoint.Y);
        }

        public override void Draw(Canvas canvas)
        {
            var ellipse = new System.Windows.Shapes.Ellipse()
            {
                Height = Height,
                Width = Width,
                Stroke = Color,
                StrokeThickness = Thickness
            };
            Tag = ellipse.GetHashCode();
            StartPoint(ellipse, canvas);
            canvas.Children.Add(ellipse);
        }

        private void StartPoint(System.Windows.Shapes.Ellipse ellipse, Canvas canvas)
        {
            if ((FirstPoint.X < SecondPoint.X) && (FirstPoint.Y <= SecondPoint.Y))
            {
                Canvas.SetLeft(ellipse, FirstPoint.X);
                Canvas.SetTop(ellipse, FirstPoint.Y);
            }
            else if ((FirstPoint.X <= SecondPoint.X) && (FirstPoint.Y > SecondPoint.Y))
            {
                Canvas.SetLeft(ellipse, FirstPoint.X);
                Canvas.SetBottom(ellipse, canvas.ActualHeight - FirstPoint.Y);
            }
            else if ((FirstPoint.X > SecondPoint.X) && (FirstPoint.Y >= SecondPoint.Y))
            {
                Canvas.SetRight(ellipse, canvas.ActualWidth - FirstPoint.X);
                Canvas.SetBottom(ellipse, canvas.ActualHeight - FirstPoint.Y);
            }
            else
            {
                Canvas.SetTop(ellipse, FirstPoint.Y);
                Canvas.SetRight(ellipse, canvas.ActualWidth - FirstPoint.X);
            }
        }
    }
}