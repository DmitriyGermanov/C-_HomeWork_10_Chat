namespace Client.Messages.Fabric
{
    internal class MessageCreatorUserIsNotExist : BaseMessageFabric
    {
        public override BaseMessage FactoryMethod() => new UserIsOnlineMessage();
    }
}
