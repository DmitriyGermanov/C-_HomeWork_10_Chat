using System.Net;

namespace Client.Messages
{
    internal class DefaultMessage : BaseMessage
    {
        public DefaultMessage() {
            Ask = false;
            DisconnectRequest = false;
            UserDoesNotExist = false;
            UserIsOnline = true;
        }
        public DefaultMessage(IPEndPoint iPEndPoint)
        {
            LocalEndPoint = iPEndPoint;
            Ask = false;
            DisconnectRequest = false;
            UserDoesNotExist = false;
            UserIsOnline = true;
        }
    }
}
