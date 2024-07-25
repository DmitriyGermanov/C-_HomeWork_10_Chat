using Server.Clients;
using Server.Messages;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal delegate void ServerDelegate(BaseMessage message);
    public class Server
    {
        internal event ServerDelegate? IncomingMessage;

        private CancellationTokenSource cancellationToken;
        private CancellationToken cToken;
        private ClientsInDb clientList;
        private MessagesInDB messagesInDb;

        public Server()
        {
            cancellationToken = new CancellationTokenSource();
            cToken = cancellationToken.Token;
        }
        internal Server(CancellationTokenSource cancellationToken, ClientsInDb clientList, MessagesInDB messagesInDb)
        {
            this.cancellationToken = cancellationToken;
            cToken = cancellationToken.Token;
            this.clientList = clientList;
            this.messagesInDb = messagesInDb;
        }


        public async Task StartAsync()
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            using (System.Net.Sockets.UdpClient udpClient = new System.Net.Sockets.UdpClient(12345))
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
                            ServerClient client = clientList.GetClientByNameFromDb(message.NicknameFrom);
                            if (client != null && client.IsOnline && !message.DisconnectRequest)
                                clientList.SetClientAskTimeInDb(client, message);
                            if (client == null && message.LocalEndPoint != null)
                            {
                                clientList.ClientRegistrationInDb(message);
                                IncomingMessage?.Invoke(message);
                            }
                            else if (client != null && !client.IsOnline && !message.Ask)
                            {
                                clientList.SetClientAskTimeInDb(client, message);
                                messagesInDb.ShowUnrecievedMessagesAsync(client);
                                IncomingMessage?.Invoke(message);
                            }
                            else if (client != null && message.Ask && !message.UserDoesNotExist && !message.DisconnectRequest)
                            {
                                clientList.SetClientAskTimeInDb(client, message);
                            }
                            else if (client != null && message.Ask && !message.UserDoesNotExist && message.DisconnectRequest)
                            {
                                clientList.SetClientOfflineInDb(client);
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