using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphicEditor.Shapes
{
    abstract class Figure : UIElement
    {
        protected Brush color;
        protected Point firstPoint, secondPoint;
        protected double thickness;

        public Figure(Point firstPoint, Point secondPoint, Brush color, double thickness)
        {
            this.firstPoint = firstPoint;
            this.secondPoint = secondPoint;
            this.color = color;
            this.thickness = thickness;
        }

        public abstract void Draw(Canvas canvas);
    }
}
