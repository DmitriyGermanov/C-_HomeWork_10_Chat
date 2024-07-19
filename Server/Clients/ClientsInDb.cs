using Server.Messages;

namespace Server.Clients
{
    internal class ClientsInDb : ClientList
    {
        public void ClientRegistration(BaseMessage message)
        {
            bool flag = false;
            using (var ctx = new UdpServerContext())
            {

                foreach (var item in ctx.Clients)
                {
                    if (item.Name.Equals(message.NicknameFrom))
                    {
                        item.IsOnline = true;
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    ctx.Clients.Add(new ServerClient(this, base.Messenger) { Name = message.NicknameFrom, AskTime = DateTime.Now, IsOnline = true});
                }
            }
        }
    }
}

