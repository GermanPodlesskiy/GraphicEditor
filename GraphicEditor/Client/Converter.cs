using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GraphicEditor.Client
{
    public class Converter<T>
    {
        private BinaryFormatter _formatter = new BinaryFormatter();
        private Stream _stream = new MemoryStream();

        public byte[] ConvertToBytes(T data)
        {
            _formatter.Serialize(_stream, data);
            var buffer = new byte[(int) _stream.Length];
            _stream.Position = 0;
            _stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public T ConvertToObject(byte[] data)
        {
            _stream.Write(data, 0, data.Length);
            _stream.Position = 0;
            return (T) _formatter.Deserialize(_stream);
        }
    }
}