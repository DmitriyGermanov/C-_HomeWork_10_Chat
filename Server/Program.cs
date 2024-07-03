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
            Thread serverThread = new Thread(server.Start);
            serverThread.Start();

            /*            Message? message2 = Message.DeserializeFromJson(message.SerializeMessageToJson());
                        Console.WriteLine(message2?.NicknameTo);*/
        }
        private static void OnMessageReceived(string incomingMessage)
        {
            Message message = Message.DeserializeFromJson(incomingMessage);
            LastProcessedMessage = message;
            Console.WriteLine(message);

        }
    }
}
