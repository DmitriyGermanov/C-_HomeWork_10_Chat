using Moq;
using Server;
using Server.Clients;
using Server.Clients.ClientsMenegement;
using Server.Messages;
using Server.Messages.MesagesMenegement;
using System.Net;
using System.Text;

namespace ServerTests
{
    [TestClass]
    public class UdpServerTests
    {
        private Mock<IClientMeneger> mockClientManager;
        private Mock<IMessagesMenegement> mockMessageMeneger;
        private UdpServer server;
        private CancellationTokenSource cancellationTokenSource;

        [TestInitialize]
        public void Setup()
        {
            mockClientManager = new Mock<IClientMeneger>();
            mockMessageMeneger = new Mock<IMessagesMenegement>();
            cancellationTokenSource = new CancellationTokenSource();
            server = new UdpServer(cancellationTokenSource, mockClientManager.Object);
        }

        [TestMethod]
        public void StartAsync_ShouldStartServer()
        {
            var task = server.StartAsync();
            Task.Delay(100).Wait(); 

            Assert.IsFalse(task.IsCompleted);
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
        public async Task Test_HandleRegisteredClientMessage()
        {

            BaseMessage message = new DefaultMessage
            {
                NicknameFrom = "TestUser",
                DisconnectRequest = false,
                Ask = false
            };

            mockClientManager
                .Setup(m => m.GetClientByName(It.IsAny<string>()))
                .Returns(new ServerClient { Name = "TestUser", IsOnline = true, ClientEndPoint = IPEndPoint.Parse("127.0.0.1:5555") });

            server.StartAsync();

            var client = new System.Net.Sockets.UdpClient();
            var messageBytes = Encoding.UTF8.GetBytes(message.SerializeMessageToJson());
            await client.SendAsync(messageBytes, messageBytes.Length, "localhost", 12345);

            await Task.Delay(1000);

            mockClientManager.Verify(m => m.SetClientAskTime(It.IsAny<ServerClient>(), It.IsAny<BaseMessage>()), Times.Once);

            server.Stop();
        }
    }
}