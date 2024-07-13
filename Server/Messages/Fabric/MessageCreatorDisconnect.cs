namespace Server.Messages.Fabric
{
    internal class MessageCreatorDisconnect : BaseMessageFabric
    {
        public override BaseMessage FactoryMethod() => new DisconnectMessage();
    }
}
