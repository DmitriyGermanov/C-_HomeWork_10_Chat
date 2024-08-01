using Server.Clients.ClientsMenegement;
using Server.Messages;
using System.Net;

namespace Server
{
    internal class Program
    {
        private static CancellationTokenSource cancellationTokenSource = new();
        private static MessageCollector<IPEndPoint>? messageCollector;
        private static IClientMeneger? clientInDbMeneger;
        static void Main(string[] args)
        {
            CancellationToken cTokenStopAll = cancellationTokenSource.Token;
            clientInDbMeneger = new ClientsInDb();
            UdpServer server = new UdpServer(cancellationTokenSource, clientInDbMeneger);
            messageCollector = new MessageCollector<IPEndPoint>(cancellationTokenSource, clientInDbMeneger, server.Messenger);
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
            messageCollector.MessagesCollector(incomingMessage);
            messageCollector.EndpointCollector(incomingMessage.LocalEndPoint);
        }
    }
}