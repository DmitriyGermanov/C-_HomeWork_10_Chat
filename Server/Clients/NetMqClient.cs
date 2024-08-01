using Server.Clients.ClientsMenegement;
using Server.Messages;
using Server.ServerMessenger;

namespace Server.Clients
{
    public class NetMqClient : ClientBase
    {
        private byte[] _clientNetId;
        public byte[] ClientNetId
        {
            get { return _clientNetId; }
            set { _clientNetId = value; }
        }

        public override void Receive(BaseMessage message)
        {
            throw new NotImplementedException();
        }

        public override void Send(BaseMessage message, IClientMeneger mediator)
        {
            throw new NotImplementedException();
        }

        internal override Task SendToClientAsync<T>(ClientBase? client, BaseMessage message, IMessageSourceServer<T> ms)
        {
            throw new NotImplementedException();
        }
    }
}
