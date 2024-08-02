using ClientMessengerLibrary;
using System.Net;

namespace Client.Messages.Fabric
{
    public abstract class BaseMessageFabric
    {
        public abstract BaseMessage FactoryMethod();
        public abstract BaseMessage FactoryMethod(IPEndPoint iPEndPoint);

    }
}
