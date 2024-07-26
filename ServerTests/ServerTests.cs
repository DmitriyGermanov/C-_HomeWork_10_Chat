using Moq;
using Server;
using Server.Clients;
using Server.Clients.ClientsMenegement;
using Server.Messages;
using Server.Messages.MesagesMenegement;
using System.Net;

namespace ServerTests
{
    [TestClass]
    public class ServerTests
    {
        private Mock<IClientMeneger> mockClientList;
        private Mock<IMessagesMenegement> mockMessageMenegerInDb;
        private UdpServer server;
        private CancellationTokenSource cancellationTokenSource;

        [TestInitialize]
        public void Setup()
        {
            mockClientList = new Mock<IClientMeneger>();
            mockMessageMenegerInDb = new Mock<IMessagesMenegement>();
            cancellationTokenSource = new CancellationTokenSource();
            server = new UdpServer(cancellationTokenSource, mockClientList.Object);
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
            Task.Delay(100).Wait(); 

            server.Stop();

            Assert.IsTrue(task.IsCompleted);
        }
     
    }
}