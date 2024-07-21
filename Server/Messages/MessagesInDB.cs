
using MySqlX.XDevAPI;
using Server.Clients;

namespace Server.Messages
{
    internal class MessagesInDB
    {
        internal void SaveMessageToDb(BaseMessage baseMessage)
        {
            using (var ctx = new UdpServerContext())
            {
                ctx.Add(baseMessage);
                try
                {
                    ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                }
        }

        internal void ShowUnrecievedMessages(ServerClient serverClient)
        {
            throw new NotImplementedException();
        }
    }
}
