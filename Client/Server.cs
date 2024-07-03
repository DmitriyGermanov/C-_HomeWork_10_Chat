using System.Net.Sockets;
namespace Client
{
    public class Server
    {
        private readonly UdpClient udpClient;
        public event Action IncomingMessage;

        public Server(UdpClient client)
        {
            udpClient = client;
        }

        public async Task RecieverStartAsync()
        {
 /*           while (true)
            {*/
            //Console.WriteLine("Я запустился и жду сообщений");
            var result = await udpClient.ReceiveAsync();
            //Console.WriteLine("Сообщение получено");
            IncomingMessage.Invoke();
            /*         }*/
        }


    }
}