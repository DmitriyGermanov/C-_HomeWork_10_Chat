using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public delegate void ServerDelegate(Message messaget);
    public class Server
    {
        public event ServerDelegate? IncomingMessage;

        private CancellationTokenSource cancellationToken;
        private CancellationToken cToken;

        public Server()
        {
            cancellationToken = new CancellationTokenSource();
            cToken = cancellationToken.Token;
        }
        public Server(CancellationTokenSource cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            cToken = cancellationToken.Token;
        }
            

        public async Task StartAsync()
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            using (UdpClient udpClient = new UdpClient(12345))
            {
                while (!cToken.IsCancellationRequested)
                {
                    try
                    {
                        var receiveTask = udpClient.ReceiveAsync(); 
                        var completedTask = await Task.WhenAny(receiveTask, Task.Delay(Timeout.Infinite, cancellationToken.Token));

                        if (completedTask == receiveTask)
                        {
             
                            UdpReceiveResult result = receiveTask.Result;
                            Message? message = messageGetter(receiveTask);
                            //TODO: Сюда добавляем, если получено сообщение с ASK = true, то мы проверяем у клиента isonline, если false делаем true, если Ask false, то инвокаем, если нет, то не инвокаем.
                            //TODO: Также добавить проверку на DisconnectRequest, если ASK = true и DisconnectRequest = true, то IsOnline = false;
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
        }

        public void Stop() => cancellationToken.Cancel();
           private static Message? messageGetter(Task<UdpReceiveResult> receiveTask)
        {
            var result = receiveTask.Result;
            var messageString = Encoding.UTF8.GetString(result.Buffer);
            var message = Message.DeserializeFromJson(messageString);
            return message;
        }
        }
}