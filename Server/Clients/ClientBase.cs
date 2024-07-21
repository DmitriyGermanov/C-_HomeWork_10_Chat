using Server.Messages;
using System.Net;

namespace Server.Clients
{
    abstract class ClientBase
    {
        protected Mediator mediator;
        public virtual Mediator Mediator { get { return mediator; } set { } }
    
/*        public ClientBase(Mediator mediator)
        {
            this.mediator = mediator;
        }*/

        public ClientBase()
        {
        }

        public abstract void Send (BaseMessage message, Mediator mediator);
        public abstract void Receive (BaseMessage message); 
    }
}
