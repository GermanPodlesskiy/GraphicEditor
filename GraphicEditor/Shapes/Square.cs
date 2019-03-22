using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphicEditor.Shapes
{
    [Serializable]
    public class Square : Rectangle
    {
        public Square() { }

        public Square(Point firstPoint, Point secondPoint, Brush color, double thickness)
            : base(firstPoint, secondPoint, color, thickness) { }

        public override void Draw(Canvas canvas)
        {
            width = height = (int) Math.Abs(secondPoint.X - firstPoint.X);
            base.Draw(canvas);
        }
    }
}