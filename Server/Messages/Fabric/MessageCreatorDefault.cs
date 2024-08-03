using ServerMessengerLibrary.Messages;

namespace Server.Messages.Fabric
{
    internal class MessageCreatorDefault : BaseMessageFabric
    {
        public override BaseMessage FactoryMethod() => new DefaultMessage();
        public BaseMessage FactoryMethodWIthText(string text) => new DefaultMessage(text);

    }
}

