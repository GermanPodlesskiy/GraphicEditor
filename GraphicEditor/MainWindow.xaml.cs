using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using GraphicEditor.Shapes;
using OpenSaveFile;
using Ellipse = GraphicEditor.Shapes.Ellipse;
using Figure = GraphicEditor.Shapes.Figure;
using Line = GraphicEditor.Shapes.Line;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Rectangle = GraphicEditor.Shapes.Rectangle;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace GraphicEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Serializable]
    public class Shape 
    {
        public List<Figure> Figures { get; set; }
    }
    public partial class MainWindow : Window
    {
        private double x1, x2, y1, y2;
        private ushort tag;
        private List<UIElement> deleteFigure = new List<UIElement>(); 
        private List<Figure> deleteFiguresForSerialezer = new List<Figure>();
        private Dictionary<int,Figure> figures = new Dictionary<int, Figure>();
        private Brush color = Brushes.Blue;
        private bool draw;
        private bool oneFigure;
        private Shape shape = new Shape(){Figures = new List<Figure>()};
        private Figure[] FIGURES_TYPE;
        private string nameOpenOnceJpeg;

        public MainWindow()
        {
            InitializeComponent();
        }

        private SaveFileDialog InitializeSaveFile(string fitler)
        {
            var file = new SaveFileDialog();
            file.Filter = $"{fitler} |*.{fitler}";
            file.AddExtension = true;
            file.Title = "Save file";
            return file;
        }
        private OpenFileDialog InitializeOpenFile(string fitler)
        {
            var file = new OpenFileDialog();
            file.Filter = $"{fitler} |*.{fitler}";
            file.AddExtension = true;
            file.Title = "Open file";
            return file;
        }

        private void ChangingObjects()
        {
            figures.Add(0, new Line(new Point(x1, y1), new Point(x2, y2), color, SliderThickness.Value));
            figures.Add(1, new Circle(new Point(x1, y1), new Point(x2, y2), color, SliderThickness.Value));
            figures.Add(2, new Ellipse(new Point(x1, y1), new Point(x2, y2), color, SliderThickness.Value));
            figures.Add(3, new Rectangle(new Point(x1, y1), new Point(x2, y2), color, SliderThickness.Value));
            figures.Add(4, new Square(new Point(x1, y1), new Point(x2, y2), color, SliderThickness.Value));
            figures.Add(5, new Triangle(new Point(x1, y1), new Point(x2, y2), color, SliderThickness.Value));
            figures.Add(6, new RightTriangle(new Point(x1, y1), new Point(x2, y2), color, SliderThickness.Value));
        }
        
        

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(CanvasMain);
            x1 = point.X;
            y1 = point.Y;
            draw = true;
            oneFigure = false;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            CanvasMain.Children.Clear();
            deleteFiguresForSerialezer.Clear();
            deleteFigure.Clear();
        }

        private void UndoPicture_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CanvasMain.Children.Count != 0)
            {
                deleteFigure.Add(CanvasMain.Children[CanvasMain.Children.Count - 1]);
                deleteFiguresForSerialezer.Add(shape.Figures[CanvasMain.Children.Count - 1]);
                shape.Figures.RemoveAt(CanvasMain.Children.Count - 1);
                CanvasMain.Children.RemoveAt(CanvasMain.Children.Count - 1);
            }
        }

        private void RedoPicture_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (deleteFigure.Count != 0)
            {
                CanvasMain.Children.Add(deleteFigure[deleteFigure.Count - 1]);
                shape.Figures.Add(deleteFiguresForSerialezer[deleteFigure.Count - 1]);
                deleteFigure.RemoveAt(deleteFigure.Count-1);
                deleteFiguresForSerialezer.RemoveAt(deleteFiguresForSerialezer.Count-1);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            LabelX.Content = "X = " + e.GetPosition(CanvasMain).X;
            LabelY.Content = "Y = " + e.GetPosition(CanvasMain).Y;
            if (draw && (e.LeftButton == MouseButtonState.Pressed))
            {
                Point point = e.GetPosition(CanvasMain);
                x2 = point.X;
                y2 = point.Y;
                ChangingObjects();
                if (oneFigure)
                {
                    CanvasMain.Children.RemoveAt(CanvasMain.Children.Count - 1);
                    shape.Figures.RemoveAt(shape.Figures.Count - 1);
                }
                figures[tag].Draw(CanvasMain);
                shape.Figures.Add(figures[tag]);
                oneFigure = true;

                figures.Clear();
            }
        }

        private void ColorBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                color = new SolidColorBrush(Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B));
                ColorBorder.Background = color;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var serializationWindow = new SerializationWindow();
            if (serializationWindow.ShowDialog() == true)
            {
                SaveFileDialog saveFile = InitializeSaveFile(serializationWindow.TypeSerialization);
                if (saveFile.ShowDialog(this) == true)
                {
                    if (serializationWindow.TypeSerialization == "jpeg")
                    {
                        if (saveFile.FileName != nameOpenOnceJpeg)
                        {
                            MessageBox.Show(Save.SaveJPEG<Canvas>(saveFile, CanvasMain));
                            nameOpenOnceJpeg = null;
                        }
                        else MessageBox.Show("Can't save to open file.");

                    }
                    else if (serializationWindow.TypeSerialization == "dat")
                    {
                        MessageBox.Show(Save.SaveDAT<Shape>(saveFile, shape));
                    }
                    else if (serializationWindow.TypeSerialization == "xml")
                    {
                        MessageBox.Show(Save.SaveXML<Shape>(saveFile, shape));
                    }
                    else if (serializationWindow.TypeSerialization == "json")
                    {
                        MessageBox.Show(Save.SaveJSON<Shape>(saveFile, shape));
                    }
                    else MessageBox.Show(SaveTXT(saveFile, shape)); ;
                }
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var serializationWindow = new SerializationWindow();
            if (serializationWindow.ShowDialog() == true)
            {
                CanvasMain.Children.Clear();
                OpenFileDialog openFile = InitializeOpenFile(serializationWindow.TypeSerialization);
                if (openFile.ShowDialog(this) == true)
                {
                    try
                    {
                        if (serializationWindow.TypeSerialization == "jpeg")
                        {
                            Open.OpenJPEG<Canvas>(openFile, CanvasMain);
                            nameOpenOnceJpeg = openFile.FileName;
                        }
                        else if (serializationWindow.TypeSerialization == "dat")
                        {
                            shape = Open.OpenDAT<Shape>(openFile);
                        }
                        else if (serializationWindow.TypeSerialization == "xml")
                        {
                            shape = Open.OpenXML<Shape>(openFile);
                        }
                        else if (serializationWindow.TypeSerialization == "json")
                        {
                            shape = Open.OpenJSON<Shape>(openFile);
                        }
                        else
                        {
                            shape = OpenTXT(openFile);
                        }

                        if (serializationWindow.TypeSerialization != "jpeg")
                        {
                            Parser(shape);
                        }
                    }
                    catch (Exception)
                    {
                        shape = new Shape() { Figures = new List<Figure>() };
                    }
                }
            }
            else
            {
                MessageBox.Show("No view selected");
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            draw = false;
        }

        private void Picture_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;
            tag = Convert.ToUInt16(image.Tag);
        }

        private void Parser(Shape shape)
        {
            if (shape?.Figures != null)
                foreach (var figure in shape.Figures)
                {
                    figure.SetColor();
                    figure.Draw(CanvasMain);
                }
        }
        private string SaveTXT(SaveFileDialog saveFile, Shape shape)
        {
            using (var sw = new StreamWriter(saveFile.FileName, false, Encoding.Default))
            {
                if (shape?.Figures != null)
                    foreach (var figure in shape.Figures)
                    {
                        sw.WriteLine($"{figure.GetType().Name} {figure.firstPoint.X} {figure.firstPoint.Y} {figure.secondPoint.X}" +
                                     $" {figure.secondPoint.Y} {figure.serializedColor} {figure.thickness}");
                    }

                return "File saved, " + saveFile.FileName;
            }
        }
        private Shape OpenTXT(OpenFileDialog openFile)
        {
            var shape = new Shape() { Figures = new List<Figure>() };
            using (var fs = new FileStream(openFile.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var sr = new StreamReader(fs, Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        shape.Figures.Add(ParserTXT(line));
                    }
                }
            }



            return shape;
        }

        private Figure[] InitializeArrayFigure()=>  new Figure[] { new Line(), new Circle(), new Ellipse(), new Rectangle(), new Square(), new Triangle(), new RightTriangle() };
        private Figure ParserTXT(string line)
        {
            string[] data = line.Split(new char[] { ' ' });
            FIGURES_TYPE = InitializeArrayFigure();
            byte tag =  FindTypeFigure(data[0]);
            var elem = FIGURES_TYPE[tag];
            elem.firstPoint.X = int.Parse(data[1]);
            elem.firstPoint.Y = int.Parse(data[2]);
            elem.secondPoint.X = int.Parse(data[3]);
            elem.secondPoint.Y = int.Parse(data[4]);
            elem.thickness = int.Parse(data[6]);
            elem.serializedColor = ToColor(data[5]);
            return elem;
        }
        private byte FindTypeFigure(string type)
        {
            byte i = 0;
            for (; i < FIGURES_TYPE.Length; i++)
                if(FIGURES_TYPE[i].GetType().Name == type)
                    break;
            return i;
        }

        private Color ToColor(string colorLine)
        {
            colorLine = colorLine.Remove(0, 1);
            var color = new Color();
            color.A = Convert.ToByte(new String(new char[]{colorLine[0],colorLine[1]}),16);
            color.R = Convert.ToByte(new String(new char[]{colorLine[2],colorLine[3]}),16);
            color.G = Convert.ToByte(new String(new char[]{colorLine[4],colorLine[5]}),16);
            color.B = Convert.ToByte(new String(new char[]{colorLine[6],colorLine[7]}),16);
            return color;
        }
    }
}