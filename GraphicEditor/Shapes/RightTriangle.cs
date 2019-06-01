using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphicEditor.Shapes
{
    [Serializable]
    public class RightTriangle : Triangle
    {
        [NonSerialized] private PointCollection _points;
        [NonSerialized] private Point _left, _up, _right;

        public RightTriangle()
        {
        }

        public RightTriangle(Point firstPoint, Point secondPoint, Brush color, double thickness)
            : base(firstPoint, secondPoint, color, thickness)
        {
        }

        public override void Draw(Canvas canvas)
        {
            _points = new PointCollection();
            if ((FirstPoint.X < SecondPoint.X) && (FirstPoint.Y < SecondPoint.Y))
            {
                _up.X = FirstPoint.X;
                _up.Y = FirstPoint.Y;
                _left.X = FirstPoint.X;
                _left.Y = SecondPoint.Y;
                _right.X = SecondPoint.X;
                _right.Y = SecondPoint.Y;
            }
            else if ((FirstPoint.X < SecondPoint.X) && (FirstPoint.Y > SecondPoint.Y))
            {
                _up.X = FirstPoint.X;
                _up.Y = SecondPoint.Y;
                _left.X = FirstPoint.X;
                _left.Y = FirstPoint.Y;
                _right.X = SecondPoint.X;
                _right.Y = FirstPoint.Y;
            }
            else if ((FirstPoint.X > SecondPoint.X) && (FirstPoint.Y < SecondPoint.Y))
            {
                _up.X = SecondPoint.X;
                _up.Y = FirstPoint.Y;
                _left.X = SecondPoint.X;
                _left.Y = SecondPoint.Y;
                _right.X = FirstPoint.X;
                _right.Y = SecondPoint.Y;
            }
            else
            {
                _up.X = SecondPoint.X;
                _up.Y = SecondPoint.Y;
                _left.X = SecondPoint.X;
                _left.Y = FirstPoint.Y;
                _right.X = FirstPoint.X;
                _right.Y = FirstPoint.Y;
            }

            _points.Add(_up);
            _points.Add(_left);
            _points.Add(_right);

            var triangle = new System.Windows.Shapes.Polygon()
            {
                Points = _points,
                Stroke = Color,
                StrokeThickness = Thickness,
                Tag = Tag
            };
            Tag = triangle.GetHashCode();
            canvas.Children.Add(triangle);
        }
    }
}