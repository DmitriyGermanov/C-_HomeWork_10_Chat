﻿using ServerMessengerLibrary.Messages;

namespace Server.Messages.Fabric
{
    abstract class BaseMessageFabric
    {
        public abstract BaseMessage FactoryMethod();
    }
}
