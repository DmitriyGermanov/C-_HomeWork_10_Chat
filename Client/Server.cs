using System.Net.Sockets;
namespace Client
{
    public delegate void IncomingMess(bool isRecieved);
    public class Server : IDisposable
    {
        private readonly UdpClient udpClient;
        private bool disposedValue;

        public event IncomingMess? IncomingMessage;

        public Server(UdpClient client) => udpClient = client;

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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    udpClient.Close();
                    udpClient.Dispose();
                }

                disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}