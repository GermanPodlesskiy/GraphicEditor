using System;
using System.Collections.Generic;

namespace GraphicEditor.Shapes
{
    [Serializable]
    public class Shape
    {
        public List<Figure> Figures { get; set; }
    }
}