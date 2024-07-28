using Server.Clients;
using Server.Clients.ClientsMenegement;
using Server.Messages;
using Server.Messages.MesagesMenegement;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public delegate void ServerDelegate(BaseMessage message);
    public class UdpServer
    {
        public event ServerDelegate? IncomingMessage;

        private CancellationTokenSource cancellationToken;
        private CancellationToken cToken;
        private IClientMeneger clientList;
        private IMessagesMenegement _messageMenegerInDb;

        public UdpServer()
        {
            cancellationToken = new CancellationTokenSource();
            cToken = cancellationToken.Token;
        }
        public UdpServer(CancellationTokenSource cancellationToken, IClientMeneger clientList)
        {
            this.cancellationToken = cancellationToken;
            cToken = cancellationToken.Token;
            this.clientList = clientList;
            this._messageMenegerInDb = new MessagesMenegementInDb(clientList);
        }

        public async Task StartAsync()
        {
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
                            ServerClient client = clientList.GetClientByName(message.NicknameFrom);
                            if (client != null && client.IsOnline && !message.DisconnectRequest)
                                clientList.SetClientAskTime(client, message);
                            if (client == null && message.LocalEndPoint != null)
                            {
                                clientList.ClientRegistration(message);
                                IncomingMessage?.Invoke(message);
                            }
                            else if (client != null && !client.IsOnline && !message.Ask)
                            {
                                clientList.SetClientAskTime(client, message);
                                _messageMenegerInDb.ShowUnrecievedMessagesAsync(client);
                                IncomingMessage?.Invoke(message);
                            }
                            /*                    else if (client != null && message.Ask && !message.UserDoesNotExist && !message.DisconnectRequest)
                                                {
                                                    clientList.SetClientAskTimeInDb(client, message);
                                                }*/
                            else if (client != null && message.Ask && !message.UserDoesNotExist && message.DisconnectRequest)
                            {
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
                    catch (SocketException)
                    {
                        Console.WriteLine("Не удалось подтвердить получение сообщения! Проверьте доступность клиента!");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        throw;
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