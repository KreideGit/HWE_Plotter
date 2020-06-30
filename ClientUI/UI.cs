using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace ClientUI
{
    public partial class UI : Form
    {
        bool firstDown;
        bool lastDown;
        bool down;
        Graphics panelGraphics;
        List<int[]> coordinates;
        Socket client;

        public UI()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            Width = 297 * 5;
            Height = 210 * 5;
            Panel p = new Panel();
            AllocConsole();

            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //client.Connect(IPAddress.Parse("192.168.0.163"), 5555);

            p.Width = 297 * 5;
            p.Height = 210 * 5;

            coordinates = new List<int[]>();
            Controls.Add(p);

            p.MouseMove += P_MouseMove;
            p.MouseDown += P_MouseDown;
            p.MouseUp += P_MouseUp;
            KeyDown += UI_KeyDown;

            panelGraphics = p.CreateGraphics();
            panelGraphics.Clear(Color.White);
        }

        private void UI_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.R)
            {
                if(!down)
                {
                    panelGraphics.Clear(Color.White);
                }
            }
            else if(e.KeyData == Keys.D)
            {
                byte[] data;
                using (var ms = new MemoryStream())
                {
                    new BinaryFormatter().Serialize(ms, coordinates);
                    data = ms.ToArray();
                }

                byte[] amountOfBytes = BitConverter.GetBytes(data.Length);
                byte[] packet = new byte[data.Length + amountOfBytes.Length];
                amountOfBytes.CopyTo(packet, 0);
                data.CopyTo(packet, amountOfBytes.Length);

                client.Send(packet);
                coordinates.Clear();
            }
        }

        private void P_MouseMove(object sender, MouseEventArgs e)
        {
            if(firstDown)
            {
                //coordinates.Clear();
                firstDown = false;
            }

            if(down)
            {
                coordinates.Add(new int[] { e.X, e.Y });
                panelGraphics.FillEllipse(Brushes.Black, new Rectangle(e.X - 3, e.Y - 3, 6, 6));
            }

            if (lastDown)
            {
                lastDown = false;
            }
        }

        private void P_MouseUp(object sender, MouseEventArgs e)
        {
            lastDown = true;
            down = false;
        }

        private void P_MouseDown(object sender, MouseEventArgs e)
        {
            firstDown = true;
            down = true;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();
    }
}