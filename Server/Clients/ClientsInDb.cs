using Server.Messages;

namespace Server.Clients
{
    internal class ClientsInDb : ClientList
    {
        public void ClientRegistration(BaseMessage message)
        {
            using (var ctx = new UdpServerContext())
            {
                Console.WriteLine("1");
                var existingClient = ctx.Clients.FirstOrDefault(c => c.Name == message.NicknameFrom);
                if (existingClient != null)
                {
                    existingClient.IsOnline = true;
                }
                else
                {
                    Console.WriteLine("2");
                    ctx.Clients.Add(new ServerClient(this, base.Messenger) { Name = message.NicknameFrom, AskTime = DateTime.Now, IsOnline = true, IpEndPointToString = message.LocalEndPointString });
                    ctx.SaveChanges();
                }
            }
        }
    }
}

