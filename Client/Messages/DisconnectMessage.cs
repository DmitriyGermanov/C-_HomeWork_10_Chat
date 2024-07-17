namespace Client.Messages
{
    internal class DisconnectMessage : BaseMessage
    {
        public DisconnectMessage()
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
    }
}
