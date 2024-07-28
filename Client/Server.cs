﻿using Client.Messages.Fabric;
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
                    var receiveTask = udpClient.ReceiveAsync();
                    var completedTask = await Task.WhenAny(receiveTask, Task.Delay(Timeout.Infinite, cancellationToken.Token));

                    if(cToken.IsCancellationRequested)
                    {
                        break;
                    }
                    if (completedTask == receiveTask)
                    {
                        UdpReceiveResult result = receiveTask.Result;
                        BaseMessage? message = messageGetter(receiveTask);
                        if (!message.Ask)
                        {
                           IncomingMessage?.Invoke(message);
                        } else if (message.Ask && !message.UserIsOnline && !message.UserDoesNotExist)
                        {
                            IncomingMessage?.Invoke(new MessageCreatorDefault().FactoryMethodWIthText("Получатель не в сети, сообщение будет доставлено позже!"));
                        }
           
                        else if(message.Ask && message.UserDoesNotExist)
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
        public static BaseMessage? messageGetter(Task<UdpReceiveResult> receiveTask)
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