
using ServerMessengerLibrary.ClientsMenegement;
using ServerMessengerLibrary.Messages;

namespace ServerMessengerLibrary.Clients
{
    public class NetMqClient<T> : ClientBase
    {
        private T _clientNetId;
        public virtual T ClientNetId
        {
            get { return _clientNetId; }
            set { _clientNetId = value; }
        }

        public override void Receive<T>(BaseMessage message, IMessageSourceServer<T> ms, T endpoint)
        {
            ms.SendMessageAsync(message, endpoint);
        }

        public override void Send(BaseMessage message, IClientMeneger mediator)
        {
            mediator.Send(message, this);
        }

        internal override async Task SendToClientAsync<T>(ClientBase? client, BaseMessage message, IMessageSourceServer<T> ms)
        {
            if (client is NetMqClient<byte[]> clientMqClient) {
               await ms.SendMessageAsync(message, clientMqClient.ClientNetId);
            }
        }
    }
}
