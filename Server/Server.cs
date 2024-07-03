using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    /*    Убедившись что у нас есть все для отправки и получения сообщения напишем прообраз нашего чата.Это будет утилита которая умеет работать как сервер или же как клиент в зависимости от параметров командной строки.Сервер будет уметь отправлять сообщения тогда как клиент принимать.*/
    public delegate void ServerDelegate(string message);
    public class Server
    {

        public event ServerDelegate? IncomingMessage;
        private bool flag = true;
        public void Start()
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            using (UdpClient udpClient = new UdpClient(12345))
            {
                //udpClient.Connect(iPEndPoint);
                Console.WriteLine("Сервер ждет сообщения от клиента: ");
                while (flag)
                {
                    try
                    {
                        byte[] buffer = udpClient.Receive(ref remoteEP);
                        string result = Encoding.UTF8.GetString(buffer);
                        IncomingMessage.Invoke(result);
                        string responseMessage = "Сообщение получено!";
                        byte[] responseData = Encoding.UTF8.GetBytes(responseMessage);
                        //Console.WriteLine(remoteEP);
                        udpClient.Send(responseData, responseData.Length, remoteEP);
                    }
                    catch (SocketException se)
                    {
                        Console.WriteLine("Не удалось подтвердить получение сообщения! Проверьте доступность клиента!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }
        public void Stop()
        {
            {
                flag = false;
            }
        }
    }
}
