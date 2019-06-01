using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace GraphicEditor.Shapes
{
    [Serializable]
    public class Rectangle : Figure
    {
        protected int Width, Height;

        public Rectangle()
        {
        }

        public Rectangle(Point firstPoint, Point secondPoint, Brush color, double thickness)
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
            var rectangle = new System.Windows.Shapes.Rectangle()
            {
                Height = Height,
                Width = Width,
                Stroke = Color,
                StrokeThickness = Thickness,
                Tag = Tag
            };
            Tag = rectangle.GetHashCode();
            StartPoint(rectangle, canvas);
            canvas.Children.Add(rectangle);
        }

        protected void StartPoint(System.Windows.Shapes.Rectangle  rectangle, Canvas canvas)
        {
            if ((FirstPoint.X < SecondPoint.X) && (FirstPoint.Y <= SecondPoint.Y))
            {
                Canvas.SetLeft(rectangle, FirstPoint.X);
                Canvas.SetTop(rectangle, FirstPoint.Y);
            }
            else if ((FirstPoint.X <= SecondPoint.X) && (FirstPoint.Y > SecondPoint.Y))
            {
                Canvas.SetLeft(rectangle, FirstPoint.X);
                Canvas.SetBottom(rectangle, canvas.ActualHeight - FirstPoint.Y);
            }
            else
            if ((FirstPoint.X > SecondPoint.X) && (FirstPoint.Y >= SecondPoint.Y))
            {
                Canvas.SetRight(rectangle, canvas.ActualWidth - FirstPoint.X);
                Canvas.SetBottom(rectangle, canvas.ActualHeight - FirstPoint.Y);
            }
            else
            {
                Canvas.SetTop(rectangle, FirstPoint.Y);
                Canvas.SetRight(rectangle, canvas.ActualWidth - FirstPoint.X);
            }
        }
    }
}