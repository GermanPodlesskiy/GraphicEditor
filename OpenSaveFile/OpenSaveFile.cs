using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace OpenSaveFile
{
    public static class Archive
    {
        public static string SaveArchive(string fileName, string safeFileName, string filter)
        {
            string zipFileName = Regex.Replace(fileName, filter, "zip");
            using (var fs = new FileStream(zipFileName, FileMode.Create))
            using (var archive = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                archive.CreateEntryFromFile(fileName, safeFileName);
                return "File saved, " + fileName;
            }
        }

        public static OpenFileDialog InitializeOpenFile(string filter)
        {
            var file = new OpenFileDialog();
            file.Filter = $"{filter} |*.{filter}";
            file.AddExtension = true;
            file.Title = "Open file";
            return file;
        }

        public static void UnArchive(OpenFileDialog archive, string filter)
        {
            try
            {
                using (var zip = ZipFile.Open(archive.FileName, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in zip.Entries)
                        if (entry.FullName.EndsWith($".{filter}", StringComparison.OrdinalIgnoreCase))
                        {
                            var path = archive.FileName;
                            path = (path.Replace(archive.SafeFileName, ""));
                            path = path.Remove(path.Length - 1, 1) + $"\\{entry.Name}";
                            entry.ExtractToFile(path);
                        }
                }
            }
            catch
            {
            }
        }
    }

    public class Dat : SerializerInterface.ISerializer
    {
        public T Deserialize<T>(OpenFileDialog archive, string filter) where T : class
        {
            Archive.UnArchive(archive, filter);
            OpenFileDialog openFile = Archive.InitializeOpenFile(filter.ToLower());
            if (openFile.ShowDialog() != true) return null;

            using (var fs = new FileStream(openFile.FileName, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                return (T) formatter.Deserialize(fs);
            }
        }

        public string Serialize<T>(SaveFileDialog saveFile, T obj) where T : class
        {
            using (var fs = new FileStream(saveFile.FileName, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(fs, obj);
            }

            return Archive.SaveArchive(saveFile.FileName, saveFile.SafeFileName, "dat");
        }
    }

    public class Json : SerializerInterface.ISerializer
    {
        public T Deserialize<T>(OpenFileDialog archive, string filter) where T : class
        {
            Archive.UnArchive(archive, filter);
            OpenFileDialog openFile = Archive.InitializeOpenFile(filter.ToLower());
            if (openFile.ShowDialog() != true) return null;

            using (var fs = new FileStream(openFile.FileName, FileMode.Open))
            {
                var formatter = new DataContractJsonSerializer(typeof(T));
                return (T) formatter.ReadObject(fs);
            }
        }

        public string Serialize<T>(SaveFileDialog saveFile, T obj) where T : class
        {
            using (var fs = new FileStream(saveFile.FileName, FileMode.Create))
            {
                var formatter = new DataContractJsonSerializer(typeof(T));
                formatter.WriteObject(fs, obj);
            }

            return Archive.SaveArchive(saveFile.FileName, saveFile.SafeFileName, "json");
        }
    }

    public class Xml : SerializerInterface.ISerializer
    {
        public T Deserialize<T>(OpenFileDialog archive, string filter) where T : class
        {
            Archive.UnArchive(archive, filter);
            OpenFileDialog openFile = Archive.InitializeOpenFile(filter.ToLower());
            if (openFile.ShowDialog() != true) return null;

            using (var fs = new FileStream(openFile.FileName, FileMode.Open))
            {
                var formatter = new XmlSerializer(typeof(T));
                return (T) formatter.Deserialize(fs);
            }
        }

        public string Serialize<T>(SaveFileDialog saveFile, T obj) where T : class
        {
            using (var fs = new FileStream(saveFile.FileName, FileMode.Create))
            {
                var formatter = new XmlSerializer(typeof(T));
                formatter.Serialize(fs, obj);
            }

            return Archive.SaveArchive(saveFile.FileName, saveFile.SafeFileName, "xml");
        }
    }

    public class Jpeg : SerializerInterface.IJpegFile
    {
        public void Deserialize<T>(OpenFileDialog archive, T obj) where T : Panel
        {
            Archive.UnArchive(archive, "Jpeg");
            OpenFileDialog openFile = Archive.InitializeOpenFile("jpeg");
            if (openFile.ShowDialog() == true)
            {
                var bmp = new BitmapImage(new Uri(openFile.FileName));
                var image = new Image()
                {
                    Source = bmp,
                    Width = bmp.Width,
                    Height = bmp.Height
                };

                obj.Children.Clear();
                obj.Children.Add(image);
            }
        }

        public string Serialize<T>(SaveFileDialog saveFile, T obj) where T : Panel
        {
            using (var fs = new FileStream(saveFile.FileName, FileMode.OpenOrCreate))
            {
                Transform transform = obj.LayoutTransform;
                obj.LayoutTransform = null;

                Thickness margin = obj.Margin;
                obj.Margin = new Thickness(0, 0,
                    margin.Right - margin.Left, margin.Bottom - margin.Top);

                var size = new Size(obj.ActualWidth, obj.ActualHeight);

                obj.Measure(size);
                obj.Arrange(new Rect(size));

                var rtb = new RenderTargetBitmap((int) obj.ActualWidth, (int) obj.ActualHeight, 96, 96,
                    PixelFormats.Pbgra32);
                rtb.Render(obj);

                var jpegEnc = new JpegBitmapEncoder();
                jpegEnc.Frames.Add(BitmapFrame.Create(rtb));
                jpegEnc.Save(fs);
                obj.LayoutTransform = transform;
                margin.Top = 0.01;
                obj.Margin = margin;
            }

            return Archive.SaveArchive(saveFile.FileName, saveFile.SafeFileName, "jpeg");
        }
    }
}