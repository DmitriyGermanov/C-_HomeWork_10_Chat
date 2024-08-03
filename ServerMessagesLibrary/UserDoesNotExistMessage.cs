namespace ServerMessagesLibrary
{
    internal class UserDoesNotExistMessage : BaseMessage
    {
        public UserDoesNotExistMessage()
        {
            DisconnectRequest = true;
            Ask = true;
            Text = string.Empty;
            NicknameFrom = string.Empty;
            NicknameTo = string.Empty;
            DateTime = new DateTime();
            UserDoesNotExist = true;
            UserIsOnline = false;
        }
    }
}
