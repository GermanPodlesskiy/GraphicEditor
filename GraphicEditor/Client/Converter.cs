using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GraphicEditor.Client
{
    public class Converter<T>
    {
        private BinaryFormatter formatter = new BinaryFormatter();
        private Stream stream = new MemoryStream();

        public byte[] ConvertToBytes(T data)
        {
            formatter.Serialize(stream, data);
            var buffer = new byte[(int) stream.Length];
            stream.Position = 0;
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public T ConvertToObject(byte[] data)
        {
            stream.Write(data, 0, data.Length);
            stream.Position = 0;
            return (T) formatter.Deserialize(stream);
        }
    }
}