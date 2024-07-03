using System.Net.Sockets;
namespace Client
{
    public delegate void IncomingMess(bool isRecieved);
    public class Server
    {
        private readonly UdpClient udpClient;
        public event IncomingMess IncomingMessage;

        public Server(UdpClient client)
        {
            udpClient = client;
        }

        public async Task RecieverStartAsync()
        {
            /*           while (true)
                       {*/
            // Console.WriteLine("Я запустился и жду сообщений");
            var receiveTask = udpClient.ReceiveAsync();
            if (await Task.WhenAny(receiveTask, Task.Delay(5000)) == receiveTask)
            {
                // Сообщение получено
                var result = receiveTask.Result;
                IncomingMessage?.Invoke(true);
            }
            else
            {
                // Тайм-аут
                IncomingMessage?.Invoke(false);
            }
            //Console.WriteLine("Сообщение получено");

        }


    }
}