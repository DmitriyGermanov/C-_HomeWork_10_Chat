using ClientMessengerLibrary;
using System.Net;

namespace Client.Messages.Fabric
{
    public class MessageCreatorDefault : BaseMessageFabric
    {
        public override BaseMessage FactoryMethod() => new DefaultMessage();
        public override BaseMessage FactoryMethod(IPEndPoint iPEndPoint) => new DefaultMessage(iPEndPoint);
        public BaseMessage FactoryMethodWIthText(string text) => new DefaultMessage(text);
    }
}
