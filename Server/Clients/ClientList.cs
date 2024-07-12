using System.Net;

namespace Server.Clients
{
    class ClientList : Mediator
    {
        //TODO: Для базы нужен будет синглтон
        private List<Client> clients;
        private Messenger messenger;

        public ClientList()
        {
            clients = new();
            messenger = new Messenger();
        }

        public List<Client> Clients => clients;
        public void ClientRegistration(Message message, IPEndPoint clientEndPoint)
        {
            if (clients.Find(client => client.ClientEndPoint.Equals(clientEndPoint)) == null)
            {
                clients.Add(new Client(this, messenger) { Name = message.NicknameFrom, ClientEndPoint = clientEndPoint, AskTime = DateTime.Now });
            }

        }
        public Client? GetClientByName(string name) => clients.Find(client => client.Name.Equals(name));
        public Client? GetClientByEndPoint(IPEndPoint clientEndPoint) => clients.Find(client => client.ClientEndPoint.Equals(clientEndPoint));
        public bool RemoveClientByEndPoint(IPEndPoint clientEndPoint) => clients.Remove(clients.Find(client => client.ClientEndPoint.Equals(clientEndPoint)));

        public override void Send(Message message, Client client)
        {
            if (client != null)
            {
                Console.WriteLine("Зашли");
                clients.ForEach(thisClient =>
                {
                    if (thisClient != client)
                        thisClient.Receive(message);
                });
           }
        }
    }
}
