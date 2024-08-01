using NetMQ;
using NetMQ.Sockets;
using Server.Messages;

namespace Server.ServerMessenger
{
    internal class NetMqMessenger : IMessageSourceServer<byte[]>, IDisposable
    {
        private readonly PublisherSocket _publisherSocket;
        private bool _disposed = false;
        private NetMQRuntime _runtime;
        private bool disposedValue;
        private bool disposedValue1;

        public NetMqMessenger()
        {
            _publisherSocket = new PublisherSocket();
            _publisherSocket.Bind("tcp://*:12345");

        }

        public async Task SendMessageAsync(BaseMessage message, byte[] endPoint)
        {

            string jsonToSend = message.SerializeMessageToJson();
            _publisherSocket.SendMoreFrame(endPoint).SendFrame(jsonToSend);
        }

        public async Task<BaseMessage> RecieveMessageAsync(CancellationToken ctoken)
        {
            string incomingFrame = String.Empty;
            if (_disposed)
                throw new ObjectDisposedException(nameof(NetMqMessenger));
            try
            {
                incomingFrame = _publisherSocket.ReceiveFrameString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            var message = BaseMessage.DeserializeFromJson(incomingFrame);
            return message;
        }

        public byte[] GetServerEndPoint()
        {
            return System.Text.Encoding.UTF8.GetBytes("@tcp://127.0.0.1:12345");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue1)
            {
                if (disposing)
                {
                    _publisherSocket?.Dispose();
                }


                disposedValue1 = true;
            }
        }

     

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}