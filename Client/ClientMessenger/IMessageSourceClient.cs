using Client.Messages;
using System.Net;
using System.Net.Sockets;

namespace Client.ClientMessenger
{
    public interface IMessageSourceClient : IDisposable
    {
        public Task SendMessageAsync(BaseMessage message);
        public Task<BaseMessage> RecieveMessageAsync(CancellationToken ctoken);
        public IPEndPoint GetServerEndPoint();

    }
}
