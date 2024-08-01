using Server.Clients;
using Server.ServerMessenger;

namespace Server.Messages.MesagesMenegement
{
    public interface IMessagesMenegement
    {
        Task ShowUnrecievedMessagesAsync<T>(ClientBase serverClient, IMessageSourceServer<T> ms);
    }
}
