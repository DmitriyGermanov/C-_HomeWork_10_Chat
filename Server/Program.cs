﻿using ServerMessengerLibrary.Messages;

namespace Server
{
    internal class Program
    {
        private static CancellationTokenSource cancellationTokenSource = new();
        private static MessageCollector<byte[]>? messageCollector;
        static void Main(string[] args)
        {
            CancellationToken cTokenStopAll = cancellationTokenSource.Token;
            
            MainServer server = new MainServer(cancellationTokenSource);
            messageCollector = new MessageCollector<byte[]>(cancellationTokenSource, server.ClientMeneger, server.Messenger);
            server.IncomingMessage += OnMessageReceived;
            Task serverTask = Task.Run(server.StartAsync);
            Task messengerAnswerToEndpointsRow = Task.Run(() => messageCollector.SendAnswerFromEndpointRow());
            Task messengerAnswerToMessageRow = Task.Run(() => messageCollector.SendMessagesFromRow());
            Console.WriteLine("Сервер ждет сообщения от клиента (нажмите enter для остановки): ");
            Console.ReadKey();
            cancellationTokenSource.Cancel();
            messengerAnswerToMessageRow.Wait();
            serverTask.Wait();
            messengerAnswerToEndpointsRow.Wait();
            Console.WriteLine("Сервер остановлен!");
        }

        private static void OnMessageReceived(BaseMessage incomingMessage)
        {
            Console.WriteLine(incomingMessage);
            
            messageCollector.MessagesCollector(incomingMessage);
            
            messageCollector.EndpointCollector(incomingMessage.ClientNetId);
        }
    }
}