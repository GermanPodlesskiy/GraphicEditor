using System;
using System.Windows.Forms.VisualStyles;
using GraphicEditor.Shapes;

namespace GraphicEditor.Client
{
    [Serializable]
    public class PackageForSending
    {
        public Commands Command { get; set; }
        public bool IsOneFigure { get; set; }
        public byte[] Data { get; set; }
    }
}