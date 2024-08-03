
using ServerMessengerLibrary.Messages;

namespace ServerMessengerLibrary
{
    public interface IMessageSourceServer<T> : IDisposable
    {
        public abstract Task SendMessageAsync<T>(BaseMessage message, T endpoint);
        public Task<BaseMessage> RecieveMessageAsync(CancellationToken ctoken);
        public T GetServerEndPoint();

    }
}
