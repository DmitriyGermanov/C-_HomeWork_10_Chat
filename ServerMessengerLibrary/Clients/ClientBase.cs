using System.Net;
using System.Text.Json.Serialization;
using ServerMessengerLibrary.ClientsMenegement;
using ServerMessengerLibrary.Messages;

namespace ServerMessengerLibrary.Clients
{
    public abstract class ClientBase
    {
        private IClientMeneger mediator;
        private int clientID;
        private string name;
        private DateTime askTime;
        private bool isOnline;
        [JsonIgnore]
        public virtual ICollection<BaseMessage> MessagesTo { get; set; }
        [JsonIgnore]
        public virtual ICollection<BaseMessage> MessagesFrom { get; set; }
        public virtual IClientMeneger Mediator { get { return mediator; } set { } }
        public bool IsOnline { get { return isOnline; } set { isOnline = value; } }
        public virtual int ClientID
        {
            get { return clientID; }
            set { clientID = value; }
        }
        public virtual DateTime AskTime
        {
            get { return askTime; }
            set { askTime = value; }
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
        /*        public ClientBase(Mediator mediator)
                {
                    this.mediator = mediator;
                }*/

        public ClientBase()
        {
        }

        public abstract void Send(BaseMessage message, IClientMeneger mediator);
        public abstract void Receive<T>(BaseMessage message, IMessageSourceServer<T> ms, T endpoint);
        public abstract Task SendToClientAsync<T>(ClientBase? client, BaseMessage message, IMessageSourceServer<T> ms);
    }
}
