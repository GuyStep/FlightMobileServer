using System;
using System.Net.Sockets;
using System.Text;

namespace FlightMobileServer.Model
{
    public class Client
    {
        private int maxChars = 100;
        private TcpClient tcpClient;
        private NetworkStream stream;
        private string ip;
        private int port;
        public Client(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }
        public void Connect()
        {
            this.tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(ip, port);
                stream = tcpClient.GetStream();
            }
            catch (Exception e) //Could not connect to simulator
            {
                if (e.Message.Contains("No connection")) throw new Exception("Could not connect to simulator");
            }
        }
        public void Disconnect()
        {
            tcpClient.Close();
        }
        public string Read()
        {
            string stringResult = "";
            byte[] receivedData = new byte[maxChars];
            int bytesLength = this.stream.Read(receivedData, 0, maxChars);
           
            for (int i = 0; i < bytesLength; i++) //Convert response bytes to string
                stringResult += (Convert.ToChar(receivedData[i]));
            return stringResult;
        }
        public void Write(string data)
        {
            this.stream = this.tcpClient.GetStream();
            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] commandInBytes = ascii.GetBytes(data); //Convert command string to bytes
            stream.Write(commandInBytes, 0, commandInBytes.Length);
        }
        public void SetTimeOutRead(int time)
        {
            this.tcpClient.ReceiveTimeout = time;
        }

        public TcpClient getTcpClient()
        {
            return this.tcpClient;
        }

        public bool IsConnected()
        {
            return this.tcpClient.Connected;
        }
    }
}