﻿using System.Net;
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
                        /*               udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpClient); //Второй вариант асинхронной реализации, возможной для использования с потоком
                                       await Task.Delay(500);*/

                        var receiveTask = udpClient.ReceiveAsync(); //Первый рабочий вариант реализации
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
        }

        public void Stop()
        {
            cancellationToken.Cancel();

        }
        private static Message? messageGetter(Task<UdpReceiveResult> receiveTask)
        {
            var result = receiveTask.Result;
            var messageString = Encoding.UTF8.GetString(result.Buffer);
            var message = Message.DeserializeFromJson(messageString);
            return message;
        }
     /*   private void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient udpClient = (UdpClient)ar.AsyncState;
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                byte[] receiveBytes = udpClient.EndReceive(ar, ref remoteEP);
                var message = Encoding.UTF8.GetString(receiveBytes);
                IncomingMessage?.Invoke(message, remoteEP);
                string responseMessage = "Сообщение получено!";
                byte[] responseData = Encoding.UTF8.GetBytes(responseMessage);
                udpClient.Send(responseData, responseData.Length, remoteEP);
                udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }*/
    }
}