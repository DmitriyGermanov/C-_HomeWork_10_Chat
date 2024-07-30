using Server.Messages;
using Server.Clients.ClientsMenegement;
using Server.Clients;
using Server.Messages.MesagesMenegement;
using Server.ServerMessenger;
using System.Net.Sockets;
using System.Net;

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
        private IMessageSourceServer<IPEndPoint> _messenger;

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
            this._messenger = new Messenger(new UdpClient(new IPEndPoint(IPAddress.Loopback, 12345)));
        }

        public async Task StartAsync()
        {
            while (!cToken.IsCancellationRequested)
            {
                try
                {
                    var receiveTask = _messenger.RecieveMessageAsync(cToken);
                    var completedTask = await Task.WhenAny(receiveTask, Task.Delay(Timeout.Infinite, cancellationToken.Token));
                    if (completedTask == receiveTask)
                    {
                        BaseMessage? message = await receiveTask;
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


        public void Stop()
        {
            cancellationToken.Cancel();
            _messenger.Dispose();
        }
    }
}