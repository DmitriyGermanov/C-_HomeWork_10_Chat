using Server.Clients;
using Server.Clients.ClientsMenegement;
using Server.Messages;
using Server.Messages.Fabric;
using Server.Messages.MesagesMenegement;
using System.Net;
using System.Net.Sockets;
using System.Text;
//TO DO: связать через интерфейс IMessageSource
namespace Server
{
    public class Messenger
    {
        private CancellationTokenSource? cancellationToken;
        private CancellationToken cToken;
        private Stack<IPEndPoint> endPoints;
        public virtual Stack<IPEndPoint> EndPoints => endPoints;
        private BaseMessage? message;
        public virtual int MessengerID { get; set; }
        private static Stack<BaseMessage>? messages = new();
        private IClientMeneger? clientList;
        public Messenger()
        {
            cancellationToken = new CancellationTokenSource();
            cToken = cancellationToken.Token;
            endPoints = new Stack<IPEndPoint>();
        }
        internal Messenger(CancellationTokenSource cancellationToken, IClientMeneger clientList)
        {
            this.cancellationToken = cancellationToken;
            cToken = cancellationToken.Token;
            endPoints = new Stack<IPEndPoint>();
            this.clientList = clientList;
        }
        internal Messenger(CancellationTokenSource cancellationToken, BaseMessage message)
        {
            this.cancellationToken = cancellationToken;
            cToken = cancellationToken.Token;
            endPoints = new Stack<IPEndPoint>();
            this.message = message;
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
            using (UdpClient? udpClient = new UdpClient())
            {
                {
                    while (!cToken.IsCancellationRequested)
                    {
                        if (endPoints.Count > 0)
                        {
                            string jSonToSend = message.SerializeMessageToJson();
                            byte[] responseData = Encoding.UTF8.GetBytes(jSonToSend);
                            await udpClient.SendAsync(responseData, responseData.Length, endPoints.Pop());
                        }

                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
            }
        }
        internal async static Task AnswerSenderAsync(BaseMessage message, IPEndPoint clientEndpoint)
        {
            using (UdpClient? udpClient = new UdpClient())
            {
                //Console.WriteLine("Отправляю сообщение для" + clientEndpoint);
                string jSonToSend = message.SerializeMessageToJson();
                byte[] responseData = Encoding.UTF8.GetBytes(jSonToSend);
               await udpClient.SendAsync(responseData, responseData.Length, clientEndpoint);
            }
        }
    }
}

