namespace Server.Messages
{
    internal class DefaultMessage : BaseMessage
    {
        public DefaultMessage() {
            Ask = false;
            DisconnectRequest = false;
            UserDoesNotExist = false;
            UserIsOnline = true;
        }

        public DefaultMessage(string text)
        {
            Text = text;
            Ask = false;
            DisconnectRequest = false;
            UserDoesNotExist = false;
            UserIsOnline = true;
            DateTime = DateTime.Now;
        }
    }
}
