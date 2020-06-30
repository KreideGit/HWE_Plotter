using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Main
{
    class Program
    {
        private static Socket server;
        private static Plotter p;

        static void Main(string[] args)
        {
            p = new Plotter();

            //for(double i = 0; i < 4 * Math.PI; i += 0.001)
            //{
            //    double ydist = 3000 * Math.Sin(i);
            //    p.MoveTo((int)(200*i), (int)ydist);
            //}

            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Parse("192.168.0.163"), 5555));
            server.Listen(1);

            Socket client = server.Accept();

            while(true)
            {
                byte[] buffer = new byte[4];
                int receivedBytes = client.Receive(buffer);
                int amountOfBytes = BitConverter.ToInt32(buffer, 0);

                buffer = new byte[amountOfBytes];
                List<byte> result = new List<byte>();

                while(true)
                {
                    receivedBytes = client.Receive(buffer);

                    byte[] data = new byte[receivedBytes];
                    Array.Copy(buffer, data, data.Length);

                    result.AddRange(data);
                    amountOfBytes -= receivedBytes;

                    if (amountOfBytes <= 0)
                        break;
                }

                List<int[]> positions;
                using (var ms = new MemoryStream(result.ToArray()))
                {
                    positions = (List<int[]>)new BinaryFormatter().Deserialize(ms);
                }

                foreach (var position in positions)
                {
                    p.MoveTo(position[0] * 12, position[1] * (-12), position[2]);
                }
            }
        }
    }
}