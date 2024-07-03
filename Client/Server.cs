using System.Net.Sockets;
namespace Client
{
    public delegate void IncomingMess(bool isRecieved);
    public class Server : IDisposable
    {
        private readonly UdpClient udpClient;
        private bool disposedValue;

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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты)
                    udpClient.Close();
                    udpClient.Dispose();
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей
                disposedValue = true;
            }
        }

        // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        // ~Server()
        // {
        //     // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}