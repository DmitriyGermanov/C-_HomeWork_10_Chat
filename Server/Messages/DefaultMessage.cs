namespace Server.Messages
{
    internal class DefaultMessage : BaseMessage
    {
        public DefaultMessage() {
            Ask = false;
            DisconnectRequest = false;
        }
    }
}
