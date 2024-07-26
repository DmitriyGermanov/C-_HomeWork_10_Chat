using Server.Clients.ClientsMenegement;
using Server.Messages;

namespace Server
{
    internal class Program
    {
        private static CancellationTokenSource cancellationTokenSource = new();
        private static Messenger? messenger;
        private static IClientMeneger? clientInDbMeneger;
        static void Main(string[] args)
        {
            CancellationToken cTokenStopAll = cancellationTokenSource.Token;
            clientInDbMeneger = new ClientsInDb();
            messenger = new Messenger(cancellationTokenSource, clientInDbMeneger);
            UdpServer server = new UdpServer(cancellationTokenSource, clientInDbMeneger);
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