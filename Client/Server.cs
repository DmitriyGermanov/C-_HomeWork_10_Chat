using Client.ClientMessenger;
using Client.Messages;
using Client.Messages.Fabric;
using System.Net;
using System.Net.Sockets;
namespace Client
{
    public delegate void IncomingMessage(BaseMessage message);

    public class Server
    {
        public event IncomingMessage? IncomingMessage;
        private bool disposedValue;
        private CancellationTokenSource cancellationToken;
        private CancellationToken cToken;
        private IMessageSourceClient messenger;
        public IPEndPoint LocalEndPoint
        {
            get
            {
                return messenger.GetServerEndPoint();
            }
        }
        public Server()
        {
            cancellationToken = new CancellationTokenSource();
            cToken = cancellationToken.Token;
        }
        public Server(CancellationTokenSource cancellationToken, IMessageSourceClient messenger)
        {
            this.cancellationToken = cancellationToken;
            cToken = cancellationToken.Token;
            this.messenger = messenger;
        }
        public async Task WaitForAMessageAsync()
        {
            while (!cToken.IsCancellationRequested)
            {
                try
                {
                    var receiveTask = messenger.RecieveMessageAsync(cancellationToken.Token);
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

    }
}

