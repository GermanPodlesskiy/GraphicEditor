using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;

namespace GraphicEditor.Shapes
{
    [Serializable]
    [KnownType(typeof(Line))]
    [KnownType(typeof(Circle))]
    [KnownType(typeof(Ellipse))]
    [KnownType(typeof(Rectangle))]
    [KnownType(typeof(RightTriangle))]
    [KnownType(typeof(Square))]
    [KnownType(typeof(Triangle))]
    [KnownType(typeof(SolidColorBrush))]
    [DataContract]
    [XmlInclude(typeof(Line))]
    [XmlInclude(typeof(Circle))]
    [XmlInclude(typeof(Ellipse))]
    [XmlInclude(typeof(Rectangle))]
    [XmlInclude(typeof(RightTriangle))]
    [XmlInclude(typeof(Square))]
    [XmlInclude(typeof(Triangle))]
    public abstract class Figure
    {
        [NonSerialized] [XmlIgnore] protected Brush color;
        [DataMember] public Point firstPoint;
        [DataMember] public Point secondPoint;
        [DataMember] public double thickness;
        [DataMember] [NonSerialized] public Color serializedColor;
        [XmlIgnore] public byte A, G, R, B;

        protected Figure() { }

        protected Figure(Point firstPoint, Point secondPoint, Brush color, double thickness)
        {
            this.firstPoint = firstPoint;
            this.secondPoint = secondPoint;
            this.color = color;
            this.thickness = thickness;

            var colorTemp = (Color) color.GetValue(SolidColorBrush.ColorProperty);
            serializedColor.A = A = colorTemp.A;
            serializedColor.G = G = colorTemp.G;
            serializedColor.R = R = colorTemp.R;
            serializedColor.B = B = colorTemp.B;
        }

        public void SetColor()
        {
            color = serializedColor.ToString() == "#00000000"
                ? new SolidColorBrush(new Color() {A = A, G = G, R = R, B = B})
                : new SolidColorBrush(serializedColor);
        }

        public abstract void Draw(Canvas canvas);
    }
}