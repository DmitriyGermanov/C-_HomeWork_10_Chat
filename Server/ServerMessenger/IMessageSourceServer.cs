using System.Net;
using Server.Messages;

namespace Server.ServerMessenger
{
    public interface IMessageSourceServer<T>: IDisposable
    {
        public abstract static Task SendMessageAsync(BaseMessage message, T endPoint);
        public Task<BaseMessage> RecieveMessageAsync(CancellationToken ctoken);
        public T GetServerEndPoint();

    }
}
