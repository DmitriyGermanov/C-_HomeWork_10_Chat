using Server.Messages.Fabric;
using Server.Messages.MesagesMenegement;
using ServerMessengerLibrary;
using ServerMessengerLibrary.Clients;
using ServerMessengerLibrary.ClientsMenegement;
using ServerMessengerLibrary.Messages;
namespace Server
{
    public class MessageCollector<T>
    {
        private CancellationTokenSource? cancellationToken;
        private CancellationToken cToken;
        private Stack<T> endPoints;
        public virtual Stack<T> EndPoints => endPoints;
        private BaseMessage? message;
        private static Stack<BaseMessage>? messages = new();
        private IClientMeneger? clientList;
        private IMessageSourceServer<T> _messenger;
        public MessageCollector(IMessageSourceServer<T> messenger)
        {
            cancellationToken = new CancellationTokenSource();
            cToken = cancellationToken.Token;
            endPoints = new Stack<T>();
            _messenger = messenger;
        }
        internal MessageCollector(CancellationTokenSource cancellationToken, IClientMeneger clientList, IMessageSourceServer<T> messenger)
        {
            this.cancellationToken = cancellationToken;
            cToken = cancellationToken.Token;
            endPoints = new Stack<T>();
            this.clientList = clientList;
            _messenger = messenger;
        }
        internal MessageCollector(CancellationTokenSource cancellationToken, BaseMessage message, IMessageSourceServer<T> messenger)
        {
            this.cancellationToken = cancellationToken;
            cToken = cancellationToken.Token;
            endPoints = new Stack<T>();
            this.message = message;
            _messenger = messenger;
        }
        public void EndpointCollector(T endPoint) => endPoints.Push(endPoint);
        internal void MessagesCollector(BaseMessage message) => messages.Push(message);

        public async Task SendMessagesFromRow()
        {
            while (!cToken.IsCancellationRequested)
            {
                if (messages.Count > 0)
                {
                    //TODO: Добавить возможность проверки статуса получателя, после проверки перемещаем сообщения в отложенный лист, при смене статуса с offline на online клиента проверяем есть ли сообщения для этого клиента и отправляем ему их
                    BaseMessage? message = messages.Pop();
                    var clientFrom = clientList.GetClientByName(message.NicknameFrom) as NetMqClient<byte[]>;
                    //TODO: Заблокировать возможность использовать ники повторно
                    var clientTo = clientList.GetClientByName(message.NicknameTo) as NetMqClient<byte[]>;

                    if (clientFrom != null && message.NicknameTo == "")
                    {
                        clientFrom.Send(message, clientList);
                    }
                    else if (clientFrom != null && clientTo != null && clientTo.IsOnline)
                    {
                        clientFrom.SendToClientAsync(clientTo, message, _messenger);
                    }
                    else if (clientFrom != null && clientTo != null && !clientTo.IsOnline)
                    {
                        clientFrom.SendToClientAsync(clientFrom, new MessageCreatorUserIsOnlineCreator().FactoryMethod(), _messenger);
                        message.ClientTo = clientTo;
                        message.ClientFrom = clientFrom;
                        MessagesMenegementInDb.SaveMessageToDb(message);
                    }
                    else if (clientTo == null && clientFrom != null)
                    {
                        clientFrom.SendToClientAsync(clientFrom, new MessageCreatorUserIsNotExistCreator().FactoryMethod(), _messenger);
                    }
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }
        public async Task SendAnswerFromEndpointRow()
        {

            BaseMessage? message = new MessageCreatorDefault().FactoryMethodWIthText("Сообщение было получено серверо");
            while (!cToken.IsCancellationRequested)
            {

                if (endPoints.Count > 0)
                {
                    Console.WriteLine("Сработал SendAnswerFromEndpointRow");
                    await _messenger.SendMessageAsync(message, endPoints.Pop());
                }

                else
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}


