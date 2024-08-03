namespace ServerMessagesLibrary
{
    public class DefaultMessage : BaseMessage
    {
        public DefaultMessage() {
            Ask = false;
            DisconnectRequest = false;
            UserDoesNotExist = false;
            UserIsOnline = true;
            DateTime = DateTime.Now;
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
