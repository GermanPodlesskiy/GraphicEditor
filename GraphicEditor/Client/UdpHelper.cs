using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using GraphicEditor.Client;
using GraphicEditor.Shapes;

namespace GraphicEditor
{
    static class UdpHelper
    {
        public static void Send(byte[] data, UdpClient client)
        {
            var usedPorts = GetUsedPorts();
            foreach (var usedPort in usedPorts)
            {
                client.Send(data, data.Length, ConectInfo.GroupAddr, usedPort);
            }
        }

        public static void SendBeginPaint(UdpClient client, bool isOneFigure)
        {
            var data = GetPointDataForSending(Commands.BeginPaint, new Line(), isOneFigure);
            Send(data, client);
        }

        public static void SendFigure(Figure figure, UdpClient client, bool isOneFigure)
        {
            byte[] data = GetPointDataForSending(Commands.Point, figure, isOneFigure);
            Send(data, client);
        }

        public static void SendMessage(UdpClient client, string message)
        {
            byte[] data = GetDataForSending(Commands.Message, Encoding.Default.GetBytes(message), true);
            Send(data, client);
        }

        public static byte[] GetPointDataForSending(Commands command, Figure figure, bool isOneFigure)
        {
            var serialazideData = new Converter<Figure>().ConvertToBytes(figure);
            return GetDataForSending(command, serialazideData, isOneFigure);
        }

        public static int GetFreePort()
        {
            var usedPorts = GetUsedPorts();
            var freePort = Enumerable.Range(ConectInfo.StartPort, ConectInfo.MaxPortCount).Except(usedPorts)
                .FirstOrDefault();
            if (freePort == 0)
            {
                throw new Exception("There aren't free ports");
            }

            return freePort;
        }

        private static IEnumerable<int> GetUsedPorts()
        {
            var portsForChecking = Enumerable.Range(ConectInfo.StartPort, ConectInfo.MaxPortCount);
            var usedPorts = from port in portsForChecking
                join usedPort in IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners() on port equals
                    usedPort.Port
                select port;
            return usedPorts;
        }


        private static byte[] GetDataForSending(Commands command, byte[] data, bool isOneFigure)
        {
            var package = new PackageForSending
            {
                Command = command,
                IsOneFigure = isOneFigure,
                Data = data
            };

            return new Converter<PackageForSending>().ConvertToBytes(package);
        }
    }
}