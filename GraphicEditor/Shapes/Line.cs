using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphicEditor.Shapes
{
    [Serializable]
    public class Line : Figure
    {
        public Line() { }

        public Line(Point firstPoint, Point secondPoint, Brush color, double thickness)
            : base(firstPoint, secondPoint, color, thickness) { }

        public override void Draw(Canvas canvas)
        {
            canvas.Children.Add(new System.Windows.Shapes.Line()
            {
                X1 = firstPoint.X,
                Y1 = firstPoint.Y,
                X2 = secondPoint.X,
                Y2 = secondPoint.Y,
                Stroke = color,
                StrokeThickness = thickness
            });
        }
    }
}