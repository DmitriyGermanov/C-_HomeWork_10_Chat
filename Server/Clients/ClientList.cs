using Server.Messages;
using System.Net;

namespace Server.Clients
{
    class ClientList : Mediator
    {
        //TODO: Для базы нужен будет синглтон
        //TODO: Переделываем на Dictionary<Client, Stack<Message> Логика: для каждого Client в Dictionary копим Stack Message, если клиент IsOnline держим Stack.Count = 0, если клиент IsOffline копим Stack (при смене статуса отдельным методом опустошаем Stack). Если статус Client IsOffline, то Server не делает Invoke и передает управление накопительному методу, если статус Client IsOnline, то делается Invoke => message поступает в Program, Client отправитель записывается в ClientFrom. Метод проверяет есть ли в Stack этого клиента message и освобождает Stack отправляя messages клиенту.
        public  Messenger Messenger;
   

        public ClientList()
        {
            Clients = new();
            Messenger = new Messenger();
        }

        public virtual void ClientRegistration(BaseMessage message, IPEndPoint clientEndPoint)
        {
            ServerClient client = Clients.Find(client => client.ClientEndPoint.Equals(clientEndPoint));
            if (client == null)
            {
                Clients.Add(new ServerClient(this, Messenger) { Name = message.NicknameFrom, ClientEndPoint = clientEndPoint, AskTime = DateTime.Now, IsOnline = true });

            } else
            {
                if (!client.IsOnline)
                {
                    client.IsOnline = true;
                    client.AskTime = DateTime.Now;
                }
            }

        }
        public ServerClient? GetClientByName(string name) => Clients.Find(client => client.Name.Equals(name));
        public ServerClient? GetClientByEndPoint(IPEndPoint clientEndPoint)
        {
            if (clientEndPoint != null)
                return Clients.Find(client => client.ClientEndPoint.Equals(clientEndPoint));
            else
                return null;
        }
        public bool RemoveClientByEndPoint(IPEndPoint clientEndPoint) => Clients.Remove(Clients.Find(client => client.ClientEndPoint.Equals(clientEndPoint)));
        public bool SetClientOffline(ServerClient client) => client.IsOnline = false;

        public override void Send(BaseMessage message, ServerClient client)
        {
            if (client != null)
            {
                Clients.ForEach(thisClient =>
                {
                    if (thisClient != client)
                        thisClient.Receive(message);
                });
            }
        }
    }
}
