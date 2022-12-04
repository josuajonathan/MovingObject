using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        private Socket clientSocket;
        private byte[] buffer;
        private ShapePackage tempShape;

        Pen red = new Pen(Color.Red);
        Rectangle rect = new Rectangle(100, 100, 30, 30);
        SolidBrush fillBlue = new SolidBrush(Color.Blue);

        public Form1()
        {
            InitializeComponent();
            ConnectServer();
            timer1.Interval = 50;
            timer1.Enabled = true;
        }

        private static void ShowErrorDialog(string message)
        {
            MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ConnectServer()
        {
            try{
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // Connect to the specified host.
                var endPoint = new IPEndPoint(IPAddress.Any, 3333);
                clientSocket.BeginConnect(endPoint, ConnectCallback, null);
            }

            catch (SocketException ex){
                ShowErrorDialog(ex.Message);
            }

            catch (ObjectDisposedException ex){
                ShowErrorDialog(ex.Message);
            }
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            try{
                int received = clientSocket.EndReceive(AR);

                if (received == 0)
                {
                    return;
                }

                tempShape = new ShapePackage(buffer);
                if (buffer == null)
                    Debug.WriteLine("Null");
                Console.WriteLine(tempShape.X + " " + tempShape.Y);

                // Start receiving data again.
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }

            // Avoid Pokemon exception handling in cases like these.
            catch (SocketException ex){
                ShowErrorDialog(ex.Message);
            }

            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }

        private void ConnectCallback(IAsyncResult AR)
        {
            try
            {
                clientSocket.EndConnect(AR);
                buffer = new byte[clientSocket.ReceiveBufferSize];
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (SocketException ex){
                ShowErrorDialog(ex.Message);
            }

            catch (ObjectDisposedException ex){
                ShowErrorDialog(ex.Message);
            }
        }

        private void SendCallback(IAsyncResult AR)
        {
            try{
                clientSocket.EndSend(AR);
            }

            catch (SocketException ex){
                ShowErrorDialog(ex.Message);
            }

            catch (ObjectDisposedException ex){
                ShowErrorDialog(ex.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ApplySpecShape(tempShape);
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawRectangle(red, rect);
            g.FillRectangle(fillBlue, rect);
        }

        private void ApplySpecShape(ShapePackage shape)
        {
            rect.X = shape.X;
            rect.Y = shape.Y;
        }

    }
}
