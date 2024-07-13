namespace Server.Messages.Fabric
{
    internal class MessageCreatorDefault : BaseMessageFabric
    {
        public override BaseMessage FactoryMethod() => new DefaultMessage();
    }
}
