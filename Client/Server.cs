using System.Net.Sockets;
using System.Text;
using System.Threading;
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
            /*         while (true)
                        {
                            var receiveTask = udpClient.ReceiveAsync();
                            var completedTask = await Task.WhenAny(receiveTask, Task.Delay(Timeout.Infinite));
                            if (completedTask == receiveTask)
                            {

                                IncomingMessage?.Invoke(true);

                            }
                        }*/

            //Console.WriteLine("Я запустился и жду сообщений");

            var receiveTask = udpClient.ReceiveAsync();
            if (await Task.WhenAny(receiveTask, Task.Delay(5000)) == receiveTask)
            {
                var result = receiveTask.Result;
                IncomingMessage?.Invoke(true);
            }
            else
            {
                IncomingMessage?.Invoke(false);
            }

        }


    }
}