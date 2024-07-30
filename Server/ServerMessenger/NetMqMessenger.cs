using NetMQ;
using NetMQ.Sockets;
using Server.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server.ServerMessenger
{
    internal class NetMqMessenger : IMessageSourceServer<byte[]>, IDisposable
    {
        private readonly ResponseSocket _serverSocket;
        private bool _disposed = false;

        public NetMqMessenger()
        {
            _serverSocket = new ResponseSocket("@tcp://127.0.0.1:12345"); 
        }

        public async Task SendMessageAsync(BaseMessage message, byte[] endPoint)
        {

            string jsonToSend = message.SerializeMessageToJson();
            _serverSocket.SendMoreFrame(endPoint).SendFrame(jsonToSend);
        }

        public async Task<BaseMessage> RecieveMessageAsync(CancellationToken ctoken)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(NetMqMessenger));

            var message = await _serverSocket.ReceiveFrameStringAsync(ctoken);
            return BaseMessage.DeserializeFromJson(message.Item1);
        }

        public byte[] GetServerEndPoint()
        {
            return System.Text.Encoding.UTF8.GetBytes("@tcp://127.0.0.1:12345");
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _serverSocket?.Dispose();
                _disposed = true;
            }
        }
    }
}