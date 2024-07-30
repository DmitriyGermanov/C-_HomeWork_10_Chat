using Server.Clients;
using Server.Clients.ClientsMenegement;
using Server.Messages;
using Server.Messages.Fabric;
using Server.Messages.MesagesMenegement;
using Server.ServerMessenger;
using System.Net;
namespace Server
{
    public class MessageCollector
    {
        private CancellationTokenSource? cancellationToken;
        private CancellationToken cToken;
        private Stack<IPEndPoint> endPoints;
        public virtual Stack<IPEndPoint> EndPoints => endPoints;
        private BaseMessage? message;
        private static Stack<BaseMessage>? messages = new();
        private IClientMeneger? clientList;
        private IMessageSourceServer<IPEndPoint> _messenger;
        public MessageCollector()
        {
            cancellationToken = new CancellationTokenSource();
            cToken = cancellationToken.Token;
            endPoints = new Stack<IPEndPoint>();
            _messenger = new Messenger();
        }
        internal MessageCollector(CancellationTokenSource cancellationToken, IClientMeneger clientList)
        {
            this.cancellationToken = cancellationToken;
            cToken = cancellationToken.Token;
            endPoints = new Stack<IPEndPoint>();
            this.clientList = clientList;
            _messenger = new Messenger();
        }
        internal MessageCollector(CancellationTokenSource cancellationToken, BaseMessage message)
        {
            this.cancellationToken = cancellationToken;
            cToken = cancellationToken.Token;
            endPoints = new Stack<IPEndPoint>();
            this.message = message;
            _messenger = new Messenger();
        }
        public void EndpointCollector(IPEndPoint endPoint) => endPoints.Push(endPoint);
        internal void MessagesCollector(BaseMessage message) => messages.Push(message);

        public async Task SendMessagesFromRow()
        {
            ServerClient? clientFrom = new();
            ServerClient? clientTo = new();
            while (!cToken.IsCancellationRequested)
            {
                if (messages.Count > 0)
                {
                    //TODO: Добавить возможность проверки статуса получателя, после проверки перемещаем сообщения в отложенный лист, при смене статуса с offline на online клиента проверяем есть ли сообщения для этого клиента и отправляем ему их
                    BaseMessage? message = messages.Pop();
                    clientFrom = clientList.GetClientByName(message.NicknameFrom);
                    //TODO: Заблокировать возможность использовать ники повторно
                    clientTo = clientList.GetClientByName(message.NicknameTo);

                    if (clientFrom != null && message.NicknameTo == "")
                    {
                        clientFrom.Send(message, clientList);
                    }
                    else if (clientFrom != null && clientTo != null && clientTo.IsOnline)
                    {
                        clientFrom.SendToClientAsync(clientTo, message);
                    }
                    else if (clientFrom != null && clientTo != null && !clientTo.IsOnline)
                    {
                        clientFrom.SendToClientAsync(clientFrom, new MessageCreatorUserIsOnlineCreator().FactoryMethod());
                        message.ClientTo = clientTo;
                        message.ClientFrom = clientFrom;
                        MessagesMenegementInDb.SaveMessageToDb(message);
                    }
                    else if (clientTo == null && clientFrom != null)
                    {
                        clientFrom.SendToClientAsync(clientFrom, new MessageCreatorUserIsNotExistCreator().FactoryMethod());
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
            BaseMessage? message = new MessageCreatorDefault().FactoryMethodWIthText("Сообщение было получено сервером");
            while (!cToken.IsCancellationRequested)
            {
                if (endPoints.Count > 0)
                {
                    await Messenger.SendMessageAsync(message, endPoints.Pop());
                }

                else
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}


