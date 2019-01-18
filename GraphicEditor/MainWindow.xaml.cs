using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GraphicEditor.Shapes;
using Ellipse = GraphicEditor.Shapes.Ellipse;
using Figure = GraphicEditor.Shapes.Figure;
using Line = GraphicEditor.Shapes.Line;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Rectangle = GraphicEditor.Shapes.Rectangle;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace GraphicEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double x1, x2, y1, y2;
        private ushort tag;
        private const string  FILE_FITLER = "*.jpeg|*.jpeg";
        private List<UIElement> deleteFigure = new List<UIElement>(); 
        private Dictionary<int,Figure> figures = new Dictionary<int, Figure>();
        private Brush color = Brushes.Blue;
        private bool draw;
        private bool oneFigure;

        public MainWindow()
        {
            InitializeComponent();
        }

        private SaveFileDialog InitializeSaveFile()
        {
            SaveFileDialog file = new SaveFileDialog();
            file.Filter = FILE_FITLER;
            file.AddExtension = true;
            file.Title = "Save file";
            return file;
        }

        private void ChangingObjects()
        {
            figures.Add(0, new Line(new Point(x1, y1), new Point(x2, y2), color, sliderThickness.Value));
            figures.Add(1, new Circle(new Point(x1, y1), new Point(x2, y2), color, sliderThickness.Value));
            figures.Add(2, new Ellipse(new Point(x1, y1), new Point(x2, y2), color, sliderThickness.Value));
            figures.Add(3, new Rectangle(new Point(x1, y1), new Point(x2, y2), color, sliderThickness.Value));
            figures.Add(4, new Square(new Point(x1, y1), new Point(x2, y2), color, sliderThickness.Value));
            figures.Add(5, new Triangle(new Point(x1, y1), new Point(x2, y2), color, sliderThickness.Value));
            figures.Add(6, new RightTriangle(new Point(x1, y1), new Point(x2, y2), color, sliderThickness.Value));
        }
        
        

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(canvas);
            x1 = point.X;
            y1 = point.Y;
            labelX.Content = "X = " + x1;
            labelY.Content = "Y = " + y1;
            draw = true;
            oneFigure = false;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            deleteFigure.Clear();
        }

        private void UndoPicture_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (canvas.Children.Count != 0)
            {
                deleteFigure.Add(canvas.Children[canvas.Children.Count - 1]);
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
            }
        }

        private void RedoPicture_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (deleteFigure.Count != 0)
            {
                canvas.Children.Add(deleteFigure[deleteFigure.Count - 1]);
                deleteFigure.RemoveAt(deleteFigure.Count-1);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (draw && (e.LeftButton == MouseButtonState.Pressed))
            {
                Point point = e.GetPosition(canvas);
                x2 = point.X;
                y2 = point.Y;
                ChangingObjects();
                if (oneFigure)
                    canvas.Children.RemoveAt(canvas.Children.Count - 1);
                figures[tag].Draw(canvas);
                oneFigure = true;

                figures.Clear();
            }
        }

        private void ColorPicture_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                color = new SolidColorBrush(Color.FromArgb(dlg.Color.A,dlg.Color.R,dlg.Color.G,dlg.Color.B));
            }
        }

        private byte[] BitmapSourceToByteArray(BitmapSource bmpSrc)
        {
            var encoder = new JpegBitmapEncoder();
            encoder.QualityLevel = 100;
            encoder.Frames.Add(BitmapFrame.Create(bmpSrc));

            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(canvas);
            x2 = point.X;
            y2 = point.Y;
            ChangingObjects();
            figures[tag].Draw(canvas);
            figures.Clear();
            draw = false;
        }

        private void Picture_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            tag = Convert.ToUInt16(image.Tag);
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = InitializeSaveFile();
            if (saveFile.ShowDialog(this) == true)
            {
                using (FileStream fs = new FileStream(saveFile.FileName,FileMode.Create))
                {
                    RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.Width, (int)canvas.Height, 96, 96, PixelFormats.Default);
                    rtb.Render(canvas);
                    JpegBitmapEncoder jpegEnc = new JpegBitmapEncoder();
                    jpegEnc.Frames.Add(BitmapFrame.Create(rtb));
                    jpegEnc.Save(fs);
                    MessageBox.Show("File saved, " + saveFile.FileName);
                }
            }
        }
    }
}
