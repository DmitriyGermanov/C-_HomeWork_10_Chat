namespace Server.Messages.Fabric
{
    internal class MessageCreatorUserIsNotExistCreator : BaseMessageFabric
    {
        public override BaseMessage FactoryMethod() => new UserDoesNotExistMessage();
    }
}
