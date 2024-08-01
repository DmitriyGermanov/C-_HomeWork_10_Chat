using Server.Clients.ClientsMenegement;
using Server.Messages;
using Server.Messages.MesagesMenegement;
using Server.ServerMessenger;
using System.Net;
using System.Net.Sockets;

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
        public IMessageSourceServer<IPEndPoint> Messenger { get => _messenger; private set { } }

        public UdpServer()
        {
            cancellationToken = new CancellationTokenSource();
            cToken = cancellationToken.Token;
            this._messageMenegerInDb = new MessagesMenegementInDb(clientList);
            this._messenger = new UdpMessenger(new UdpClient(new IPEndPoint(IPAddress.Loopback, 12345)));
            this.clientList = new ClientsInDb();
        }
        public UdpServer(CancellationTokenSource cancellationToken, IClientMeneger clientList)
        {
            this.cancellationToken = cancellationToken;
            cToken = cancellationToken.Token;
            this.clientList = clientList;
            this._messageMenegerInDb = new MessagesMenegementInDb(clientList);
            this._messenger = new UdpMessenger(new UdpClient(new IPEndPoint(IPAddress.Loopback, 12345)));
        }

        public async Task StartAsync()
        {
            try
            {
                while (!cToken.IsCancellationRequested)
                {
                    var receiveTask = _messenger.RecieveMessageAsync(cToken);
                    var message = await receiveTask;
                    ProcessMessage(message);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Операция отменена.");
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


        public void Stop()
        {
            cancellationToken.Cancel();
            _messenger.Dispose();
        }
        private async Task ProcessMessage(BaseMessage message)
        {
            var client = clientList.GetClientByName(message.NicknameFrom);

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
                _messageMenegerInDb.ShowUnrecievedMessagesAsync(client, _messenger);
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
}