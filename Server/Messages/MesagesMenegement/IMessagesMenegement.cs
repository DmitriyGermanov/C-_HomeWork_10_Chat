using Server.Clients;

namespace Server.Messages.MesagesMenegement
{
    public interface IMessagesMenegement
    {
        Task ShowUnrecievedMessagesAsync(ServerClient serverClient);
    }
}
