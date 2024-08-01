using Server.Messages;

namespace Server.Clients.ClientsMenegement
{
    public interface IClientMeneger
    {
        public void Send(BaseMessage message, IPEndPointClient client);
        public void ClientRegistration(BaseMessage message);
        public IPEndPointClient GetClientByName(string name);
        public IPEndPointClient GetClientByID(int? clientId);
        public void SetClientAskTime(IPEndPointClient client, BaseMessage message);
        internal void SetClientOffline(IPEndPointClient client);

    }
}
