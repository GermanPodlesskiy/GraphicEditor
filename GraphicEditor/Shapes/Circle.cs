using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphicEditor.Shapes
{
    [Serializable]
    public class Circle : Ellipse
    {
        public Circle()
        {
        }

        public Circle(Point firstPoint, Point secondPoint, Brush color, double thickness)
            : base(firstPoint, secondPoint, color, thickness)
        {
        }

        public override void Draw(Canvas canvas)
        {
            Width = Height = (int) Math.Abs(SecondPoint.X - FirstPoint.X);
            base.Draw(canvas);
        }
    }
}