using System.Net;
using System.Net.Sockets;
using Client.Messages;

namespace Client
{
    public class Messenger : IDisposable
    {
        private IPEndPoint? localEndPoint;
        private UdpClient udpClient;
        private bool disposedValue;
        public UdpClient UdpClient { get => udpClient; set => udpClient = value; }

        public Messenger()
        {
            udpClient = new UdpClient();
        }


        public async Task SendMessageAsync(BaseMessage message)
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