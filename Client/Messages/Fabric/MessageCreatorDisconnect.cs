﻿using ClientMessengerLibrary;
using System.Net;

namespace Client.Messages.Fabric
{
    public class MessageCreatorDisconnect : BaseMessageFabric
    {
        public override BaseMessage FactoryMethod(IPEndPoint iPEndPoint) => new DisconnectMessage(iPEndPoint);

        public override BaseMessage FactoryMethod() => new DisconnectMessage();
    }
}
