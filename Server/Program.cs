using System.Net;
using Server.Clients;
using Server.Messages;

namespace Server
{
    internal class Program
    {
        private static CancellationTokenSource cancellationTokenSource = new();
        public static Stack<BaseMessage>? Messages = new();
        private static Messenger? messenger;
        private static ClientList clientList;
        static void Main(string[] args)
        {
            //TODO: Сделать фабрику клиентов
            CancellationToken cTokenStopAll = cancellationTokenSource.Token;
            clientList = new ClientList();
            //TODO: Заменить этот вызов на фабричный
            messenger = new Messenger();
            Client clientFrom = new(clientList, messenger);
            Client clientTo = new(clientList, messenger);
            Server server = new Server(cancellationTokenSource, clientList);
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
                        clientFrom = clientList.GetClientByEndPoint(IPEndPoint.Parse(message.LocalEndPointString));
                        //TODO: Заблокировать возможность использовать ники повторно
                        clientTo = clientList.GetClientByName(message.NicknameTo);
                        if (clientFrom != null && message.NicknameTo == "")
                        {
                            clientFrom.Send(message);
                        }
                        else if (clientFrom != null && clientTo != null && clientTo.IsOnline)
                        {
                            clientFrom.SendToClient(clientTo, message);
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
            IPEndPoint iPEndPoint = IPEndPoint.Parse(incomingMessage.LocalEndPointString);
            messenger.EndpointCollector(iPEndPoint);
            Task.Run(() => clientList.ClientRegistration(incomingMessage, iPEndPoint));

        }
    }
}