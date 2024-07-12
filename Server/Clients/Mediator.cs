namespace Server.Clients
{
    abstract class Mediator
    {
        public abstract void Send(Message message, Client client);
    }
}
