using System.Net;

namespace Client.Messages.Fabric
{
    internal class MessageCreatorDisconnect : BaseMessageFabric
    {
        public override BaseMessage FactoryMethod(IPEndPoint iPEndPoint) => new DisconnectMessage(iPEndPoint);

        public override BaseMessage FactoryMethod() => new DisconnectMessage();
    }
}
