using Server.Clients;
using Server.ServerMessenger;

namespace Server.Messages.MesagesMenegement
{
    public interface IMessagesMenegement
    {
        Task ShowUnrecievedMessagesAsync<T>(IPEndPointClient serverClient, IMessageSourceServer<T> ms);
    }
}
