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
            CancellationToken cTokenStopAll = cancellationTokenSource.Token;
            clientBase = new ClientList();
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
                        var message = Messages.Pop();
                        var client = clientBase.GetClientByEndPoint(IPEndPoint.Parse(message.LocalEndPointString));
                        Console.WriteLine("Сюда зашли");
                        if (client != null) {
                            Console.WriteLine("даааа");
                            client.Send(message);
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