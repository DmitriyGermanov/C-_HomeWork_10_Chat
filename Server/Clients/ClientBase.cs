using Server.Messages;

namespace Server.Clients
{
    abstract class ClientBase
    {
        protected Mediator mediator;
        public ClientBase(Mediator mediator)
        {
            this.mediator = mediator;
        }
        public abstract void Send (BaseMessage message);
        public abstract void Receive (BaseMessage message); 
    }
}
