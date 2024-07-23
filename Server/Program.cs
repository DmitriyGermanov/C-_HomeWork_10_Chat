using Server.Clients;
using Server.Messages;

namespace Server
{
    internal class Program
    {
        private static CancellationTokenSource cancellationTokenSource = new();
        private static Messenger? messenger;
        private static ClientsInDb? clientList;
        static void Main(string[] args)
        {
            CancellationToken cTokenStopAll = cancellationTokenSource.Token;
            clientList = new ClientsInDb();
            messenger = new Messenger(cancellationTokenSource, clientList);
            MessagesInDB messagesInDB = new(messenger, clientList);
            Server server = new Server(cancellationTokenSource, clientList, messagesInDB);
            server.IncomingMessage += OnMessageReceived;
            Task serverTask = Task.Run(server.StartAsync);
            Task messengerTask = Task.Run(() => messenger.SendAnswerFromEndpointRow());
            Task printerTask = Task.Run(() =>
            {
   
            });

            Console.WriteLine("Сервер ждет сообщения от клиента (нажмите enter для остановки): ");
            Console.ReadKey();
            cancellationTokenSource.Cancel();
            printerTask.Wait();
            serverTask.Wait();
            messengerTask.Wait();
            Console.WriteLine("Сервер остановлен!");
        }

        private static void OnMessageReceived(BaseMessage incomingMessage)
        {
            messenger.MessagesCollector(incomingMessage);
            messenger.EndpointCollector(incomingMessage.LocalEndPoint);
        }
    }
}