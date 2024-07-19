using Server.Messages;
using System.Net;

namespace Server.Clients
{
    abstract class ClientBase
    {
        protected Mediator mediator;
        public virtual Mediator Mediator { get { return mediator; } }
    
        public ClientBase(Mediator mediator)
        {
            this.mediator = mediator;
        }

        public ClientBase()
        {
            mediator = new ClientList();
        }

        public abstract void Send (BaseMessage message);
        public abstract void Receive (BaseMessage message); 
    }
}
