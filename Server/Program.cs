using System.Net;
using MySqlX.XDevAPI;
using Server.Clients;
using Server.Messages;
using Server.Messages.Fabric;

namespace Server
{
    internal class Program
    {
        private static CancellationTokenSource cancellationTokenSource = new();
        public static Stack<BaseMessage>? Messages = new();
        private static Messenger? messenger;
        private static ClientsInDb clientList;
        static void Main(string[] args)
        {
            CancellationToken cTokenStopAll = cancellationTokenSource.Token;
            clientList = new ClientsInDb();
            messenger = new Messenger();
            ServerClient clientFrom = new();
            ServerClient clientTo = new();
            MessagesInDB messagesInDB = new();
            Server server = new Server(cancellationTokenSource, clientList, messagesInDB);
            messenger = new Messenger(cancellationTokenSource);
            server.IncomingMessage += OnMessageReceived;
            bool threadFlag = true;
            Task serverTask = Task.Run(server.StartAsync);
            Task messengerTask = Task.Run(() => messenger.Sender());
            Task printerTask = Task.Run(() =>
            {
                while (!cTokenStopAll.IsCancellationRequested)
                {
                    if (Messages.Count > 0)
                    {
                        //TODO: Добавить возможность проверки статуса получателя, после проверки перемещаем сообщения в отложенный лист, при смене статуса с offline на online клиента проверяем есть ли сообщения для этого клиента и отправляем ему их
                        BaseMessage message = Messages.Pop();
                        clientFrom = clientList.GetClientByNameFromDb(message.NicknameFrom);
                        //TODO: Заблокировать возможность использовать ники повторно
                        clientTo = clientList.GetClientByNameFromDb(message.NicknameTo);
    
                        if (clientFrom != null && message.NicknameTo == "")
                        {
                            clientFrom.Send(message, clientList);
                        }
                        else if (clientFrom != null && clientTo != null && clientTo.IsOnline)
                        {
                            clientFrom.SendToClient(clientTo, message);
                        }
                        else if (clientFrom != null && clientTo != null && !clientTo.IsOnline)
                        {
                            clientFrom.SendToClient(clientFrom, new MessageCreatorUserIsOnlineCreator().FactoryMethod());
                            message.ClientTo = clientTo;
                            messagesInDB.SaveMessageToDb(message);
                        }
                        else if (clientTo == null && clientFrom != null)
                        {
                            clientFrom.SendToClient(clientFrom, new MessageCreatorUserIsNotExistCreator().FactoryMethod());
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            });

            Console.WriteLine("Сервер ждет сообщения от клиента (нажмите enter для остановки): ");
            Console.ReadKey();
            cancellationTokenSource.Cancel();
            printerTask.Wait();
            serverTask.Wait();
            messengerTask.Wait();
            Console.WriteLine("Сервер остановлен!");
        }

        private static void OnMessageReceived(BaseMessage incomingMessage)
        {
            Messages.Push(incomingMessage);
            messenger.EndpointCollector(incomingMessage.LocalEndPoint);
        }
    }
}