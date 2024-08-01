using Client.Messages;

namespace Client.ClientMessenger
{
    public interface IMessageSourceClient<T> : IDisposable
    {
        public abstract Task SendMessageAsync(BaseMessage message);
        public Task<BaseMessage> RecieveMessageAsync(CancellationToken ctoken);
        public T GetServerEndPoint();
    }
}
