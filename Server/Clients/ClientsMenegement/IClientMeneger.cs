using Server.Messages;

namespace Server.Clients.ClientsMenegement
{
    public interface IClientMeneger
    {
        public void Send(BaseMessage message, ServerClient client);
        public void ClientRegistration(BaseMessage message);
        public ServerClient GetClientByName(string name);
        public ServerClient GetClientByID(int? clientId);
        public void SetClientAskTime(ServerClient client, BaseMessage message);
        internal void SetClientOffline(ServerClient client);

    }
}
