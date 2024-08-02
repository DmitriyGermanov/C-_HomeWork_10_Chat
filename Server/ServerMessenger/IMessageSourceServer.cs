using System.Net;
using Server.Messages;

namespace Server.ServerMessenger
{
    public interface IMessageSourceServer<T>: IDisposable
    {
        public abstract  Task SendMessageAsync<T>(BaseMessage message, T endpoint);
        public Task<BaseMessage> RecieveMessageAsync(CancellationToken ctoken);
        public T GetServerEndPoint();

    }
}
