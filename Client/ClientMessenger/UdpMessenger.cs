using System.Net;
using System.Net.Sockets;
using System.Text;
using Client.Messages;

namespace Client.ClientMessenger
{
    public class UdpMessenger : IDisposable, IMessageSourceClient<IPEndPoint>
    {
        private IPEndPoint? _localEndPoint;
        private UdpClient _udpClient;
        private bool _disposedValue;
        public UdpClient UdpClient { get => _udpClient; set => _udpClient = value; }

        public UdpMessenger()
        {
            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
        }

        public async Task SendMessageAsync(BaseMessage message)
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
            string jSonToSend = message.SerializeMessageToJson();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(jSonToSend);
            await _udpClient.SendAsync(data, data.Length, remoteEndPoint);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _disposedValue = true;
                    _udpClient?.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task<BaseMessage> RecieveMessageAsync(CancellationToken ctoken)
        {
            var result = await _udpClient.ReceiveAsync(ctoken);
            var messageString = Encoding.UTF8.GetString(result.Buffer);
            return BaseMessage.DeserializeFromJson(messageString);
        }

        public IPEndPoint GetServerEndPoint() => _udpClient.Client.LocalEndPoint as IPEndPoint;
    }

}