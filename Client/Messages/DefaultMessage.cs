﻿namespace Client.Messages
{
    internal class DefaultMessage : BaseMessage
    {
        public DefaultMessage() {
            Ask = false;
            DisconnectRequest = false;
            UserDoesNotExist = false;
            UserIsOnline = true;
        }
    }
}
