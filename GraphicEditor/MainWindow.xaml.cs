using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using GraphicEditor.Client;
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
    public partial class MainWindow : Window
    {
        private double _x1, _x2, _y1, _y2;
        private ushort _tag;
        private readonly List<UIElement> _deleteFigure = new List<UIElement>();
        private readonly List<Figure> _deleteFiguresForSerialezer = new List<Figure>();
        private readonly Dictionary<int, Figure> _figures = new Dictionary<int, Figure>();
        private Brush _color = Brushes.Blue;
        private bool _isDraw, _isOneFigure, _isMove;
        private bool _isConnected;
        private Shape _shape = new Shape {Figures = new List<Figure>()};
        private Figure[] _figuresType;
        private string _nameOpenOnceJpeg;
        private static UdpClient _client;
        private bool _isSendMessage;
        private Figure _figure;
        private System.Windows.Shapes.Shape _chousenShape;
        private Point _oldPointFirst, _oldPointSecond;
        private string _userName;


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
            _figures.Add(0, new Line(new Point(_x1, _y1), new Point(_x2, _y2), _color, SliderThickness.Value));
            _figures.Add(1,
                new Circle(new Point(_x1, _y1), new Point(_x2, _y2), _color, SliderThickness.Value));
            _figures.Add(2,
                new Ellipse(new Point(_x1, _y1), new Point(_x2, _y2), _color, SliderThickness.Value));
            _figures.Add(3,
                new Rectangle(new Point(_x1, _y1), new Point(_x2, _y2), _color, SliderThickness.Value));
            _figures.Add(4,
                new Square(new Point(_x1, _y1), new Point(_x2, _y2), _color, SliderThickness.Value));
            _figures.Add(5, new Triangle(new Point(_x1, _y1), new Point(_x2, _y2), _color, SliderThickness.Value));
            _figures.Add(6, new RightTriangle(new Point(_x1, _y1), new Point(_x2, _y2), _color, SliderThickness.Value));
        }


        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition(CanvasMain);
            _x1 = point.X;
            _y1 = point.Y;
            _isDraw = true;
            _isOneFigure = _isMove;
            if (_isConnected)
            {
                if (!_isMove)
                {
                    UdpHelper.SendBeginPaint(_client, _isOneFigure);
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            CanvasMain.Children.Clear();
            _deleteFiguresForSerialezer.Clear();
            _deleteFigure.Clear();
        }

        private void UndoPicture_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CanvasMain.Children.Count == 0)
            {
                return;
            }

            _deleteFigure.Add(CanvasMain.Children[CanvasMain.Children.Count - 1]);
            _deleteFiguresForSerialezer.Add(_shape.Figures[CanvasMain.Children.Count - 1]);
            _shape.Figures.RemoveAt(CanvasMain.Children.Count - 1);
            CanvasMain.Children.RemoveAt(CanvasMain.Children.Count - 1);
        }


        private void RedoPicture_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_deleteFigure.Count == 0)
            {
                return;
            }

            CanvasMain.Children.Add(_deleteFigure[_deleteFigure.Count - 1]);
            _shape.Figures.Add(_deleteFiguresForSerialezer[_deleteFigure.Count - 1]);
            _deleteFigure.RemoveAt(_deleteFigure.Count - 1);
            _deleteFiguresForSerialezer.RemoveAt(_deleteFiguresForSerialezer.Count - 1);
        }

        private void OnMouseUpFigure(object s, EventArgs f)
        {
            _isMove = false;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            LabelX.Content = "X = " + e.GetPosition(CanvasMain).X;
            LabelY.Content = "Y = " + e.GetPosition(CanvasMain).Y;
            var point = e.GetPosition(CanvasMain);
            if (_isDraw && !_isMove && (e.LeftButton == MouseButtonState.Pressed))
            {
                _x2 = point.X;
                _y2 = point.Y;
                ChangingObjects();

                PrintFigure(_figures[_tag]);
                CanvasMain.Children[CanvasMain.Children.Count - 1].MouseDown += OnMouseDownFigure;
                CanvasMain.Children[CanvasMain.Children.Count - 1].MouseUp += OnMouseUpFigure;

                if (_isConnected)
                {
                    UdpHelper.SendFigure(_figures[_tag], _client, _isOneFigure);
                }


                _figures.Clear();
            }
            else if (_isMove && e.LeftButton == MouseButtonState.Pressed)
            {
                _figure.FirstPoint.X = _oldPointFirst.X + (point.X - _x1);
                _figure.SecondPoint.X = _oldPointSecond.X + (point.X - _x1);
                _figure.FirstPoint.Y = _oldPointFirst.Y + (point.Y - _y1);
                _figure.SecondPoint.Y = _oldPointSecond.Y + (point.Y - _y1);
                if (_isOneFigure)
                {
                    CanvasMain.Children.Remove(_chousenShape);
                }
                else
                {
                    CanvasMain.Children.RemoveAt(CanvasMain.Children.Count - 1);
                }

                _isOneFigure = false;
                _figure.SetColor();
                _figure.Draw(CanvasMain);

                CanvasMain.Children[CanvasMain.Children.Count - 1].MouseDown += OnMouseDownFigure;
                CanvasMain.Children[CanvasMain.Children.Count - 1].MouseUp += OnMouseUpFigure;
            }
        }

        private void ReceiveData()
        {
            while (_isConnected)
            {
                IPEndPoint remoteIp = null;
                byte[] data = _client.Receive(ref remoteIp);
                var package = new Converter<PackageForSending>().ConvertToObject(data);
                _isOneFigure = package.IsOneFigure;
                switch (package.Command)
                {
                    case Commands.BeginPaint:
                    case Commands.Point:
                        var figure = new Converter<Figure>().ConvertToObject(package.Data);
                        PrintFigure(figure);

                        break;

                    case Commands.Message:
                        ReceiveMessage(Encoding.Default.GetString(package.Data));
                        break;
                }
            }
        }

        private void PrintFigure(Figure figure)
        {
            Dispatcher.Invoke(() =>
            {
                if (_isOneFigure)
                {
                    CanvasMain.Children.RemoveAt(CanvasMain.Children.Count - 1);
                    _shape.Figures.RemoveAt(_shape.Figures.Count - 1);
                }

                _shape.Figures.Add(figure);
                figure.SetColor();
                figure.Draw(CanvasMain);
                _isOneFigure = true;
            });
        }

        private void ColorBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _color = new SolidColorBrush(Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B));
                ColorBorder.Background = _color;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var serializationWindow = new SerializationWindow();
            if (serializationWindow.ShowDialog() != true)
            {
                return;
            }

            SaveFileDialog saveFile = InitializeSaveFile(serializationWindow.TypeSerialization.ToLower());

            if (saveFile.ShowDialog(this) != true)
            {
                return;
            }

            if (serializationWindow.TypeSerialization == "Jpeg")
            {
                if (saveFile.FileName != _nameOpenOnceJpeg)
                {
                    SaveOpenJpegFile<IJpegFile>(true, saveFile, null);
                    _nameOpenOnceJpeg = null;
                }
                else
                {
                    MessageBox.Show("Can't save to open file.");
                }
            }
            else if (serializationWindow.TypeSerialization == "Txt")
            {
                MessageBox.Show(SaveTXT(saveFile, _shape));
            }
            else
            {
                SaveOpenFile<ISerializer>(serializationWindow.TypeSerialization, true, saveFile, null, _shape);
            }
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
                foreach (var type in serializerAssembly.GetExportedTypes())
                {
                    if (type.IsClass && typeof(T).IsAssignableFrom(type) && (type.Name == someType))
                    {
                        var serializer = (T) Activator.CreateInstance(type);
                        if (choice)
                        {
                            MessageBox.Show(serializer.Serialize(saveFile, shape));
                        }
                        else
                        {
                            return serializer.Deserialize<Shape>(openFile, someType);
                        }

                        return null;
                    }
                }
            }
            catch
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
            foreach (var type in serializerAssembly.GetExportedTypes())
            {
                if (type.IsClass && typeof(T).IsAssignableFrom(type) && (type.Name == "Jpeg"))
                {
                    var serializer = (T) Activator.CreateInstance(type);
                    try
                    {
                        if (choice)
                        {
                            MessageBox.Show(serializer.Serialize(saveFile, CanvasMain));
                        }
                        else
                        {
                            serializer.Deserialize(openFile, CanvasMain);
                        }

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
                if (openFile.ShowDialog(this) != true)
                {
                    return;
                }

                try
                {
                    if (serializationWindow.TypeSerialization == "Jpeg")
                    {
                        SaveOpenJpegFile<IJpegFile>(false, null, openFile);
                        _nameOpenOnceJpeg = openFile.FileName;
                    }
                    else if (serializationWindow.TypeSerialization == "Txt")
                    {
                        _shape = OpenTxt(openFile, serializationWindow.TypeSerialization);
                    }
                    else
                        _shape = SaveOpenFile<ISerializer>(serializationWindow.TypeSerialization, false, null,
                            openFile, null);

                    if (serializationWindow.TypeSerialization != "Jpeg")
                    {
                        Parser(_shape);
                        _tag = 0;
                    }
                }
                catch (Exception)
                {
                    _shape = new Shape {Figures = new List<Figure>()};
                }
            }
            else
            {
                MessageBox.Show("No view selected");
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDraw = _isMove = false;
        }


        private void Picture_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;
            _tag = Convert.ToUInt16(image?.Tag);
            _isMove = false;
        }

        private void Parser(Shape shape)
        {
            if (shape?.Figures == null)
            {
                return;
            }

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
                {
                    foreach (var figure in shape.Figures)
                    {
                        sw.WriteLine(
                            $"{figure.GetType().Name} {figure.FirstPoint.X} {figure.FirstPoint.Y} {figure.SecondPoint.X}" +
                            $" {figure.SecondPoint.Y} {figure.SerializedColor} {figure.Thickness}");
                    }
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

        private Shape OpenTxt(OpenFileDialog archive, string filter)
        {
            var shape = new Shape {Figures = new List<Figure>()};
            try
            {
                using (var zip = ZipFile.Open(archive.FileName, ZipArchiveMode.Read))
                    foreach (var entry in zip.Entries)
                    {
                        if (entry.FullName.EndsWith($".{filter}", StringComparison.OrdinalIgnoreCase))
                        {
                            var path = archive.FileName;
                            path = (path.Replace(archive.SafeFileName, ""));
                            path = path.Remove(path.Length - 1, 1) + $"\\{entry.Name}";
                            entry.ExtractToFile(path);
                        }
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
                        {
                            shape.Figures.Add(ParserTxt(line));
                        }
                    }
                }
            }

            return shape;
        }

        private Figure[] InitializeArrayFigure(int x1, int y1, int x2, int y2, double thickness)
        {
            return new Figure[]
            {
                new Line(new Point(x1, y1), new Point(x2, y2), _color, thickness),
                new Circle(new Point(x1, y1), new Point(x2, y2), _color, thickness),
                new Ellipse(new Point(x1, y1), new Point(x2, y2), _color, thickness),
                new Rectangle(new Point(x1, y1), new Point(x2, y2), _color, thickness),
                new Square(new Point(x1, y1), new Point(x2, y2), _color, thickness),
                new Triangle(new Point(x1, y1), new Point(x2, y2), _color, thickness),
                new RightTriangle(new Point(x1, y1), new Point(x2, y2), _color, thickness),
            };
        }

        private void OnMouseDownFigure(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(CanvasMain);
            _chousenShape = (System.Windows.Shapes.Shape) sender;


            if (_shape.Figures.Count(x => x.Tag == _chousenShape.GetHashCode()) == 0)
            {
                return;
            }

            _figure = _shape.Figures.First(x => x.Tag == _chousenShape.GetHashCode());
            _isMove = true;
            _oldPointFirst = _figure.FirstPoint;
            _oldPointSecond = _figure.SecondPoint;
            _isOneFigure = true;
            _x1 = p.X;
            _y1 = p.Y;
        }

        private void TextBoxUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            ButtonEnter.IsEnabled = TextBoxUsername.Text != "";
        }

        private async void Enter(object sender, RoutedEventArgs e)
        {
            try
            {
                _client = new UdpClient(UdpHelper.GetFreePort());
                _client.JoinMulticastGroup(IPAddress.Parse(ConectInfo.GroupAddr));
                _isConnected = _isSendMessage = true;
                Task.Run(ReceiveData).Start();
            }
            catch
            {
            }

            _userName = TextBoxUsername.Text;
            SendMessage(_userName + " joined");

            GridMain.Visibility = Visibility.Visible;
            GridRegister.Visibility = Visibility.Hidden;
            GridChat.Visibility = Visibility.Visible;

            for (var i = 0; i <= 150; i += 5)
            {
                GridChat.Width = i;
                await Task.Delay(20);
            }
        }

        private void ReceiveMessage(string message)
        {
            try
            {
                if (!_isSendMessage)
                {
                    PrintStr(message);
                }

                _isSendMessage = false;
            }
            catch
            {
                Disconnect();
            }
        }

        private void PrintStr(string str)
        {
            Dispatcher.Invoke(() =>
            {
                var time = DateTime.Now.ToShortTimeString();
                TextBoxMessages.AppendText($"{time}: {str}{Environment.NewLine}");
                OnTextMessageChanged();
            });
        }

        private static void Disconnect()
        {
            _client.DropMulticastGroup(IPAddress.Parse(ConectInfo.GroupAddr));
        }

        private void TextBoxEnter_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.Enter || TextBoxEnter.Text == "")
            {
                return;
            }

            _isSendMessage = true;
            SendMessage($"{_userName}: {TextBoxEnter.Text}");
            var time = DateTime.Now.ToShortTimeString();
            TextBoxMessages.AppendText($"{time}: {TextBoxEnter.Text}{Environment.NewLine}");
            OnTextMessageChanged();
            TextBoxEnter.Text = string.Empty;
            _isSendMessage = false;
        }

        static void SendMessage(string message)
        {
            UdpHelper.SendMessage(_client, message);
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            GridMain.Visibility = Visibility.Hidden;
            GridRegister.Visibility = Visibility.Visible;
        }

        private async void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (!_isConnected)
            {
                return;
            }

            SendMessage(_userName + " leave");
            Disconnect();
            _isConnected = false;
            var width = GridChat.Width;
            for (var i = width; i >= 0; i -= 5)
            {
                GridChat.Width = i;
                await Task.Delay(20);
            }

            GridChat.Visibility = Visibility.Hidden;
        }

        private Figure ParserTxt(string line)
        {
            string[] data = line.Split(' ');
            _figuresType = InitializeArrayFigure(int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]),
                int.Parse(data[4]), int.Parse(data[6]));
            byte tag = FindTypeFigure(data[0]);
            var elem = _figuresType[tag];
            elem.SerializedColor = ToColor(data[5]);
            return elem;
        }

        private byte FindTypeFigure(string type)
        {
            byte i = 0;
            for (; i < _figuresType.Length; i++)
            {
                if (_figuresType[i].GetType().Name == type)
                    break;
            }

            return i;
        }

        private void OnTextMessageChanged()
        {
            TextBoxMessages.ScrollToEnd();
        }

        private Color ToColor(string colorLine)
        {
            colorLine = colorLine.Remove(0, 1);
            var color = new Color
            {
                A = Convert.ToByte(new string(new[] {colorLine[0], colorLine[1]}), 16),
                R = Convert.ToByte(new string(new[] {colorLine[2], colorLine[3]}), 16),
                G = Convert.ToByte(new string(new[] {colorLine[4], colorLine[5]}), 16),
                B = Convert.ToByte(new string(new[] {colorLine[6], colorLine[7]}), 16)
            };
            return color;
        }
    }
}