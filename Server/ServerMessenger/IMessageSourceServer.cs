using System.Net;
using Server.Messages;

namespace Server.ServerMessenger
{
    public interface IMessageSourceServer: IDisposable
    {
        public abstract static Task SendMessageAsync(BaseMessage message, IPEndPoint endPoint);
        public Task<BaseMessage> RecieveMessageAsync(CancellationToken ctoken);
        public IPEndPoint GetServerEndPoint();

    }
}
