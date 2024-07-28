using Client.Messages.Fabric;
using Client.Messages;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Client.ClientMessenger;
namespace Client
{
    public delegate void IncomingMessage(BaseMessage message);

    public class Server : IDisposable
    {
        public event IncomingMessage? IncomingMessage;
        private readonly UdpClient udpClient;
        private bool disposedValue;
        private CancellationTokenSource cancellationToken;
        private CancellationToken cToken;
        private IMessageSourceClient messenger;
        public IPEndPoint LocalEndPoint
        {
            get
            {
                return (IPEndPoint)udpClient.Client.LocalEndPoint;
            }
        }
        public Server(UdpClient client) => udpClient = client;
        public Server()
        {
            cancellationToken = new CancellationTokenSource();
            cToken = cancellationToken.Token;
            udpClient = new(new IPEndPoint(IPAddress.Loopback, 0));
        }
        public Server(CancellationTokenSource cancellationToken, IMessageSourceClient messenger)
        {
            this.cancellationToken = cancellationToken;
            cToken = cancellationToken.Token;
            udpClient = new(new IPEndPoint(IPAddress.Loopback, 0));
            this.messenger = messenger;
        }
        public async Task WaitForAMessageAsync()
        {
            while (true)
            {
                try
                {
                    if (cToken.IsCancellationRequested)
                    {
                        break;
                    }
                    var receiveTask = messenger.RecieveMessageAsync(udpClient, cancellationToken.Token);
                    var completedTask = await Task.WhenAny(receiveTask, Task.Delay(Timeout.Infinite, cancellationToken.Token));
                    if (completedTask == receiveTask)
                    {
                        BaseMessage? message = receiveTask.Result;
                        if (!message.Ask)
                        {
                            IncomingMessage?.Invoke(message);
                        }
                        else if (message.Ask && !message.UserIsOnline && !message.UserDoesNotExist)
                        {
                            IncomingMessage?.Invoke(new MessageCreatorDefault().FactoryMethodWIthText("Получатель не в сети, сообщение будет доставлено позже!"));
                        }

                        else if (message.Ask && message.UserDoesNotExist)
                        {
                            IncomingMessage?.Invoke(new MessageCreatorDefault().FactoryMethodWIthText("Такой получатель отсутствует!"));
                        }
                        else
                        {
                            messenger.SendMessageAsync(new MessageCreatorAsk().FactoryMethod(LocalEndPoint));
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
   
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cancellationToken.Cancel();
                    udpClient.Close();
                    udpClient.Dispose();
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}