using System.Net;

namespace Client.Messages.Fabric
{
    abstract class BaseMessageFabric
    {
        public abstract BaseMessage FactoryMethod();
        public abstract BaseMessage FactoryMethod(IPEndPoint iPEndPoint);

    }
}
