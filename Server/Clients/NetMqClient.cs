using MySqlX.XDevAPI;
using Server.Clients.ClientsMenegement;
using Server.Messages;
using Server.ServerMessenger;

namespace Server.Clients
{
    public class NetMqClient : ClientBase
    {
        private byte[] _clientNetId;
        public virtual byte[] ClientNetId
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

        internal override Task SendToClientAsync<T>(ClientBase? client, BaseMessage message, IMessageSourceServer<T> ms)
        {
            throw new NotImplementedException();
        }
    }
}
