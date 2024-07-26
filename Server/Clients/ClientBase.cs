using Server.Clients.ClientsMenegement;
using Server.Messages;
using System.Net;

namespace Server.Clients
{
    public abstract class ClientBase
    {
        protected IClientMeneger mediator;
        public virtual IClientMeneger Mediator { get { return mediator; } set { } }
    
/*        public ClientBase(Mediator mediator)
        {
            this.mediator = mediator;
        }*/

        public ClientBase()
        {
        }

        public abstract void Send (BaseMessage message, IClientMeneger mediator);
        public abstract void Receive (BaseMessage message); 
    }
}
