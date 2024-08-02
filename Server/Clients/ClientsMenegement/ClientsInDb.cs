using Server.Messages;
using Server.ServerMessenger;
namespace Server.Clients.ClientsMenegement
{
    public class ClientsInDb : IClientMeneger
    {
        private IMessageSourceServer<byte[]> _messageSourceServer;
        public ClientsInDb(IMessageSourceServer<byte[]> messageSourceServer)
        {
            _messageSourceServer = messageSourceServer;
        }

        //to-do: заменить возвращаемые типы на clientbase
        public void ClientRegistration(BaseMessage message)
        {
            Console.WriteLine("сработал ClientReg");
            using var ctx = new UdpServerContext();
            var existingClient = ctx.Clients.FirstOrDefault(c => c.Name.Equals(message.NicknameFrom));
            if (existingClient != null)
            {
                existingClient.IsOnline = true;
                existingClient.AskTime = DateTime.Now;
                if (existingClient is IPEndPointClient client)
                    client.IpEndPointToString = message.LocalEndPointString;
                ctx.SaveChanges();
            }
            else
            {
                if (message.LocalEndPoint != null)
                {
                    IPEndPointClient client = new IPEndPointClient
                    {
                        Name = message.NicknameFrom,
                        AskTime = DateTime.Now,
                        IsOnline = true,
                        IpEndPointToString = message.LocalEndPointString,

                    };

                    ctx.Add(client);
                    ctx.SaveChanges();
                }
                else
                {
                    NetMqClient client = new NetMqClient
                    {
                        Name = message.NicknameFrom,
                        AskTime = DateTime.Now,
                        IsOnline = true,
                        ClientNetId = message.ClientNetId,
                    };
                    ctx.Add(client);
                    ctx.SaveChanges();
                }
            }
        }

        public ClientBase GetClientByName(string name)
        {
            using var ctx = new UdpServerContext();
            return ctx.Clients.FirstOrDefault(c => c.Name.Equals(name));
        }
        public ClientBase GetClientByID(int? clientId)
        {
            using var ctx = new UdpServerContext();
            return ctx.Clients.FirstOrDefault(c => c.ClientID == clientId);
        }

        public void SetClientAskTime(ClientBase client, BaseMessage message)
        {
            Console.WriteLine("сработал SetClientAskTimeInDb");
            using var ctx = new UdpServerContext();
            if (client != null)
            {
                ctx.Attach(client);
                client.IsOnline = true;
                client.AskTime = DateTime.Now;
                if (client is IPEndPointClient ipClient)
                {
                    ipClient.IpEndPointToString = message.LocalEndPointString;
                }
                else if (client is NetMqClient netMqClient)
                {
                    netMqClient.ClientNetId = message.ClientNetId;
                }
                ctx.SaveChanges();
            }
            else
            {
                throw new Exception("Ошибка! Клиент не найден методом SetClientAskTimeInDb, проверьте логику работы");
            }
        }

        public void SetClientOffline(ClientBase client)
        {
            Console.WriteLine("сработал SetClientOfflineInDb");
            using var ctx = new UdpServerContext();
            if (client != null)
            {
                ctx.Attach(client);
                client.IsOnline = false;
                ctx.SaveChanges();
            }
            else
            {
                throw new Exception("Ошибка! Клиент не найден методом SetClientOfflineInDb, проверьте логику работы");
            }
        }
        public void Send(BaseMessage message, ClientBase client)
        {
            Console.WriteLine("сработал SendToAll");
            using var ctx = new UdpServerContext();
            if (client != null)
            {
                foreach (var item in ctx.Clients)
                {
                    if (!item.Name.Equals(client.Name) && item.IsOnline)
                    {
                        if (item is NetMqClient clientNetMQ)
                        item.Receive(message, _messageSourceServer, clientNetMQ.ClientNetId);
                    }
                }
            }
        }
    }
}
