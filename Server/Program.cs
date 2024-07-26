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
            MessagesInDB messagesInDB = new(clientList);
            Server server = new Server(cancellationTokenSource, clientList, messagesInDB);
            server.IncomingMessage += OnMessageReceived;
            Task serverTask = Task.Run(server.StartAsync);
            Task messengerAnswerToEndpointsRow = Task.Run(() => messenger.SendAnswerFromEndpointRow());
            Task messengerAnswerToMessageRow = Task.Run(() => messenger.SendMessagesFromRow());
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
            messenger.MessagesCollector(incomingMessage);
            messenger.EndpointCollector(incomingMessage.LocalEndPoint);
        }
    }
}