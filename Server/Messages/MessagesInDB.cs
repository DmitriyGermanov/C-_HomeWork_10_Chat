
using MySqlX.XDevAPI;
using Server.Clients;
using Server.Messages.Fabric;

namespace Server.Messages
{
    internal class MessagesInDB
    {
        private ClientsInDb clientsInDb;

        public MessagesInDB(ClientsInDb clientsInDb)
        {
            this.clientsInDb = clientsInDb;
        }

        internal static void SaveMessageToDb(BaseMessage baseMessage)
        {
            using (var ctx = new UdpServerContext())
            {

                try
                {
                    ctx.Attach(baseMessage.ClientFrom);
                    ctx.Attach(baseMessage.ClientTo);
                    ctx.Add(baseMessage);
                    ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                }
        }

        internal async Task ShowUnrecievedMessagesAsync(ServerClient serverClient)
        {
            Console.WriteLine("Сработал ShowUnrecievedMessages");
            using (var ctx = new UdpServerContext())
            {
                try
                {
                    var messages = ctx.Messages.Where(message => message.UserIdTo == serverClient.ClientID).ToList();
                    if (messages.Count > 0)
                    {

                        foreach (var message in messages)
                        {
                            message.NicknameFrom = (clientsInDb.GetClientByIdFromDb(message.UserIDFrom)).Name;
                            Console.WriteLine(message);
                            // Отправка каждого сообщения
                            await serverClient.SendToClientAsync(serverClient, message);
                        }
                        await serverClient.SendToClientAsync(serverClient, new MessageCreatorDefault().FactoryMethodWIthText($"У вас {messages.Count} непрочитанных сообщений:"));

                        ctx.Messages.RemoveRange(messages);
                        await ctx.SaveChangesAsync();
                }
                catch
                {
                    throw new Exception("Не удалось распечатать пропущенные сообщения");
                }
            }
        }
    }
}
