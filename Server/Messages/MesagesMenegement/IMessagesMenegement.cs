using ServerMessengerLibrary;
using ServerMessengerLibrary.Clients;

namespace Server.Messages.MesagesMenegement
{
    public interface IMessagesMenegement
    {
        Task ShowUnrecievedMessagesAsync<T>(ClientBase serverClient, IMessageSourceServer<T> ms);
    }
}
