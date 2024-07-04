namespace Server
{
    internal class Program
    {
        public static Message LastProcessedMessage { get; set; }
        static void Main(string[] args)
        {
            /*        Message message = new();
                    message.Text = "Hello";
                    message.NicknameFrom = "Beast";
                    message.NicknameTo = "PinkRabbit";
                    message.DateTime = DateTime.Now;*/
            Server server = new();
            server.IncomingMessage += OnMessageReceived;
            //var serverTask = Task.Run(() => server.StartAsync()); //Реализация для первого варианта, на асинхронном RecieveAsync()
            new Thread(() => server.StartAsync()).Start();
            
            Console.ReadLine();
            server.Stop();

        }
        private static void OnMessageReceived(string incomingMessage)
        {
            Message message = Message.DeserializeFromJson(incomingMessage);
            LastProcessedMessage = message;
            Console.WriteLine(message);

        }
    }
}
