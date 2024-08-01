using Server.Messages;
namespace Server.Clients.ClientsMenegement
{
    public class ClientsInDb : IClientMeneger
    {
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
                else
                {
                    //Добавить реализацию для NetMQ
                }
                ctx.SaveChanges();
            }
            else
            {
                //добавляем логику, что опр тип клиента добавляется, если выбран определенный тип контекста.
                if (ctx is UdpServerContext)
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
                    //Добавить реализацию для NetMQ
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
                else
                {
                    //Добавить реализацию для NetMQ
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
                        item.Receive(message);
                    }
                }
            }
        }
    }
}
