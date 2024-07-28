using Client.Messages;
using System.Net.Sockets;

namespace Client.ClientMessenger
{
    public interface IMessageSourceClient
    {
        public Task SendMessageAsync(BaseMessage message);
        public Task<BaseMessage> RecieveMessageAsync(UdpClient udpClient, CancellationToken ctoken);

    }
}
