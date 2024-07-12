namespace Server.Clients
{
    abstract class ClientBase
    {
        protected Mediator mediator;
        public ClientBase(Mediator mediator)
        {
            this.mediator = mediator;
        }
        public abstract void Send (Message message);
        public abstract void Receive (Message message); 
    }
}
