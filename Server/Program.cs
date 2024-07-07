namespace Server
{
    internal class Program
    {
        public static Stack<Message>? LastProcessedMessages = new();
        static void Main(string[] args)
        {
            Server server = new();
            server.IncomingMessage += OnMessageReceived;
            bool threadFlag = true;
            Thread serverThread = new(() => server.StartAsync());
            Thread printerThread = new(() =>
            {
                while (threadFlag)
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
            serverThread.Start();
            printerThread.Start();
            Console.WriteLine("Сервер ждет сообщения от клиента (нажмите enter для остановки): ");
            Console.ReadKey();
            threadFlag = false;
            printerThread.Join();
            //Console.WriteLine(printerThread.ThreadState);
            server.Stop();
            serverThread.Join();
            //Console.WriteLine(serverThread.ThreadState);
            Console.WriteLine("Сервер остановлен!");
        }

        private static void OnMessageReceived(string incomingMessage)
        {
            Message? message = Message.DeserializeFromJson(incomingMessage);
            LastProcessedMessages.Push(message);
        }
    }
}