using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphicEditor.Shapes
{
    [Serializable]
    public class Line : Figure
    {
        public Line()
        {
        }

        public Line(Point firstPoint, Point secondPoint, Brush color, double thickness)
            : base(firstPoint, secondPoint, color, thickness)
        {
        }

        public override void Draw(Canvas canvas)
        {
            var line = new System.Windows.Shapes.Line()
            {
                X1 = FirstPoint.X,
                Y1 = FirstPoint.Y,
                X2 = SecondPoint.X,
                Y2 = SecondPoint.Y,
                Stroke = Color,
                StrokeThickness = Thickness,
            };
            Tag = line.GetHashCode();
            line.Tag = Tag;
            canvas.Children.Add(line);
        }
    }
}