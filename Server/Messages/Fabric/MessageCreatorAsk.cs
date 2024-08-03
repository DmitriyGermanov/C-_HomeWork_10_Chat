using ServerMessengerLibrary.Messages;

namespace Server.Messages.Fabric
{
    internal class MessageCreatorAsk : BaseMessageFabric
    {
        public override BaseMessage FactoryMethod() => new AskMessage();
    }
}
