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
        private Message message;
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
        public Messenger(CancellationTokenSource cancellationToken, Message message)
        {
            this.cancellationToken = cancellationToken;
            cToken = cancellationToken.Token;
            endPoints = new Stack<IPEndPoint>();
            this.message = message;
        }
        public void EndpointCollector(IPEndPoint endPoint)
        {
            endPoints.Push(endPoint);
        }

        public async Task Sender()
        {

            Message message = new Message { Text = "Сообщение поступило на сервер", DateTime = DateTime.Now, Ask = false };

            using (UdpClient udpClient = new UdpClient())
            {
                {
                    while (!cToken.IsCancellationRequested)
                    {
                        if (endPoints.Count > 0)
                        {
                            string jSonToSend = message.SerializeMessageToJson();
                            byte[] responseData = Encoding.UTF8.GetBytes(jSonToSend);
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
        public async Task AnswerSender(Message message, IPEndPoint clientEndpoint)
        {
            using (UdpClient udpClient = new UdpClient())
            {
                Console.WriteLine("Отправляю сообщение для" + clientEndpoint);
                string jSonToSend = message.SerializeMessageToJson();
                byte[] responseData = Encoding.UTF8.GetBytes(jSonToSend);
                await udpClient.SendAsync(responseData, responseData.Length, clientEndpoint);
            }
        }
    }
}

