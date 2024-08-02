namespace ClientMessengerLibrary
{
    public interface IMessageSourceClient<T> : IDisposable
    {
        public abstract Task SendMessageAsync(BaseMessage message);
        public Task<BaseMessage> RecieveMessageAsync(CancellationToken ctoken);
        public T GetServerEndPoint();
    }
}
