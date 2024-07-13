using Server.Messages;

namespace Server.Clients
{
    abstract class Mediator
    {
        public abstract void Send(BaseMessage message, Client client);
    }
}
