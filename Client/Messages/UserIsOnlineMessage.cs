using ClientMessengerLibrary;
using System.Net;

namespace Client.Messages
{
    internal class UserIsOnlineMessage : BaseMessage
    {
        public UserIsOnlineMessage()
        {
            DisconnectRequest = true;
            Ask = true;
            Text = string.Empty;
            NicknameFrom = string.Empty;
            NicknameTo = string.Empty;
            DateTime = new DateTime();
            UserDoesNotExist = false;
            UserIsOnline = false;
        }
        public UserIsOnlineMessage(IPEndPoint iPEndPoint)
        {
            LocalEndPoint = iPEndPoint;
            DisconnectRequest = true;
            Ask = true;
            Text = string.Empty;
            NicknameFrom = string.Empty;
            NicknameTo = string.Empty;
            DateTime = new DateTime();
            UserDoesNotExist = false;
            UserIsOnline = false;
        }
    }
}
