using System.Net;

namespace Client.Messages.Fabric
{
    public class MessageCreatorUserIsOnlineCreator : BaseMessageFabric
    {
        public override BaseMessage FactoryMethod() => new UserIsOnlineMessage();
        public override BaseMessage FactoryMethod(IPEndPoint iPEndPoint) => new UserIsOnlineMessage(iPEndPoint);
    }
}
