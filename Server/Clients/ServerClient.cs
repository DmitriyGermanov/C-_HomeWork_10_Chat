using Server.Messages;
using System.Net;
using System.Runtime.CompilerServices;
/*GabilyAslanov: Мы собираемся сделать наш класс полностью клиент-серверным с возможностью отправки данных сразу нескольким клиентам. Доработаем наш код следующим образом. Представьте что наш сервер умеет работать как медиатор (умеет отправлять сообщения по имени клиента), а также умеет возвращать список всех подключенных к нему клиентов. Для этого доработаем наш класс сообщений добавив поле ToName.
GabilyAslanov: Доработаем систему команд. Имя пользователя сервера всегда будет Server. Если сервер получает команду (как текст сообщения):
register: то он добавляет клиента в свой список.
delete: он удаляет клиента из списка
если сервер не получает имени получателя то он отправляет сообщение всем клиентам
если сервер получает имя получателя то он отправляет сообщение одному конкретному клиенту. Код сервера должен выглядеть примерно следующим образом:*/
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Server.Clients
{
    internal class ServerClient : ClientBase
    {

       
        public ServerClient()
        { }

        private int clientID;
        private string name;
        private IPEndPoint clientEndPoint;
        private DateTime askTime;
        private bool isOnline;
        public virtual ICollection<BaseMessage> MessagesTo { get; set; }
        public virtual ICollection<BaseMessage> MessagesFrom { get; set; }
        public bool IsOnline { get { return isOnline; } set { isOnline = value; } }
        public virtual int ClientID
        {
            get { return clientID; }
            set { clientID = value; }
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual IPEndPoint ClientEndPoint
        {
            get { return clientEndPoint; }
            set { clientEndPoint = value; }
        }

        public virtual DateTime AskTime
        {
            get { return askTime; }
            set { askTime = value; }
        }



        public virtual string IpEndPointToString
        {
            get { return clientEndPoint.ToString(); }
            set
            {
                try
                {
                    clientEndPoint = IPEndPoint.Parse(value);
                }
                catch
                {
                    throw new Exception("Ошибка преобразования");
                }
            }
        }
        public override void Receive(BaseMessage message)
        {
            Task.Run(() =>
            {
                new Messenger().AnswerSender(message, ClientEndPoint);
            });
        }

        public override void Send(BaseMessage message, Mediator mediator)
        {
            mediator.Send(message, this);
        }

        public override string? ToString()
        {
            return $"Клиент в базе: {name} с {clientEndPoint.ToString()}";
        }

        internal void SendToClient(ServerClient? client, BaseMessage message)
        {
            Console.WriteLine(client.clientEndPoint);
            new Messenger().AnswerSender(message, client.ClientEndPoint);
        }
    }
}
