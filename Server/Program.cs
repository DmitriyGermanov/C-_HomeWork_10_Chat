using System.Net;

namespace Server
{
    internal class Program
    {
        private static CancellationTokenSource cancellationTokenSource = new(); 
        public static Stack<Message>? LastProcessedMessages = new();
        private static Messenger messenger;
        static void Main(string[] args)
        {
            CancellationToken cTokenStopAll = cancellationTokenSource.Token;
            Server server = new Server(cancellationTokenSource);
            messenger = new Messenger(cancellationTokenSource);
            server.IncomingMessage += OnMessageReceived;
            bool threadFlag = true;
            //Thread serverThread = new(() => server.StartAsync());
            Task serverTask = Task.Run(server.StartAsync);
            Task messengerTask = Task.Run(messenger.Sender); 
            Task printerTask = Task.Run(() =>
            {
                while (!cTokenStopAll.IsCancellationRequested)
                {
                    if (LastProcessedMessages.Count > 0)
                    {
                        Console.WriteLine(LastProcessedMessages.Pop());
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

        private static void OnMessageReceived(string incomingMessage, IPEndPoint remoteEndPoint)
        {
            Message? message = Message.DeserializeFromJson(incomingMessage);
            LastProcessedMessages.Push(message);
            messenger.EndpointCollector(remoteEndPoint);
            
        }
    }
}