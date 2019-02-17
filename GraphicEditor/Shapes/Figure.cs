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
        [NonSerialized]
        [XmlIgnore]
        protected Brush color;
        [DataMember] public Point firstPoint;
        [DataMember] public Point secondPoint;
        [DataMember] public double thickness;
        [DataMember] [NonSerialized] public Color serializedColor;
        [XmlIgnore]public byte A, G, R, B;

        public Figure() { }
        public Figure(Point firstPoint, Point secondPoint, Brush color, double thickness)
        {
            this.firstPoint = firstPoint;
            this.secondPoint = secondPoint;
            this.color = color;
            this.thickness = thickness;
            serializedColor.A = A = ((Color) color.GetValue(SolidColorBrush.ColorProperty)).A;
            serializedColor.G = G = ((Color) color.GetValue(SolidColorBrush.ColorProperty)).G;
            serializedColor.R = R = ((Color) color.GetValue(SolidColorBrush.ColorProperty)).R;
            serializedColor.B = B = ((Color) color.GetValue(SolidColorBrush.ColorProperty)).B;
        }

        public void SetColor()
        {
            this.color = serializedColor.ToString() == "#00000000"? new SolidColorBrush(new Color(){A = A, G = G, R = R, B = B}) : new SolidColorBrush(serializedColor);
        }

        public abstract void Draw(Canvas canvas);
    }
}
