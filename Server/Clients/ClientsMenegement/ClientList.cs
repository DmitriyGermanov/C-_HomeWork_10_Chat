using Server.Clients;
using Server.Clients.ClientsMenegement;
using Server.Messages;
using System.Net;

namespace Server.clients.clientsMenegement
{
    public class ClientList : IClientMeneger
    {
        //TODO: Для базы нужен будет синглтон
        //TODO: Переделываем на Dictionary<Client, Stack<Message> Логика: для каждого Client в Dictionary копим Stack Message, если клиент IsOnline держим Stack.Count = 0, если клиент IsOffline копим Stack (при смене статуса отдельным методом опустошаем Stack). Если статус Client IsOffline, то Server не делает Invoke и передает управление накопительному методу, если статус Client IsOnline, то делается Invoke => message поступает в Program, Client отправитель записывается в ClientFrom. Метод проверяет есть ли в Stack этого клиента message и освобождает Stack отправляя messages клиенту.
        private List<IPEndPointClient> clients;

        public ClientList()
        {
            this.clients = new();
        }

        public virtual void ClientRegistration(BaseMessage message)
        {
            IPEndPointClient client = clients.Find(client => client.ClientEndPoint.Equals(message.LocalEndPoint));
            if (client == null)
            {
                clients.Add(new IPEndPointClient() { Name = message.NicknameFrom, ClientEndPoint = message.LocalEndPoint, AskTime = DateTime.Now, IsOnline = true });

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
        public IPEndPointClient? GetClientByName(string name) => clients.Find(client => client.Name.Equals(name));
        public IPEndPointClient? GetClientByID(int? id) => clients.Find(client => client.ClientID.Equals(id));
        public IPEndPointClient? GetClientByEndPoint(IPEndPoint clientEndPoint)
        {
            if (clientEndPoint != null)
                return clients.Find(client => client.ClientEndPoint.Equals(clientEndPoint));
            else
                return null;
        }
        public bool RemoveClientByEndPoint(IPEndPoint clientEndPoint) => clients.Remove(clients.Find(client => client.ClientEndPoint.Equals(clientEndPoint)));
        public void SetClientOffline(IPEndPointClient client) => client.IsOnline = false;
        public void SetClientAskTime(IPEndPointClient client, BaseMessage message) { }

        public  void Send(BaseMessage message, IPEndPointClient client)
        {
            if (client != null)
            {
                clients.ForEach(thisClient =>
                {
                    if (thisClient != client)
                        thisClient.Receive(message);
                });
            }
        }
    }
}
