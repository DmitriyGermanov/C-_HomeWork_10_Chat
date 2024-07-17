namespace Client.Messages.Fabric
{
    internal class MessageCreatorDisconnect : BaseMessageFabric
    {
        public override BaseMessage FactoryMethod() => new DisconnectMessage();
    }
}
