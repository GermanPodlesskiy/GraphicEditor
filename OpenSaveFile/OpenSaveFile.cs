using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace OpenSaveFile
{
    public class Open
    {
        public static T OpenDAT<T>(OpenFileDialog openFile) where T : class
        {
            using (FileStream fs = new FileStream(openFile.FileName, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    return (T)formatter.Deserialize(fs);
                }
                catch (Exception){}
                
                return null;
            }
        }
        public static T OpenJSON<T>(OpenFileDialog openFile) where T : class
        {
            using (FileStream fs = new FileStream(openFile.FileName, FileMode.Open))
            {
                DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(T));
                try
                {
                    return (T) formatter.ReadObject(fs);
                }
                catch (Exception){}

                return null;
            }
        }
        public static T OpenXML<T>(OpenFileDialog saveFile) where T : class
        {
            using (FileStream fs = new FileStream(saveFile.FileName, FileMode.Open))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                try
                {
                    return (T) formatter.Deserialize(fs);
                }
                catch (Exception) { }

                return null;
            }
        }
        public static void OpenJPEG<T>(OpenFileDialog openFile, T obj) where T : Panel
        {
            BitmapImage bmp = new BitmapImage(new Uri(openFile.FileName));
            Image image = new Image();
            image.Source = bmp;
            image.Width = bmp.Width;
            image.Height = bmp.Height;
            obj.Children.Clear();
            obj.Children.Add(image);
        }
    }

    
    public class Save
    {
        public static string SaveDAT<T>(SaveFileDialog saveFile, T obj)
        {
            using (FileStream fs = new FileStream(saveFile.FileName, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, obj);
                return "File saved, " + saveFile.FileName;
            }
        }
        public static string SaveJSON<T>(SaveFileDialog saveFile, T obj)
        {
            using (FileStream fs = new FileStream(saveFile.FileName, FileMode.Create))
            {
                DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(T));
                formatter.WriteObject(fs, obj);
                return "File saved, " + saveFile.FileName;
            }
        }
        public static string SaveXML<T>(SaveFileDialog saveFile, T obj)
        {
            using (FileStream fs = new FileStream(saveFile.FileName, FileMode.Create))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                formatter.Serialize(fs, obj);
                return "File saved, " + saveFile.FileName;
            }
        }
        public static string SaveJPEG<T>(SaveFileDialog saveFile, T obj) where T : Panel
        {
            using (FileStream fs = new FileStream(saveFile.FileName, FileMode.CreateNew))
            {
                Transform transform = obj.LayoutTransform;
                obj.LayoutTransform = null;

                Thickness margin = obj.Margin;
                obj.Margin = new Thickness(0,0,
                margin.Right - margin.Left, margin.Bottom - margin.Top);

                Size size = new Size(obj.ActualWidth, obj.ActualHeight);

                obj.Measure(size);
                obj.Arrange(new Rect(size));

                RenderTargetBitmap rtb = new RenderTargetBitmap((int)obj.ActualWidth, (int)obj.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(obj);

                JpegBitmapEncoder jpegEnc = new JpegBitmapEncoder();
                jpegEnc.Frames.Add(BitmapFrame.Create(rtb));
                jpegEnc.Save(fs);
                obj.LayoutTransform = transform;
                margin.Top = 0.0;
                obj.Margin = margin;

                return "File saved, " + saveFile.FileName;
            }
        }
    }
}
