using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SerializerInterface
{
    public interface ISerializer
    {
        string Serialize<T>(SaveFileDialog saveFile, T obj) where T : class;
        T Deserialize<T>(OpenFileDialog openFile, string filter) where T : class;

    }
    public interface IJpegFile
    {
        void Deserialize<T>(OpenFileDialog archive, T obj) where T : Panel;
        string Serialize<T>(SaveFileDialog saveFile, T obj) where T : Panel;
    }
}
