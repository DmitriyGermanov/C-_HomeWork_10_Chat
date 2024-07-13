using System.Net;

namespace Client
{
    internal class Program
    {
        private static CancellationTokenSource cancellationTokenSource = new();
        public static Stack<Message>? Messages = new();
        private static readonly object lockObject = new();
        static async Task Main(string[] args)
        {
            CancellationToken cTokenStopAll = cancellationTokenSource.Token;
            Message message = new();
            int counter = 0;
            Messenger client = new Messenger();
            Server server = new Server();
            message.LocalEndPoint = server.LocalEndPoint;

            server.IncomingMessage += (Message message) =>
            {
                Messages.Push(message);
            };



            Task printerTask = Task.Run(() =>
            {
                Message message = new Message();
                while (!cTokenStopAll.IsCancellationRequested)
                {
                    lock (lockObject)
                    {
                        if (Messages.Count > 0)
                        {
                            message = Messages.Pop();
                            Console.SetCursorPosition(0, Console.CursorTop);
                            Console.WriteLine(new string(' ', Console.WindowWidth));
                            Console.SetCursorPosition(0, Console.CursorTop - 1);
                            Console.WriteLine($"[{message.DateTime}] {message.NicknameFrom}: {message.Text}");
                            Console.Write("Введите сообщение или Exit для выхода: ");
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
            });
            Task waitingForMessage = Task.Run(server.WaitForAMessage);
            Console.WriteLine("Введите Ваш Ник: ");
            message.NicknameFrom = Console.ReadLine();
            // var serverTask = Task.Run(() => server.RecieverStartAsync());
            //Себе: Для использования сервера в асинхронном режиме, должен быть новый экземпляр udpClient, передавать нельзя

            // Здесь реализуем паттерн, который фабрика на основе другого класса!!!
            do
            {
                Console.Write("Введите сообщение или Exit для выхода: ");
                message.Text = Console.ReadLine();
                if (message.Text.Equals("Exit"))
                    break;
                Console.Write("Введите для кого сообщение: ");
                message.NicknameTo = Console.ReadLine();
                message.DateTime = DateTime.Now;
                await client.SendMessageAsync(message);
            } while (true);
        }

    }
}