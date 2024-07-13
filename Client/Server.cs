using System.Net;
using System.Net.Sockets;
using System.Text;
namespace Client
{
    public delegate void IncomingMess(bool isRecieved, Message message);
    public delegate void IncomingMessage(Message message);

    public class Server : IDisposable
    {
        private readonly UdpClient udpClient;
        private bool disposedValue;
        private CancellationTokenSource cancellationToken;
        private CancellationToken cToken;
        public IPEndPoint LocalEndPoint
        {
            get
            {
                return (IPEndPoint)udpClient.Client.LocalEndPoint;
            }
        }

        public event IncomingMess? IncomingMessageCheck;
        public event IncomingMessage? IncomingMessage;

        public Server(UdpClient client) => udpClient = client;
        public Server() => udpClient = new(new IPEndPoint(IPAddress.Loopback, 0));

        /*public async Task IsRessived()
        {
            *//*         while (true)
                        {
                            var receiveTask = udpClient.ReceiveAsync();
                            var completedTask = await Task.WhenAny(receiveTask, Task.Delay(Timeout.Infinite));
                            if (completedTask == receiveTask)
                            {

                                IncomingMessage?.Invoke(true);

                            }
                        }*//*

            //Console.WriteLine("Я запустился и жду сообщений");

            var receiveTask = udpClient.ReceiveAsync();
            if (await Task.WhenAny(receiveTask, Task.Delay(5000)) == receiveTask)
            {
                Message? message = messageGetter(receiveTask);
                IncomingMessageCheck?.Invoke(true, message);
            }
            else
            {
                Message? message = null;
                IncomingMessageCheck.Invoke(false, message);
            }

        }*/
        public async Task WaitForAMessage()
        {
            cancellationToken = new CancellationTokenSource();
            cToken = cancellationToken.Token;
            while (!cToken.IsCancellationRequested)
            {
                try
                {
                    //ToDO: Если ask - true, тогда invoke не делаем и сразу отправляем ответ серверу с ask=true 
                    var receiveTask = udpClient.ReceiveAsync(); 
                    var completedTask = await Task.WhenAny(receiveTask, Task.Delay(Timeout.Infinite, cancellationToken.Token));

                    if (completedTask == receiveTask)
                    {
                        UdpReceiveResult result = receiveTask.Result;
                        Message? message = messageGetter(receiveTask);
                        IncomingMessage?.Invoke(message);

                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Операция отменена.");
                    break;
                }
                catch (SocketException se)
                {
                    Console.WriteLine("Не удалось подтвердить получение сообщения! Проверьте доступность клиента!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private static Message? messageGetter(Task<UdpReceiveResult> receiveTask)
        {
            var result = receiveTask.Result;
            var messageString = Encoding.UTF8.GetString(result.Buffer);
            var message = Message.DeserializeFromJson(messageString);
            return message;
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