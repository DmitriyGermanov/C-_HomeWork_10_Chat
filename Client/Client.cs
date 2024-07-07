using System.Net;
using System.Net.Sockets;

namespace Client
{
    public class Client : IDisposable
    {
        private IPEndPoint? localEndPoint;
        private UdpClient udpClient;
        private bool disposedValue;
        public IPEndPoint LocalEndPoint { get => localEndPoint; set => localEndPoint = value; }
        public UdpClient UdpClient { get => udpClient; set => udpClient = value; }

        public Client()
        {
            //this.localEndPoint = localEndPoint;
            udpClient = new UdpClient();
        }


        public async Task SendMessageAsync(Message message)
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
            string jSonToSend = message.SerializeMessageToJson();

            byte[] data = System.Text.Encoding.UTF8.GetBytes(jSonToSend);
            await udpClient.SendAsync(data, data.Length, remoteEndPoint);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    disposedValue = true;
                    udpClient?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}