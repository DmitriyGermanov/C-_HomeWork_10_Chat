using Client.Messages.Fabric;
using Client.Messages;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace Client
{
    public delegate void IncomingMess(bool isRecieved, BaseMessage message);
    public delegate void IncomingMessage(BaseMessage message);

    public class Server : IDisposable
    {
        public event IncomingMess? IncomingMessageCheck;
        public event IncomingMessage? IncomingMessage;
        private readonly UdpClient udpClient;
        private bool disposedValue;
        private CancellationTokenSource cancellationToken;
        private CancellationToken cToken;
        private Messenger messenger;
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
        public Server(CancellationTokenSource cancellationToken, Messenger messenger)
        {
            this.cancellationToken = cancellationToken;
            cToken = cancellationToken.Token;
            udpClient = new(new IPEndPoint(IPAddress.Loopback, 0));
            this.messenger = messenger;
        }
        public async Task WaitForAMessage()
        {
            while (true)
            {
                try
                {
                    //ToDO: Если ask - true, тогда invoke не делаем и сразу отправляем ответ серверу с ask=true 
                    var receiveTask = udpClient.ReceiveAsync();
                    var completedTask = await Task.WhenAny(receiveTask, Task.Delay(Timeout.Infinite, cancellationToken.Token));

                    if(cToken.IsCancellationRequested)
                    {
                        messenger.SendMessageAsync(new MessageCreatorDisconnect().FactoryMethod());
                        break;
                    }
                    if (completedTask == receiveTask)
                    {
                        UdpReceiveResult result = receiveTask.Result;
                        BaseMessage? message = messageGetter(receiveTask);
                        if (!message.Ask)
                        {
                            IncomingMessage?.Invoke(message);
                        }
                        else
                        {
                            messenger.SendMessageAsync(new MessageCreatorAsk().FactoryMethod());
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
        private static BaseMessage? messageGetter(Task<UdpReceiveResult> receiveTask)
        {
            var result = receiveTask.Result;
            var messageString = Encoding.UTF8.GetString(result.Buffer);
            var message = BaseMessage.DeserializeFromJson(messageString);
            return message;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
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