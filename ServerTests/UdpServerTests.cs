using Moq;
using Server;
using Server.Messages.MesagesMenegement;
using ServerMessengerLibrary.Clients;
using ServerMessengerLibrary.ClientsMenegement;
using ServerMessengerLibrary.Messages;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerTests
{
    [TestClass]
    public class UdpServerTests
    {
        private Mock<IClientMeneger> mockClientManager;
        private Mock<IMessagesMenegement> mockMessageMeneger;
        private MainServer server;
        private CancellationTokenSource cancellationTokenSource;

        [TestInitialize]
        public void Setup()
        {
            mockClientManager = new Mock<IClientMeneger>();
            mockMessageMeneger = new Mock<IMessagesMenegement>();
            cancellationTokenSource = new CancellationTokenSource();
            server = new MainServer(cancellationTokenSource);
        }

        [TestMethod]
        public void StartAsync_ShouldStartServer()
        {
            var task = server.StartAsync();
            Task.Delay(100).Wait();

            Assert.IsFalse(task.IsCompleted);
            server.Stop();
        }

        [TestMethod]
        public void Stop_ShouldStopServer()
        {
            var task = server.StartAsync();
            server.Stop();
            Task.Delay(100).Wait();
            Assert.IsTrue(task.IsCompleted);
        }
        [TestMethod]
        public async Task Test_Message_Invoked()
        {
            BaseMessage message = new DefaultMessage
            {
                NicknameFrom = "TestUser",
                Text = String.Empty,
                DisconnectRequest = false,
                Ask = false,
                LocalEndPoint = new IPEndPoint(IPAddress.Loopback, 5555)
            };
            bool eventTriggered = false;
            server.IncomingMessage += (msg) =>
            {
                eventTriggered = true;
            };
            mockClientManager.Setup(m => m.GetClientByName(It.IsAny<string>())).Returns(new IPEndPointClient<IPEndPoint> { Name = "TestUser", ClientEndPoint = new IPEndPoint(IPAddress.Loopback, 5555), IsOnline = true });
            mockClientManager.Setup(m => m.SetClientAskTime(It.IsAny<IPEndPointClient<IPEndPoint>>(), message)).Verifiable();
            var serverTask = Task.Run(() => server.StartAsync());
            using (UdpClient? client = new UdpClient(0))
            {
                var messageBytes = Encoding.UTF8.GetBytes(message.SerializeMessageToJson());
                await client.SendAsync(messageBytes, messageBytes.Length, IPEndPoint.Parse("127.0.0.1:12345"));
            }
            await Task.Delay(1000);
            server.Stop();
            await serverTask;
            Assert.IsTrue(eventTriggered, "IncomingMessage event was not triggered.");
        }
    }
}