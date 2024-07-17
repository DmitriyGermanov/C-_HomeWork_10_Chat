using System.Net;

namespace Client.Messages.Fabric
{
    internal class MessageCreatorAsk : BaseMessageFabric
    {
        public override BaseMessage FactoryMethod() => new AskMessage();
        public override BaseMessage FactoryMethod(IPEndPoint iPEndPoint) => new AskMessage(iPEndPoint);
    }
}
