using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using GraphicEditor.Shapes;
using SerializerInterface;
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
    public partial class MainWindow : Window
    {
        private double x1, x2, y1, y2;
        private ushort tag;
        private List<UIElement> deleteFigure = new List<UIElement>();
        private List<Figure> deleteFiguresForSerialezer = new List<Figure>();
        private Dictionary<int, Figure> figures = new Dictionary<int, Figure>();
        private Brush color = Brushes.Blue;
        private bool draw;
        private bool oneFigure;
        private Shape shape = new Shape() {Figures = new List<Figure>()};
        private Figure[] figuresType;
        private string nameOpenOnceJpeg;
        private bool resize;

        public MainWindow()
        {
            InitializeComponent();
        }

        private SaveFileDialog InitializeSaveFile(string filter)
        {
            var file = new SaveFileDialog
            {
                Filter = $"{filter} |*.{filter}",
                AddExtension = true,
                Title = "Save file"
            };
            return file;
        }

        private OpenFileDialog InitializeOpenFile(string filter)
        {
            var file = new OpenFileDialog
            {
                Filter = $"{filter} |*.{filter}",
                AddExtension = true,
                Title = "Open file"
            };
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
            if (CanvasMain.Children.Count == 0) return;

            deleteFigure.Add(CanvasMain.Children[CanvasMain.Children.Count - 1]);
            deleteFiguresForSerialezer.Add(shape.Figures[CanvasMain.Children.Count - 1]);
            shape.Figures.RemoveAt(CanvasMain.Children.Count - 1);
            CanvasMain.Children.RemoveAt(CanvasMain.Children.Count - 1);
        }

        private void RedoPicture_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (deleteFigure.Count == 0) return;

            CanvasMain.Children.Add(deleteFigure[deleteFigure.Count - 1]);
            shape.Figures.Add(deleteFiguresForSerialezer[deleteFigure.Count - 1]);
            deleteFigure.RemoveAt(deleteFigure.Count - 1);
            deleteFiguresForSerialezer.RemoveAt(deleteFiguresForSerialezer.Count - 1);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            LabelX.Content = "X = " + e.GetPosition(CanvasMain).X;
            LabelY.Content = "Y = " + e.GetPosition(CanvasMain).Y;
            if (!draw || (e.LeftButton != MouseButtonState.Pressed)) return;

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
            if (serializationWindow.ShowDialog() != true) return;

            SaveFileDialog saveFile = InitializeSaveFile(serializationWindow.TypeSerialization.ToLower());

            if (saveFile.ShowDialog(this) != true) return;

            if (serializationWindow.TypeSerialization == "Jpeg")
            {
                if (saveFile.FileName != nameOpenOnceJpeg)
                {
                    SaveOpenJpegFile<IJpegFile>(true, saveFile, null);
                    nameOpenOnceJpeg = null;
                }
                else MessageBox.Show("Can't save to open file.");
            }
            else if (serializationWindow.TypeSerialization == "Txt")
            {
                MessageBox.Show(SaveTXT(saveFile, shape));
            }
            else
                SaveOpenFile<ISerializer>(serializationWindow.TypeSerialization, true, saveFile, null, shape);
        }

        private Shape SaveOpenFile<T>(string someType, bool choice, SaveFileDialog saveFile, OpenFileDialog openFile,
            Shape shape) where T : ISerializer
        {
            string serializerLibName = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                "CreateSerializerLibrary.dll");

            if (!File.Exists(serializerLibName))
            {
                MessageBox.Show("File not found");
                return null;
            }

            try
            {
                var serializerAssembly = Assembly.LoadFrom(serializerLibName);
                foreach (Type type in serializerAssembly.GetExportedTypes())
                {
                    if (type.IsClass && typeof(T).IsAssignableFrom(type) && (type.Name == someType))
                    {
                        var serializer = (T) Activator.CreateInstance(type);
                        if (choice)
                            MessageBox.Show(serializer.Serialize(saveFile, shape));
                        else
                            return serializer.Deserialize<Shape>(openFile, someType);
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Well, I managed to upload a file.");
            }


            MessageBox.Show("The file does not contain the necessary data");
            return null;
        }

        private void SaveOpenJpegFile<T>(bool choice, SaveFileDialog saveFile, OpenFileDialog openFile)
            where T : IJpegFile
        {
            string serializerLibName = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                "CreateSerializerLibrary.dll");

            if (!File.Exists(serializerLibName))
            {
                MessageBox.Show("File not found");
                return;
            }

            var serializerAssembly = Assembly.LoadFrom(serializerLibName);
            foreach (Type type in serializerAssembly.GetExportedTypes())
            {
                if (type.IsClass && typeof(T).IsAssignableFrom(type) && (type.Name == "Jpeg"))
                {
                    var serializer = (T) Activator.CreateInstance(type);
                    try
                    {
                        if (choice)
                            MessageBox.Show(serializer.Serialize(saveFile, CanvasMain));
                        else
                            serializer.Deserialize(openFile, CanvasMain);
                        return;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }

            MessageBox.Show("The file does not contain the necessary data");
        }


        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var serializationWindow = new SerializationWindow();
            if (serializationWindow.ShowDialog() == true)
            {
                CanvasMain.Children.Clear();
                OpenFileDialog openFile = InitializeOpenFile("zip");
                if (openFile.ShowDialog(this) != true) return;

                try
                {
                    if (serializationWindow.TypeSerialization == "Jpeg")
                    {
                        SaveOpenJpegFile<IJpegFile>(false, null, openFile);
                        nameOpenOnceJpeg = openFile.FileName;
                    }
                    else if (serializationWindow.TypeSerialization == "Txt")
                    {
                        shape = OpenTXT(openFile, serializationWindow.TypeSerialization);
                    }
                    else
                        shape = SaveOpenFile<ISerializer>(serializationWindow.TypeSerialization, false, null,
                            openFile, null);

                    if (serializationWindow.TypeSerialization != "Jpeg")
                    {
                        Parser(shape);
                        tag = 0;
                    }
                }
                catch (Exception)
                {
                    shape = new Shape() {Figures = new List<Figure>()};
                }
            }
            else
            {
                MessageBox.Show("No view selected");
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e) => draw = false;

        private void Picture_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;
            tag = Convert.ToUInt16(image.Tag);
        }

        private void Parser(Shape shape)
        {
            if (shape?.Figures == null) return;

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
                        sw.WriteLine(
                            $"{figure.GetType().Name} {figure.firstPoint.X} {figure.firstPoint.Y} {figure.secondPoint.X}" +
                            $" {figure.secondPoint.Y} {figure.serializedColor} {figure.thickness}");
                    }
            }

            string zipFileName = Regex.Replace(saveFile.FileName, "txt", "zip");
            using (var fs = new FileStream(zipFileName, FileMode.Create))
            using (var archive = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                archive.CreateEntryFromFile(saveFile.FileName, saveFile.SafeFileName);
                return "File saved, " + saveFile.FileName;
            }
        }

        private Shape OpenTXT(OpenFileDialog archive, string filter)
        {
            var shape = new Shape() {Figures = new List<Figure>()};
            try
            {
                using (var zip = ZipFile.Open(archive.FileName, ZipArchiveMode.Read))
                    foreach (ZipArchiveEntry entry in zip.Entries)
                        if (entry.FullName.EndsWith($".{filter}", StringComparison.OrdinalIgnoreCase))
                        {
                            var path = archive.FileName;
                            path = (path.Replace(archive.SafeFileName, ""));
                            path = path.Remove(path.Length - 1, 1) + $"\\{entry.Name}";
                            entry.ExtractToFile(path);
                        }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                OpenFileDialog openFile = InitializeOpenFile(filter.ToLower());
                if (openFile.ShowDialog(this) == true)
                {
                    using (var fs = new FileStream(openFile.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (var sr = new StreamReader(fs, Encoding.Default))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                            shape.Figures.Add(ParserTXT(line));
                    }
                }
            }

            return shape;
        }

        private Figure[] InitializeArrayFigure() => new Figure[]
        {
            new Line(), new Circle(), new Ellipse(), new Rectangle(), new Square(), new Triangle(), new RightTriangle()
        };

        private Figure ParserTXT(string line)
        {
            string[] data = line.Split(' ');
            figuresType = InitializeArrayFigure();
            byte tag = FindTypeFigure(data[0]);
            var elem = figuresType[tag];
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
            for (; i < figuresType.Length; i++)
                if (figuresType[i].GetType().Name == type)
                    break;
            return i;
        }

        private Color ToColor(string colorLine)
        {
            colorLine = colorLine.Remove(0, 1);
            var color = new Color
            {
                A = Convert.ToByte(new String(new char[] {colorLine[0], colorLine[1]}), 16),
                R = Convert.ToByte(new String(new char[] {colorLine[2], colorLine[3]}), 16),
                G = Convert.ToByte(new String(new char[] {colorLine[4], colorLine[5]}), 16),
                B = Convert.ToByte(new String(new char[] {colorLine[6], colorLine[7]}), 16)
            };
            return color;
        }
    }
}