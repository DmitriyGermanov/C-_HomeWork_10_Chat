using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class Messenger
    {
        private CancellationTokenSource cancellationToken;
        private CancellationToken cToken;
        private Stack<IPEndPoint> endPoints;
        public Stack<IPEndPoint> EndPoints => endPoints;
        public Messenger()
        {
            cancellationToken = new CancellationTokenSource();
            cToken = cancellationToken.Token;
            endPoints = new Stack<IPEndPoint>();
        }
        public Messenger(CancellationTokenSource cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            cToken = cancellationToken.Token;
            endPoints = new Stack<IPEndPoint>();
        }
        public void EndpointCollector(IPEndPoint endPoint)
        {
            endPoints.Push(endPoint);
        }

        public async Task Sender()
        {
            using (UdpClient udpClient = new UdpClient())
            {

                {
                    while (!cToken.IsCancellationRequested)
                    {
                        if (endPoints.Count > 0)
                        {
                            string responseMessage = "Сообщение получено!";
                            byte[] responseData = Encoding.UTF8.GetBytes(responseMessage);
                            await udpClient.SendAsync(responseData, responseData.Length, endPoints.Pop());
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
            }
        }

    }
}
