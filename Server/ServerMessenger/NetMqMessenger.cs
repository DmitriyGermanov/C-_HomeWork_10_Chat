using NetMQ;
using NetMQ.Sockets;
using Server.Messages;

namespace Server.ServerMessenger
{
    internal class NetMqMessenger : IMessageSourceServer<byte[]>
    {
        private readonly ResponseSocket _serverSocket;

        public NetMqMessenger()
        {
            _serverSocket = new ResponseSocket("@tcp://127.0.0.1:123456");
        }

        public static Task SendMessageAsync(BaseMessage message, byte[] endPoint)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public byte[] GetServerEndPoint()
        {
            throw new NotImplementedException();
        }

        public async Task<BaseMessage> RecieveMessageAsync(CancellationToken ctoken)
        {
            var message = await _serverSocket.ReceiveFrameStringAsync(ctoken);
            return BaseMessage.DeserializeFromJson(message.Item1);
        }
    }
}
