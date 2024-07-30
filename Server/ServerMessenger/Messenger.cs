﻿using Server.Messages;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server.ServerMessenger
{

    public class Messenger : IDisposable, IMessageSourceServer<IPEndPoint>
    {
        private IPEndPoint? _localEndPoint;
        private UdpClient _udpClient;
        private bool _disposedValue;
        public UdpClient UdpClient { get => _udpClient; set => _udpClient = value; }

        public Messenger()
        {
            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
        }
        public Messenger(UdpClient udpClient)
        {
            _udpClient = udpClient;
        }
        public async static Task SendMessageAsync(BaseMessage message, IPEndPoint endPoint)
        {
            using UdpClient udpClient = new UdpClient(0);
            string jSonToSend = message.SerializeMessageToJson();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(jSonToSend);
            await udpClient.SendAsync(data, data.Length, endPoint);
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
