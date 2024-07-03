using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public delegate void ServerDelegate(string message);
    public class Server
    {
        public event ServerDelegate? IncomingMessage;

        private CancellationTokenSource cancellationToken;

        public Server()
        {
            cancellationToken = new CancellationTokenSource();
        }

        public async Task StartAsync()
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            using (UdpClient udpClient = new UdpClient(12345))
            {
                Console.WriteLine("Сервер ждет сообщения от клиента (нажмите enter для остановки): ");
                while (true)
                {
                    if (cancellationToken.Token.IsCancellationRequested)
                    {
                        Console.WriteLine("Сервер остановлен.");
                        break;
                    }

                    try
                    {
                        var receiveTask = udpClient.ReceiveAsync();
                        var completedTask = await Task.WhenAny(receiveTask, Task.Delay(Timeout.Infinite, cancellationToken.Token));

                        if (completedTask == receiveTask)
                        {
                            UdpReceiveResult result = receiveTask.Result;
                            string message = Encoding.UTF8.GetString(result.Buffer);
                            IncomingMessage?.Invoke(message);
                            string responseMessage = "Сообщение получено!";
                            byte[] responseData = Encoding.UTF8.GetBytes(responseMessage);
                            await udpClient.SendAsync(responseData, responseData.Length, result.RemoteEndPoint);
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

        public void Stop()
        {
            cancellationToken.Cancel();
        }
    }
}