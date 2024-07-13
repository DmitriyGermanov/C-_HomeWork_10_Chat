using Server.Messages;
using System.Net;
/*GabilyAslanov: Мы собираемся сделать наш класс полностью клиент-серверным с возможностью отправки данных сразу нескольким клиентам. Доработаем наш код следующим образом. Представьте что наш сервер умеет работать как медиатор (умеет отправлять сообщения по имени клиента), а также умеет возвращать список всех подключенных к нему клиентов. Для этого доработаем наш класс сообщений добавив поле ToName.
GabilyAslanov: Доработаем систему команд. Имя пользователя сервера всегда будет Server. Если сервер получает команду (как текст сообщения):
register: то он добавляет клиента в свой список.
delete: он удаляет клиента из списка
если сервер не получает имени получателя то он отправляет сообщение всем клиентам
если сервер получает имя получателя то он отправляет сообщение одному конкретному клиенту. Код сервера должен выглядеть примерно следующим образом:*/
namespace Server.Clients
{
    class Client : ClientBase
    {
        public Client(Mediator mediator, Messenger messenger) : base(mediator)
        {
            this.messenger = messenger;
        }
        private string name;
        private IPEndPoint clientEndPoint;
        private DateTime askTime;
        private Messenger messenger;
        private bool isOnline;
        public bool IsOnline { get { return isOnline; } set { isOnline = value; } }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public IPEndPoint ClientEndPoint
        {

            get { return clientEndPoint; }
            set { clientEndPoint = value; }
        }
        public DateTime AskTime
        {
            get { return askTime; }
            set { askTime = value; }
        }

        public override void Receive(BaseMessage message)
        {
            Task.Run(() =>
            {
                messenger.AnswerSender(message, ClientEndPoint);
            });
        }

        public override void Send(BaseMessage message)
        {
            mediator.Send(message, this);
        }

        public override string? ToString()
        {
            return $"Клиент в базе: {name} с {clientEndPoint.ToString()}";
        }

        internal void SendToClient(Client? client, BaseMessage message)
        {
            messenger.AnswerSender(message, client.ClientEndPoint);
        }
    }
}
