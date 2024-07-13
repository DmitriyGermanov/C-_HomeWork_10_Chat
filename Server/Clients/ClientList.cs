using Server.Messages;
using System.Net;

namespace Server.Clients
{
    class ClientList : Mediator
    {
        //TODO: Для базы нужен будет синглтон
        //TODO: Переделываем на Dictionary<Client, Stack<Message> Логика: для каждого Client в Dictionary копим Stack Message, если клиент IsOnline держим Stack.Count = 0, если клиент IsOffline копим Stack (при смене статуса отдельным методом опустошаем Stack). Если статус Client IsOffline, то Server не делает Invoke и передает управление накопительному методу, если статус Client IsOnline, то делается Invoke => message поступает в Program, Client отправитель записывается в ClientFrom. Метод проверяет есть ли в Stack этого клиента message и освобождает Stack отправляя messages клиенту.
        private List<Client> clients;
        private Messenger messenger;
   

        public ClientList()
        {
            clients = new();
            messenger = new Messenger();
        }

        public List<Client> Clients => clients;
        public void ClientRegistration(BaseMessage message, IPEndPoint clientEndPoint)
        {
            Client client = clients.Find(client => client.ClientEndPoint.Equals(clientEndPoint));
            if (client == null)
            {
                clients.Add(new Client(this, messenger) { Name = message.NicknameFrom, ClientEndPoint = clientEndPoint, AskTime = DateTime.Now, IsOnline = true });

            } else
            {
                if (!client.IsOnline)
                {
                    client.IsOnline = true;
                    client.AskTime = DateTime.Now;
                }
            }

        }
        public Client? GetClientByName(string name) => clients.Find(client => client.Name.Equals(name));
        public Client? GetClientByEndPoint(IPEndPoint clientEndPoint) => clients.Find(client => client.ClientEndPoint.Equals(clientEndPoint));
        public bool RemoveClientByEndPoint(IPEndPoint clientEndPoint) => clients.Remove(clients.Find(client => client.ClientEndPoint.Equals(clientEndPoint)));
        public bool SetClientOffline(Client client) => client.IsOnline = false;

        public override void Send(BaseMessage message, Client client)
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
