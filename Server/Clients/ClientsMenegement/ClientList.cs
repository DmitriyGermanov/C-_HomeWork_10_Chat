using Server.Clients;
using Server.Clients.ClientsMenegement;
using Server.Messages;
using Server.ServerMessenger;
using System.Net;

namespace Server.clients.clientsMenegement
{
    public class ClientList : IClientMeneger
    {
        //TODO: Для базы нужен будет синглтон
        //TODO: Переделываем на Dictionary<Client, Stack<Message> Логика: для каждого Client в Dictionary копим Stack Message, если клиент IsOnline держим Stack.Count = 0, если клиент IsOffline копим Stack (при смене статуса отдельным методом опустошаем Stack). Если статус Client IsOffline, то Server не делает Invoke и передает управление накопительному методу, если статус Client IsOnline, то делается Invoke => message поступает в Program, Client отправитель записывается в ClientFrom. Метод проверяет есть ли в Stack этого клиента message и освобождает Stack отправляя messages клиенту.
        private List<ClientBase> clients;
        private IMessageSourceServer<byte[]> messageSourceServer;

        public ClientList(IMessageSourceServer<byte[]> ms)
        {
            messageSourceServer = ms;
        }
        

        public virtual void ClientRegistration(BaseMessage message)
        {
            var client = clients.Find(client => client is IPEndPointClient<IPEndPoint> ipClient && ipClient.ClientEndPoint.Equals(message.LocalEndPoint)); if (client == null)
            {
                clients.Add(new IPEndPointClient<IPEndPoint>() { Name = message.NicknameFrom, ClientEndPoint = message.LocalEndPoint, AskTime = DateTime.Now, IsOnline = true });

            }
            else
            {
                if (!client.IsOnline)
                {
                    client.IsOnline = true;
                    client.AskTime = DateTime.Now;
                }
            }

        }
        public ClientBase? GetClientByName(string name) => clients.Find(client => client.Name.Equals(name));
        public ClientBase? GetClientByID(int? id) => clients.Find(client => client.ClientID.Equals(id));
        public ClientBase? GetClientByEndPoint(IPEndPoint clientEndPoint)
        {
            if (clientEndPoint != null )
                return clients.Find(client => client is IPEndPointClient<IPEndPoint> ipClient && ipClient.Equals(clientEndPoint));
            else
                return null;
        }
        public bool RemoveClientByEndPoint(ClientBase clientEndPoint)
        {
            var clientToRemove = clients.Find(client => client is IPEndPointClient<IPEndPoint> ipClient && ipClient.Equals(clientEndPoint));
            if (clientToRemove != null)
            {
                return clients.Remove(clientToRemove);
            }
            return false;
        }
        public void SetClientOffline(ClientBase client) => client.IsOnline = false;
        public void SetClientAskTime(ClientBase client, BaseMessage message) { }

        public  void Send(BaseMessage message, ClientBase client)
        {
            if (client != null)
            {
                clients.ForEach(thisClient =>
                {
                    if (thisClient != client)
                        thisClient.Receive(message, messageSourceServer, message.ClientNetId);
                });
            }
        }
    }
}
