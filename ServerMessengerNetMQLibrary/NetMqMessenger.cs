using NetMQ;
using NetMQ.Sockets;
using ServerMessengerLibrary;
using ServerMessengerLibrary.Messages;

namespace ServerMessengerNetMQLibrary
{
    public class NetMqMessenger : IMessageSourceServer<byte[]>, IDisposable
    {
        private readonly RouterSocket _routerSocket;
        private bool _disposed = false;
        private NetMQRuntime _runtime;
        private bool disposedValue1;
        private readonly Action<string> _messageHandler;


        public NetMqMessenger()
        {
            _routerSocket = new RouterSocket();
            _routerSocket.Bind("tcp://*:12345");

        }

        public async Task SendMessageAsync<T>(BaseMessage message, T endPoint)
        {

            string jsonToSend = message.SerializeMessageToJson();
            if (endPoint is byte[] bytes)
            _routerSocket.SendMoreFrame(bytes).SendFrame(jsonToSend);
            await Task.CompletedTask; 
        }

        public async Task<BaseMessage> RecieveMessageAsync(CancellationToken ctoken)
        {

            var clientId = _routerSocket.ReceiveFrameBytes();
            var df  = _routerSocket.ReceiveFrameString();
            var message = BaseMessage.DeserializeFromJson(df);
            message.ClientNetId = clientId;
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
                    _routerSocket?.Dispose();
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