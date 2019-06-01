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
        [DataMember] public Point FirstPoint;
        [DataMember] public Point SecondPoint;
        [DataMember] public double Thickness;
        [DataMember] [NonSerialized] public Color SerializedColor;
        [XmlIgnore] public byte A, G, R, B;
        [NonSerialized] [XmlIgnore] protected Brush Color;
        [NonSerialized] [XmlIgnore] public int Tag;

        protected Figure()
        {
        }

        protected Figure(Point firstPoint, Point secondPoint, Brush color, double thickness)
        {
            FirstPoint = firstPoint;
            SecondPoint = secondPoint;
            Color = color;
            Thickness = thickness;

            var colorTemp = (Color) color.GetValue(SolidColorBrush.ColorProperty);
            SerializedColor.A = A = colorTemp.A;
            SerializedColor.G = G = colorTemp.G;
            SerializedColor.R = R = colorTemp.R;
            SerializedColor.B = B = colorTemp.B;
        }

        public void SetColor()
        {
            Color = SerializedColor.ToString() == "#00000000"
                ? new SolidColorBrush(new Color() {A = A, G = G, R = R, B = B})
                : new SolidColorBrush(SerializedColor);
        }

        public abstract void Draw(Canvas canvas);
    }
}