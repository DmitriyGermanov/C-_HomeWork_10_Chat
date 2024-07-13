using System.Net;

namespace Client
{
    internal class Program
    {
        private static CancellationTokenSource cancellationTokenSource = new();
        public static Stack<Message>? Messages = new();
        static async Task Main(string[] args)
        {
            CancellationToken cTokenStopAll = cancellationTokenSource.Token;
            Message message = new();
            int counter = 0;
            Client client = new Client();
            Server server = new Server();
            message.LocalEndPoint = server.LocalEndPoint;

            server.IncomingMessage += (Message message) =>
            {
                Messages.Push(message);
            };

       /*     server.IncomingMessageCheck += (bool isRecieved, Message message) =>
            {
                if (isRecieved)
                    Console.WriteLine("Сообщение было успешно доставлено!");
                else
                    Console.WriteLine("Возможно сообщение не было доставлено, повторите отправку!");
            };*/

            Task printerTask = Task.Run(() =>
            {
                Message message = new Message();
                while (!cTokenStopAll.IsCancellationRequested)
                {
                    if (Messages.Count > 0)
                    {
                        message = Messages.Pop();
                        Console.WriteLine(message);
                    }
                    else
                    {
                        Thread.Sleep(100);
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
                Console.WriteLine("Введите сообщение или Exit для выхода: ");
                message.Text = Console.ReadLine();
                if (message.Text.Equals("Exit"))
                    break;
                Console.WriteLine("Введите для кого сообщение: ");
                message.NicknameTo = Console.ReadLine();
                message.DateTime = DateTime.Now;
                await client.SendMessageAsync(message);
            } while (true);
        }

    }
}