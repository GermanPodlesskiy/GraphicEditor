using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphicEditor.Shapes
{
    class Circle : Ellipse
    {
        public Circle(Point firstPoint, Point secondPoint, Brush color, double thickness)
            : base(firstPoint, secondPoint, color, thickness) { }

        public override void Draw(Canvas canvas)
        {
            width = height = (int)Math.Abs(secondPoint.X - firstPoint.X);
            base.Draw(canvas);
        }
    }
}
