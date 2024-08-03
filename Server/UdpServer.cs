using Server.Messages.MesagesMenegement;
using ServerMessengerLibrary;
using ServerMessengerLibrary.ClientsMenegement;
using ServerMessengerLibrary.Messages;
using ServerMessengerNetMQLibrary;
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
        public IClientMeneger ClientMeneger { get => clientList; private set { } }
        private IMessagesMenegement _messageMenegerInDb;
        private IMessageSourceServer<byte[]> _messenger;
        public IMessageSourceServer<byte[]> Messenger { get => _messenger; private set { } }

        public UdpServer()
        {
            cancellationToken = new CancellationTokenSource();
            cToken = cancellationToken.Token;
            this._messageMenegerInDb = new MessagesMenegementInDb(clientList);
            this._messenger = new NetMqMessenger();
            this.clientList = new ClientsInDb(_messenger);
        }
        public UdpServer(CancellationTokenSource cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            cToken = cancellationToken.Token;

            this._messageMenegerInDb = new MessagesMenegementInDb(clientList);
            this._messenger = new NetMqMessenger();
            this.clientList = new ClientsInDb(_messenger);
        }

        public async Task StartAsync()
        {
            try
            {

                while (!cToken.IsCancellationRequested)
                {
                    var receiveTask = Task.Run(() => _messenger.RecieveMessageAsync(cToken), cToken);
                    var completedTask = await Task.WhenAny(receiveTask, Task.Delay(Timeout.Infinite, cToken));
                    if (completedTask == receiveTask)
                    {
                        var message = await receiveTask;
                        ProcessMessage(message);
                    }
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
            if (client == null)
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