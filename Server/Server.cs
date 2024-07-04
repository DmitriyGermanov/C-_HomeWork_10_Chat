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
                //ToDo: Убрать вывод сообщений в консоль в данном классе, реализовать обмен данными с помощью событий
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
                        udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpClient); //Второй вариант асинхронной реализации, возможной для использования с потоком
                        await Task.Delay(500);

                        /*    var receiveTask = udpClient.ReceiveAsync(); //Первый рабочий вариант реализации
                                 var completedTask = await Task.WhenAny(receiveTask, Task.Delay(Timeout.Infinite, cancellationToken.Token));

                                 if (completedTask == receiveTask)
                                 {
                                     UdpReceiveResult result = receiveTask.Result;
                                     string message = Encoding.UTF8.GetString(result.Buffer);
                                     IncomingMessage?.Invoke(message);
                                     string responseMessage = "Сообщение получено!";
                                     byte[] responseData = Encoding.UTF8.GetBytes(responseMessage);
                                     await udpClient.SendAsync(responseData, responseData.Length, result.RemoteEndPoint);
                                 }*/
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
        private void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient udpClient = (UdpClient)ar.AsyncState;
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                byte[] receiveBytes = udpClient.EndReceive(ar, ref remoteEP);
                string message = Encoding.UTF8.GetString(receiveBytes);
                IncomingMessage?.Invoke(message);

                string responseMessage = "Сообщение получено!";
                byte[] responseData = Encoding.UTF8.GetBytes(responseMessage);
                udpClient.Send(responseData, responseData.Length, remoteEP);

                udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}