using System.Net;

namespace Client.Messages
{
    internal class AskMessage : BaseMessage
    {

        public AskMessage()
        {
            Ask = true;
            DisconnectRequest = false;
            Text = string.Empty;
            NicknameFrom = string.Empty;
            NicknameTo = string.Empty;
            DateTime = new DateTime();
        }
        public AskMessage(IPEndPoint iPEndPoint)
        {
            LocalEndPoint = iPEndPoint;
            Ask = true;
            DisconnectRequest = false;
            Text = string.Empty;
            NicknameFrom = string.Empty;
            NicknameTo = string.Empty;
            DateTime = new DateTime();
        }

    }
}
