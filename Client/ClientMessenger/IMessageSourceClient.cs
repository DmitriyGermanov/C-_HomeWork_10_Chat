using Client.Messages;
using System.Net;
using System.Net.Sockets;

namespace Client.ClientMessenger
{
    public interface IMessageSourceClient<T> : IDisposable
    {
        public Task SendMessageAsync(BaseMessage message);
        public Task<BaseMessage> RecieveMessageAsync(CancellationToken ctoken);
        public T GetServerEndPoint();
    }
}
