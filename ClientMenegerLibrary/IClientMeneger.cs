﻿using Server.Messages;

namespace ServerClientMenegerLibrary
{
    public interface IClientMeneger
    {
        public void Send(BaseMessage message, ClientBase client);
        public void ClientRegistration(BaseMessage message);
        public ClientBase GetClientByName(string name);
        public ClientBase GetClientByID(int? clientId);
        public void SetClientAskTime(ClientBase client, BaseMessage message);
        internal void SetClientOffline(ClientBase client);

    }
}
