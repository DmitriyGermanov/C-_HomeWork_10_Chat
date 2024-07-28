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
        public async Task Server_InvokesIncomingMessage_WhenMessageReceived()
        {
            var mockMessenger = new Mock<IMessageSourceClient>();
            var udpClient = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
            var cts = new CancellationTokenSource();
            var server = new Server(cts, mockMessenger.Object);

            var testMessage = new DefaultMessage { NicknameFrom = "TestUser", Text = "Test Message", Ask = false };
            mockMessenger.Setup(m => m.RecieveMessageAsync(It.IsAny<UdpClient>(), cts.Token))
                         .ReturnsAsync(testMessage);
            Console.WriteLine("1");
            bool messageReceived = false;
            server.IncomingMessage += (msg) => messageReceived = true;
            Console.WriteLine("2");
            var serverTask = Task.Run(() => server.WaitForAMessageAsync());
            await Task.Delay(1000);
            cts.Cancel();
            await serverTask;

            Assert.True(messageReceived, "The IncomingMessage event was not triggered.");
            mockMessenger.Verify(m => m.RecieveMessageAsync(It.IsAny<UdpClient>(), cts.Token), Times.AtLeastOnce);
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
        public async Task Messenger_RecieveMessageAsync_ReturnsExpectedBaseMessage()
        {
            var mockMessenger = new Mock<IMessageSourceClient>();
            var udpClient = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
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

            mockMessenger.Setup(m => m.RecieveMessageAsync(udpClient, cancellationTokenSource.Token))
                         .ReturnsAsync(expectedMessage);
            var actualMessage = await mockMessenger.Object.RecieveMessageAsync(udpClient, cancellationTokenSource.Token);
            Assert.NotNull(actualMessage);
            Assert.Equal(expectedMessage.NicknameFrom, actualMessage.NicknameFrom);
            Assert.Equal(expectedMessage.Text, actualMessage.Text);
            Assert.Equal(expectedMessage.DisconnectRequest, actualMessage.DisconnectRequest);
            Assert.Equal(expectedMessage.Ask, actualMessage.Ask);
            Assert.Equal(expectedMessage.UserIsOnline, actualMessage.UserIsOnline);
            Assert.Equal(expectedMessage.UserDoesNotExist, actualMessage.UserDoesNotExist);
            Assert.Equal(expectedMessage.LocalEndPoint, actualMessage.LocalEndPoint);
        }
    }
}