using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphicEditor.Shapes
{   
    [Serializable]
    public class Triangle : Figure
    {
        PointCollection points = new PointCollection();
        Point left, up, right;

        public Triangle() { }
        public Triangle(Point firstPoint, Point secondPoint, Brush color, double thickness) 
            : base(firstPoint,secondPoint, color, thickness){ }

        public override void Draw(Canvas canvas)
        {
            if ((firstPoint.X < secondPoint.X) && (firstPoint.Y < secondPoint.Y))
            {
                up.X = Math.Abs((firstPoint.X - secondPoint.X) / 2) + firstPoint.X;
                up.Y = firstPoint.Y;
                left.X = firstPoint.X;
                left.Y = secondPoint.Y;
                right.X = secondPoint.X;
                right.Y = secondPoint.Y;
            }
            else if ((firstPoint.X < secondPoint.X) && (firstPoint.Y > secondPoint.Y))
            {
                up.X = Math.Abs((firstPoint.X - secondPoint.X) / 2) + firstPoint.X;
                up.Y = secondPoint.Y;
                left.X = firstPoint.X;
                left.Y = firstPoint.Y;
                right.X = secondPoint.X;
                right.Y = firstPoint.Y;
            }
            else if ((firstPoint.X > secondPoint.X) && (firstPoint.Y < secondPoint.Y))
            {
                up.X = firstPoint.X - Math.Abs((firstPoint.X - secondPoint.X) / 2);
                up.Y = firstPoint.Y;
                left.X = secondPoint.X;
                left.Y = secondPoint.Y;
                right.X = firstPoint.X;
                right.Y = secondPoint.Y;
            }
            else
            {
                up.X = firstPoint.X - Math.Abs((firstPoint.X - secondPoint.X) / 2);
                up.Y = secondPoint.Y;
                left.X = secondPoint.X;
                left.Y = firstPoint.Y;
                right.X = firstPoint.X;
                right.Y = firstPoint.Y;
            }

            points.Add(up);
            points.Add(left);
            points.Add(right);
            
            System.Windows.Shapes.Polygon triangle = new System.Windows.Shapes.Polygon()
            {
                Points = points,
                Stroke = color,
                StrokeThickness = thickness
            };
            canvas.Children.Add(triangle);
        }
    }
}
