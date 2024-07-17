namespace Client.Messages.Fabric
{
    internal class MessageCreatorUserIsOnlineCreator : BaseMessageFabric
    {
        public override BaseMessage FactoryMethod() => new UserIsOnlineMessage();
    }
}
