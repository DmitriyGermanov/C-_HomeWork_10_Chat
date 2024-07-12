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

            server.IncomingMessage += (bool isRecieved) =>
            {
                if(isRecieved)
                Console.WriteLine("Сообщение успешно доставлено");
                else
                    Console.WriteLine("Возможно сообщение не было доставлено, повторите отправку!");
            };
            Console.WriteLine("Введите Ваш Ник: ");
                 message.NicknameFrom = Console.ReadLine();
           // var serverTask = Task.Run(() => server.RecieverStartAsync());
           //Себе: Для использования сервера в асинхронном режиме, должен быть новый экземпляр udpClient, передавать нельзя
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
                await server.RecieverStartAsync();
            } while (true);
        }
    }
}