using Server.Messages;

namespace Server.Clients
{
    abstract class Mediator
    {
        public abstract void Send(BaseMessage message, ServerClient client);
        public  List<ServerClient> Clients { get; set; }
        public int MediatorID { get; set; }
    }
}
