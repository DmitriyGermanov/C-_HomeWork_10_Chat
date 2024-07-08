namespace Server
{
    internal class Program
    {
        private static CancellationTokenSource cancellationTokenSource = new(); 
        public static Stack<Message>? LastProcessedMessages = new();
        static void Main(string[] args)
        {
            CancellationToken cTokenStopAll = cancellationTokenSource.Token;
            Server server = new Server(cancellationTokenSource);
            server.IncomingMessage += OnMessageReceived;
            bool threadFlag = true;
            //Thread serverThread = new(() => server.StartAsync());
            Task serverTask = Task.Run(server.StartAsync);
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
            //Console.WriteLine(printerThread.ThreadState);
/*            server.Stop();
            serverThread.Join();*/
            //Console.WriteLine(printerTask.Status);
            Console.WriteLine("Сервер остановлен!");
        }

        private static void OnMessageReceived(string incomingMessage)
        {
            Message? message = Message.DeserializeFromJson(incomingMessage);
            LastProcessedMessages.Push(message);
        }
    }
}