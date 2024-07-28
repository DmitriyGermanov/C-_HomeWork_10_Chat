using Client.Messages;

namespace Client.ClientMessenger
{
    public interface IMessageSourceClient
    {
        public Task SendMessageAsync(BaseMessage message);
    }
}
