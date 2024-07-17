using System.Net;

namespace Client.Messages.Fabric
{
    internal class MessageCreatorUserIsNotExist : BaseMessageFabric
    {
        public override BaseMessage FactoryMethod() => new UserDoesNotExistMessage();
        public override BaseMessage FactoryMethod(IPEndPoint iPEndPoint) => new UserDoesNotExistMessage(iPEndPoint);
    }
}
