namespace Client
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            Message message = new();
            int counter = 0;
            Client client = new Client();
            Server server = new Server(client.UdpClient);
            bool flag = true;

            server.IncomingMessage += () =>
            {
                Console.WriteLine("Сообщение успешно доставлено");
            };
            Console.WriteLine("Введите Ваш Ник: ");
                 message.NicknameFrom = Console.ReadLine();
            //_ = Task.Run(async () =>  server.RecieverStartAsync());
            do
            {
                server.RecieverStartAsync();        
             Console.WriteLine("Введите сообщение: ");
                message.Text = Console.ReadLine();
                if (String.IsNullOrEmpty(message.Text))
                {
                    flag = false;
                    continue;
                }
                Console.WriteLine("Введите для кого сообщение: ");
                message.NicknameTo = Console.ReadLine();
                message.DateTime = DateTime.Now;
                await client.SendMessageAsync(message);
                await Task.Delay(50);
            } while (flag);
            Console.ReadLine();
        }
    }
}