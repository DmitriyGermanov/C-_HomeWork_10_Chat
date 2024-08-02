using NetMQ;
using NetMQ.Sockets;
using ClientMessengerLibrary;

namespace ClientMessengerNetMQLibrary
{

    internal class NetMqMessenger : IMessageSourceClient<byte[]>, IDisposable
    {
        private readonly DealerSocket _dealerSocket;
        private bool _disposed = false;
        private bool disposedValue;

        public NetMqMessenger()
        {
            _dealerSocket = new DealerSocket();
            _dealerSocket.Connect("tcp://localhost:12345");
        }

        public async Task SendMessageAsync(BaseMessage message)
        {
            string jsonToSend = message.SerializeMessageToJson();
            _dealerSocket.SendFrame(jsonToSend);
            await Task.CompletedTask;
        }


        public async Task<BaseMessage> RecieveMessageAsync(CancellationToken ctoken)
        {
            var message = _dealerSocket.ReceiveFrameString();
            return BaseMessage.DeserializeFromJson(message);

        }

        public byte[] GetServerEndPoint()
        {
            return System.Text.Encoding.UTF8.GetBytes("@tcp://127.0.0.1:12345");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _dealerSocket?.Dispose();
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