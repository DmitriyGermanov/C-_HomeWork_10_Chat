﻿using Client.Messages.Fabric;
using System.Net;
using NetMQ;
using NetMQ.Sockets;
using ClientMessengerLibrary;
using ClientMessengerNetMQLibrary;

namespace Client
{
    internal class Program
    {
        private static CancellationTokenSource cancellationTokenSource = new();
        public static Stack<BaseMessage>? Messages = new();
        private static readonly object lockObject = new();
        private static BaseMessageFabric messageFabric;
        static async Task Main(string[] args)
        {

            CancellationToken cTokenStopAll = cancellationTokenSource.Token;
            IMessageSourceClient<byte[]> messenger = new NetMqMessenger();
            var server = new Server<byte[]>(cancellationTokenSource, messenger);
            server.IncomingMessage += (BaseMessage message) =>
            {
                Messages.Push(message);
            };
            Task printerTask = Task.Run(() =>
            {
                BaseMessage message1 = new MessageCreatorDefault().FactoryMethod();
                while (!cTokenStopAll.IsCancellationRequested)
                {
                    lock (lockObject)
                    {
                        if (Messages.Count > 0)
                        {
                            message1 = Messages.Pop();
                            Console.SetCursorPosition(0, Console.CursorTop);
                            Console.WriteLine(new string(' ', Console.WindowWidth));
                            Console.SetCursorPosition(0, Console.CursorTop - 1);
                            Console.WriteLine($"[{message1.DateTime}] {message1.NicknameFrom}: {message1.Text}");
                            Console.Write("Введите сообщение или Exit для выхода: ");
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
            });
            using var runtime = new NetMQRuntime();
            Task waitingForMessage = Task.Run(() => server.WaitForAMessageAsync());
            Console.WriteLine("Введите Ваш Ник: ");
            BaseMessage message = new MessageCreatorDefault().FactoryMethod();
            message.NicknameFrom = Console.ReadLine();
            do
            {
                Console.Write("Введите сообщение или Exit для выхода: ");
                message.Text = Console.ReadLine();
                if (message.Text.Equals("Exit"))
                {
                    cancellationTokenSource.Cancel();
                    waitingForMessage.Wait();
                    printerTask.Wait();
                 
                    Console.WriteLine("Спасибо за использование, возвращайтесь!");
                    message.DisconnectRequest = true;
                    message.Ask = true;
                    await messenger.SendMessageAsync(message);
                    break;
                }
                Console.Write("Введите для кого сообщение (пустое поле - отпр. всем): ");
                message.NicknameTo = Console.ReadLine();
                message.DateTime = DateTime.Now;
                await messenger.SendMessageAsync(message);
            } while (true);
        }
    }
}