namespace ServerMessengerLibrary
{
    public interface IMessageSourceServer<T> : IDisposable
    {
        Task SendMessageAsync(BaseMessage message, T endpoint);
        Task<BaseMessage> RecieveMessageAsync(CancellationToken ctoken);
        T GetServerEndPoint();
    }
}
