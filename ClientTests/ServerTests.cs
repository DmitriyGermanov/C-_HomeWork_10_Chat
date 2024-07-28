using System.Net.Sockets;
using System.Net;
using Client.Messages;
using Client;
using System.Text;
using Moq;
using Client.ClientMessenger;

namespace ClientTests
{
    public class ServerTests
    {
        [Fact]
        public void Server_Constructor_SetsLocalEndPoint()
        {
            var udpClient = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
            var server = new Client.Server(udpClient);
            var localEndPoint = server.LocalEndPoint;
            Assert.Equal(IPAddress.Loopback, localEndPoint.Address);
        }
        [Fact]
        public async Task Server_WaitForAMessageAsync_RaisesIncomingMessageEvent()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var mockClientManager = new Mock<IMessageSourceClient>();
            var server = new Server(cancellationTokenSource, mockClientManager.Object);
            bool eventTriggered = false;
            BaseMessage receivedMessage = null;


            server.IncomingMessage += (msg) =>
            {
                eventTriggered = true;
                receivedMessage = msg;
            };

            
            var serverTask = Task.Run(() => server.WaitForAMessageAsync());

            using (UdpClient? client = new UdpClient(0))
            {
                var message = new DefaultMessage
                {
                    NicknameFrom = "TestUser",
                    Text = string.Empty,
                    DisconnectRequest = false,
                    Ask = false,
                    LocalEndPoint = new IPEndPoint(IPAddress.Loopback, 5555)
                };
                var messageBytes = Encoding.UTF8.GetBytes(message.SerializeMessageToJson());
                await client.SendAsync(messageBytes, messageBytes.Length, server.LocalEndPoint);
            }

            await Task.Delay(1000); 
            cancellationTokenSource.Cancel();
            await serverTask;

            Assert.True(eventTriggered, "IncomingMessage event was not triggered.");
            Assert.NotNull(receivedMessage);
            Assert.Equal("TestUser", receivedMessage.NicknameFrom);
        }
        [Fact]
        public async Task Server_ShouldStop()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var mockClientManager = new Mock<IMessageSourceClient>();
            var server = new Server(cancellationTokenSource, mockClientManager.Object);
            var serverTask = Task.Run(() => server.WaitForAMessageAsync());
            cancellationTokenSource.Cancel();
            await serverTask;
            Assert.True(serverTask.IsCompleted);
        }
        [Fact]
        public void MessageGetter_CorrectlyDeserializesMessage()
        {
            var expectedMessage = new DefaultMessage
            {
                NicknameFrom = "TestUser",
                Text = "Hello, world!",
                DisconnectRequest = false,
                Ask = false,
                UserIsOnline = true,
                UserDoesNotExist = false,
                LocalEndPoint = new IPEndPoint(IPAddress.Loopback, 12345)
            };

            var jsonMessage = expectedMessage.SerializeMessageToJson();
            var buffer = Encoding.UTF8.GetBytes(jsonMessage);
            var receiveResult = new UdpReceiveResult(buffer, new IPEndPoint(IPAddress.Loopback, 12345));
            var result = Server.messageGetter(Task.FromResult(receiveResult));
            Assert.NotNull(result);
            Assert.Equal(expectedMessage.NicknameFrom, result.NicknameFrom);
            Assert.Equal(expectedMessage.Text, result.Text);
            Assert.Equal(expectedMessage.DisconnectRequest, result.DisconnectRequest);
            Assert.Equal(expectedMessage.Ask, result.Ask);
            Assert.Equal(expectedMessage.UserIsOnline, result.UserIsOnline);
            Assert.Equal(expectedMessage.UserDoesNotExist, result.UserDoesNotExist);
            Assert.Equal(expectedMessage.LocalEndPoint, result.LocalEndPoint);
        }
    }
}