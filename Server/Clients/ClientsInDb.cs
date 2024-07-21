using Org.BouncyCastle.Tls;
using Server.Messages;

namespace Server.Clients
{
    internal class ClientsInDb : ClientList
    {
        public void ClientRegistrationInDb(BaseMessage message)
        {
            using (var ctx = new UdpServerContext())
            {
                var existingClient = ctx.Clients.FirstOrDefault(c => c.Name.Equals(message.NicknameFrom));
                if (existingClient != null)
                {
                    existingClient.IsOnline = true;
                    existingClient.AskTime = DateTime.Now;
                    existingClient.IpEndPointToString = message.LocalEndPointString;
                    ctx.SaveChanges();
                }
                else
                {
                    ServerClient client = new ServerClient
                    {
                        Name = message.NicknameFrom,
                        AskTime = DateTime.Now,
                        IsOnline = true,
                        IpEndPointToString = message.LocalEndPointString,

                    };
                    ctx.Add(client);
                    ctx.SaveChanges();
                }
            }
        }

        public ServerClient GetClientByNameFromDb(string name)
        {
            using (var ctx = new UdpServerContext())
            {
                return ctx.Clients.FirstOrDefault(c => c.Name.Equals(name));
            }
        }

        public void SetClientAskTimeInDb(ServerClient client, BaseMessage message)
        {
            Console.WriteLine("Ура, сработал");
            using (var ctx = new UdpServerContext())
            {

                if (client != null)
                {
                    ctx.Attach(client);
                    client.IsOnline = true;
                    client.AskTime = DateTime.Now;
                    client.ClientEndPoint = message.LocalEndPoint;
                    ctx.SaveChanges();
                }
                else
                {
                    throw new Exception("Ошибка! Клиент не найден методом SetClientAskTimeInDb, проверьте логику работы");
                }
            }
        }

        internal void SetClientOfflineInDb(ServerClient client)
        {
            using (var ctx = new UdpServerContext())
            {
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
        }
        public override void Send(BaseMessage message, ServerClient client)
        {
            Console.WriteLine("Uses Send to all!");
            using (var ctx = new UdpServerContext())
            {
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
}
