using ServerMessengerLibrary.Clients;
using ServerMessengerLibrary.Messages;
using System.Net;
namespace ServerMessengerLibrary.ClientsMenegement
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
            using var ctx = new MainServerContext();
            var existingClient = ctx.Clients.FirstOrDefault(c => c.Name.Equals(message.NicknameFrom));
            if (existingClient != null)
            {
                existingClient.IsOnline = true;
                existingClient.AskTime = DateTime.Now;
                if (existingClient is IPEndPointClient<IPEndPoint> client)
                    client.IpEndPointToString = message.LocalEndPointString;
                if (existingClient is NetMqClient<byte[]> clientMQ)
                    clientMQ.ClientNetId = message.ClientNetId;
                ctx.SaveChanges();
            }
            else
            {
                if (message.LocalEndPoint != null)
                {
                    IPEndPointClient<IPEndPoint> client = new IPEndPointClient<IPEndPoint>
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
                    NetMqClient<byte[]> client = new NetMqClient<byte[]>
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

        public ClientBase? GetClientByName(string name)
        {
            using var ctx = new MainServerContext();
            var client = ctx.Clients.FirstOrDefault(c => c.Name.Equals(name));
            if (client == null)
            {
                return null;
            }
            else
            {
                return client;
            }
        }
        public ClientBase GetClientByID(int? clientId)
        {
            using var ctx = new MainServerContext();
            return ctx.Clients.FirstOrDefault(c => c.ClientID == clientId);
        }

        public void SetClientAskTime(ClientBase client, BaseMessage message)
        {
            Console.WriteLine("сработал SetClientAskTimeInDb");
            using var ctx = new MainServerContext();
            if (client != null)
            {
                ctx.Attach(client);
                client.IsOnline = true;
                client.AskTime = DateTime.Now;
                if (client is IPEndPointClient<IPEndPoint> ipClient)
                {
                    ipClient.IpEndPointToString = message.LocalEndPointString;
                }
                else if (client is NetMqClient<byte[]> netMqClient)
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
            using var ctx = new MainServerContext();
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
            using var ctx = new MainServerContext();
            if (client != null)
            {
                foreach (var item in ctx.Clients)
                {
                    if (!item.Name.Equals(client.Name) && item.IsOnline)
                    {
                        if (item is NetMqClient<byte[]> clientNetMQ)
                        item.Receive(message, _messageSourceServer, clientNetMQ.ClientNetId);
                    }
                }
            }
        }
    }
}
