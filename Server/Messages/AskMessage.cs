namespace Server.Messages
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
            UserDoesNotExist = false;
            UserIsOnline = false;
        }

    }
}
