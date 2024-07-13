using Server.Clients;
using Server.Messages;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public delegate void ServerDelegate(BaseMessage message);
    public class Server
    {
        public event ServerDelegate? IncomingMessage;

        private CancellationTokenSource cancellationToken;
        private CancellationToken cToken;
        private ClientList clientList;

        public Server()
        {
            cancellationToken = new CancellationTokenSource();
            cToken = cancellationToken.Token;
        }
        internal Server(CancellationTokenSource cancellationToken, ClientList clientList)
        {
            this.cancellationToken = cancellationToken;
            cToken = cancellationToken.Token;
            this.clientList = clientList;
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
                            BaseMessage? message = messageGetter(receiveTask);
                            
                            if (message.Ask && !message.DisconnectRequest)
                            {
                                Client client = clientList.GetClientByEndPoint(message.LocalEndPoint);
                                if (client != null)
                                {
                                    if (!client.IsOnline)
                                    {
                                        client.IsOnline = true;
                                        client.AskTime = DateTime.Now;
                                    }
                                }
                            }
                            else if (message.DisconnectRequest && message.Ask)
                            {
                                Client client = clientList.GetClientByEndPoint(message.LocalEndPoint);
                                if (client != null)
                                    clientList.SetClientOffline(client);
                            }
                            else
                            {
                                IncomingMessage?.Invoke(message);
                            }

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
           private static BaseMessage? messageGetter(Task<UdpReceiveResult> receiveTask)
        {
            var result = receiveTask.Result;
            var messageString = Encoding.UTF8.GetString(result.Buffer);
            var message = BaseMessage.DeserializeFromJson(messageString);
            return message;
        }
        }
}