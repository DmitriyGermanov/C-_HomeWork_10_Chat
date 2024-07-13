using System.Net;
using Server.Clients;

namespace Server
{
    internal class Program
    {
        private static CancellationTokenSource cancellationTokenSource = new();
        public static Stack<Message>? Messages = new();
        private static Messenger? messenger;
        private static ClientList clientBase;
        static void Main(string[] args)
        {
            //TODO: Сделать фабрику клиентов
            CancellationToken cTokenStopAll = cancellationTokenSource.Token;
            clientBase = new ClientList();
            //TODO: Заменить этот вызов на фабричный
            messenger = new Messenger();
            Client clientFrom = new(clientBase, messenger);
            Client clientTo = new(clientBase, messenger);
            Server server = new Server(cancellationTokenSource);
            messenger = new Messenger(cancellationTokenSource);
            server.IncomingMessage += OnMessageReceived;
            bool threadFlag = true;
            //Thread serverThread = new(() => server.StartAsync());
            Task serverTask = Task.Run(server.StartAsync);
            Task messengerTask = Task.Run(() => messenger.Sender());
            Task printerTask = Task.Run(() =>
            {
                while (!cTokenStopAll.IsCancellationRequested)
                {
                    if (Messages.Count > 0)
                    {
                        //TODO: Добавить возможность проверки статуса получателя, после проверки перемещаем сообщения в отложенный лист, при смене статуса с offline на online клиента проверяем есть ли сообщения для этого клиента и отправляем ему их
                        var message = Messages.Pop();
                        clientFrom = clientBase.GetClientByEndPoint(IPEndPoint.Parse(message.LocalEndPointString));
                        //TODO: Заблокировать возможность использовать ники повторно
                        clientTo = clientBase.GetClientByName(message.NicknameTo);
                        if (clientFrom != null) {
                            clientFrom.Send(message);
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            });

            //serverThread.Start();
            Console.WriteLine("Сервер ждет сообщения от клиента (нажмите enter для остановки): ");
            Console.ReadKey();
            cancellationTokenSource.Cancel();
            printerTask.Wait();
            serverTask.Wait();
            messengerTask.Wait();
            //Console.WriteLine(printerThread.ThreadState);
            /*            server.Stop();
                        serverThread.Join();*/
            //Console.WriteLine(printerTask.Status);
            Console.WriteLine("Сервер остановлен!");
        }

        private static void OnMessageReceived(Message incomingMessage)
        {
            Messages.Push(incomingMessage);
            IPEndPoint iPEndPoint = IPEndPoint.Parse(incomingMessage.LocalEndPointString);
            messenger.EndpointCollector(iPEndPoint);
            Task.Run(() => clientBase.ClientRegistration(incomingMessage, iPEndPoint));
            
        }
    }
}